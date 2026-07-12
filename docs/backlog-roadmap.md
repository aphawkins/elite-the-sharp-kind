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

- [ ] [Apps] [LARGE] Introduce a real composition root: add
      `Microsoft.Extensions.DependencyInjection` to both apps, register
      `IAbstraction`/`IGraphics`/`ISound`/`IKeyboard`, `AssetLocator`,
      `ConfigSettings` and the game as container-owned singletons in
      `Program.Main`, and move all object creation out of `EliteMain`'s
      ~90-line constructor ([EliteMain.cs:61-141](../src/elite/libs/EliteSharpLib/EliteMain.cs),
      `// TODO: Use DI` at [SDLProgram.cs:46](../src/elite/apps/EliteSharp/SDLProgram.cs))
      so domain classes receive collaborators instead of building them.
      Subsumes issues.md "Inject options" (including `AudioController`'s
      hardcoded `_musicOn`/`_effectsOn`,
      [AudioController.cs:27-33](../src/useful/libs/Useful.Audio/AudioController.cs)).
- [ ] [Useful] [LARGE] Add library logging: reference
      `Microsoft.Extensions.Logging.Abstractions` from the `Useful.*` and
      game libraries, accept `ILogger<T>` by constructor, define
      per-library `[LoggerMessage]` partials (the pattern the apps'
      `LogMessages.cs` already uses), and convert the operational
      `Debug.Fail`/`Debug.WriteLine` calls (21/14 across the libraries,
      heaviest in [Combat.cs](../src/elite/libs/EliteSharpLib/Conflict/Combat.cs))
      to thrown exceptions or logged Warnings/Errors per the architecture
      doc's logging policy — today they all vanish in Release builds.
- [ ] [EliteSharpLib] Move user data out of the CWD: resolve `elitesharp.cfg`
      ([ConfigFile.cs:13](../src/elite/libs/EliteSharpLib/Config/ConfigFile.cs))
      and `.cmdr` saves ([SaveFile.cs:17](../src/elite/libs/EliteSharpLib/Save/SaveFile.cs))
      against `Environment.SpecialFolder.ApplicationData` with an injected
      base path (fixes "launched from a shortcut" breakage and makes both
      classes testable — the ConfigFile fix couldn't be tested for exactly
      this reason).
- [ ] [StuntCarRacerLib] Give SCR screens their real dependencies: every
      screen currently receives the whole `StuntCarRacerMain`
      (`new RaceScreen(this)`, [StuntCarRacerMain.cs:82-85](../src/scr/libs/StuntCarRacerLib/StuntCarRacerMain.cs))
      and reaches through it service-locator style; pass what each screen
      actually uses (`CarPhysics`, `IKeyboard`, `SceneCamera`, ...) so
      dependencies are visible in signatures and screens are unit-testable.
- [ ] [Useful.Controls] Split `IKeyboard` (interface segregation): it mixes
      the producer API (`KeyDown`/`KeyUp`, called by `SDLInput`) with the
      consumer API (`IsPressed`/`IsHeld`/`LastPressed`, called by games)
      ([IKeyboard.cs](../src/useful/libs/Useful.Controls/IKeyboard.cs)); split into a
      sink interface and a state interface so game code can't inject key
      events.
- [ ] [Useful] Remove two-phase construction: `SDLSound` must have
      `Initialize(assetLocator)` called after construction (temporal
      coupling — a forgotten call is a runtime `KeyNotFoundException`), and
      `SoftwareGraphics.Create`/`ShipFactory.Create` mutate internal
      settable properties post-construction; fold initialisation into
      constructors/factory parameters so no instance is observable
      half-built.
- [ ] [EliteSharpLib] Replace the static crypto RNG: `RNG.Random` delegates
      every call to `RandomNumberGenerator.GetInt32`
      ([RNG.cs:98](../src/elite/libs/EliteSharpLib/RNG.cs)) — orders of magnitude
      slower than needed in hot per-tick paths — and `RNG`/`RNG.Seed` is
      static mutable state that makes Elite's logic untestable; make it an
      injected, seedable random service (the pattern `CarPhysics._random`
      already uses in SCR).

### Defects and gaps

