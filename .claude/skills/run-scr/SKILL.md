---
description: Launch and visually smoke-test StuntCarRacerSharp (the SDL desktop app under src/scr/apps/StuntCarRacerSharp) by driving its real OS window - inject key presses, capture screenshots. Use when asked to run, screenshot, or visually verify SCR/Stunt Car Racer, or to confirm a change to it works in the real app.
---

StuntCarRacerSharp is a native Win32/SDL desktop app - there is no
browser or Electron surface, so it can't be driven with Playwright.
Verifying it visually means capturing the real OS window. This skill
drives it via the shared driver at
`.claude/skills/sdl-drive/drive.ps1` (also used by `run-elite` for
Elite — the driver itself is fully game-agnostic; only the exe path,
key sequence, and screen flow below are SCR-specific).

## Build first

```bash
dotnet build TheSharpKind.slnx -c Debug
```

Exe path:
`src/scr/apps/StuntCarRacerSharp/bin/Debug/net10.0/StuntCarRacerSharp.exe`.

## Run

Use the PowerShell tool and invoke with the call operator `&`, **not**
by prefixing `pwsh` (see Gotchas — nested `pwsh.exe` breaks array
binding):

```powershell
& ".claude/skills/sdl-drive/drive.ps1" `
  -ExePath "src\scr\apps\StuntCarRacerSharp\bin\Debug\net10.0\StuntCarRacerSharp.exe" `
  -Steps @(
    "launch",
    "screenshot:01-trackmenu",
    "key:S", "wait:800", "screenshot:02-trackpreview",
    "key:S", "wait:1200", "screenshot:03-race",
    "key:Up:800", "wait:200", "screenshot:04-race-accel",
    "key:Left:500", "wait:200", "screenshot:05-race-steer-left",
    "key:Escape",
    "quit"
  )
```

Screenshots land in `%TEMP%\sdl-app-shots\` (override with the
`SCREENSHOT_DIR` env var or `-ScreenshotDir`). Then actually open each
PNG with the Read tool — don't just check the process didn't crash.

### Steps (shared driver — same for every skill that uses it)

| step | what it does |
|---|---|
| `launch` | start the exe, wait for its window (throws after `-LaunchTimeoutMs`, default 15s), foreground it |
| `screenshot:<name>` | capture the window to `<ScreenshotDir>/<name>.png` |
| `key:<KeyName>` | press and release a key (150ms hold) |
| `key:<KeyName>:<holdMs>` | press, hold for `<holdMs>`, then release — use this for SCR's driving keys (`Up`/`Down`/`Left`/`Right`/`Space`), which read as *held*, not *pressed* (see Gotchas) |
| `wait:<ms>` | sleep |
| `quit` | stop the process |

See `ConvertTo-VirtualKeyCode` in `../sdl-drive/drive.ps1` for
supported key names (`F1`-`F12`, `A`-`Z`, `0`-`9`, `Space`, `Enter`,
`Esc`/`Escape`, `Up`/`Down`/`Left`/`Right`); add more there if you need
one — it's shared, so a name you add works for both games. Any step
not given a `quit` is cleaned up automatically at the end of the
script (with a warning), so a thrown error mid-sequence doesn't leave
the process running.

## Known screen flow (from Screens/*.cs, GameMode.cs)

Useful for building new `-Steps` sequences. Four modes
(`GameMode` enum): `TrackMenu` (0, the start screen) → `TrackPreview`
(1) → `GameInProgress` (2, the race) → `GameOver` (3).

- **TrackMenu** ([TrackMenuScreen.cs](../../../src/scr/libs/StuntCarRacerSharpLib/Screens/TrackMenuScreen.cs)):
  digits `1`-`8` pick a track (defaults to Little Ramp); `S` goes to
  **TrackPreview**; `Escape` quits.
- **TrackPreview** ([TrackPreviewScreen.cs](../../../src/scr/libs/StuntCarRacerSharpLib/Screens/TrackPreviewScreen.cs)):
  `S` starts the race (**GameInProgress**), `M` back to **TrackMenu**.
- **GameInProgress / race** ([RaceScreen.cs](../../../src/scr/libs/StuntCarRacerSharpLib/Screens/RaceScreen.cs)):
  driving keys are read with `IsHeld`, not `IsPressed` — `Left`/`Right`
  = steer, `Up` = accelerate, `Down` = brake, `Space` = boost. `N`
  changes scenery. There's no key back to the menu mid-race (a known
  backlog gap) — use `Escape` to quit instead.
- **GameOver** ([GameOverScreen.cs](../../../src/scr/libs/StuntCarRacerSharpLib/Screens/GameOverScreen.cs)):
  `M` back to **TrackMenu**.
- `Escape` quits from anywhere ([StuntCarRacerMain.cs](../../../src/scr/libs/StuntCarRacerSharpLib/StuntCarRacerMain.cs)).

## Gotchas

These apply to `sdl-drive/drive.ps1` itself, so they're the same for
`run-elite` too:

- **Invoking as `pwsh drive.ps1 -Steps @(...)` from inside an
  already-running `pwsh` session spawns a *nested* `pwsh.exe` process,
  and its CLI argument parsing silently breaks array binding** —
  `-Steps` only picks up the first element, and later `-Steps` items
  shift into `-ScreenshotDir`/`-LaunchTimeoutMs` positionally,
  producing a confusing type-conversion error on `-LaunchTimeoutMs`
  (e.g. `Cannot convert value "wait:600" to type "System.Int32"`). Use
  the call operator (`& "drive.ps1" -Steps @(...)`) instead — that
  runs the script in the *current* session, where PowerShell's real
  parameter binder handles the array correctly. Hit and confirmed
  while building this skill, not a theoretical concern.
- **`System.Windows.Forms.SendKeys` and the `SendInput` API do not
  work against these apps.** Both were tried first: the target window
  was confirmed correctly foregrounded and focused
  (`GetForegroundWindow()` matched the target `hwnd` after
  `SetForegroundWindow` and even after a synthetic click on the
  window), but SDL's event pump never observed either kind of
  injected input — screenshots kept showing the same frame,
  unchanged. Posting `WM_KEYDOWN`/`WM_KEYUP` directly to the window
  handle via `PostMessage` is the only method that produced an actual
  in-game screen transition. If you're tempted to "simplify" this to
  `SendKeys` — don't, without re-verifying against a screenshot first.
- **Hold the key down across a poll cycle.** Both games tick well
  under 100ms per frame, and `SoftwareKeyboard.KeyUp` resets the same
  "just-pressed" state that `IsPressed` reads, so a down+up pair that
  both land in the same `SDL_PollEvent` drain can cancel out before
  `IsPressed` ever sees them. The driver's default 150ms hold covers
  this; SCR's driving controls use `IsHeld` instead (see above) and
  read correctly for as long as you hold the key via `key:<Name>:<ms>`.
- **The window must actually be on-screen and unobstructed** —
  capture is a real `CopyFromScreen`, not an off-screen render. Don't
  minimize or cover the window between `launch` and `quit`.
