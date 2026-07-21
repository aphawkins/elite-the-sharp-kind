# Backlog and Roadmap — The Sharp Kind

The single consolidated backlog for the repository, prioritised with MoSCoW
(per [architecture.md](architecture.md)). It merges the 2026-07-11
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

## Decisions needed

Maintainer decisions, not code tasks — each blocks or reshapes items below.

- [ ] **Elite per-object draw lists**: Elite composes the frame inside
      `Update` at 13.5Hz (authentic to The New Kind), so raising `Fps`
      draws nothing new above the tick rate and tactics stay interleaved
      with rendering (`Space.UpdateUniverse`). Moving to draw lists so
      `Draw` can render interpolated frames is a big, Elite-only refactor
      that trades away period-accurate feel for smoother rendering; needs a
      research spike/prototype before committing either way — not resolved.

### Resolved (2026-07-19) — ptitSeb parity audit

A full feature-by-feature comparison of `C:\code\github\ptitSeb\stuntcarremake`
(the definitive conversion source) against `StuntCarRacerLib` was run on
2026-07-19, prompted by concerns that earlier analysis had missed
functionality. Conclusions:

- **Verified ported and faithful**: the four-mode screen flow, orbiting
  menu camera + menu.png art with named tracks, preview camera (incl.
  the Draw Bridge high viewpoint), race/game-over flow with the
  six-second flashing RACE WON/LOST then GAME OVER text, opponent-name
  announcement, the full atlas cockpit (wheels/bounce/spin frames,
  engine + boost-flame animation, crack + smash holes, speed bar with
  over-max colour, lap/boost/distance read-outs), backdrop horizon +
  all five scenery types, track rendering with road lines around the
  player, opponent shadow, drawbridge animation incl. its opponent
  speed tables, opponent AI (random selection, names, attribute flags,
  wheelies, steering randomisation, obstruction/push interaction,
  car-to-car collision), lap/damage/boost/engine-revs models, the
  pitch-shifted engine loop, and effect-sound triggers.
- **Chains**: neither C++ reference implements them (`on_chains` is
  hardcoded FALSE, "won't implement chains at first"); ptitSeb instead
  drops the car above its current piece after 64 off-track frames. The
  port's crane chain-recovery (commit 7039a11) deliberately goes beyond
  ptitSeb toward the Amiga original — keep it.
