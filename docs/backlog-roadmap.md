# Backlog and Roadmap — The Sharp Kind

The single consolidated backlog for the repository, prioritised with MoSCoW
(per [architecture-principles.md](architecture-principles.md)). It merges the 2026-07-11
architecture/code-quality review, the business-application practices review,
and the retired `issues.md`, `release-plan.md` and `scr-conversion-plan.md`.

How to use this file:

- Each item is one concern, small enough for a single focused session, and
  self-contained (paths, line numbers, problem, fix direction). Items too
  big for one session are tagged **[LARGE]** — split them in a follow-up
  conversation before starting, not here.
- Definition of done (from the retired SCR plan, now repo-wide): build the
  full solution, run the complete test suite, and smoke-test the affected
  app(s) live if the change touches shared code or either game loop.
- When an item completes, delete it here and record it in
  [CHANGELOG.md](../CHANGELOG.md).
- Line numbers date from the review; verify before editing.

## Decisions

Maintainer decisions live in [decisions.md](decisions.md), not here — each
one blocks or reshapes items below; check there before starting an item
that mentions a decision.

## Must

(none open — see [CHANGELOG.md](../CHANGELOG.md) for completed items)

## Should

### Asset structure for the 8-bit/16-bit tiers

Implements the 2026-07-28 asset-structure decision in
[decisions.md](decisions.md); the full design, including the rejected
alternatives and the measured colour-count baseline, is in
[asset-structure.md](asset-structure.md). Steps are ordered — each is
independently verifiable, and steps 1-5 do not depend on the 8-bit art
existing.

- [ ] [Assets] Author SCR's 8-bit asset set: a 16-colour
      `palette.json`, `atlas.bmp` and `menu.bmp` under
      `Images/EightBit/`, and an 8-bit bitmap font (Elite's 8-bit tier
      uses a single 8x8 `bbc-micro.bmp` for both sizes, declared in an
      `AssetManifest.EightBit.json`). Hand-authored art, as Elite's was
      — mechanically downscaling the 16-bit files gives filtered 16-bit
      imagery, not 8-bit-era art. The tier plumbing is done (2026-07-29,
      see CHANGELOG): the manifest declares only `SixteenBit`, so
      selecting `EightBit` fails at startup until the files exist, and
      adding them plus `"EightBit"` to `Tiers` is all that is left.
- [ ] [Useful.Assets] Decide whether the asset validator should also
      check bitmap dimensions against the tier's resolution. Today the
      colour cap is the only tier constraint, so nothing catches art
      authored at the wrong size. Elite's 8-bit lasers are exactly half
      their 16-bit counterparts, but `scanner.bmp` is 260x56 against the
      16-bit 512x129 — not a halving, so a naive "must be 2x" rule would
      reject art the maintainer intends. Needs a decision on what the
      rule is before it can be written.

### Tier presentation architecture

The controller/view split and the per-tier view split are both **done** (see
[CHANGELOG.md](../CHANGELOG.md) for what landed when); the rules that came
out of them live in
[architecture-principles.md](architecture-principles.md)'s "Screens:
controllers, models and per-tier views". What is left open:

- [ ] [EliteSharpLib] Polish the 8-bit view layouts in
      `Views/EightBit/`. Every screen has one and renders without
      overflowing, but they are first drafts: verification was "does it
      build, does it render, does it avoid the obvious overflow", not the
      worst-case-content check the rules ask for. `CommanderStatusView8Bit`
      is the one screen that got that treatment and is the reference for
      what "done" looks like. The maintainer took this judgement over
      personally, so this is spacing/authoring work, not a refactor.
      `engine.tier` in `%APPDATA%\The Sharp Kind\elite.sharp` is set to
      `EightBit` and stays that way while this is the tier being worked on
      (maintainer, 2026-07-30), so verifying a layout is just a run - no
      config edit, and nothing to restore afterwards.
- [ ] [EliteSharpLib] Author `Scanner8Bit`'s HUD layout against the real
      320x56 scanner art. Its ~20 positions are the 16-bit offsets halved,
      which is close enough to render but visibly wrong: the dial bars
      overrun their slots and the compass and indicator cluster are
      crowded. Same character of work as the view-layout item above, and
      every number is a named member in one file. `ScannerBase` holds no
      coordinates, so nothing else has to move.
