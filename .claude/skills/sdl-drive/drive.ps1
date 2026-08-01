# Generic driver for launching and visually smoke-testing a native
# Win32/SDL desktop app by driving its real OS window: launch the exe,
# inject key presses, capture screenshots, then tear down. Shared by
# this repo's per-game skills (run-elite, run-scr) - see their
# SKILL.md files for game-specific exe paths, screen flows, and key
# mappings. Not tied to any one game: any SDL window on Windows works
# the same way.
#
# Each element of -Steps is one of:
#   launch                 - start the exe, wait for its window, foreground it
#   screenshot:<name>      - capture the window to <ScreenshotDir>/<name>.png
#   key:<KeyName>          - press a key (see ConvertTo-VirtualKeyCode below
#                             for supported names). Modifiers are prefixed
#                             with +, e.g. key:Ctrl+M, key:Shift+Ctrl+H
#   key:<KeyName>:<ms>     - press a key and hold it for <ms> milliseconds
#                             before releasing (for IsHeld-style controls,
#                             e.g. a racing game's steer/accelerate keys)
#   wait:<ms>               - sleep for <ms> milliseconds
#   quit                    - stop the process
#
# Example (invoke with the call operator, NOT `pwsh drive.ps1 ...` - see
# the per-game SKILL.md Gotchas for why):
#   & ".claude/skills/sdl-drive/drive.ps1" -ExePath "C:\...\Some.exe" -Steps @(
#     "launch", "wait:800", "screenshot:01-start",
#     "key:Enter", "wait:600", "screenshot:02-after-enter",
#     "quit"
#   )

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ExePath,
    [Parameter(Mandatory = $true)]
    [string[]]$Steps,
    [string]$ScreenshotDir = $(if ($env:SCREENSHOT_DIR) { $env:SCREENSHOT_DIR } else { Join-Path $env:TEMP "sdl-app-shots" }),
    [int]$LaunchTimeoutMs = 15000
)

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

Add-Type @"
using System;
using System.Runtime.InteropServices;

public static class SdlDriveWin32 {
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] public static extern uint MapVirtualKey(uint uCode, uint uMapType);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")] public static extern bool IsIconic(IntPtr hWnd);

    public const uint WM_KEYDOWN = 0x0100;
    public const uint WM_KEYUP = 0x0101;
    public const int SW_RESTORE = 9;
}
"@

if (-not (Test-Path $ExePath)) {
    throw "Executable not found at '$ExePath'. Build it first."
}

New-Item -ItemType Directory -Force -Path $ScreenshotDir | Out-Null

$script:proc = $null
$script:hwnd = [IntPtr]::Zero

# SDL windows on Windows only responded reliably to keyboard input that
# was posted directly to the window handle (WM_KEYDOWN/WM_KEYUP via
# PostMessage) when this was built and verified against EliteSharp.
# System.Windows.Forms.SendKeys and the SendInput API both looked like
# they should work - the target window was confirmed foregrounded and
# focused via GetForegroundWindow() - but SDL's event pump never
# observed either. Do not switch this back to SendKeys/SendInput
# without re-verifying against a screenshot.
function ConvertTo-VirtualKeyCode([string]$KeyName) {
    switch -regex ($KeyName) {
        '^F(1[0-2]|[1-9])$' { return 0x70 + [int]($KeyName.Substring(1)) - 1 }
        '^[A-Z]$'           { return [byte][char]$KeyName }
        '^[0-9]$'           { return [byte][char]$KeyName }
        '^Space$'           { return 0x20 }
        '^(Enter|Return)$'   { return 0x0D }
        '^Esc(ape)?$'        { return 0x1B }
        '^Tab$'               { return 0x09 }
        '^Back(space)?$'      { return 0x08 }
        '^Up$'                { return 0x26 }
        '^Down$'              { return 0x28 }
        '^Left$'              { return 0x25 }
        '^Right$'             { return 0x27 }
        # Elite uses these for roll/speed and for menu left/right; the
        # arrow keys are extended-key codes that PostMessage doesn't
        # deliver to SDL, so these are the reliable alternatives.
        '^Ctrl$'              { return 0x11 }
        '^Shift$'             { return 0x10 }
        '^Alt$'               { return 0x12 }
        '^Comma$'             { return 0xBC }
        '^Period$'            { return 0xBE }
        '^Slash$'             { return 0xBF }
        default { throw "Unknown key name '$KeyName' - add it to ConvertTo-VirtualKeyCode in drive.ps1" }
    }
}

