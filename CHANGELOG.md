# Changelog

All notable changes to this project are documented in this file. The format
is based on [Keep a Changelog](https://keepachangelog.com/); the project does
not yet cut versioned releases, so everything sits under Unreleased.
Completed items from the [backlog](docs/backlog-roadmap.md) move here.

## [Unreleased]

### Changed (config, 2026-07-12)

- `ConfigFile.ReadConfig` now reads `elitesharp.cfg` (renamed from
  `sharpkind.cfg`) through
  `Microsoft.Extensions.Configuration`'s JSON provider (bound onto
  `ConfigSettings`) instead of `System.Text.Json.Deserialize`, with startup
  validation (`Fps > 0` and each enum value in range) falling back to
  defaults on failure. Writing now goes behind a new `IConfigWriter`
  interface, which `SettingsView` depends on instead of the concrete
  `ConfigFile`.

### Fixed (logging, 2026-07-12)

- Both apps' logs were invisible without a debugger (Serilog's only sink was
  `Debug`, minimum level Verbose). Switched to a console sink plus a rolling
  daily file (`logs/elite-.log` / `logs/scr-.log`, 7 days retained), minimum
  level Information, overridable via the `ELITE_LOG_LEVEL` /
  `SCR_LOG_LEVEL` environment variables.

### Fixed (SCR track rendering, 2026-07-12)

- Track visibility artifacts (white triangles on corners, triangular bites
  in the track edges up close, spurious triangles on the side walls, torn
  bottom edge): the painter's sort (one averaged depth per segment) was
  replaced with a software z-buffer in `SoftwareGraphics`
  (`ClearDepth`/`DrawPolygonFilledDepth`/`DrawPolygonTexturedDepth`, 1/z
  depth test, perspective-correct textured fill), matching the original
  remake's Direct3D z-buffered `DrawTrack`; `Scene3D` now clips in float at
  the remake's 0.5-unit near plane instead of the Amiga fixed-point
  engine's integer `Z_CLIP_BOUNDARY = 128`. Also closes the backlog's
  "near-road sliver artifact" defect (no longer reproduces in
  `VisualDumpTests` frames) and obsoletes its `TrackRenderer`
  double-transform cleanup item (that code path was removed).
- View dipping under the track surface on bumpy landings: ported the
  remake's `LimitViewpointY` (road "tearing" prevention) into `CarPhysics`
  and wired it into `SceneCamera.FollowCar`, with unit tests.

### Removed

- Dead NAudio-backed audio stack: `SoftwareSound`, `SoundSampleProvider`
  and `PitchedLoopSampleProvider` had zero production references since
  `SoftwareAbstraction` switched to `SDLSound`, and `AudioController`'s
  `GenerateWaveFromMidi`/`WriteStereoWav` helpers were never called; deleted
  along with the `NAudio`/`NAudio.Vorbis`/`MeltySynth` package references in
  `Useful.Audio.csproj` and `EliteSharpLib.csproj`.

### Fixed (2026-07-11 architecture review — all Must items, plus one Should)

