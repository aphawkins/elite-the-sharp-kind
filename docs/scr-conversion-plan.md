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
- Track geometry/data model (`Tracks/Track.cs`, `AmigaTrackData.cs`), incl.
  the draw bridge animation (`Tracks/DrawBridge`).
- Car physics (`Cars/CarPhysics.*`, fixed-point trig in `Cars/AmigaTrig`):
  sound triggers, car-to-car collision/slipstream, wheel rotation/bounce
  and smash-hole tracking for the cockpit HUD.
- Opponent AI (`Cars/OpponentPhysics`): scripted speeds, wheel-spring
  dynamics, steering, obstruct/push/move-aside interaction, lap counting
  and win calculation.
- 3D projection/camera pipeline (`Rendering/Scene3D`, `SceneCamera`,
  `ScrPalette`), flat-shaded and textured polygons via
  `Useful.Graphics.DrawPolygonFilled`/`DrawPolygonTextured`.
- Backdrop/horizon/scenery rendering (`Rendering/BackdropRenderer`), five
  scenery types, N cycles them in-game.
- Car mesh (`Rendering/CarMesh`) — wheels + wedge body, used to draw the
  opponent.
- HUD: bitmap-font text overlays (opponent name, race result, game over)
  plus a graphical cockpit dashboard ported from ptitSeb's `DrawCockpit`
  (`Rendering/HudRenderer`, `CockpitState`) — wheel sprites, engine/boost
  flame, damage crack/holes, speed bar, lap/boost/distance read-outs, all
  sprited from one converted `atlas.bmp` via the new
  `IGraphics.DrawImagePart`. Super League atlas variants not used yet.
- Track menu background (`Screens/TrackMenuScreen`) draws ptitSeb's
  `menu.png` as a transparent-centre overlay over the 3D world — a
  cosmetic addition beyond strict porting, since ptitSeb's own menu is
  still plain text.
- Sound via `Useful.Audio`: variable-pitch engine loop
  (`PitchedLoopSampleProvider`), effect triggers, samples converted to WAV
  assets under `Assets/SFX`.
- Game-mode/screen flow (`Screens/` classes: TrackMenu, TrackPreview, Race,
  GameOver) with camera orbit/preview logic and track selection (keys 1-8).
- Full game loop wired up in `StuntCarRacerMain`: one track drivable and
  rendered end-to-end, keyboard-controlled.
- Fixed-timestep game loop shared between Elite and SCR
  (`Useful.Timing.GameLoop`, `Useful.Abstraction.IGame`/`GameHost`).
- Floating-track and spurious-triangle rendering bugs fixed in
  `TrackRenderer`/`BackdropRenderer`, with regression coverage.
- Shared game-mode/screen state machine (`Useful.Abstraction.IGameScreen`,
  `ScreenManager<TId, TScreen>`) used by both Elite's `IView`s and SCR's
  `Screens/` classes.
- Shared sound-effect throttling (`AudioController`/`SfxSample`) used by
  both games instead of a SCR-local throttle.
- Road-line textures regenerated from the palette (`Rendering/
  RoadTextures`) and textured onto the road ±11 segments around the
  player.
- `CarPhysics.DisplaySpeed` matches ptitSeb's revised formula (dead zone
  raised to `< 0x1100`, rescaled by `200/128` to fill the cockpit gauge).
- Control scheme rewritten to match ptitSeb: independent
  `Accelerate`/`Brake`/`Boost` `CarInput` flags (replacing the old
  combined-key scheme), mapped to arrow keys + Space in
  `RaceScreen.ReadInput`.
- Fixed two pre-existing keyboard bugs surfaced by the control scheme
  rewrite (shared by Elite and SCR): `SDLHelper` mapped the physical Right
  Arrow key to the wrong `ConsoleKey`, and `SoftwareKeyboard.IsPressed`'s
  one-shot semantics broke continuous controls once a second key was held.
  Added non-consuming `IKeyboard.IsHeld` for continuous polling
  (`RaceScreen`, Elite's `PilotView`); one-shot menu keys still use
  `IsPressed`.

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
  (`SCR_BASE_COLOUR+16/17/18`), alternate player-car and opponent-car body
  colours (`SCR_BASE_COLOUR+19/20/21` in `Car.cpp`'s `DrawCar`), alternate
  engine power/boost constants (`engine_power`/`boost_unit_value`/
  `opp_engine_power`: 240/16/236 standard vs 320/12/314 super, in
  `Car_Behaviour.cpp`), alternate opponent speed tables
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
- **Opponent speed-value algorithm was rewritten in ptitSeb**: fluffyfreak
  precomputed a random per-track speed table once (`opponents_speed_values`,
  seeded from `opp_track_speed_values`). ptitSeb replaced it with
  `Opponent_Speed_Value()` in `Opponent_Behaviour.cpp`, computed per-piece
  from `Piece_Angle_And_Template`/`sections_car_can_be_put_on` with a
  memoized accumulator — a direct port of the authentic Amiga assembly
  (inlined as a comment in that function), not a random approximation.
  `OpponentPhysics`/`OpponentData.SpeedValues`/`TrackSpeedValues` still use
  the old random-table approach.
- **F9/F10 frame-gap tuning keys**: present in both C++ versions
  (increment/decrement the physics frame gap live); `StuntCarRacerMain
  .FrameGap` exists for exactly this but isn't wired to any key.
- **Road-line textures could source the shared atlas instead of a
  procedural strip**: ptitSeb consolidated the six road textures into
  `atlas.png` (`eRoadYellowDark` etc., already converted to `atlas.bmp` for
  the cockpit HUD) rather than separate `Road*.bmp` files. `Rendering
  /RoadTextures` still generates its strips procedurally from the palette;
  sampling the real atlas art instead would be a closer visual match, but
  the current procedural strips already look correct — cosmetic refinement,
  not a bug.

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
