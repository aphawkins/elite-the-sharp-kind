# Generic driver for launching and visually smoke-testing a native
# Win32/SDL desktop app by driving its real OS window: launch the exe,
# inject key presses, capture screenshots, then tear down. Shared by
# this repo's per-game skills (run-elite, run-scr) - see their
# SKILL.md files for game-specific exe paths, screen flows, and key
# mappings. Not tied to any one game: any SDL window on Windows works
# the same way.
#
# TWO MODES.
#
# -KeyScript (PREFERRED, and the only reliable one for capture): the app
# drives itself. GAME_KEY_SCRIPT feeds tick-exact input straight into the
# game's own keyboard sink, and GAME_FRAME_DUMP_DIR makes the game write its
# own framebuffer out on a SaveFrame command. Nothing depends on the window
# being focused, on top, or even visible, and nothing depends on wall-clock
# timing - two runs of the same script produce byte-identical frames. The
# dump is the native render target (320x256 for Elite's 8-bit rendition), so
# there is no window chrome and no magnification to undo. Frames are
# converted to PNG and named in the order they were taken.
#
#   & ".claude/skills/sdl-drive/drive.ps1" -ExePath "...\Some.exe" -Name market -KeyScript @'
#   30 Tap N
#   90 Tap Spacebar
#   150 Tap F8
#   210 SaveFrame
#   '@
#
# Script syntax is "<tick> <Tap|Hold|Release> <ConsoleKey> [modifiers]" or
# "<tick> SaveFrame"; # starts a comment. A tick is one game update, so the
# tick numbers depend on the app's configured update rate (Elite: engine.fps
# in elite.sharp, 60 by default). See KeyScriptParser in SharpKind.Input.
#
# -Steps (legacy): OS-level key injection and a screen-scrape of the window
# rect. Keep using it to poke at a running window interactively, but know
# that its screenshots capture whatever is covering the window - if anything
# is, they silently capture that instead, and the only clue is a warning.
# PrintWindow was tried as a fix and returns a black client area for this
# SDL window, as it does for any composited one.
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
    [string[]]$Steps,
    [string]$KeyScript,
    [string]$Name = "frame",
    [int]$TimeoutSeconds = 30,
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
    [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
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

if (-not $Steps -and -not $KeyScript) {
    throw "Give either -KeyScript (preferred) or -Steps."
}

if ($Steps -and $KeyScript) {
    throw "Give -KeyScript or -Steps, not both: one drives the app from inside, the other from outside."
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

# Is the app on top? Compare the foreground window by *process*, not by
# handle: SDL's foregrounded window is not always the handle .NET reports
# as MainWindowHandle, so an hwnd comparison alone reports failure while
# the app is plainly in front and the screenshots are fine.
function Test-AppForeground {
    $foreground = [SdlDriveWin32]::GetForegroundWindow()
    if ($foreground -eq $script:hwnd) { return $true }
    if ($foreground -eq [IntPtr]::Zero) { return $false }
    if (-not $script:proc -or $script:proc.HasExited) { return $false }

    $foregroundPid = 0
    [SdlDriveWin32]::GetWindowThreadProcessId($foreground, [ref]$foregroundPid) | Out-Null
    return $foregroundPid -eq $script:proc.Id
}

# Bring the app's window to the front and confirm it got there.
# SetForegroundWindow routinely fails on the first call: Windows' foreground
# lock refuses a process that doesn't own the current foreground window, so
# the call returns and the window stays behind whatever was already on top.
# Screenshots are a real CopyFromScreen of the window rect, so a window that
# is merely *behind* another one captures the other one's pixels - which
# looks like the app never responded to a key. Retry until
# GetForegroundWindow agrees, then warn rather than silently capturing
# somebody else's window.
function Set-AppForeground([int]$TimeoutMs = 2000, [switch]$Quiet) {
    if ($script:hwnd -eq [IntPtr]::Zero) { return $false }

    if ([SdlDriveWin32]::IsIconic($script:hwnd)) {
        [SdlDriveWin32]::ShowWindow($script:hwnd, [SdlDriveWin32]::SW_RESTORE) | Out-Null
    }

    $deadline = (Get-Date).AddMilliseconds($TimeoutMs)
    while ((Get-Date) -lt $deadline) {
        if (Test-AppForeground) { return $true }
        [SdlDriveWin32]::SetForegroundWindow($script:hwnd) | Out-Null
        Start-Sleep -Milliseconds 100
    }

    if (Test-AppForeground) { return $true }

    if (-not $Quiet) {
        Write-Warning "could not bring the app window to the foreground - screenshots may capture whatever is covering it"
    }

    return $false
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

    # quiet here: SDL's window often is not ready to take focus this early,
    # and every screenshot foregrounds again (and warns) in any case
    Set-AppForeground -Quiet | Out-Null
    Start-Sleep -Milliseconds 500  # let the first frame render
    Write-Output "launched: PID $($script:proc.Id), hwnd $($script:hwnd)"
}

function Invoke-Screenshot([string]$Name) {
    if ($script:hwnd -eq [IntPtr]::Zero) { throw "not launched - add a 'launch' step first" }

    # capture is CopyFromScreen, so the window has to be on top right now -
    # anything that stole focus since launch would be captured instead
    Set-AppForeground | Out-Null

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

function Invoke-ScriptedRun {
    # The script may be given inline or as a path; the game reads a file, so
    # inline text is written to one.
    $ownsScriptFile = -not (Test-Path -LiteralPath $KeyScript -PathType Leaf)
    $scriptPath = if ($ownsScriptFile) {
        $temp = Join-Path ([System.IO.Path]::GetTempPath()) "sdl-drive-$([Guid]::NewGuid().ToString('N')).keys"
        Set-Content -LiteralPath $temp -Value $KeyScript -Encoding UTF8
        $temp
    }
    else {
        (Resolve-Path -LiteralPath $KeyScript).Path
    }

    $scriptText = Get-Content -LiteralPath $scriptPath -Raw
    $expected = ([regex]::Matches($scriptText, '(?im)^\s*\d+\s+SaveFrame\s*$')).Count
    if ($expected -eq 0) {
        Write-Warning "the script has no SaveFrame command, so no frames will be captured"
    }

    $dumpDir = Join-Path ([System.IO.Path]::GetTempPath()) "sdl-drive-frames-$([Guid]::NewGuid().ToString('N'))"
    New-Item -ItemType Directory -Force -Path $dumpDir | Out-Null

    $env:GAME_KEY_SCRIPT = $scriptPath
    $env:GAME_FRAME_DUMP_DIR = $dumpDir

    try {
        $proc = Start-Process -FilePath $ExePath -PassThru -WorkingDirectory (Split-Path $ExePath)
        Write-Output "launched: PID $($proc.Id) (scripted, no window focus needed)"

        # Stop as soon as every frame the script asked for has been written,
        # rather than sleeping for a fixed time and hoping.
        $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
        while ((Get-Date) -lt $deadline) {
            if ($expected -gt 0 -and (Get-ChildItem $dumpDir -Filter *.bmp).Count -ge $expected) { break }
            if ($proc.HasExited) { break }
            Start-Sleep -Milliseconds 200
        }

        # A moment for the last file to be closed before the process is torn
        # down under it.
        Start-Sleep -Milliseconds 300

        if (-not $proc.HasExited) { Stop-Process -Id $proc.Id -Force }
    }
    finally {
        Remove-Item Env:GAME_KEY_SCRIPT, Env:GAME_FRAME_DUMP_DIR -ErrorAction SilentlyContinue

        # Only the script's own temp file; a path the caller gave is theirs.
        if ($ownsScriptFile) { Remove-Item -LiteralPath $scriptPath -Force -ErrorAction SilentlyContinue }
    }

    $frames = @(Get-ChildItem $dumpDir -Filter *.bmp | Sort-Object Name)
    if ($frames.Count -lt $expected) {
        Write-Warning "the script asked for $expected frame(s) but $($frames.Count) were written - raise -TimeoutSeconds, or check the tick numbers against the app's update rate"
    }

    # BMP is what the game writes; PNG is what an image viewer will open.
    $index = 0
    foreach ($frame in $frames) {
        $index++
        $suffix = if ($frames.Count -gt 1) { "-{0:D2}" -f $index } else { "" }
        $png = Join-Path $ScreenshotDir "$Name$suffix.png"
        $image = [System.Drawing.Image]::FromFile($frame.FullName)
        try { $image.Save($png, [System.Drawing.Imaging.ImageFormat]::Png) } finally { $image.Dispose() }
        Write-Output "frame: $png"
    }

    Remove-Item $dumpDir -Recurse -Force -ErrorAction SilentlyContinue
}

if ($KeyScript) {
    Invoke-ScriptedRun
    return
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