- Cross-platform audio: `SoftwareAbstraction` now uses the SDL_mixer-backed
  `SDLSound` instead of NAudio's Windows-only `WaveOutEvent`, fixing startup
  on Linux/ARM64. Follow-ups fixed in the same pass: mixer channel count
  raised 2 → 16 with channel 0 reserved for the engine loop (overlapping
  effects no longer crash with "No free channels available"), a dropped
  one-shot effect is no longer treated as fatal, and pitch-shifted looping
  was implemented in `SDLSound` via a `Mix_RegisterEffect` resampler
  (mirroring `PitchedLoopSampleProvider`'s algorithm) so SCR's engine sound
  works on all platforms.
- `SDLInput` no longer force-quits on Escape — quit policy belongs to each
  game, and Elite's ESC = launch escape capsule works again. Window close
  (`SDL_QUIT`) still exits.
- Elite contraband calculation counted Slaves twice and Narcotics never;
  now `(slaves + narcotics) * 2 + firearms` per the original, with unit
  tests.
- A missing or malformed `sharpkind.cfg` no longer crashes Elite at
  startup; it falls back to default settings.
- `SDLSound.Dispose` freed music/sfx handles with the wrong SDL_mixer APIs
  and double-freed music; now `Mix_FreeMusic` for music, `Mix_FreeChunk`
  for effects, once each.
- `SoftwareGraphics` rectangle drawing clamped Y against the screen *width*
  and overshot by one pixel — out-of-bounds crash / wrong-row artifacts on
  non-square screens (SCR is 640x400). Regression tests added.
- `FastBitmap.Resize` read one row/column past the source bitmap when
  growing. Tests added.
- `Space.JumpWarp` crashed with a NullReferenceException in witchspace
  (no planet/sun) once all Thargoids were destroyed.

### Added

- Root `README.md` landing page linking both games, docs and changelog.
- Consolidated all planning docs (`issues.md`, `release-plan.md`,
  `scr-conversion-plan.md`) into a single TODO list at
  `docs/review-findings.md`; folded `config.md` into the Elite readme;
  renamed `docs/readme.md` → `docs/elite-readme.md` and
  `docs/images/screenshot.png` → `elite-screenshot.png`.
- Business-application practices section in `docs/architecture.md`
  (composition root/DI, logging, configuration, error handling, lifetimes,
  testability seams).

## Stunt Car Racer conversion — progress to date

Summarised from the retired `scr-conversion-plan.md` "Done" list. Remaining
conversion work now lives in the [backlog](docs/backlog-roadmap.md).

- Project skeleton (`src/scr/*`) wired into the solution; builds clean.
- Track geometry/data model from the original Amiga track data, including
  the draw bridge animation.
- Car physics (fixed-point, ported from the original 68000 algorithms):
  sound triggers, car-to-car collision/slipstream, wheel rotation/bounce,
  smash-hole tracking; `DisplaySpeed` matches ptitSeb's revised formula.
- Opponent AI: scripted speeds, wheel-spring dynamics, steering,
  obstruct/push/move-aside interaction, lap counting and win calculation.
- 3D projection/camera pipeline (`Scene3D`, `SceneCamera`, `ScrPalette`)
  with flat-shaded and textured polygons via `Useful.Graphics`.
- Backdrop/horizon/scenery rendering, five scenery types, N cycles them.
- Car mesh (wheels + wedge body) used to draw the opponent.
- HUD: bitmap-font text overlays plus the graphical cockpit dashboard
  ported from ptitSeb's `DrawCockpit`, sprited from one converted
  `atlas.bmp` via the new `IGraphics.DrawImagePart`.
- Track menu background drawing ptitSeb's `menu.png` over the 3D world.
- Sound via `Useful.Audio`: variable-pitch engine loop, effect triggers,
  samples converted to WAV assets.
- Game-mode/screen flow (TrackMenu, TrackPreview, Race, GameOver) with
  camera orbit/preview logic and track selection; full game loop wired up,
  all eight tracks drivable end-to-end, keyboard-controlled.
- Fixed-timestep game loop, screen state machine and sound-effect
  throttling shared with Elite (`Useful.Timing.GameLoop`,
  `ScreenManager<TId, TScreen>`, `AudioController`/`SfxSample`).
- Floating-track and spurious-triangle rendering bugs fixed with
  regression coverage; road-line textures regenerated from the palette and
  textured onto the road ±11 segments around the player.
- Control scheme rewritten to match ptitSeb (independent
  accelerate/brake/boost), fixing two pre-existing shared keyboard bugs on
  the way (wrong SDL Right-Arrow mapping; one-shot `IsPressed` breaking
  continuous controls — added non-consuming `IKeyboard.IsHeld`).
