# Stunt Car Racer → EliteSharp-style C# conversion plan

Source: `C:\code\github\ptitSeb\stuntcarremake` (C++, DirectX9/DXUT + SDL2;
a maintained fork of the original `fluffyfreak/stuntcarracer` remake, with
a graphical cockpit HUD, gamepad/Linux/web ports and other fixes). Earlier
work in this conversion was ported from `fluffyfreak/stuntcarracer` before
the switch; where the two diverge, ptitSeb's is now the source of truth.
Target: `C:\code\github\aphawkins\elite-the-sharp-kind` (this repo), as a new sibling
game alongside Elite, sharing the `src/useful/*` libraries.

## Goal

Hand-port Stunt Car Racer (SCR) from C++/DirectX9 to cross-platform C#
(.NET, SDL2), following the architecture already established by the Elite
conversion in this repo: hardware access hidden behind interfaces, a software
renderer as the primary rendering path, and maximum reuse of the `Useful.*`
libraries rather than SCR-specific duplicates.

## Maintaining this plan

This file is used as the prompt to resume the conversion in new sessions.
Keep it accurate and lean:

- Update it as part of every work item, in the same commit/step as the code
  change: move the item from "Remaining work" (or the architecture section)
  into "Done" with a short summary, or add a new item if new work is
  identified while working.
- Keep "Done" entries short — a line or two naming what landed and where,
  not a narrative of how it was fixed or debugged. Git history has the
  detail if it's ever needed again.
- Keep "Remaining work" items flat and self-contained — one concern per
  item, not nested sub-bullets — so any single item can be picked up without
  reading the whole file first.
- Prune detail that's no longer needed to make forward progress (e.g. once
  a bug is fixed and has a regression test, the debugging narrative can go).

## Architecture principles

- Mirror `src/elite/*`'s structure: `src/scr/apps/StuntCarRacer`,
  `src/scr/libs/StuntCarRacerLib`, `src/scr/test/StuntCarRacerLib.{Tests,Fakes}`,
  wired into `EliteSharp.slnx`, same project-reference/analyzer conventions
  as `EliteSharpLib` (`Directory.Build.props` applies).
- Hardware access stays behind interfaces (`Useful.Abstraction.IAbstraction`,
  `IGraphics`, `IKeyboard`, `ISound`); the software rasterizer
  (`Useful.Graphics.SoftwareGraphics`) is the primary rendering path.
- Before writing SCR-specific code, check whether the equivalent already
  exists in `src/useful/*` and extend that library instead of duplicating
  it: math → `System.Numerics`, rendering → `IGraphics`/`SoftwareGraphics`,
  assets → `Useful.Assets.AssetLocator`, input → `Useful.Controls.IKeyboard`,
  audio → `Useful.Audio`, game loop/state → `Useful.Abstraction`/
  `Useful.Timing`. A genuine SCR-only need (e.g. track-segment collision) is
  fine to keep local — the bar is "does this belong in a shared library,"
  not "force everything to be shared."
- Behavioural fidelity ("feels like the original"), not bit-exact numerical
  replication, is the bar — there is no requirement to match the original's
  frame-by-frame physics output.
- Treat the original's binary asset formats (tracks/bitmaps/sounds) as
  read-once inputs to a one-time conversion step, not a live format the C#
  code must parse identically forever.

## Done

- Project skeleton (`src/scr/*`) wired into the solution, builds clean.
- Track geometry/data model (`Tracks/Track.cs`, `AmigaTrackData.cs`) with
  unit tests, including the draw bridge animation (`Tracks/DrawBridge`).
- Car physics (`Cars/CarPhysics.*`, fixed-point trig in `Cars/AmigaTrig`),
  including sound-effect triggers and car-to-car collision/slipstream.
- Opponent AI (`Cars/OpponentPhysics`): scripted speeds, wheel-spring
  dynamics, steering, obstruct/push/move-aside interaction, lap counting
  and win calculation.
- 3D projection/camera pipeline (`Rendering/Scene3D`, `SceneCamera`,
  `ScrPalette`) rendering flat-shaded track/side polygons via
  `Useful.Graphics.DrawPolygonFilled` (matches the Amiga original's look;
  no textured-fill extension was needed for this).