function Invoke-Launch {
    if ($script:proc -and -not $script:proc.HasExited) {
        Write-Output "already launched (PID $($script:proc.Id))"
        return
    }

    $script:proc = Start-Process -FilePath $ExePath -PassThru -WorkingDirectory (Split-Path $ExePath)
    $deadline = (Get-Date).AddMilliseconds($LaunchTimeoutMs)
    $hwnd = [IntPtr]::Zero
    while ((Get-Date) -lt $deadline) {
        $script:proc.Refresh()
        if ($script:proc.MainWindowHandle -ne [IntPtr]::Zero) { $hwnd = $script:proc.MainWindowHandle; break }
        Start-Sleep -Milliseconds 100
    }

    if ($hwnd -eq [IntPtr]::Zero) {
        throw "Timed out waiting for the app's main window (PID $($script:proc.Id))"
    }

    $script:hwnd = $hwnd
    if ([SdlDriveWin32]::IsIconic($script:hwnd)) {
        [SdlDriveWin32]::ShowWindow($script:hwnd, [SdlDriveWin32]::SW_RESTORE) | Out-Null
    }

    [SdlDriveWin32]::SetForegroundWindow($script:hwnd) | Out-Null
    Start-Sleep -Milliseconds 500  # let the first frame render
    Write-Output "launched: PID $($script:proc.Id), hwnd $($script:hwnd)"
}

function Invoke-Screenshot([string]$Name) {
    if ($script:hwnd -eq [IntPtr]::Zero) { throw "not launched - add a 'launch' step first" }

    $rect = New-Object SdlDriveWin32+RECT
    [SdlDriveWin32]::GetWindowRect($script:hwnd, [ref]$rect) | Out-Null
    $w = $rect.Right - $rect.Left
    $h = $rect.Bottom - $rect.Top

    $bmp = New-Object System.Drawing.Bitmap $w, $h
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CopyFromScreen($rect.Left, $rect.Top, 0, 0, (New-Object System.Drawing.Size $w, $h))

    $path = Join-Path $ScreenshotDir "$Name.png"
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose()
    $bmp.Dispose()
    Write-Output "screenshot: $path"
}

function Send-KeyDown([int]$Vk) {
    $scan = [SdlDriveWin32]::MapVirtualKey($Vk, 0)
    $lParam = [IntPtr]((1) -bor ($scan -shl 16))
    [SdlDriveWin32]::PostMessage($script:hwnd, [SdlDriveWin32]::WM_KEYDOWN, [IntPtr]$Vk, $lParam) | Out-Null
}

function Send-KeyUp([int]$Vk) {
    $scan = [SdlDriveWin32]::MapVirtualKey($Vk, 0)
    $lParam = [IntPtr]((1) -bor ($scan -shl 16) -bor (1 -shl 30) -bor (1 -shl 31))
    [SdlDriveWin32]::PostMessage($script:hwnd, [SdlDriveWin32]::WM_KEYUP, [IntPtr]$Vk, $lParam) | Out-Null
}

# A key name may carry modifiers, "Ctrl+M" or "Shift+Ctrl+H": each is held
# down around the key itself and released in reverse, as a real chord would
# be. Games that read modifiers separately from the key (Elite's Ctrl-H
# galactic hyperspace, its Ctrl-M mission jump) need both down at once.
function Invoke-Key([string]$KeyName, [int]$HoldMs = 150) {
    if ($script:hwnd -eq [IntPtr]::Zero) { throw "not launched - add a 'launch' step first" }

    $names = $KeyName -split '\+'
    $key = $names[-1]
    $modifiers = @($names[0..($names.Length - 2)])

    $modifierVks = @($modifiers | ForEach-Object { ConvertTo-VirtualKeyCode $_ })
    $vk = ConvertTo-VirtualKeyCode $key

    foreach ($modifierVk in $modifierVks) { Send-KeyDown $modifierVk }
    Send-KeyDown $vk
    Start-Sleep -Milliseconds $HoldMs
    Send-KeyUp $vk
    for ($i = $modifierVks.Length - 1; $i -ge 0; $i--) { Send-KeyUp $modifierVks[$i] }
    Write-Output "key: $KeyName (held ${HoldMs}ms)"
}

function Invoke-Quit {
    if ($script:proc -and -not $script:proc.HasExited) {
        Stop-Process -Id $script:proc.Id -Force
        Write-Output "quit: stopped PID $($script:proc.Id)"
    }
    else {
        Write-Output "quit: not running"
    }

    $script:proc = $null
    $script:hwnd = [IntPtr]::Zero
}

foreach ($step in $Steps) {
    $parts = $step -split ':'
    $verb = $parts[0]

    switch ($verb) {
        'launch' { Invoke-Launch }
        'screenshot' { Invoke-Screenshot -Name $parts[1] }
        'key' {
            if ($parts.Length -ge 3) { Invoke-Key -KeyName $parts[1] -HoldMs ([int]$parts[2]) }
            else { Invoke-Key -KeyName $parts[1] }
        }
        'wait' { Start-Sleep -Milliseconds ([int]$parts[1]); Write-Output "wait: $($parts[1])ms" }
        'quit' { Invoke-Quit }
        default { Write-Warning "unknown step '$step' - expected launch, screenshot:<name>, key:<name>[:<holdMs>], wait:<ms>, or quit" }
    }
}

if ($script:proc -and -not $script:proc.HasExited) {
    Write-Warning "process PID $($script:proc.Id) is still running - no 'quit' step was given. Stopping it now."
    Invoke-Quit
}
