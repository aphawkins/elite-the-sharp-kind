# Changelog

All notable changes to this project are documented in this file. The format
is based on [Keep a Changelog](https://keepachangelog.com/); the project does
not yet cut versioned releases, so everything sits under Unreleased.
Completed items from the [backlog](docs/backlog-roadmap.md) move here.

## [Unreleased]

### Added (Injectable AudioController options, 2026-07-20)

- `AudioController`'s `_musicOn`/`_effectsOn` were hardcoded `true` behind
  a pointless `#if DEBUG`/`#else` with identical branches; the constructor
  now takes a new `Useful.Audio.AudioOptions` (`MusicOn`/`EffectsOn`, both
  defaulting `true`) instead. Elite's `ConfigSettings` gained matching
  `MusicOn`/`EffectsOn` properties (default `true`, no behaviour change),
  and `EliteServiceCollectionExtensions`'s `AudioController` registration
  now builds the options from the already-resolved `GameState.Config`
  rather than a second `ConfigFile.ReadConfig()` call. SCR
  (`StuntCarRacerMain`) has no settings infrastructure at all yet, so it
  passes a plain default `AudioOptions` at its construction site — same
  always-on behaviour as before, just explicit instead of hardcoded
  inside `AudioController`. Added `PlayEffectDoesNothingWhenEffectsAreOff`
  and `PlayMusicAndStopMusicDoNothingWhenMusicIsOff` to
  `AudioControllerTests`, which needed `FakeSound.Play(string, bool)`/
  `StopMusic()` to actually count calls (previously no-ops).

### Added (Elite view registrations in the container, 2026-07-20)

- The last composition-root item: `EliteMain` no longer constructs its ~25
  `IView` screens itself. Each view (`Intro1View` through `GameOverView`)
  is now registered as a singleton in a new private
  `EliteServiceCollectionExtensions.AddEliteViews`, resolved by factory
  delegate for the same internal-accessibility reason as the domain
  services. `AddEliteMain`'s `EliteMain` factory now populates the
  `ScreenManager<Screen, IView>` singleton (`views.Add(Screen.X, ...)` for
  all 25 screens) by resolving each view from the container before
  constructing `EliteMain` — this has to happen there rather than inside
  `AddEliteViews` itself, since IServiceCollection registration is lazy
  and population needs actual built instances. `EliteMain`'s constructor
  shrinks accordingly: `Trade`, `PlanetController`, `IShipFactory`,
  `ConfigFile` and `ScreenManager<Screen, IView>` are all gone from its
  parameter list (they were only ever used to build views), leaving just
  the collaborators its `Update`/`Draw`/`Run` logic actually touches.

### Added (Elite domain services in the container, 2026-07-20)

- `EliteMain`'s constructor no longer builds its own domain graph: `GameState`,
  `PlayerShip`, `Trade`, `PlanetController`, `EliteDraw` (as `IEliteDraw`),
  `IShipFactory` (via `ShipFactory.Create`), `Universe`, `Stars`, `Pilot`,
  `Combat`, `SaveFile`, `Space`, `Scanner`, `AudioController` and the
  `ScreenManager<Screen, IView>` backing `GameState` are all now registered
  as singletons in `EliteServiceCollectionExtensions.AddEliteMain` (which
  also forwards `AssetLocator` as `IAssetLocator`), and `EliteMain`'s
  constructor just receives them. Since every one of these types is
  `internal` to `EliteSharpLib`, the container can't auto-wire them by
  reflection (that only sees public constructors), so each is registered via
  an explicit factory delegate rather than a bare `AddSingleton<T>()` — this
  is deliberately verbose over hiding it behind a bigger, riskier
  internal-to-public accessibility change. The `AudioController` `SfxSample`
  cooldown table (`// TODO: improve this`) moved into a private
  `BuildEliteSfx` helper alongside the other registrations, unchanged.
  `EliteDraw`'s `_colorText = _draw.Palette["White"]` read and `SaveFile`'s
  `ConfigFile.BaseDirectory` path both stay exactly where they were,
  now sourced from the injected `IEliteDraw`/`ConfigFile`.
  `EliteMain` still builds the ~25 views itself — that's the next backlog
  item.

### Added (Elite composition root, 2026-07-20)

- Mirrors the SCR composition root: `EliteSharp.SDLProgram.Main` builds a
  `ServiceCollection` instead of `new`-ing `SoftwareAbstraction` and
  `EliteMain` directly, registering `IAbstraction` and its forwarded
  `IGraphics`/`ISound`/`IKeyboard`, the Serilog `ILoggerFactory`, and
  `AssetLocator`. `ConfigFile` (and `EliteMain` itself, whose constructor
  now takes it) are `internal` to `EliteSharpLib` with no
  `InternalsVisibleTo` for the `EliteSharp` app, so `Program.Main` can't
  register or construct them directly; new `EliteServiceCollectionExtensions`
  in `EliteSharpLib` adds `AddEliteConfig(userDataPath)` and `AddEliteMain()`
  extension methods that do this from inside the assembly that can see
  those types, and registers `EliteMain` as `IGame` too. `EliteMain`'s
  constructor now takes `AssetLocator`/`ConfigFile` as parameters instead
  of creating them (the user-data path moved out to `Program.Main`, with
  `ConfigFile` gaining an internal `BaseDirectory` property so `EliteMain`
  can still build `SaveFile`'s path from it), and is `internal` rather
  than `public` since only the in-assembly registrar factory calls it
  now. `EliteMain`'s constructor otherwise keeps building the rest of the
  domain graph as before — that's the separate "move Elite's domain
  services into the container" item.

### Added (SCR composition root, 2026-07-20)

- `StuntCarRacer.SDLProgram.Main` now builds a `ServiceCollection`
  (`Microsoft.Extensions.DependencyInjection`, newly referenced by the
  `StuntCarRacer` project) instead of `new`-ing `SoftwareAbstraction` and
  `StuntCarRacerMain` directly: `SoftwareAbstraction` is registered as
  `IAbstraction` via a factory (container-owned and disposed with the
  provider), `IGraphics`/`ISound`/`IKeyboard` are forwarded from it, the
  existing Serilog-backed `ILoggerFactory` is registered as an instance,
  and `StuntCarRacerMain` is registered as itself and as `IGame`. `Main`
  resolves the concrete `StuntCarRacerMain` (not `IGame`, which has no
  `Run`) and calls `.Run()`. This is the first of the composition-root
  items — Elite's `Program.Main` mirrors the pattern next.

### Added (SCR per-effect sound volume/pitch/pan, 2026-07-20)

- Sound effects now vary per play instead of always sounding identical,
  matching the Amiga original's `DSSetMode`/`DrawDustClouds`/`DrawSparks`/
  `UpdateDamage` behaviour: Creak and Grounded volume scale with impact
  damage (`CarPhysics.CalculateDamageVolume`, shared by both, matching
  the original's identical formula in both places), and the off-road/
  edge-scrape sounds are pitched (randomly for off-road, by speed for
  the edge scrape) instead of playing at a flat pitch. Each effect also
  gets the original's fixed stereo pan (engine and Smash left,
  everything else right) and HitCar's fixed quieter volume.
  `Useful.Audio` grew the plumbing for this: `ISound.Play` and
  `AudioController.PlayEffect` take volume/pan/pitch, `SfxSample` carries
  a per-effect static volume/pan profile, and `SDLSound` implements
  pitch-shifted one-shots with the same resample-on-a-reserved-channel
  technique the engine loop already used (a new second reserved channel,
  stopping after one pass instead of looping). Elite's 9 `PlayEffect`
  call sites are unaffected (same simple no-args overload).
  Deliberate deviations: skips the original's `AmigaVolumeToDirectX` dB
  round-trip since SDL_mixer's volume is already linear (see
  `CalculateDamageVolume`'s comment for the maths); the two pitched
  effects have no "recorded rate" reference in the original either (both
  always override the frequency), so pitch=1.0 is anchored at each
  formula's own range midpoint (464 for off-road, 360 for the edge
  scrape) rather than a verified original value. Covered by new
  `AudioControllerTests` (volume/pan/pitch pass-through) and
  `CarPhysicsTests` (damage-scaled volume in range, off-road/wreck pitch
  in the derived ranges); actual audio output was not manually verified
  by ear.

### Added (SCR lap times, 2026-07-20)

- The dashboard now shows a current-lap clock and best-lap time
  (`T0:00.00`/`B0:00.00`, `M:SS.CC`), mirroring the Amiga original's
  `print.lap.time`/`show.lap.time` read-outs. `CarPhysics` tracks
  `CurrentLapTicks` (advanced once per 50Hz `ApplyEngineRevs` tick,
  the same hook the wheel-spin-rate fix uses) and `BestLapTicks`
  (updated and the current lap reset at each lap boundary in
  `UpdateLapData`); `HudRenderer` formats ticks as `M:SS.CC` and draws
  them beside the existing lap/boost/distance read-outs. Deliberate
  deviation: this is a straightforward wall-clock timer, not a port of
  the original's 3-byte BCD stopwatch, whose exact increment/wrap
  semantics were not fully reverse-engineered from the raw disassembly
  (ptitSeb never implements this feature, so there is no clean C++
  reference to check against). The follow-up "BCD fidelity" backlog
  item was closed won't-fix 2026-07-20: without a working emulator to
  verify the exact wrap semantics against, chasing byte-for-byte
  fidelity risks silently reproducing a misread of the disassembly,
  and the current wall-clock-accurate timer is arguably the more
  useful behaviour anyway. Dashboard placement is a
  reasonable slot (confirmed to fit an existing empty panel via
  `VisualDumpTests`), not verified against the original's exact
  layout. Covered by new `CarPhysicsTests` (tick advance only via
  `ApplyEngineRevs`, reset on new race, full-lap best-time recording)
  and a `HudRendererTests` case for the current/best text.

### Fixed (SCR cockpit wheel-spin rate, 2026-07-20)

- Cockpit front wheel sprites were spinning at a quarter of their
  correct rate: the wheel-angle advance lived in `CarMovement`, which
  only runs every `FrameGap`-th tick (12.5Hz), instead of the original
  `FramesWheelsEngine`'s full 50Hz rate. Split `SetWheelRotationSpeed`
  into a physics-rate speed calculation (unchanged) and a new
  `AdvanceWheelAngles`, now called from `CarPhysics.ApplyEngineRevs` —
  already the 50Hz hook, called every tick from `RaceScreen.Update` —
  keeping the original's right-wheel-reads-left-angle quirk. Covered by
  a new `CarPhysicsTests.WheelAnglesAdvanceOnEveryEngineRevsTickNotJustPhysicsFrames`
  test that drives `ApplyEngineRevs` alone (no physics frames) and
  checks the wheel frame moves.

### Changed (SCR full damage wrecks the car, 2026-07-20)

- `CarPhysics.Wrecked` now goes true when `NewDamage` reaches 240,
  matching the Amiga original's `damage.line`/`car.is.wrecked` (the HUD
  crack previously just capped out with no effect). The C# port already
  carried the rest of the Amiga's (and ptitSeb's dormant, never-triggered)
  wreck plumbing — `_wreckWheelHeightReduction`, the wheel-height
  subtraction, the `!Wrecked`-gated engine power/boost cuts, and the
  scrape-sound gate — so the only change is setting that field once the
  damage threshold is crossed; those existing paths do the rest. The race
  flow is otherwise unchanged (the opponent still finishes and the race is
  lost). Covered by new `CarPhysicsTests` (`FullDamageWrecksTheCar`,
  `PartialDamageDoesNotWreckTheCar`, `WreckedResetsOnNewRace`).