- Backdrop/horizon/scenery rendering (`Rendering/BackdropRenderer`), five
  scenery types, N cycles them in-game.
- Car mesh (`Rendering/CarMesh`) — wheels + wedge body, used to draw the
  opponent.
- Bitmap-font text overlays: opponent name at race start, flashing race
  result, game over.
- Graphical cockpit HUD ported from ptitSeb's `DrawCockpit`/atlas
  (`Rendering/HudRenderer`, `Rendering/CockpitState`): front wheel sprites
  that bounce with suspension and spin with road speed (`CarPhysics`
  gained `LeftWheelFrame`/`RightWheelFrame`/`LeftWheelBounce`/
  `RightWheelBounce`, ported from `SetWheelRotationSpeed`), an engine with
  a boost flame animation, the damage crack image revealed progressively
  by damage plus smash holes (`CarPhysics.SmashHoles`, from `UpdateDamage`/
  `nholes`), the speed bar, and lap/boost/opponent-distance read-outs.
  Sprites come from a single converted `atlas.bmp` (`Assets/Images`,
  registered in `AssetManifest.json`) via the new
  `IGraphics.DrawImagePart` (scaled sub-rectangle blit with horizontal
  mirroring, implemented in `SoftwareGraphics`/`SDLGraphics` and both
  fakes). Super League ("2"-suffixed) atlas variants are not used yet.
- Track menu background (`Screens/TrackMenuScreen`): draws ptitSeb's
  `Bitmap/menu.png` (converted to `Assets/Images/menu.bmp`) as a
  full-screen overlay after the 3D world; its centre panel is transparent
  so the orbiting track view still shows through, with the track list
  positioned inside it. This is a cosmetic addition beyond strict
  porting — the actual ptitSeb game never loads `menu.png` (confirmed: no
  `LoadTexture`/`CreateTextureFromResource` call for it in `StuntCarRacer.cpp`);
  its own track menu is still plain DXUT text, same as before the source
  switch, so this is the only intentional visual divergence from ptitSeb's
  actual behaviour.
- Sound via `Useful.Audio`: variable-pitch engine loop
  (`PitchedLoopSampleProvider`), effect triggers, samples converted to WAV
  assets under `Assets/SFX`.
- Game-mode/screen flow (`Screens/` classes: TrackMenu, TrackPreview, Race,
  GameOver) with camera orbit/preview logic and track selection (keys 1-8).
- Full game loop wired up in `StuntCarRacerMain`: one track drivable and
  rendered end-to-end, keyboard-controlled.
- Fixed-timestep game loop shared between Elite and SCR
  (`Useful.Timing.GameLoop`, `Useful.Abstraction.IGame`/`GameHost`) — fixed
  a bug where SCR's car physics ran ~2.4x too fast.
- Floating-track bug fixed (backdrop/track projection mismatch;
  `BackdropRendererTests` regression coverage) and spurious-triangle
  rendering bug fixed (near-plane clip before fan-triangulation in
  `TrackRenderer.AddPolygon`; regression coverage across all eight tracks).
- Shared game-mode/screen state machine extracted to `Useful.Abstraction`
  (`IGameScreen`, `ScreenManager<TId, TScreen>`); both Elite's `IView`s and
  SCR's `Screens/` classes use it.
- Shared sound-effect throttling extracted: SCR now plays effects through
  the same `AudioController`/`SfxSample` cooldown mechanism Elite uses,
  rather than a duplicate SCR-local throttle.