- [ ] [EliteSharpLib] `ScannerBase.ScannerExtent` is `(28, 50)` on both
      tiers, so scanner contacts only ever use ±50px around the centre.
      The 16-bit radar area grew from 311px wide to 439px with the 640x512
      widening, so contacts now fill less of the dial than they did.
      Pre-existing rather than a regression — it was ±50 at 512 too — but
      decide whether the extent should be derived from the scanner art's
      plot area rather than fixed. Affects how far away ships appear, so
      it is a feel decision, not a layout fix.
- [ ] [EliteSharpLib] `OptionsView8Bit` word-wraps the credits itself
      because the longest ("The New Kind - Christian Pinder 1999-2001", 41
      characters) is one character wider than the 8-bit screen's 40-character
      row, and `IBaseView.DrawTextPretty` is unusable for it (it breaks text
      that already fits, and draws left-aligned where these lines are
      centred). Either fix `DrawTextPretty`'s off-by-one break so views can
      share one wrapper, or shorten the credit wording in
      `OptionsController`; the local `Wrap` helper is a stopgap either way.
      Note each tier now has its own copy of the wrapper, on
      `BaseView8Bit`/`BaseView16Bit`, so a fix has to land in both.
- [x] [EliteSharpLib] `ShortRangeChartViewBase` was the one screen still
      combining controller and view. Split into
      `ShortRangeChartController`/`ShortRangeChartModel` with per-tier
      `ShortRangeChartView8Bit`/`16Bit`, taking the shape this entry
      preferred: the tier's layout metrics go into the controller, so the row
      packing and the `CarryFlag` quirk stay in one place and the model
      carries finished screen positions. That is why this model is in screen
      space where `GalacticChartModel` is in galaxy space — the packing
      decides which planets get a name at all, and `blob_size` depends on
      `GameState.CarryFlag`, a side effect of the `NamePlanet` call that only
      happens when a name wins a free row (a faithful port of the original's
      quirk), so neither half survives being deferred to the view.
- [ ] [EliteSharpLib] `ShortRangeChartController.CrossBounds` derives both
      tiers' cross-hair clamps from `Scale` (`18 * scale + 1` and
      `Height - (16 * scale + 1)`), which reproduces the previous per-tier
      constants exactly at scale 1 and 2. If a tier ever wants clamps that
      are not a straight multiple of the 8-bit ones, this goes back to being
      per-tier data on the model.

### Release engineering (from the retired release plan)

(none open — see [CHANGELOG.md](../CHANGELOG.md) for completed items)


## Could

### From decisions (2026-07-27)

Committed by the maintainer decisions in [decisions.md](decisions.md);
not yet scoped into concrete steps.

- [ ] **[LARGE]** [EliteSharpLib] Decouple Elite's frame composition from
      the fixed 13.5Hz tick: compose frames at the configured `Fps`
      setting instead of only at 13.5Hz (not via interpolation). Requires
      auditing everything currently timed against the 13.5Hz tick
      (tactics/AI pacing in `Space.UpdateUniverse`, animations, etc.) and
      reworking it to stay correct at other `Fps` values. Scope the audit
      before starting.
- [ ] **[LARGE]** [EliteSharpLib] Data-driven game content model: replace
      hardcoded/reflection-based game data — `EquipmentType`, `StockType`,
      ship definitions, and `ShipFactory.CreateShipFromName`'s
      reflection-based construction (see the smaller interim cleanup
      below) — with a proper config-driven model. Design/scope the config
      shape before starting.

### Cleanups and small refactors

- [ ] [Repo] `sdl-drive/drive.ps1`'s `ConvertTo-VirtualKeyCode` has no
      entry for `Return`, so a `key:Return` step throws "Unknown key
      name". Hit while driving SCR's menus (2026-07-28); add Return and
      any other obvious missing keys.