### Changed (SCR opponent speed values, 2026-07-19)

- The opponent's per-piece required speeds are now computed by a port of
  ptitSeb's `Opponent_Speed_Value()` (itself derived from the Amiga's
  `opponents.speed.values` creation assembly) instead of the old
  fluffyfreak per-track random tables: a per-track random mask and base
  from the full 64-byte `opp_track_speed_values` table (now carried
  verbatim in `OpponentData.TrackSpeedValues`, super-league rows
  included for the future Super League item), ten faster on sections
  the car can be put on, memoized so the value only re-rolls when the
  opponent changes piece. Two deliberate choices beyond the reference:
  the can-be-put-on test uses bit 7 as the Amiga's `bpl` does (ptitSeb
  tests `b < 0` on an unsigned value, which never fires), and the draw
  bridge's `SetSpeedValue` writes still take precedence via a per-piece
  override (the Amiga modified its precomputed table; ptitSeb's own
  writes go unread since nothing reads the table any more — a
  regression not copied). Covered by new `SpeedValue` unit tests
  (deterministic standard-league values, RNG-stream stability while on
  a piece, draw-bridge override precedence) and smoke-tested live.

### Audited (SCR ptitSeb parity, 2026-07-19)

- Ran a full feature-by-feature comparison of the C# port against
  `ptitSeb/stuntcarremake` (see the backlog's "Resolved (2026-07-19) —
  ptitSeb parity audit" note for the findings). One backlog item was
  found already complete and removed: **opponent name announcement** —
  `StuntCarRacerMain.DrawHud` has shown "Opponent: <name>" for the
  first four seconds of a race since the atlas HUD work (commit
  2a476b4); the remaining gaps were re-verified and tightened into
  discrete backlog items (wreck-at-full-damage plumbing references,
  wheel-spin tick rate, sound frequency/volume/pan table, mid-race 'M'
  side effects, and the unplugged racewin/racelost/wrecked/heads art
  screens).

### Fixed (Elite ship rendering, 2026-07-14)

- Elite's ships now render through the shared software z-buffer
  (`DrawPolygonFilledDepth`, the path Stunt Car Racer already used)
  instead of pure painter overdraw, fixing the long-standing "bits of
  hidden surfaces show through" artefact — with the max-Z face sort
  (checked against The New Kind's `threed.c`: `zavg` = MAX is
  authentic, not a porting bug), far-side decals and detail lines beat
  near hull faces that wrap toward the tail. Each face now rasterizes
  with one flat per-pixel depth: the mean Z of its "root" face's
  transformed points. `ShipBase` computes face roots once per instance
  from the model geometry — decal faces (cockpit windows, engine
  plates) and 2-point detail lines lie exactly (distance 0.000) in the
  plane of an earlier larger face and inherit that face's key, so they
  tie exactly and the chain's later-submission tie order draws them on
  top, the convention the models were built for. Flat rather than
  interpolated per-vertex depth is deliberate: measurement showed the
  rasterizer's clamped edge interpolation deviates from a coplanar
  face's plane by far more than any safe bias, punching seam-shaped
  holes through decals (Transporter panel, Cobra engine plates).
  Wireframe mode is unchanged apart from the sort key. This supersedes
  the backlog defect about the `zavg` max-vs-mean sort. Verified with a
  new `VisualDumpTests` that renders lone-ship, decal-heavy
  (Transporter spin, rear Cobra) and overlap/interpenetration scenes
  through the real rasterizer to BMPs for visual inspection.

### Changed (user data location, 2026-07-12)

- `ConfigFile` and `SaveFile` (`.cmdr` commander saves) now resolve their
  files against an injected base directory instead of the current working
  directory. `EliteMain` computes the default once
  (`%AppData%\EliteSharp` on Windows, `~/.config/EliteSharp` on
  Linux/macOS via `Environment.SpecialFolder.ApplicationData`) and passes
  it to both, fixing the "launched from a shortcut" breakage where the CWD
  wasn't the app's install directory, and making both classes testable
  against a temp directory. The shipped default `elitesharp.cfg` next to
  the executable is no longer read and was removed.

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