- **racewin/racelost/wrecked/heads/menu bitmaps**: present in ptitSeb's
  `Bitmap/` but never loaded by its code (upstream commit 7ad79f7 "More
  bitmaps, but not pluged in yet"). menu.png is already wired up in the
  port; the rest are now discrete items under Could.
- **Remaining genuine gaps** are all tracked as items below: Super
  League, pause, 'R' turn-around, mid-race 'M', F9/F10, gamepad,
  outside view, art screens, widescreen/resolution work.
  (`Opponent_Speed_Value` was ported 2026-07-19; wreck-at-full-damage,
  the cockpit wheel-spin rate fix, a (simplified) lap-time clock, and
  per-effect sound volume/pitch/pan all landed 2026-07-20, see
  CHANGELOG.)

### Resolved (2026-07-11)

- **v1 release scope**: Elite + SCR, with SCR labelled preview given its
  open defects list (see Release engineering below).
- **First tag**: `v1.0.0`.
- **Claimed platforms**: win-x64, linux-x64, linux-arm64. macOS stays
  unclaimed (untested).
- **Coverage visibility**: add a badge (see Could below).
- **NuGet packaging of `Useful.*`**: defer until an external consumer
  exists.
- **Elite Intro2 parade**: keep mission ships (Cougar, Constrictor, Lone
  variants) out of the parade — status quo confirmed intentional, not a bug
  (see Won't below).

## Must

(none open — see [CHANGELOG.md](../CHANGELOG.md) for completed items)

## Should

### Architecture (business-application practices — see architecture.md)

Library logging (split 2026-07-14 from the original [LARGE] item; a
2026-07-14 survey found every operational `Debug.*` call lives in
`EliteSharpLib` — the SCR libraries have none and `Useful.*` only the
stray `DrawCircleFilled` WriteLine covered under Could — so scope is
EliteSharpLib plus the wiring pattern; do the first item first, the rest
in any order):

- [ ] Note: `SaveFile`'s four `Debug.*` calls and `EliteMain.Update`'s
      catch-all `Debug.WriteLine` are NOT separate work — they convert as
      part of their existing defect items below (`LoadCommander`
      validation, frame-path catch-all), which rewrite those exact lines;
      those items should use the logger this infrastructure provides.
- [ ] [Useful.Controls] Split `IKeyboard` (interface segregation): it mixes
      the producer API (`KeyDown`/`KeyUp`, called by `SDLInput`) with the
      consumer API (`IsPressed`/`IsHeld`/`LastPressed`, called by games)
      ([IKeyboard.cs](../src/useful/libs/Useful.Controls/IKeyboard.cs)); split into a
      sink interface and a state interface so game code can't inject key
      events.
- [ ] [Useful] Remove two-phase construction: `SDLSound` must have
      `Initialize(assetLocator)` called after construction (temporal
      coupling — a forgotten call is a runtime `KeyNotFoundException`), and
      `SoftwareGraphics.Create` populates its `Fonts`/`Images` via an
      object initializer onto `internal`-settable properties
      ([SoftwareGraphics.cs:35-37](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)),
      leaving them mutable by any other code in the assembly after
      construction rather than truly immutable; fold initialisation into
      constructors/factory parameters so no instance is observable
      half-built. (`ShipFactory.Create` sets a `private` field the same
      way, but since nothing outside the class can reach it afterwards
      this isn't the same risk — lower priority than the other two.)
- [ ] [EliteSharpLib] Replace the static crypto RNG: `RNG.Random` delegates
      every call to `RandomNumberGenerator.GetInt32`
      ([RNG.cs:98](../src/elite/libs/EliteSharpLib/RNG.cs)) — orders of magnitude
      slower than needed in hot per-tick paths — and `RNG`/`RNG.Seed` is
      static mutable state that makes Elite's logic untestable; make it an
      injected, seedable random service (the pattern `CarPhysics._random`
      already uses in SCR). Make Random a singleton in the DI container so
      the two games share a single source of entropy, it can be unit tested
      and seeded for reproducible test runs, and the per-tick RNG calls are fast.

### Defects and gaps

- [ ] [Useful.Graphics] `DrawTextCentre`/`DrawTextLeft`/`DrawTextRight` wrap the bitmap from `GenerateTextBitmap` in `using` ([SoftwareGraphics.cs:352-384](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)), but that bitmap is stored in `_textCache` and returned again on the next call — cached bitmaps are disposed (GC pin freed) while still cached, working today only because text drawing never touches `BitmapHandle`; remove the `using`s and dispose cached bitmaps only when the cache is cleared/disposed.
- [ ] [Useful.Graphics] `_textCache` grows without bound ([SoftwareGraphics.cs:13, 852-909](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)) — every distinct (font, colour, text) string caches a ~KB-to-tens-of-KB bitmap forever, and Elite renders ever-changing strings ("1234.5 Credits" per bounty, countdowns), so long sessions leak memory steadily; add an eviction policy (e.g. cap entry count) or key frequently-changing text out of the cache.
- [ ] [Useful.Graphics] `SoftwareGraphics.SetClipRegion` is an empty no-op ([SoftwareGraphics.cs:428-430](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)) while Elite actively relies on clip regions (`EliteDraw.SetViewClipRegion`) — in the primary (software) renderer, view drawing can overwrite the border/scanner area; implement rectangular clipping in the software rasterizer (all draw calls already funnel through `DrawPixel`/`SetPixel`) or remove it from `IGraphics` and clip in game code (an interface member the primary implementation can't honour violates the architecture doc's interface rules).
- [ ] [EliteSharpLib] `EliteMain.Update` wraps `UpdateConsole`/`HandleInput` in `catch (Exception) { Debug.WriteLine(...) }` ([EliteMain.cs:259-267](../src/elite/libs/EliteSharpLib/EliteMain.cs)) — in Release builds every input/console error is silently swallowed each tick; remove the catch-all (or catch the specific expected exception type and surface it via the logger), per the architecture doc's "never catch-all on the frame path".
- [ ] [EliteSharpLib] `SaveFile.LoadCommander` catches, resets to Jameson, then `throw;`s ([SaveFile.cs:70-75](../src/elite/libs/EliteSharpLib/Save/SaveFile.cs)), contradicting its bool-return contract, and `SaveStateToGameState` indexes `GalaxySeed[0..5]`, `CurrentCargo[i]`, `Lasers[0..3]` and `Enum.Parse`s strings without validation ([SaveFile.cs:167-208](../src/elite/libs/EliteSharpLib/Save/SaveFile.cs)) — a truncated or hand-edited `.cmdr` file throws instead of showing the view's "Error Loading Commander!" path; validate the deserialized `SaveState` and return false on any failure.
- [ ] [EliteSharpLib] `SaveFile.SaveCommander` builds the path as `save.CommanderName + ".cmdr"` from raw user input ([SaveFile.cs:80-92](../src/elite/libs/EliteSharpLib/Save/SaveFile.cs)) — invalid filename characters throw and path separators escape the save directory; sanitize the name (e.g. `Path.GetInvalidFileNameChars`) before using it as a filename (pairs with the user-data-location item above).
- [ ] [EliteSharpLib] `EliteDraw.DrawTextPretty` decrements `i` looking for a space/comma/period with no lower bound ([EliteDraw.cs:148-169](../src/elite/libs/EliteSharpLib/Graphics/EliteDraw.cs)) — a word longer than the line width underflows the index and throws; bound the scan at `previous` and hard-break long words.

### Tests

- [ ] [Tests] Elite's core game logic is largely untested — `EliteSharpLib.Tests` covers planets/suns/ships/universe plus the new `TradeTests`, leaving `Combat`, `Space`, `SaveFile` round-trip, `ConfigFile`, `PlanetController` and `RNG.GenerateRandomNumber` untested (the contraband bug lived here for years); start with the pure-logic classes: `PlanetController`, `SaveFile` save/load round-trip against an injected temp directory.
- [ ] [Tests] Add an `EliteMain` construction/smoke test using fakes, mirroring SCR's `StuntCarRacerMainTests` — Elite currently has no test that even constructs its composition; `EliteSharpLib.Fakes` (`FakeEliteDraw`, `FakeShipFactory`) plus `Useful.Fakes` already provide most of the doubles.
Headless smoke harness (added 2026-07-19: verifying the
`Opponent_Speed_Value` port live meant driving the SDL window with
OS-level focus stealing, key injection and full-screen screenshots —
slow, flaky, and expensive for agent-driven sessions. The repo already
has every piece for headless verification; these items assemble them
so "confirm the change works in the running game" becomes one test run
plus reading a native-resolution BMP or a text state dump, no window
needed. Do the first item first; the golden-trace harness under the
float-physics cluster shares its scripted-input machinery):

- [ ] [StuntCarRacerLib.Tests] Reusable headless game harness:
      `StuntCarRacerMainTests.StartRace` already drives menu→race
      through `FakeAbstraction`/`FakeKeyboard`
      ([StuntCarRacerMainTests.cs:100-115](../src/scr/test/StuntCarRacerLib.Tests/StuntCarRacerMainTests.cs)),
      and `FakeAbstraction` accepts any `IGraphics`
      ([FakeAbstraction.cs:15](../src/scr/test/StuntCarRacerLib.Fakes/FakeAbstraction.cs))
      — combine them: a harness that runs the real `StuntCarRacerMain`
      against a real `SoftwareGraphics`, executes a scripted key
      timeline ("S at tick 2, hold accelerate from tick 10"), and can
      dump the framebuffer to BMP at chosen ticks (lift
      `VisualDumpTests`' private `SaveBmp` into a shared test helper —
      unlike `VisualDumpTests`' hand-composed scene, this renders the
      whole game incl. screens and HUD). Prefer text assertions over
      pixels where possible: expose a minimal read-only state summary
      (current screen, race started, player/opponent piece, distance)
      so most checks never need an image at all.
- [ ] [EliteSharpLib.Tests] The Elite equivalent, building on the
      `EliteMain` construction/smoke-test item above: drive scripted
      ticks through `EliteMain.Update`/`Draw` with a real
      `SoftwareGraphics` and dump framebuffers the same way. Needs no
      SDL; note `EliteMain.Run` is unusable headlessly as-is — it hands
      off to `GameHost.Run`'s real-time, wall-clock-waiting loop
      ([GameLoop.cs:59-98](../src/useful/libs/Useful/Timing/GameLoop.cs))
      that only exits via `abstraction.Keyboard.Close` — call `Update`/
      `Draw` directly per tick instead.
- [ ] [Apps] Scripted input + frame dump in the real SDL apps, for the
      rare check that must exercise the true SDL window/present path:
      replay a key script (from a file or env var) into the `IKeyboard`
      sink inside the app — no OS focus or synthetic input needed —
      and add a debug key (e.g. F12) plus a script command that saves
      the current software framebuffer as a BMP next to the logs, so
      live verification reads a native 640x400/512x512 image instead
      of a desktop screenshot. Gate behind an environment variable;
      keyboard-sink injection gets cleaner after the `IKeyboard`
      producer/consumer split item above.

### Release engineering (from the retired release plan)

- [ ] [Release] Switch versioning from CI's date+run-number stamp to tag-driven semantic versioning (e.g. [MinVer](https://github.com/adamralph/minver)) so tagging a commit *is* the release-versioning step; do this before the first `v1.0.0` tag.
- [ ] [Release] Add a tag-triggered CI job that publishes win-x64/linux-x64/linux-arm64 self-contained builds for both Elite and StuntCarRacer (add a publish profile + CI publish step for `StuntCarRacer.csproj` mirroring Elite's) and creates a GitHub Release with the zips attached (`softprops/action-gh-release` or `gh release create --generate-notes`); ship as zip/tar.gz, not an installer. Label the SCR artifacts as preview in the release notes given its open defects list below.


## Could

### Cleanups and small refactors

- [ ] [Useful] Replace the custom `Guard.ArgumentNull` (47 call sites) with the framework's `ArgumentNullException.ThrowIfNull` and delete [Guard.cs](../src/useful/libs/Useful/Guard.cs)/`ValidatedNotNullAttribute` — the architecture doc's own "prefer dotnet framework intrinsics" rule.
- [ ] [Repo] Adopt central package management: add `Directory.Packages.props` (all 26 csproj files currently pin `PackageReference` versions independently and are already drifting) and move the copy-pasted analyzer `PackageReference` block from every csproj into `Directory.Build.props`/`.targets`.
- [ ] [Repo] Add a test coverage badge to README.md sourced from the CI-collected coverage numbers (decision: visibility only, no gate/target).
- [ ] [Useful.Abstraction] `ScreenManager` starts with `CurrentId = default!` and a nullable `Current`, forcing `Screens.Current!.Update()` at every call site ([ScreenManager.cs:29-31](../src/useful/libs/Useful.Abstraction/ScreenManager.cs)); require the initial screen at construction (or an explicit `Start(id)`) so `Current` is never null after setup.
- [ ] [Useful.Graphics] `FastBitmap` pins every bitmap with a raw `GCHandle` + finalizer ([FastBitmap.cs:19-35](../src/useful/libs/Useful.Graphics/FastBitmap.cs)); only the screen bitmap ever crosses into SDL — pin only on demand (or wrap the handle in a `SafeHandle`-style type) so short-lived text/texture bitmaps don't fragment the GC heap.
- [ ] [Apps] Fix the rename leftover: `EliteSharp`'s [LogMessages.cs](../src/elite/apps/EliteSharp/LogMessages.cs) still sits in namespace `EliteSharp.SDL` though the project is `EliteSharp` (`SDLProgram.cs` was already fixed to plain `EliteSharp`); and remove the committed benchmark reports under `src/*/perf/**/reports/` (generated artifacts, per the architecture doc's hygiene rule). How to record and monitor the historical benchmark numbers is a separate decision (e.g. a GitHub Action that runs the benchmarks and posts the results to a PR comment), what is best practice?
- [ ] [EliteSharpLib] Give the bare `throw new EliteException()` calls in the factory `switch` defaults ([ShipFactory.cs:46,59,68,77](../src/elite/libs/EliteSharpLib/Ships/ShipFactory.cs)) a message with the offending value — an empty exception is undiagnosable in a log.
- [ ] [Useful.SDL] Document and harden the `SDLSound` loop-pitch threading contract: the `Mix_RegisterEffect` callback runs on the audio thread while the game thread writes the pitch; verify the field is read/written with `Volatile` (or is inherently safe) and state the contract in a comment.
- [ ] [Useful.Graphics] Remove the `Debug.WriteLine($"{x},{y}")` left in the `DrawCircleFilled` scanline loop ([SoftwareGraphics.cs:122](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)) — debug-build log spam for every filled circle (planets, suns) every frame.
- [ ] [Useful] Remove `Vector4.Cloner()` ([Maths/Extensions.cs:13](../src/useful/libs/Useful/Maths/Extensions.cs)) — `Vector4` is a struct, so plain assignment already copies; its four call sites can just assign.
- [ ] [EliteSharpLib] Clean `EliteSharpLib.csproj`: fourteen `sfx\*.wav` items point at a folder that doesn't exist and the `<Compile Remove="Controls\**" />` block excludes a folder that no longer exists ([EliteSharpLib.csproj](../src/elite/libs/EliteSharpLib/EliteSharpLib.csproj)); the ~280-line hand-maintained asset item list could also become two glob items. (The unused NAudio refs are covered by the Must item.)
- [ ] [Repo] Delete stale build-output-only directories left behind by project renames: `src/elite/apps/EliteSharp.SDL/`, `src/elite/libs/EliteSharp/`, `src/elite/test/EliteSharp.Tests/`, `src/elite/perf/EliteSharp.Benchmarks/` contain only `bin`/`obj` and are referenced by no project or solution entry.
- [ ] [EliteSharpLib] `Stars` repeats the same star plot/respawn block three times (`FrontStarfield`, `RearStarfield`, `SideStarfield`, [Stars.cs:61-133, 151-243, 258-333](../src/elite/libs/EliteSharpLib/Stars.cs)); extract the shared plot helper to cut ~80 duplicated lines.
- [ ] [Useful.Graphics] The hardcoded `Scale { get; } = 2` on `IGraphics` and the centring math that divides by it (`DrawRectangleCentre` = `(ScreenWidth - width) / Scale`, [SoftwareGraphics.cs:29, 341-342](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)) only centres correctly because Scale happens to equal 2, and `SDLGraphics` divides positions by `(2 / Scale)` in ten places, which is a no-op ([SDLGraphics.cs:251-252,274-275,298,321-322,345-346,398-399](../src/useful/libs/Useful.SDL/SDLGraphics.cs)) — Elite-specific scaling leaking into the shared library; make centring `(ScreenWidth - width) / 2` and move scale policy to the game.
- [ ] [Useful.SDL] `SDLGraphics.DrawImage`/`DrawImagePart` create and destroy an `SDL_Texture` from the surface on every call ([SDLGraphics.cs:90-111,125-154](../src/useful/libs/Useful.SDL/SDLGraphics.cs)), and `SoftwareAbstraction.SoftwareScreenUpdate` creates a surface + texture per presented frame ([SoftwareAbstraction.cs:69-110](../src/useful/libs/Useful.SDL/SoftwareAbstraction.cs)); cache textures per image, and use one streaming texture + `SDL_UpdateTexture` for the framebuffer blit.
- [ ] [EliteSharpLib] `ShipFactory.CreateShipFromName` instantiates ship types via reflection from strings in `AssetManifest.json` ([ShipFactory.cs:114-131](../src/elite/libs/EliteSharpLib/Ships/ShipFactory.cs), flagged by its own TODOs) — data-driven `Type.GetType` + non-public `Activator.CreateInstance` is fragile and a mild input-handling risk; replace with an explicit name→factory dictionary.
- [ ] [EliteSharpLib] `ShipBase.Draw` allocates a `new Vector4[100]` per ship per tick and keeps a discarded `_ = VectorMaths.UnitVector(...)` call ([ShipBase.cs:100-115](../src/elite/libs/EliteSharpLib/Ships/ShipBase.cs)); reuse a pooled/instance buffer sized to the model and delete the dead call (same pattern for `EliteDraw._pointList`'s magic `100` vs the `MAXPOLYS` constant, [EliteDraw.cs:19-25](../src/elite/libs/EliteSharpLib/Graphics/EliteDraw.cs)).
- [ ] [EliteSharpLib] `Universe.RemoveShip` only removes from `_objects`, silently leaving `Planet`/`StationOrSun` set if passed one of those ([Universe.cs:127-135](../src/elite/libs/EliteSharpLib/Universe.cs)) — station removal currently works only because a sun immediately overwrites `StationOrSun` in `Combat.RemoveShip`; clear the matching reference explicitly.
- [ ] [Useful.SDL] `SDLGraphics.LoadFont` hardcodes the game-specific font names "Small"/"Large" and their point sizes into the shared library ([SDLGraphics.cs:407-420](../src/useful/libs/Useful.SDL/SDLGraphics.cs)); carry the size in the asset manifest instead.
- [ ] [EliteSharpLib] Wireframe planet is just a circle ([WireframePlanet.cs:50-57](../src/elite/libs/EliteSharpLib/Planets/WireframePlanet.cs)); add the two arcs and crater the original Elite drew (from issues.md).
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
      SCR dependencies ([Scene3D.cs:40-84](../src/scr/libs/StuntCarRacerLib/Rendering/Scene3D.cs));
      then evaluate adopting it in Elite's `TransformModelPoints`, whose
      `if (vec.Z <= 0) vec.Z = 1` clamp
      ([ShipBase.cs:125-128](../src/elite/libs/EliteSharpLib/Ships/ShipBase.cs))
      distorts any geometry crossing the camera plane instead of clipping
      it. Adoption changes close-range visuals — verify against The New
      Kind's behaviour before keeping it.
- [ ] [Useful.Graphics] Extract a shared perspective-projection helper
      (centre + focus·x/z): Elite inlines it twice with magic numbers
      (`* 256 / vec.Z + Centre/2, * Scale` in `TransformModelPoints`
      ([ShipBase.cs:130-131](../src/elite/libs/EliteSharpLib/Ships/ShipBase.cs))
      and again, without the `* Scale`, in `EliteDraw.DrawExplosion`
      ([EliteDraw.cs:319-320](../src/elite/libs/EliteSharpLib/Graphics/EliteDraw.cs));
      `DrawLasers` only consumes an already-projected point, it doesn't
      inline the projection itself) and SCR has `Scene3D.ProjectPoint`
      ([Scene3D.cs:116-125](../src/scr/libs/StuntCarRacerLib/Rendering/Scene3D.cs));
      a small `focus`+`centre` projector type serves both. Do together
      with (or after) the Scale-policy cleanup above so Elite's `* Scale`
      doesn't leak into the shared type.
- [ ] [Useful.Graphics] Shared text/HUD-panel helper for the two games'
      ad-hoc HUD code (Elite's `EliteDraw` header/border/text helpers,
      SCR's `HudRenderer`) — the smaller sibling of the original item;
      survey both HUDs first and only lift what both actually use (e.g.
      centred/left/right text layout in a panel rect).

### Stunt Car Racer conversion — features (from the retired conversion plan)

- [ ] [StuntCarRacerLib] F9/F10 frame-gap tuning keys: both C++ versions adjust the physics frame gap live; `StuntCarRacerMain.FrameGap` exists for exactly this but isn't wired to any key.
- [ ] [StuntCarRacerLib] Race pause: the remake pauses on 'P' and resumes on 'O' (`bPaused`, engine sound stopped while paused); not ported — no pause exists. The remake's debug freezes (F5 stats overlay, F6 player-only pause, F7 opponent-only pause) could ride along as dev aids.
- [ ] [StuntCarRacerLib] 'R' turn-around key: the remake adds 180 degrees to the player's y angle and re-initialises (`INITIALISE_PLAYER`, ptitSeb `StuntCarRacer.cpp` ~1039) so a car facing the wrong way can recover; not ported.
- [ ] [StuntCarRacerLib] Mid-race 'M' to track menu: the remake returns to the track menu from any mode on 'M' (`StuntCarRacer.cpp:1731-1741`: it also clears the opponent — `opponentsID = NO_OPPONENT` — and resets the drawbridge via `ResetDrawBridge`, and the menu mode stops the engine sound); the port only handles 'M' on the game-over and track-preview screens — `RaceScreen` has no way back to the menu short of Escape-quitting, and neither the preview's nor the game-over's 'M' resets the drawbridge today.
- [ ] [StuntCarRacerLib] Player outside/chase view: needs a chase camera plus drawing the player's own car mesh (`Rendering/CarMesh` is currently only used for the opponent).
- [ ] [StuntCarRacerLib] Road-line textures could sample the shared `atlas.bmp` (ptitSeb's `eRoadYellowDark` etc.) instead of the procedural strips in `Rendering/RoadTextures` — closer visual match, but the current strips already look correct; cosmetic.
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

- [ ] [StuntCarRacerLib] Race-result screens: draw `racewin.png` /
      `racelost.png` full-screen during the six-second result window and
      the GAME_OVER hold, with the existing flashing RACE WON / RACE
      LOST and "Press 'M'" text overlaid (timing at
      [StuntCarRacerMain.DrawHud](../src/scr/libs/StuntCarRacerLib/StuntCarRacerMain.cs)
      and `StuntCarRacer.cpp:1403-1453`) — the Amiga showed these
      screens at race end.
- [ ] [StuntCarRacerLib] Wrecked screen: draw `wrecked.png` when the
      race ends with the player's car wrecked, in place of the
      race-lost art. `CarPhysics.Wrecked` now goes true at full damage
      (landed 2026-07-20, see CHANGELOG) so this can be wired up.
- [ ] [StuntCarRacerLib] Opponent portraits: `heads.png` is a portrait
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
- [ ] [StuntCarRacerLib] Wire the gamepad into SCR: map controls per
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

- [ ] [StuntCarRacerLib] League toggle + physics constants: add an
      `IsSuperLeague` mode flag, toggle it with 'L' on `TrackMenuScreen`
      (show the current league on the menu as the remake does), and
      thread it to the physics: `CarPhysics._enginePower` 240→320 and
      the boost unit 16→12 (the "standard vs super" comments at
      [CarPhysics.cs:105-106](../src/scr/libs/StuntCarRacerLib/Cars/CarPhysics.cs)
      already mark the spots), boost reserve Standard→Super,
      `RoadCushionValue` 0→1 (exists but nothing sets it — damage
      threshold at [CarPhysics.Motion.cs:286](../src/scr/libs/StuntCarRacerLib/Cars/CarPhysics.Motion.cs)),
      `OpponentPhysics._enginePower` 236→314, and the opponent speed
      +32 super-league offsets: the `Opponent_Speed_Value()` port
      landed 2026-07-19, so `OpponentData.TrackSpeedValues` already
      carries the full 64-byte table with the super-league rows — the
      max-speed lookup in `OpponentPhysics.StartRace` and the
      mask/base lookups in `OpponentPhysics.SpeedValue` just need +32
      added when super league is active (the reference's path is
      `opp_track_speed_values[TrackID+32]`,
      `Opponent_Behaviour.cpp:366-367, 1299-1300`).
- [ ] [StuntCarRacerLib] Super League track colours: odd/even road and
      side colours swap to `SCR_BASE_COLOUR+17/16` and `+18/+15`
      (standard: `+2/+10`, `+1/+15`; reference `Track.cpp:1360-1385`)
      — add the alternates to `ScrPalette`/`RoadTextures` and select by
      the mode flag in `TrackRenderer`. Note the reference's own
      `Track.cpp:2490` TODO says some super-league values are
      unverified; match ptitSeb, don't guess beyond it.
- [ ] [StuntCarRacerLib] Super League car + cockpit visuals: opponent
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

- [ ] [StuntCarRacerLib.Tests] Golden-trace characterization harness:
      drive the existing integer physics with scripted `CarInput`
      sequences on two or three tracks, record the car/opponent state per
      physics tick (position, speeds, angles, damage, boost) to committed
      baseline files, and add a test that replays and compares. This is
      pure test code — no physics changes — and becomes the regression
      net for the conversion steps; the exact-integer unit-test
      assertions stay untouched until each class converts.
- [ ] [StuntCarRacerLib] Convert angles and trig: replace
      `AmigaTrig`'s 16384-scaled short table and `TrigCoefficients` with
      float equivalents; decide the angle unit (keeping 0..65536 as a
      float unit is the least-churn option) and replace the
      `& (MaxAngle - 1)` bitmask wraps with a float wrap helper. Decide
      the rendering boundary here too: `Scene3D.TransformPoint` consumes
      `TrigCoefficients` and `Track.LogPrecision`
      ([Scene3D.cs:102-113](../src/scr/libs/StuntCarRacerLib/Rendering/Scene3D.cs))
      — either convert its fixed-point view transform in the same step
      or leave it a shim that scales from the float trig.
- [ ] [StuntCarRacerLib] Convert `CarPhysics` (four partials:
      [CarPhysics.cs](../src/scr/libs/StuntCarRacerLib/Cars/CarPhysics.cs),
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
- [ ] [StuntCarRacerLib] Convert `OpponentPhysics` (+ `.Interaction`,
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
is closer to resolution-independence than the original item assumed):

- [ ] [Useful.SDL] Resizable window with letterboxed scaling: add
      `SDL_WINDOW_RESIZABLE` in [SDLWindow.cs:23-29](../src/useful/libs/Useful.SDL/SDLWindow.cs)
      and present the fixed-size software framebuffer via
      `SDL_RenderSetLogicalSize` (aspect-preserving letterbox) so the
      window can be any size while both games keep rendering at their
      native 512x512 / 640x400 — zero game-code changes. Handle
      `SDL_WINDOWEVENT_SIZE_CHANGED` in the event loop. Do together
      with (or after) the streaming-texture item above — the present
      path is the same code.
- [ ] [Apps] Make the render resolution configurable: both apps
      hardcode `ScreenWidth`/`ScreenHeight` consts (the "Get these from
      config" comment at [SDLProgram.cs:22-29](../src/elite/apps/EliteSharp/SDLProgram.cs),
      including the commented-out QHD block); move them into each
      game's config (Elite's `ConfigSettings` exists; SCR needs the
      equivalent) so a launch resolution/aspect can be chosen. This
      exposes rather than fixes fixed-size assumptions — pair with the
      audits below before shipping non-default values.
- [ ] [StuntCarRacerLib] SCR widescreen: with resolution configurable,
      make the 3D viewport render at the window aspect (SCR's
      `Scene3D.SetView` already takes width/height) and apply ptitSeb's
      cockpit widescreen treatment (`GetScreenDimensions`,
      `COCKPIT_WIDESCREEN_OFFSET` — side panels pushed out, HUD
      centred); audit `TrackMenuScreen` and the other 2D screens for
      640x400 assumptions. `HudRenderer`'s virtual-canvas scaling
      mostly survives as-is.
- [ ] [EliteSharpLib] Elite at non-512x512 resolutions: audit and fix
      the hardcoded coordinate-space assumptions — 511 in
      `ShipBase.DrawLasers`, `ScannerWidth = 512` in `EliteDraw`, the
      `Centre`/`Scale` maths — so Elite renders correctly at other
      resolutions. Depends on the Scale-policy cleanup above; scope
      question for the maintainer first: is widescreen Elite wanted at
      all, or only integer-scaled 512x512 (which the letterbox item
      already provides)? If the latter, close this as Won't.

## Won't

- [ ] [EliteSharpLib] Buying more than 255g of Gold/Platinum doesn't work — authentic to the original ("broken as designed"); documented, not fixed.
- [ ] [EliteSharpLib] Elite Intro2 parade shows 29 of ~33 ship models ([ShipFactory.cs:80-111](../src/elite/libs/EliteSharpLib/Ships/ShipFactory.cs)) — Cougar, Constrictor and the Lone variants are mission-specific ships, deliberately excluded from the parade; confirmed intentional, not a bug.
- [ ] [Useful.Graphics] Software rasterizer throughput (per-pixel `SetPixel`, insertion-sorted painter chain of ≤100 polys, no spans/SIMD) — the game is fixed at 13.5fps by design and none of this is a bottleneck at that rate; revisit only if the "performance as secondary objective" goal is picked up.
- [ ] [StuntCarRacerLib] The original remake's Windows-only infrastructure (DXUT registry prefs, clipboard, DirectSound path, `MessageBox` dialogs) is deliberately not ported — see the porting notes in [scr-readme.md](scr-readme.md).
- [ ] [StuntCarRacerLib] ptitSeb's remaining debug/infrastructure toggles are deliberately not ported (2026-07-19 parity audit): F1 test key, F2 triangle-list/strip vertex-buffer toggle (meaningless in the software rasterizer), the 'Z' reposition test key, the disabled action-replay / Amiga-recording harness (`#ifdef NOT_USED` / `USE_AMIGA_RECORDING` even upstream), the French-keyboard digit remaps, and the SDL command-line video flags (superseded by the resizable-window/config-resolution items under Could). The F5 stats overlay and F6/F7 per-car freezes stay listed as optional ride-alongs on the race-pause item. `Chime.wav` is unused by both code bases (kept as an asset only).