- [ ] [Useful.Audio] `AudioController.PlayEffect`'s `_sfx[effectType]`
      is an unguarded dictionary indexer
      ([AudioController.cs:48](../src/useful/libs/Useful.Audio/AudioController.cs)):
      an effect name missing from the sample dictionary throws
      `KeyNotFoundException` rather than failing clearly or no-opping.
      Hit writing `EscapeCapsuleControllerTests` (2026-07-30), which had
      to populate `Launch` and `Explode` even though the test has
      nothing to do with sound, since `EffectsOn` defaults true and the
      lookup runs regardless of what the test wants to exercise.
      Consider `TryGetValue` plus a logged no-op, matching
      `LogMessages.FailedToCreateShip`'s pattern elsewhere in this file
      for a missing lookup.
- [ ] [EliteSharpLib] Remove conditional compilation (issue #7): two
      `#if` sites remain — the FPS overlay is now the
      `engine.graphics.showFps` config option rather than a `#if DEBUG`
      gate, leaving `SaveFile`'s `#if DEBUG` default commander
      (`CommanderFactory.Max()` vs `.Jameson()`,
      [SaveFile.cs:51-55](../src/elite/libs/EliteSharpLib/Save/SaveFile.cs)),
      and a fully commented-out `////#if QHD` resolution block in
      `SDLProgram`
      ([SDLProgram.cs:26-32](../src/elite/apps/EliteSharp/SDLProgram.cs))
      that's dead weight now the render-resolution-configurable item below
      exists. Replace the two live `#if DEBUG`s with a runtime
      env-var/config check (there's already an `ELITE_LOG_LEVEL`
      env-var precedent in `SDLProgram.cs`) and delete the dead QHD
      comment block outright.
3D pipeline sharing (split 2026-07-14 from the "unify the two 3D
pipelines" [LARGE] item; a code survey found the pipelines differ more
than assumed — Elite: float `Matrix4x4` transform, `vec.Z = 1` clamp
instead of a near clip, screen-winding cull; SCR: fixed-point
Amiga-trig view transform, true near-plane polygon clip — so full
unification is off the table; instead extract the stages that are
genuinely shareable, each independently. Both games now fill via the
shared z-buffer: the spike that moved Elite's filled ships off the
painter's chain landed 2026-07-14, see CHANGELOG):

- [ ] [Useful.Graphics] Move `Scene3D.ClipPolygonToNearPlane` into
      `Useful.Graphics`: both overloads are pure static methods with no
      SCR dependencies ([Scene3D.cs:40-84](../src/scr/libs/StuntCarRacerSharpLib/Rendering/Scene3D.cs));
      then evaluate adopting it in Elite's `TransformModelPoints`, whose
      `if (vec.Z <= 0) vec.Z = 1` clamp
      ([ShipBase.cs:125-128](../src/elite/libs/EliteSharpLib/Ships/ShipBase.cs))
      distorts any geometry crossing the camera plane instead of clipping
      it. Adoption changes close-range visuals — verify against The New
      Kind's behaviour before keeping it.
- [ ] [Useful.Graphics] Extract a shared perspective-projection helper
      (centre + focus·x/z): Elite now writes exactly that form, but
      inlines it at five sites — `ShipBase.ProjectPoint`,
      `EliteDraw.ProjectExplosionPoints`,
      `PlanetRenderer.GetPlanetPosition`, `SolidSun.Draw` and
      `GradientSun.Draw` (see CHANGELOG, 2026-07-29) — against SCR's
      `Scene3D.ProjectPoint`
      ([Scene3D.cs:116-125](../src/scr/libs/StuntCarRacerSharpLib/Rendering/Scene3D.cs));
      a small `focus`+`centre` projector type serves both. Note the
      radius conversions in the planet/sun sites (`* Focus / 256`) are
      Elite-specific and stay on the Elite side of the boundary, as does
      `Scale`, which is now coordinate/window magnification only.
- [ ] [Useful.Graphics] Shared text/HUD-panel helper for the two games'
      ad-hoc HUD code (Elite's `EliteDraw` header/border/text helpers,
      SCR's `HudRenderer`) — the smaller sibling of the original item;
      survey both HUDs first and only lift what both actually use (e.g.
      centred/left/right text layout in a panel rect).

### Stunt Car Racer conversion — features (from the retired conversion plan)

- [ ] [StuntCarRacerSharpLib] F9/F10 frame-gap tuning keys: both C++ versions adjust the physics frame gap live; `StuntCarRacerMain.FrameGap` exists for exactly this but isn't wired to any key.
- [ ] [StuntCarRacerSharpLib] Race pause: the remake pauses on 'P' and resumes on 'O' (`bPaused`, engine sound stopped while paused); not ported — no pause exists. The remake's debug freezes (F5 stats overlay, F6 player-only pause, F7 opponent-only pause) could ride along as dev aids.
- [ ] [StuntCarRacerSharpLib] 'R' turn-around key: the remake adds 180 degrees to the player's y angle and re-initialises (`INITIALISE_PLAYER`, ptitSeb `StuntCarRacer.cpp` ~1039) so a car facing the wrong way can recover; not ported.
- [ ] [StuntCarRacerSharpLib] Mid-race 'M' to track menu: the remake returns to the track menu from any mode on 'M' (`StuntCarRacer.cpp:1731-1741`: it also clears the opponent — `opponentsID = NO_OPPONENT` — and resets the drawbridge via `ResetDrawBridge`, and the menu mode stops the engine sound); the port only handles 'M' on the game-over and track-preview screens — `RaceScreen` has no way back to the menu short of Escape-quitting, and neither the preview's nor the game-over's 'M' resets the drawbridge today.
- [ ] [StuntCarRacerSharpLib] Player outside/chase view: needs a chase camera plus drawing the player's own car mesh (`Rendering/CarMesh` is currently only used for the opponent).
- [ ] [StuntCarRacerSharpLib] Road-line textures could sample the shared `atlas.bmp` (ptitSeb's `eRoadYellowDark` etc.) instead of the procedural strips in `Rendering/RoadTextures` — closer visual match, but the current strips already look correct; cosmetic.
Unplugged remake art screens (added 2026-07-19 after the ptitSeb parity
audit): ptitSeb ships `Bitmap/menu.png`, `racewin.png`, `racelost.png`,
`wrecked.png` and `heads.png` but its code never loads any of them
(upstream commit 7ad79f7, "More bitmaps, but not pluged in yet") — they
are the original Amiga screens, included as an unrealised intention.
The port already wired up menu.png (the track-menu frame art, commit
2a476b4, 2026-07-08); these items wire up the rest the same way:
one-time convert to `.bmp`, add to `Assets/Images` +
`AssetManifest.json`, draw scaled from the 320x200 canvas as
`TrackMenuScreen` does. Doing them means going beyond ptitSeb's code
while staying inside its assets and the Amiga's behaviour:

- [ ] [StuntCarRacerSharpLib] Race-result screens: draw `racewin.png` /
      `racelost.png` full-screen during the six-second result window and
      the GAME_OVER hold, with the existing flashing RACE WON / RACE
      LOST and "Press 'M'" text overlaid (timing at
      [StuntCarRacerMain.DrawHud](../src/scr/libs/StuntCarRacerSharpLib/StuntCarRacerMain.cs)
      and `StuntCarRacer.cpp:1403-1453`) — the Amiga showed these
      screens at race end.
- [ ] [StuntCarRacerSharpLib] Wrecked screen: draw `wrecked.png` when the
      race ends with the player's car wrecked, in place of the
      race-lost art. `CarPhysics.Wrecked` now goes true at full damage
      (landed 2026-07-20, see CHANGELOG) so this can be wired up.
- [ ] [StuntCarRacerSharpLib] Opponent portraits: `heads.png` is a portrait
      sheet of the eleven opponents; show the matching portrait
      alongside the four-second "Opponent: <name>" announcement at race
      start. Needs the per-portrait tile coordinates measured from the
      sheet first (no atlas table exists upstream); assume the sheet
      order matches `opponentNames` (`Opponent_Behaviour.cpp:138-151`)
      and verify against the Amiga before committing.
Gamepad/joystick support (split 2026-07-14 from the [LARGE] item; do in
order — each layer builds on the previous; reference implementation is
`XBOXController.cpp/h` in the local ptitSeb checkout,
`C:\code\github\ptitSeb\stuntcarremake`):

- [ ] [Useful.Controls] Define the controller abstraction: an `IGamepad`
      covering both target device classes — XInput-style pads (analog
      axes + buttons) and generic-HID digital joysticks (USB Competition
      Pro Extra: 8-way stick + fire buttons, no analog axes; digital
      devices report axes as -1/0/+1) — with the same
      pressed-vs-held semantics `IKeyboard` documents, a software
      implementation mirroring `SoftwareKeyboard`, and a fake in
      `Useful.Fakes`. Design the producer(sink)/consumer split up front —
      the `IKeyboard` interface-segregation item above is the same
      shape, so align (or do that item first).
- [ ] [Useful.SDL] SDL plumbing: initialise the joystick +
      game-controller subsystems, handle
      `SDL_JOYDEVICEADDED`/`REMOVED` hotplug, and translate both
      `SDL_CONTROLLER*` events (XInput-class devices) and raw
      `SDL_JOY*` events (generic HID sticks that have no controller
      mapping) in [SDLInput.cs](../src/useful/libs/Useful.SDL/SDLInput.cs)
      into the `IGamepad` sink. `ppy.SDL2-CS` exposes the full SDL2 API
      including `SDL_GameController`/`SDL_Joystick`, so no binding work
      is expected — verify first.
- [ ] [StuntCarRacerSharpLib] Wire the gamepad into SCR: map controls per
      ptitSeb's `XBOXController.cpp` (steer/accelerate/brake/boost in
      `RaceScreen`, navigation on the menu/preview/game-over screens),
      thresholding analog steer to the digital left/right the physics
      expects (check how the remake did it), keyboard remaining fully
      functional alongside. Smoke-test with real hardware (XInput pad
      and the Competition Pro) as part of definition of done.
Super League (split 2026-07-14 from the [LARGE] item; reference is
ptitSeb's `bSuperLeague` — toggled at `StuntCarRacer.cpp:1222`, applied
at `:1298-1308`; do the first item first, the two visual items then in
either order):

- [ ] [StuntCarRacerSharpLib] League toggle + physics constants: add an
      `IsSuperLeague` mode flag, toggle it with 'L' on `TrackMenuScreen`
      (show the current league on the menu as the remake does), and
      thread it to the physics: `CarPhysics._enginePower` 240→320 and
      the boost unit 16→12 (the "standard vs super" comments at
      [CarPhysics.cs:105-106](../src/scr/libs/StuntCarRacerSharpLib/Cars/CarPhysics.cs)
      already mark the spots), boost reserve Standard→Super,
      `RoadCushionValue` 0→1 (exists but nothing sets it — damage
      threshold at [CarPhysics.Motion.cs:286](../src/scr/libs/StuntCarRacerSharpLib/Cars/CarPhysics.Motion.cs)),
      `OpponentPhysics._enginePower` 236→314, and the opponent speed
      +32 super-league offsets: the `Opponent_Speed_Value()` port
      landed 2026-07-19, so `OpponentData.TrackSpeedValues` already
      carries the full 64-byte table with the super-league rows — the
      max-speed lookup in `OpponentPhysics.StartRace` and the
      mask/base lookups in `OpponentPhysics.SpeedValue` just need +32
      added when super league is active (the reference's path is
      `opp_track_speed_values[TrackID+32]`,
      `Opponent_Behaviour.cpp:366-367, 1299-1300`).
- [ ] [StuntCarRacerSharpLib] Super League track colours: odd/even road and
      side colours swap to `SCR_BASE_COLOUR+17/16` and `+18/+15`
      (standard: `+2/+10`, `+1/+15`; reference `Track.cpp:1360-1385`)
      — add the alternates to `ScrPalette`/`RoadTextures` and select by
      the mode flag in `TrackRenderer`. Note the reference's own
      `Track.cpp:2490` TODO says some super-league values are
      unverified; match ptitSeb, don't guess beyond it.
- [ ] [StuntCarRacerSharpLib] Super League car + cockpit visuals: opponent
      car body colours swap to `SCR_BASE_COLOUR+19/20/21` (standard
      `+9/+10/+12`, reference `Car.cpp:659-690`) in `CarMesh`, and the
      cockpit/damage overlays use the atlas's "2"-suffixed sprites
      (`eCockpitWL2`, `eCracking2`, `eHole2`, `Car.cpp:868-886`) —
      check how `HudRenderer`/`CockpitState` draw these today; if the
      port draws them procedurally rather than from `atlas.bmp` (see
      the road-line-textures cosmetic item below), this reduces to
      alternate colours.
Float physics conversion (split 2026-07-14 from the [LARGE] item; ~4,130
lines of 68000-style scaled-integer code — `CarPhysics` 2,733 lines over
four partials (incl. `.Chains`), `OpponentPhysics` 1,397 over two. Do
strictly in order:
the golden-trace harness is the safety net for everything after it.
Sequence AFTER the pending SCR correctness items (damage-wreck,
`Opponent_Speed_Value`) and the Super League physics item —
they edit the same files and their integer semantics should be captured
by the traces):

- [ ] [StuntCarRacerSharpLib.Tests] Golden-trace characterization harness:
      drive the existing integer physics with scripted `CarInput`
      sequences on two or three tracks, record the car/opponent state per
      physics tick (position, speeds, angles, damage, boost) to committed
      baseline files, and add a test that replays and compares. This is
      pure test code — no physics changes — and becomes the regression
      net for the conversion steps; the exact-integer unit-test
      assertions stay untouched until each class converts.
- [ ] [StuntCarRacerSharpLib] Convert angles and trig: replace
      `AmigaTrig`'s 16384-scaled short table and `TrigCoefficients` with
      float equivalents; decide the angle unit (keeping 0..65536 as a
      float unit is the least-churn option) and replace the
      `& (MaxAngle - 1)` bitmask wraps with a float wrap helper. Decide
      the rendering boundary here too: `Scene3D.TransformPoint` consumes
      `TrigCoefficients` and `Track.LogPrecision`
      ([Scene3D.cs:102-113](../src/scr/libs/StuntCarRacerSharpLib/Rendering/Scene3D.cs))
      — either convert its fixed-point view transform in the same step
      or leave it a shim that scales from the float trig. Issue #3
      ("System.Numerics for all matrix and vector maths") applies to SCR
      too: SCR's rendering layer (`Scene3D`, `TrackRenderer`,
      `BackdropRenderer`, `CarMesh`, `HudRenderer`) already uses
      `System.Numerics`/`Matrix4x4` like Elite does — `TrigCoefficients`
      and the fixed-point view transform here are the one remaining
      non-`System.Numerics` piece, and this item is where that gets
      resolved; no separate item needed.
- [ ] [StuntCarRacerSharpLib] Convert `CarPhysics` (four partials:
      [CarPhysics.cs](../src/scr/libs/StuntCarRacerSharpLib/Cars/CarPhysics.cs),
      `.Motion`, `.Road`, and the crane/chain-recovery `.Chains` — the
      last landed after the original three-partial estimate and also
      uses scaled `int` state, so it's in scope too) to float: fields,
      locals, and the `>> LogPrecision` rescales. Hunt the semantics
      traps — arithmetic right-shift on negatives rounds toward
      -infinity while integer division truncates toward zero, and any
      deliberate short/int
      overflow wrap needs an explicit equivalent. Validate against the
      golden traces with tolerances; rework `CarPhysicsTests`' exact
      assertions as tolerance-based in the same session.
- [ ] [StuntCarRacerSharpLib] Convert `OpponentPhysics` (+ `.Interaction`,
      `OpponentData`) the same way, including its `_random`-driven speed
      logic (seedable, so traces stay reproducible); rework
      `OpponentPhysicsTests`; then delete the now-unused integer trig
      (`AmigaTrig`, and `TrigCoefficients` if `Scene3D` was converted)
      and drop `Track.LogPrecision` from the physics path. Full
      definition of done: race a complete lap against the opponent on
      several tracks comparing feel against the integer build.
Resizable window / widescreen (split 2026-07-14 from the [LARGE] item;
the first item is a standalone quick win delivering issues.md's "make
window resizable" for both games; the rest build on each other toward
true widescreen. A 2026-07-14 survey found `HudRenderer` already scales
from a 640x480 virtual canvas via `ScreenWidth/BaseWidth` ratios, so SCR
is closer to resolution-independence than the original item assumed.
**The 2026-07-27 multi-resolution tier decision in
[decisions.md](decisions.md) expands the two widescreen items below —
re-scope them against the 8-bit/16-bit/modern tier scheme and the
per-game `AssetManager`/config changes it requires before starting
either.**):

- [ ] [Useful.SDL] Resizable window with letterboxed scaling: add
      `SDL_WINDOW_RESIZABLE` in [SDLWindow.cs:23-29](../src/useful/libs/Useful.SDL/SDLWindow.cs)
      and handle `SDL_EVENT_WINDOW_RESIZED` in the event loop, so the
      window can be any size while both games keep rendering at their
      native 512x512 / 640x400 — zero game-code changes. Most of this
      landed with `WindowScale` (2026-07-29, see CHANGELOG):
      `SDLRenderer.SetLogicalSize` already fixes the logical coordinate
      space, so what remains is the resizable flag, switching that call's
      mode from `INTEGER_SCALE` to `LETTERBOX` when the window is free to
      be any size, and deciding how the two settings interact.
- [ ] [Apps] Decide whether the render resolution needs to be
      configurable at all. Elite no longer hardcodes it: it derives
      from the tier in `SDLProgram.ResolutionFor`, which is the shape
      the tier decision wanted (the asset set and the resolution can
      never disagree). What is left is the question this item was
      really asking — whether a launch resolution/aspect should also be
      settable *independently* of the tier, in
      `EngineConfigSettings` next to `Tier`. If yes, note it exposes
      rather than fixes fixed-size assumptions, so it must follow the
      non-512x512 audit below. SCR still hardcodes its own consts
      either way.
- [ ] [StuntCarRacerSharpLib] SCR widescreen: with resolution configurable,
      make the 3D viewport render at the window aspect (SCR's
      `Scene3D.SetView` already takes width/height) and apply ptitSeb's
      cockpit widescreen treatment (`GetScreenDimensions`,
      `COCKPIT_WIDESCREEN_OFFSET` — side panels pushed out, HUD
      centred); audit `TrackMenuScreen` and the other 2D screens for
      640x400 assumptions. `HudRenderer`'s virtual-canvas scaling
      mostly survives as-is.
- [ ] [EliteSharpLib] Elite at non-512x512 resolutions: audit and fix
      the hardcoded coordinate-space assumptions so Elite renders
      correctly at other resolutions. **[LARGE]** — the maintainer
      decided (see [decisions.md](decisions.md)) Elite should support
      the full 8-bit/16-bit/modern resolution-tier scheme, not just
      integer-scaled 512x512; re-scope this item against that decision
      before starting.

      **The 16-bit tier is now 640x512** (landed 2026-07-30). Height is
      unchanged, so everything here is width-only. Three of the four
      problems the 2026-07-29 trial exposed are resolved:
      - ~~**The HUD stops spanning the window.**~~ Resolved: the 16-bit
        `scanner.bmp` is 640 wide. `Scanner16Bit`'s right-hand cluster
        moved +128 to match — see the CHANGELOG for how that offset was
        measured off the art rather than guessed.
      - ~~**The 3D field of view changes.**~~ Resolved on master
        2026-07-29: `Focus` now follows `ScreenHeight`, so the vertical
        field of view is constant and widening shows more to the left
        and right instead of magnifying everything. See the decision in
        [decisions.md](decisions.md), which supersedes the width-derived
        `Focus` of the 2026-07-28 tier decision.
      - ~~**The stale comment** in `SDLProgram.cs`~~ — updated with the
        change.
      - **Screens using bare absolute coordinates drift out of
        alignment** with those laid out from the viewport, which stay
        centred. **This is what is left of this item.** (`Offset` is
        gone — it duplicated `ScannerLeft`, and both were removed when
        `ViewLayout` became viewport-only; screens now lay out from
        `ViewportLeft`, which is the screen origin.)
        `ThargoidMissionView16Bit` (`new(116, 132)`, the Blake portrait
        at `new(352, 46)`), `ConstrictorMissionView16Bit`,
        `PlanetDataView16Bit`, `MarketView16Bit` and the commander
        save/load screens are the known cases — and their 8-bit
        counterparts have the same shape at 320. None have been checked
        live at 640 yet. This is a latent bug at any width except the
        tier's own, independent of the tier scheme.

      Two specifics this item used to cite are now out of date: `511`
      in `ShipBase.DrawLasers` is already fixed — `ProjectToViewBoundary`
      clips to the real viewport — and the `512` literals still in the
      lib (`Combat.cs:758-759`, `ScannerBase`, `Space.cs:408-414`)
      are world-space and physics constants, not screen coordinates, so
      they are not in scope.
- [ ] [EliteSharpLib] Number of stars proportional to screen size (issue
      #4): now reachable — the 8-bit tier already renders at 320x256,
      a quarter the area of 512x512, with the same star count, and the
      planned 640x512 widening changes it again. Scale star count by
      screen area; sequence after the non-512x512 audit above so the
      coordinate space it scales against is settled.
- [ ] [Repo] **Low-priority spike**: WASM build for Playwright-driven
      visual testing. Today `run-elite`/`run-scr`
      ([sdl-drive/drive.ps1](../.claude/skills/sdl-drive/drive.ps1))
      drive the native SDL window via Win32 `PostMessage`/
      `CopyFromScreen`, since Playwright can't see a raw SDL window
      (no DOM, no meaningful UI Automation tree — same reason
      WinAppDriver wouldn't help either). `Useful.Graphics`/
      `Useful.Audio`'s Software backends and both game libs are
      already pure managed C# with no SDL dependency, so a
      browser-hosted build (new `Useful.Wasm` + per-game `*.Wasm` app
      targeting `browser-wasm`) could turn the app into an actual web
      page and let Playwright drive it for real: `page.goto()`,
      `page.keyboard.press()`, `page.screenshot()`, headless, no real
      desktop session needed. Needs: a DOM keyboard adapter into the
      existing `IKeyboardSink` seam (`SoftwareKeyboard` already
      consumes exactly this abstraction), a per-tick canvas blit of
      the software framebuffer (`putImageData`), and asset loading
      swapped from disk paths to HTTP fetch or embedded resources.
      Only exercises the `Software` graphics backend, not
      `Hardware`/SDL — would complement rather than replace the
      native driver. Spike/prototype only; not a commitment to
      shipping a browser build.

## Won't

- [ ] [Repo] Remaining code-complexity rules from issue #5 (closed
      2026-07-27, see [CHANGELOG.md](../CHANGELOG.md)): `S1541`/`S3776`
      (method/cognitive complexity, 102 sites) and `S107` (parameter
      count, 20 sites) stay `severity = none` — the survivors are mostly
      ported 6502/Amiga reference methods that are inherently long, and
      `CA1502`/`CA1506` already cover the same ground as an enforced
      gate. `S109` (magic numbers, 4087 sites) likewise stays off:
      hardcoded constants (`* 256 / vec.Z`, etc.) are endemic to the
      ported algorithms. Revisit only if a specific project is scoped for
      it.
- [ ] [EliteSharpLib] Buying more than 255g of Gold/Platinum doesn't work — authentic to the original ("broken as designed"); documented, not fixed.
- [ ] [EliteSharpLib] Elite Intro2 parade shows 29 of ~33 ship models ([ShipFactory.cs:80-111](../src/elite/libs/EliteSharpLib/Ships/ShipFactory.cs)) — Cougar, Constrictor and the Lone variants are mission-specific ships, deliberately excluded from the parade; confirmed intentional, not a bug.
- [ ] [Useful.Graphics] Software rasterizer throughput (per-pixel `SetPixel`, insertion-sorted painter chain of ≤100 polys, no spans/SIMD) — the game is fixed at 13.5fps by design and none of this is a bottleneck at that rate; revisit only if the "performance as secondary objective" goal is picked up.
- [ ] [StuntCarRacerSharpLib] The original remake's Windows-only infrastructure (DXUT registry prefs, clipboard, DirectSound path, `MessageBox` dialogs) is deliberately not ported — see the porting notes in [scr-readme.md](scr-readme.md).
- [ ] [StuntCarRacerSharpLib] ptitSeb's remaining debug/infrastructure toggles are deliberately not ported (2026-07-19 parity audit): F1 test key, F2 triangle-list/strip vertex-buffer toggle (meaningless in the software rasterizer), the 'Z' reposition test key, the disabled action-replay / Amiga-recording harness (`#ifdef NOT_USED` / `USE_AMIGA_RECORDING` even upstream), the French-keyboard digit remaps, and the SDL command-line video flags (superseded by the resizable-window/config-resolution items under Could). The F5 stats overlay and F6/F7 per-car freezes stay listed as optional ride-alongs on the race-pause item. `Chime.wav` is unused by both code bases (kept as an asset only).