- [ ] [Useful.Graphics] `DrawTextCentre`/`DrawTextLeft`/`DrawTextRight` wrap the bitmap from `GenerateTextBitmap` in `using` ([SoftwareGraphics.cs:292-324](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)), but that bitmap is stored in `_textCache` and returned again on the next call — cached bitmaps are disposed (GC pin freed) while still cached, working today only because text drawing never touches `BitmapHandle`; remove the `using`s and dispose cached bitmaps only when the cache is cleared/disposed.
- [ ] [Useful.Graphics] `_textCache` grows without bound ([SoftwareGraphics.cs:13, 589-648](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)) — every distinct (font, colour, text) string caches a ~KB-to-tens-of-KB bitmap forever, and Elite renders ever-changing strings ("1234.5 Credits" per bounty, countdowns), so long sessions leak memory steadily; add an eviction policy (e.g. cap entry count) or key frequently-changing text out of the cache.
- [ ] [Useful.Graphics] `SoftwareGraphics.SetClipRegion` is an empty no-op ([SoftwareGraphics.cs:368-370](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)) while Elite actively relies on clip regions (`EliteDraw.SetViewClipRegion`) — in the primary (software) renderer, view drawing can overwrite the border/scanner area; implement rectangular clipping in the software rasterizer (all draw calls already funnel through `DrawPixel`/`SetPixel`) or remove it from `IGraphics` and clip in game code (an interface member the primary implementation can't honour violates the architecture doc's interface rules).
- [ ] [Useful.SDL] `ToSDLColor` decodes the colour as RGBA (`r = color >> 24`) ([SDLGraphics.cs:417-423](../src/useful/libs/Useful.SDL/SDLGraphics.cs)) while every colour in the codebase is ARGB (`FastColor`, palettes) and `SetRenderDrawColor` in the same file decodes ARGB — text and filled triangles in the SDL backend get alpha interpreted as red; make `ToSDLColor` decode ARGB like `FastColor`.
- [ ] [EliteSharpLib] `EliteMain.Update` wraps `UpdateConsole`/`HandleInput` in `catch (Exception) { Debug.WriteLine(...) }` ([EliteMain.cs:262-272](../src/elite/libs/EliteSharpLib/EliteMain.cs)) — in Release builds every input/console error is silently swallowed each tick; remove the catch-all (or catch the specific expected exception type and surface it via the logger), per the architecture doc's "never catch-all on the frame path".
- [ ] [EliteSharpLib] `EliteMain.Run` calls `Environment.Exit(0)` ([EliteMain.cs:149](../src/elite/libs/EliteSharpLib/EliteMain.cs)), which terminates before `SDLProgram.Main`'s `using SoftwareAbstraction` disposes (skipping `SDL_Quit`, audio teardown); return normally and let `Main` exit (the `Environment.Exit(-1); throw;` in [SDLProgram.cs:61-62](../src/elite/apps/EliteSharp/SDLProgram.cs) has the same problem plus an unreachable `throw`).
- [ ] [EliteSharpLib] `ShipBase.GetPointIndex` linearly scans `Model.Points` with `foreach` + `IndexOf` (an O(n²) double scan) and is called 3 + N times per face, per ship, per tick ([ShipBase.cs:183-194](../src/elite/libs/EliteSharpLib/Ships/ShipBase.cs)) — the hottest CPU path in Elite's renderer; store point indices on `Face` at model-load time (in `ModelReader`) instead of object references that must be searched back to indices.
- [ ] [EliteSharpLib] `DrawModelFaces` sorts faces by a variable named `zavg` that actually holds the **maximum** Z of the face ([ShipBase.cs:152-161](../src/elite/libs/EliteSharpLib/Ships/ShipBase.cs)), feeding the painter's-algorithm parameter `averageZ` — a likely root cause of the long-standing "bits of hidden surfaces show through" issue; compute the mean Z (as the original did) and re-test the visual artefact.
- [ ] [EliteSharpLib] `SaveFile.LoadCommander` catches, resets to Jameson, then `throw;`s ([SaveFile.cs:70-75](../src/elite/libs/EliteSharpLib/Save/SaveFile.cs)), contradicting its bool-return contract, and `SaveStateToGameState` indexes `GalaxySeed[0..5]`, `CurrentCargo[i]`, `Lasers[0..3]` and `Enum.Parse`s strings without validation ([SaveFile.cs:167-208](../src/elite/libs/EliteSharpLib/Save/SaveFile.cs)) — a truncated or hand-edited `.cmdr` file throws instead of showing the view's "Error Loading Commander!" path; validate the deserialized `SaveState` and return false on any failure.
- [ ] [EliteSharpLib] `SaveFile.SaveCommander` builds the path as `save.CommanderName + ".cmdr"` from raw user input ([SaveFile.cs:80-92](../src/elite/libs/EliteSharpLib/Save/SaveFile.cs)) — invalid filename characters throw and path separators escape the save directory; sanitize the name (e.g. `Path.GetInvalidFileNameChars`) before using it as a filename (pairs with the user-data-location item above).
- [ ] [EliteSharpLib] `EliteDraw.DrawTextPretty` decrements `i` looking for a space/comma/period with no lower bound ([EliteDraw.cs:136-157](../src/elite/libs/EliteSharpLib/Graphics/EliteDraw.cs)) — a word longer than the line width underflows the index and throws; bound the scan at `previous` and hard-break long words.

