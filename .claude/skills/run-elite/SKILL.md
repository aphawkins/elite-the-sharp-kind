---
description: Launch and visually smoke-test EliteSharp (the SDL desktop app under src/elite/apps/EliteSharp) by driving its real OS window - inject key presses, capture screenshots. Use when asked to run, screenshot, or visually verify EliteSharp, or to confirm a change to it works in the real app.
---

EliteSharp is a native Win32/SDL desktop app - there is no browser or
Electron surface, so it can't be driven with Playwright. Verifying it
visually means capturing the real OS window. This skill drives it via
the shared driver at `.claude/skills/sdl-drive/drive.ps1` (also used
by `run-scr` for Stunt Car Racer — see that skill's SKILL.md and the
driver's own header comment for what it does and how; this file only
covers what's Elite-specific).

## Build first

```bash
dotnet build TheSharpKind.slnx -c Debug
```

Exe path: `src/elite/apps/EliteSharp/bin/Debug/net10.0/EliteSharp.exe`.

## Run

Use the PowerShell tool and invoke with the call operator `&`, **not**
by prefixing `pwsh` (see Gotchas in `sdl-drive`'s SKILL-shared notes
below — nested `pwsh.exe` breaks array binding):

```powershell
& ".claude/skills/sdl-drive/drive.ps1" `
  -ExePath "src\elite\apps\EliteSharp\bin\Debug\net10.0\EliteSharp.exe" `
  -Steps @(
    "launch",
    "screenshot:01-intro1",
    "key:N", "wait:600", "screenshot:02-intro2",
    "key:Space", "wait:600", "screenshot:03-commanderstatus",
    "key:F1", "wait:1500", "screenshot:04-front",
    "key:F2", "wait:600", "screenshot:05-rear",
    "key:F3", "wait:600", "screenshot:06-left",
    "key:F4", "wait:600", "screenshot:07-right",
    "quit"
  )
```

Screenshots land in `%TEMP%\sdl-app-shots\` (override with the
`SCREENSHOT_DIR` env var or `-ScreenshotDir`). Then actually open each
PNG with the Read tool — don't just check the process didn't crash.

## Known screen flow (from EliteMain.cs / Views/*.cs)

Useful for building new `-Steps` sequences:

- Launch starts on **Intro1** ("Load New Commander (Y/N)?"). `key:N`
  skips to **Intro2**.
- Intro2 (ship parade) - `key:Space` goes to **CommanderStatus**
  (docked at the start system).
- From docked, `F1` = launch/undock (goes to **Undocking** then
  **FrontView**), `F2`/`F3`/`F4` = Rear/Left/Right view (only work
  once undocked). Other `F`-keys switch to chart/status/options
  screens (`EliteMain.HandleViewKeys`) — `F1` while docked instead
  goes to **Undocking**, `F4` while docked goes to **EquipShip**.
- Full mapping: [Screen.cs](../../../src/elite/libs/EliteSharpLib/Views/Screen.cs),
  key handling: [EliteMain.cs](../../../src/elite/libs/EliteSharpLib/EliteMain.cs).

## Gotchas

See `.claude/skills/sdl-drive/drive.ps1`'s header comment and
`run-scr/SKILL.md`'s Gotchas for the shared ones (the `pwsh`-vs-`&`
invocation trap, why `PostMessage` is used instead of
`SendKeys`/`SendInput`, and the key-hold-duration timing issue) — they
apply here unchanged since both skills drive the same script. Nothing
Elite-specific beyond the screen flow above.