- Road-line textures: `IGraphics.DrawPolygonTextured` with an affine
  textured triangle fill in `SoftwareGraphics`; the six road strips are
  regenerated from the palette (`Rendering/RoadTextures`, replacing the
  original's `Road*.bmp`) and `TrackRenderer` textures the road ±11
  segments around the player as the original did.

## Architecture / refactoring work identified

Work noticed while converting SCR that isn't a new SCR feature, but improves
the shared engine or removes duplication between the two games.

- **`Scene3D` vs Elite's projection/clip code**: SCR (`Rendering/Scene3D`)
  and Elite each have their own 3D transform + near-plane-clip pipeline.
  Compare them and extract shared math into `Useful.Graphics`/`Useful` if
  it genuinely generalizes — don't force an abstraction over two pipelines
  that turn out to differ for good reason.
- **Shared text/HUD helpers**: both games draw similar HUD panels (name/
  stat text, flashing messages) with separate ad-hoc code
  (`StuntCarRacerMain.DrawHud`, Elite's `EliteDraw`/views). Look for a small
  shared helper in `Useful.Graphics` instead of each game reimplementing
  layout and flash timing.
- **Move Elite's frame composition out of `Update`**: Elite's `Update` runs
  at a fixed 13.5Hz and composes the whole frame into the framebuffer as it
  moves objects (`Space.UpdateUniverse`/`EliteDraw`), matching the original
  TNK design; `Draw` just presents it at up to `Config.Fps`. This means
  raising `Config.Fps` currently doesn't draw anything new above 13.5Hz.
  Moving to per-object draw lists so `Draw` can render fresh interpolated
  frames above the tick rate is a bigger refactor — confirm it's still
  worth doing (it's Elite-only, not required for the SCR conversion itself)
  before picking it up.

## Remaining work

- **Gamepad support**: port `XBOXController.cpp/h` via `Useful.Controls`;
  not started, keyboard-only so far.
- **Per-effect sound volume**: the original scales grounded/creak effect
  volume by damage level. `AudioController`/`SfxSample` currently has no
  per-play volume parameter.
- **Player outside/chase view**: needs a chase camera plus drawing the
  player's own car mesh (`Rendering/CarMesh` is currently only used to draw
  the opponent).
- **Near-road sliver artifact**: a thin dark-red diagonal sliver sometimes
  draws across the near road surface (see `VisualDumpTests` frame_landed).
  Pre-dates road-line textures — likely a painter's-sort or side-wall
  geometry edge case in `TrackRenderer`.
- **Boost is BCD in the original**: the Amiga stores boost.reserve as BCD
  (`abcd`, printed digit-by-digit) but `Track.StandardBoost` loads the raw
  byte and `CarPhysics.BoostReserve` counts it in binary, so e.g. a track
  byte of $30 gives 48 boost units instead of 30. Convert from BCD on load.
- **Lap times**: the original shows current/best lap times on the dashboard
  (print.lap.time/show.lap.time); not ported, no lap timing exists yet.
- **Full damage should wreck the car**: the original's damage.line wrecks
  the car (car.is.wrecked) when the crack reaches the end of the beam
  (240); the HUD caps the crack but nothing wrecks the car.
- **Super League**: ptitSeb added a `bSuperLeague` toggle (the 'L' key on
  the track menu) that swaps in alternate track colours/road-line textures
  (`SCR_BASE_COLOUR+16/17/18`), alternate opponent speed tables
  (`opp_track_speed_values[TrackID+32]`), and the atlas's "2"-suffixed
  cockpit/road sprite variants. Not started — would need a menu toggle,
  `RoadTextures`/`ScrPalette` alternates, and wiring `CarPhysics
  .RoadCushionValue`/`OpponentPhysics` speed tables to the mode (the
  `RoadCushionValue` property already exists for this but nothing sets it).
- **Widescreen/dynamic window resizing**: ptitSeb's Windows build supports
  resizable windows with dynamic cockpit/font scaling
  (`GetScreenDimensions`, `COCKPIT_WIDESCREEN_OFFSET`); out of scope while
  the app targets a fixed 640x400 window, and `HudRenderer`/
  `TrackMenuScreen` assume that fixed size when computing their scale
  factors.

## Validation

- Unit tests for physics/track/math logic in `StuntCarRacerLib.Tests`,
  following existing conventions in `EliteSharpLib.Tests`/`Useful.*.Tests`.
- Before considering any work item done: build the full solution, run the
  complete test suite, and smoke-test the affected app(s) live (the window
  must open and run without crashing) if the change touches shared code or
  either app's game loop.
- Manual comparison against the original SCR build for "does it feel
  right" — steering response, jump/landing behaviour, track segment
  transitions. No requirement to match the original frame-by-frame.