### Tests

- [ ] [Tests] Elite's core game logic is largely untested — `EliteSharpLib.Tests` covers planets/suns/ships/universe plus the new `TradeTests`, leaving `Combat`, `Space`, `SaveFile` round-trip, `ConfigFile`, `PlanetController` and `RNG.GenerateRandomNumber` untested (the contraband bug lived here for years); start with the pure-logic classes: `PlanetController`, `SaveFile` save/load round-trip against an injected temp directory.
- [ ] [Tests] Add an `EliteMain` construction/smoke test using fakes, mirroring SCR's `StuntCarRacerMainTests` — Elite currently has no test that even constructs its composition; `EliteSharpLib.Fakes` (`FakeEliteDraw`, `FakeShipFactory`) plus `Useful.Fakes` already provide most of the doubles.

### Release engineering (from the retired release plan)

- [ ] [Release] Switch versioning from CI's date+run-number stamp to tag-driven semantic versioning (e.g. [MinVer](https://github.com/adamralph/minver)) so tagging a commit *is* the release-versioning step; do this before the first `v1.0.0` tag.
- [ ] [Release] Add a tag-triggered CI job that publishes win-x64/linux-x64/linux-arm64 self-contained builds for both Elite and StuntCarRacer (add a publish profile + CI publish step for `StuntCarRacer.csproj` mirroring Elite's) and creates a GitHub Release with the zips attached (`softprops/action-gh-release` or `gh release create --generate-notes`); ship as zip/tar.gz, not an installer. Label the SCR artifacts as preview in the release notes given its open defects list below.

### Stunt Car Racer conversion — correctness (from the retired conversion plan)

- [ ] [StuntCarRacerLib] Boost is BCD in the original: the Amiga stores `boost.reserve` as BCD but [Track.cs:115](../src/scr/libs/StuntCarRacerLib/Tracks/Track.cs) loads the raw byte and `CarPhysics.BoostReserve` counts it in binary, so a track byte of `$30` gives 48 boost units instead of 30; convert from BCD on load.
- [ ] [StuntCarRacerLib] Full damage should wreck the car: the original's `damage.line` wrecks the car (`car.is.wrecked`) when the crack reaches the end of the beam (240); the HUD caps the crack but nothing wrecks the car.
- [ ] [StuntCarRacerLib] Opponent speed values still use the old fluffyfreak random-table approach (`OpponentData.SpeedValues`/`TrackSpeedValues`); ptitSeb replaced it with `Opponent_Speed_Value()` computed per-piece from `Piece_Angle_And_Template`/`sections_car_can_be_put_on` with a memoized accumulator — a direct port of the authentic Amiga assembly (inlined as a comment in that function in `Opponent_Behaviour.cpp`); port that algorithm.

## Could

### Cleanups and small refactors

- [ ] [Useful] Replace the custom `Guard.ArgumentNull` (50 call sites) with the framework's `ArgumentNullException.ThrowIfNull` and delete [Guard.cs](../src/useful/libs/Useful/Guard.cs)/`ValidatedNotNullAttribute` — the architecture doc's own "prefer dotnet framework intrinsics" rule.
- [ ] [Repo] Adopt central package management: add `Directory.Packages.props` (14 projects currently pin versions independently and are already drifting) and move the copy-pasted analyzer `PackageReference` block from every csproj into `Directory.Build.props`/`.targets`.
- [ ] [Repo] Add a test coverage badge to README.md sourced from the CI-collected coverage numbers (decision: visibility only, no gate/target).
- [ ] [Useful.Abstraction] `ScreenManager` starts with `CurrentId = default!` and a nullable `Current`, forcing `Screens.Current!.Update()` at every call site ([ScreenManager.cs:29-31](../src/useful/libs/Useful.Abstraction/ScreenManager.cs)); require the initial screen at construction (or an explicit `Start(id)`) so `Current` is never null after setup.
- [ ] [Useful.Graphics] `FastBitmap` pins every bitmap with a raw `GCHandle` + finalizer ([FastBitmap.cs:19-35](../src/useful/libs/Useful.Graphics/FastBitmap.cs)); only the screen bitmap ever crosses into SDL — pin only on demand (or wrap the handle in a `SafeHandle`-style type) so short-lived text/texture bitmaps don't fragment the GC heap.
- [ ] [Apps] Fix the rename leftover: `EliteSharp`'s `LogMessages`/`SDLProgram` sit in namespace `EliteSharp.SDL` though the project is `EliteSharp`; and remove the committed benchmark reports under `src/*/perf/**/reports/` (generated artifacts, per the architecture doc's hygiene rule).
- [ ] [EliteSharpLib] Give the bare `throw new EliteException()` calls in the factory `switch` defaults ([ShipFactory.cs:46,59,68,77](../src/elite/libs/EliteSharpLib/Ships/ShipFactory.cs)) a message with the offending value — an empty exception is undiagnosable in a log.
- [ ] [Useful.SDL] Document and harden the `SDLSound` loop-pitch threading contract: the `Mix_RegisterEffect` callback runs on the audio thread while the game thread writes the pitch; verify the field is read/written with `Volatile` (or is inherently safe) and state the contract in a comment.
- [ ] [Useful.Graphics] Remove the `Debug.WriteLine($"{x},{y}")` left in the `DrawCircleFilled` scanline loop ([SoftwareGraphics.cs:112](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)) — debug-build log spam for every filled circle (planets, suns) every frame.
- [ ] [Useful] Remove `Vector4.Cloner()` ([Maths/Extensions.cs:13](../src/useful/libs/Useful/Maths/Extensions.cs)) — `Vector4` is a struct, so plain assignment already copies; its four call sites can just assign.
- [ ] [EliteSharpLib] Clean `EliteSharpLib.csproj`: fourteen `sfx\*.wav` items point at a folder that doesn't exist and the `<Compile Remove="Controls\**" />` block excludes a folder that no longer exists ([EliteSharpLib.csproj](../src/elite/libs/EliteSharpLib/EliteSharpLib.csproj)); the ~280-line hand-maintained asset item list could also become two glob items. (The unused NAudio refs are covered by the Must item.)
- [ ] [Repo] Delete stale build-output-only directories left behind by project renames: `src/elite/apps/EliteSharp.SDL/`, `src/elite/libs/EliteSharp/`, `src/elite/test/EliteSharp.Tests/`, `src/elite/perf/EliteSharp.Benchmarks/` contain only `bin`/`obj` and are referenced by no project or solution entry.
- [ ] [EliteSharpLib] `Stars` repeats the same star plot/respawn block three times (`FrontStarfield`, `RearStarfield`, `SideStarfield`, [Stars.cs:61-133, 151-243, 258-333](../src/elite/libs/EliteSharpLib/Stars.cs)); extract the shared plot helper to cut ~80 duplicated lines.
- [ ] [Useful.Graphics] The hardcoded `Scale { get; } = 2` on `IGraphics` and the centring math that divides by it (`DrawRectangleCentre` = `(ScreenWidth - width) / Scale`, [SoftwareGraphics.cs:25, 281-282](../src/useful/libs/Useful.Graphics/SoftwareGraphics.cs)) only centres correctly because Scale happens to equal 2, and `SDLGraphics` divides positions by `(2 / Scale)` which is a no-op ([SDLGraphics.cs:239](../src/useful/libs/Useful.SDL/SDLGraphics.cs)) — Elite-specific scaling leaking into the shared library; make centring `(ScreenWidth - width) / 2` and move scale policy to the game.
- [ ] [Useful.SDL] `SDLGraphics.DrawImage`/`DrawImagePart` create and destroy an `SDL_Texture` from the surface on every call ([SDLGraphics.cs:84-148](../src/useful/libs/Useful.SDL/SDLGraphics.cs)), and `SoftwareAbstraction.SoftwareScreenUpdate` creates a surface + texture per presented frame ([SoftwareAbstraction.cs:65-106](../src/useful/libs/Useful.SDL/SoftwareAbstraction.cs)); cache textures per image, and use one streaming texture + `SDL_UpdateTexture` for the framebuffer blit.
- [ ] [EliteSharpLib] `ShipFactory.CreateShipFromName` instantiates ship types via reflection from strings in `AssetManifest.json` ([ShipFactory.cs:114-131](../src/elite/libs/EliteSharpLib/Ships/ShipFactory.cs), flagged by its own TODOs) — data-driven `Type.GetType` + non-public `Activator.CreateInstance` is fragile and a mild input-handling risk; replace with an explicit name→factory dictionary.
- [ ] [EliteSharpLib] `ShipBase.Draw` allocates a `new Vector4[100]` per ship per tick and keeps a discarded `_ = VectorMaths.UnitVector(...)` call ([ShipBase.cs:100-115](../src/elite/libs/EliteSharpLib/Ships/ShipBase.cs)); reuse a pooled/instance buffer sized to the model and delete the dead call (same pattern for `EliteDraw._pointList`'s magic `100` vs the `MAXPOLYS` constant, [EliteDraw.cs:19-25](../src/elite/libs/EliteSharpLib/Graphics/EliteDraw.cs)).
- [ ] [EliteSharpLib] `Universe.RemoveShip` only removes from `_objects`, silently leaving `Planet`/`StationOrSun` set if passed one of those ([Universe.cs:127-135](../src/elite/libs/EliteSharpLib/Universe.cs)) — station removal currently works only because a sun immediately overwrites `StationOrSun` in `Combat.RemoveShip`; clear the matching reference explicitly.
- [ ] [Useful.SDL] `SDLGraphics.LoadFont` hardcodes the game-specific font names "Small"/"Large" and their point sizes into the shared library ([SDLGraphics.cs:395-408](../src/useful/libs/Useful.SDL/SDLGraphics.cs)); carry the size in the asset manifest instead.
- [ ] [EliteSharpLib] Wireframe planet is just a circle ([WireframePlanet.cs:50-57](../src/elite/libs/EliteSharpLib/Planets/WireframePlanet.cs)); add the two arcs and crater the original Elite drew (from issues.md).
- [ ] [LARGE] [Useful.Graphics] Unify the two 3D pipelines: Elite (`ShipBase.TransformModelPoints` + `EliteDraw` painter chain) and SCR (`Scene3D` + `TrackRenderer`, now z-buffered via `DrawPolygonFilledDepth`) each implement transform → near-clip → project → visibility; per the architecture doc's "as much code as possible in Useful", compare and extract genuinely shared stages into `Useful.Graphics` — don't force an abstraction over two pipelines that turn out to differ for good reason. (A shared text/HUD-panel helper for the two games' ad-hoc HUD code is a smaller sibling of this item.)

### Stunt Car Racer conversion — features (from the retired conversion plan)

- [ ] [StuntCarRacerLib] Lap times: the original shows current/best lap times on the dashboard (`print.lap.time`/`show.lap.time`); not ported, no lap timing exists yet.
- [ ] [StuntCarRacerLib] Per-effect sound volume and pitch: the original scales grounded/creak effect volume by damage level, and pitches the off-road dust-cloud sound randomly (`DrawDustClouds`: `AMIGA_PAL_HZ / (450 + rand & 0x1c)`) and the edge-scrape spark sound by speed (`DrawSparks`); `AudioController`/`SfxSample` (and `ISound.Play`) currently have no per-play volume or pitch parameter (the engine-loop pitch is separate and already ported).
- [ ] [StuntCarRacerLib] F9/F10 frame-gap tuning keys: both C++ versions adjust the physics frame gap live; `StuntCarRacerMain.FrameGap` exists for exactly this but isn't wired to any key.
- [ ] [StuntCarRacerLib] Race pause: the remake pauses on 'P' and resumes on 'O' (`bPaused`, engine sound stopped while paused); not ported — no pause exists. The remake's debug freezes (F5 stats overlay, F6 player-only pause, F7 opponent-only pause) could ride along as dev aids.
- [ ] [StuntCarRacerLib] 'R' turn-around key: the remake adds 180 degrees to the player's y angle and re-initialises (`INITIALISE_PLAYER`, ptitSeb `StuntCarRacer.cpp` ~1039) so a car facing the wrong way can recover; not ported.
- [ ] [StuntCarRacerLib] Mid-race 'M' to track menu: the remake returns to the track menu from any mode on 'M' (also resetting the drawbridge); the port only handles 'M' on the game-over and track-preview screens — `RaceScreen` has no way back to the menu short of Escape-quitting.
- [ ] [StuntCarRacerLib] Opponent name announcement: the remake prints the opponent's name for the first four seconds of a race (`RenderText`, `opponentNames[opponentsID]`); `OpponentPhysics.Name` already exists but nothing displays it.
- [ ] [StuntCarRacerLib] Player outside/chase view: needs a chase camera plus drawing the player's own car mesh (`Rendering/CarMesh` is currently only used for the opponent).
- [ ] [StuntCarRacerLib] Road-line textures could sample the shared `atlas.bmp` (ptitSeb's `eRoadYellowDark` etc.) instead of the procedural strips in `Rendering/RoadTextures` — closer visual match, but the current strips already look correct; cosmetic.
- [ ] [LARGE] [StuntCarRacerLib] Gamepad/joystick support: port `XBOXController.cpp/h` via `Useful.Controls` for XInput-style controllers plus a generic-HID digital joystick (USB Competition Pro Extra — 8-way stick + fire buttons, no analog axes), which needs SDL's joystick API (`SDL_Joystick`/`SDL_GameController`, check what `ppy.SDL2-CS` exposes) rather than XInput; the shared abstraction should cover both device classes.
- [ ] [LARGE] [StuntCarRacerLib] Super League: ptitSeb's `bSuperLeague` toggle ('L' on the track menu) swaps alternate track/road-line colours (`SCR_BASE_COLOUR+16/17/18`), car body colours (+19/20/21), engine/boost constants (240/16/236 standard vs 320/12/314 super), opponent speed tables (`opp_track_speed_values[TrackID+32]`) and the atlas's "2"-suffixed sprite variants; needs a menu toggle, `RoadTextures`/`ScrPalette` alternates, and wiring `CarPhysics.RoadCushionValue` (which already exists but nothing sets) and the `OpponentPhysics` tables to the mode.
- [ ] [LARGE] [StuntCarRacerLib] Convert physics from integer fixed-point to float: `Cars/CarPhysics.*`, `Cars/OpponentPhysics*`, `Cars/AmigaTrig`, `Cars/TrigCoefficients` and `Track.LogPrecision`'s 16384 scale use 68000-style scaled-integer arithmetic ported line-by-line; convert the physics core to `float` throughout. Watch for: angle wrapping is `& (MaxAngle - 1)` bitmasking and needs an equivalent float wrap; check for reliance on integer truncation/overflow; the 140+ exact-integer test assertions need reworking as tolerance-based comparisons.
- [ ] [LARGE] [Useful.SDL] Resizable window / widescreen: `SDLWindow` creates a fixed non-resizable window ([SDLWindow.cs:23-29](../src/useful/libs/Useful.SDL/SDLWindow.cs)); ptitSeb's build supports resizing with dynamic cockpit/font scaling (`GetScreenDimensions`, `COCKPIT_WIDESCREEN_OFFSET`), and SCR's `HudRenderer`/`TrackMenuScreen` assume fixed 640x400 when computing scale factors (Elite similarly assumes 512x512, e.g. the hardcoded 511 in `ShipBase.DrawLasers`). Merges issues.md "make window resizable" with the conversion plan's widescreen item.

## Won't

- [ ] [EliteSharpLib] Buying more than 255g of Gold/Platinum doesn't work — authentic to the original ("broken as designed"); documented, not fixed.
- [ ] [EliteSharpLib] Elite Intro2 parade shows 29 of ~33 ship models ([ShipFactory.cs:80-111](../src/elite/libs/EliteSharpLib/Ships/ShipFactory.cs)) — Cougar, Constrictor and the Lone variants are mission-specific ships, deliberately excluded from the parade; confirmed intentional, not a bug.
- [ ] [Useful.Graphics] Software rasterizer throughput (per-pixel `SetPixel`, insertion-sorted painter chain of ≤100 polys, no spans/SIMD) — the game is fixed at 13.5fps by design and none of this is a bottleneck at that rate; revisit only if the "performance as secondary objective" goal is picked up.
- [ ] [StuntCarRacerLib] The original remake's Windows-only infrastructure (DXUT registry prefs, clipboard, DirectSound path, `MessageBox` dialogs) is deliberately not ported — see the porting notes in [scr-readme.md](scr-readme.md).
