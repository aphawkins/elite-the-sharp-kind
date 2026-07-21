# Changelog

All notable changes to this project are documented in this file. The format
is based on [Keep a Changelog](https://keepachangelog.com/); the project does
not yet cut versioned releases, so everything sits under Unreleased.
Completed items from the [backlog](docs/backlog-roadmap.md) move here.

## [Unreleased]

### Changed (SCR screens given their real dependencies, 2026-07-21)

- Every SCR screen (`RaceScreen`, `TrackMenuScreen`, `TrackPreviewScreen`,
  `GameOverScreen`) took the whole `StuntCarRacerMain` and reached through
  it service-locator style. Extracted a new `Race`
  ([Race.cs](src/scr/libs/StuntCarRacerLib/Race.cs)) that owns the
  track/car/opponent/bridge state, its renderers, and the
  `LoadTrack`/`PhysicsDue`/`DrawWorld`/`DrawHud`/`UpdateSounds`/
  `UpdateEngineSound` behaviour that operates on them; `StuntCarRacerMain`
  now keeps only the run loop, screen wiring and audio setup. Each screen's
  constructor now takes `Race` plus the specific stable dependencies it
  actually uses (`IKeyboard`, `ScreenManager<GameMode, IGameScreen>`,
  `IGraphics`, `ScrPalette`, `ISound`) instead of the whole game object, so
  dependencies are visible in the constructor signature. Built the full
  solution, ran the complete test suite (all green, including
  `StuntCarRacerLib.Tests`' 180 tests), and smoke-tested the built SCR app
  (starts and constructs its full DI graph, including `Race` and all four
  screens, without error).

### Added (Remaining EliteSharpLib Debug calls converted to logging, 2026-07-21)

- The last scattered `Debug.Fail`/`Debug.WriteLine` calls outside
  `Combat` are converted: `Space`'s three "Failed to create Planet/Sun"
  `WriteLine`s ([Space.cs](src/elite/libs/EliteSharpLib/Space.cs)) and one
  each in `ConstrictorMissionView`, `EscapeCapsuleView`, `Intro1View`,
  `Intro2View` — all legitimate runtime conditions (universe full) — now
  log a Warning via the existing `LogMessages.FailedToCreateShip`, same
  as the `GameOverView`/`Combat` exemplars. `Intro2View`'s parade-ship
  failure now logs the specific ship's name instead of a fixed
  "first Parade ship" string. `Space` and the four views all take an
  optional `ILogger<T>` (defaulting to `NullLogger<T>.Instance`) through
  their constructors, and `EliteServiceCollectionExtensions` resolves
  `ILoggerFactory` to pass each one through. This closes out the
  EliteSharpLib logging backlog item — every operational `Debug.*` call
  in the library is now converted. Built the full solution, ran the
  complete test suite (all green), and smoke-tested the built Elite app
  (starts, logs "Starting Elite - The Sharp Kind", and constructs its
  full DI graph — including every changed class — without error).

### Added (Combat's Debug.Fail calls converted to logging/exceptions, 2026-07-21)

- `Combat` was the heaviest concentration of `Debug.Fail` calls in
  `EliteSharpLib` (17, [Combat.cs](src/elite/libs/EliteSharpLib/Conflict/Combat.cs)),
  none of which survive a Release build. `Combat` now takes an optional
  `ILogger<Combat>` (defaulting to `NullLogger<Combat>.Instance`,
  following the `GameOverView` exemplar), and the 16 "Failed to create
  &lt;ship&gt;" cases — all legitimate runtime conditions (universe full)
  — log a Warning via the existing `LogMessages.FailedToCreateShip`.
  "Incorrect loot type" was a programming-error case (an unreachable
  `ShipType` branch), so it now throws `EliteException` instead —
  `LaunchLoot`'s if/else-if chain became a switch expression to keep the
  analyzer happy with the added throw arm.
  `EliteServiceCollectionExtensions.AddEliteMain` now resolves
  `ILoggerFactory` and passes `CreateLogger<Combat>()` through. Built the
  full solution, ran the complete test suite (all green), and
  smoke-tested the built Elite app (starts and constructs its full DI
  graph, including `Combat`, without error).

### Added (EliteSharpLib logging infrastructure + exemplar, 2026-07-21)

- `EliteSharpLib` had no way to log at all — every operational failure in
  the library went to `Debug.WriteLine`/`Debug.Fail`, which vanish in
  Release builds. Added a library-internal `LogMessages`
  `[LoggerMessage]` partial (`EliteSharpLib/LogMessages.cs`), following
  the same pattern as `Useful.Config.LogMessages` and the apps'
  `LogMessages.cs`. `Microsoft.Extensions.Logging.Abstractions` was
  already referenced by `EliteSharpLib.csproj`, and `ILoggerFactory` was
  already registered in the DI container (`SDLProgram`/
  `EliteServiceCollectionExtensions`), so only the per-consumer wiring
  was new.

  Proved the pattern end-to-end on `GameOverView` as the exemplar (a
  small, low-risk view with two `Debug.WriteLine("Failed to create
  ...")` calls): it now takes an optional `ILogger<GameOverView>`
  constructor parameter, defaulting to `NullLogger<GameOverView>.Instance`
  for tests/fakes, and logs both "failed to create ship" cases as a
  Warning (legitimate runtime condition — universe full — not a
  programming error) via `LogMessages.FailedToCreateShip`.
  `EliteServiceCollectionExtensions.AddEliteMain` now resolves
  `ILoggerFactory` and passes `CreateLogger<GameOverView>()` through.

  The remaining Debug.WriteLine/Debug.Fail call sites across `Combat`,
  `Space`, and the other four views convert in follow-up backlog items
  using this same infrastructure. Built the full solution, ran the
  complete test suite (all green), and smoke-tested the built Elite app
  (starts and constructs its full DI graph, including `GameOverView`,
  without error).

### Added (ConfigFile logs failures through the app's logger, 2026-07-21)

- `ConfigFile<T>`'s read/write failures went to `Debug.WriteLine` only,
  so nothing appeared in the Serilog file/console sinks the apps
  actually use — a purposefully-broken config (e.g. an invalid enum
  string) produced no trace in the log file at all, only in an attached
  debugger's Output window. `ConfigFile<T>` now takes an optional
  `ILogger<ConfigFile<T>>` (constructor overload, defaulting to
  `NullLogger<ConfigFile<T>>.Instance` — same pattern the backlog's
  library-logging item describes for `ILogger<T>` adoption) and a new
  library-internal `LogMessages` `[LoggerMessage]` partial
  (`Useful/Config/LogMessages.cs`) logs:
  - a Warning "Failed to read config file '{path}'; using defaults." —
    always visible at the apps' default Information level, no exception
    attached (so no stack-trace noise by default);
  - a separate Debug-level message carrying the actual exception, so the
    full stack trace only appears once `ELITE_LOG_LEVEL`/`SCR_LOG_LEVEL`
    is raised to `Debug`;
  - a Warning when the config fails validation (no exception, nothing
    was thrown);
  - an Error (with exception) on `WriteConfig` failure, alongside the
    existing `Debug.Fail` dev-time assertion (left as-is — orthogonal to
    logging).

  `AddEliteConfig`/`AddScrConfig` now resolve `ILoggerFactory` from the
  container (already registered by both `SDLProgram`s) and pass a
  `CreateLogger<ConfigFile<T>>()` logger through; `Useful`,
  `EliteSharpLib` and `StuntCarRacerLib` each gained a
  `Microsoft.Extensions.Logging.Abstractions` package reference for
  this. `IsValidConfig` stayed `internal` (was already made so for the
  earlier DRY-unification tests) so `EliteServiceCollectionExtensions`
  can pass it as the validation predicate unchanged.

  Reproduced the exact reported scenario (`{"PlanetStyle": "QWERTY"}`)
  against the built app: at the default level the log file now shows
  `[WRN] Failed to read config file '...'; using defaults.` with no
  stack trace; with `ELITE_LOG_LEVEL=Debug` the full exception chain
  (down to `ConfigurationBinder.Bind`) appears too. Added
  `Useful.Tests` coverage (`RecordingLogger<T>` fake — Moq's generic
  `ILogger.Log<TState>` verification is awkward, a fake is simpler)
  asserting the Warning+Debug split on read failure and the
  no-exception Warning on validation failure.

### Changed (Logs moved under the shared TheSharpKind user-data folder, 2026-07-21)

- Both apps' Serilog file sink used a `logs` path relative to the
  process's current working directory, so where log files actually
  landed depended on how the app was launched (repo root via `dotnet
  run`, or next to the exe when run directly) — inconsistent with the
  config files, which are already rooted at a fixed
  `%AppData%\TheSharpKind`. `SDLProgram.Main` in both apps now computes
  `userDataPath` first and points the file sink at
  `Path.Combine(userDataPath, "logs", "elite-.log")` /
  `"scr-.log"`, so logs always land at
  `%AppData%\TheSharpKind\logs\` (`~/.config/TheSharpKind/logs/` on
  Linux/macOS) regardless of launch method. Serilog's file sink creates
  the `logs` subdirectory itself, same as it always has.

  Smoke-tested both built apps: confirmed `elite-<date>.log`/
  `scr-<date>.log` now appear under the shared user-data folder. Docs
  (`elite-readme.md`, `scr-readme.md`) updated to mention log location
  alongside the existing config-file description.

### Changed (Config file handling unified into a generic Useful type, 2026-07-21)

- Elite's `ConfigFile`/`IConfigWriter` and SCR's `ScrConfigFile` were
  near-identical (JSON read/write via `Microsoft.Extensions.Configuration`,
  same defaults-on-failure behaviour, same catch clauses) except for their
  settings type and filename — a DRY violation now that both games have
  one. Replaced both with a single generic `Useful.Config.ConfigFile<T>`
  (`where T : new()`), implementing a new generic `IConfigWriter<T>`, with
  the filename passed to the constructor and an optional
  `Func<T, bool> isValid` predicate for game-specific validation (Elite's
  `Fps > 0`/enum-range checks; SCR has none). `EliteSharpLib`'s
  `ConfigSettings` and `StuntCarRacerLib`'s `ScrConfigSettings` stay put
  (they're genuinely game-specific), now bound as `ConfigFile<ConfigSettings>`
  / `ConfigFile<ScrConfigSettings>` respectively — `AddEliteConfig`/
  `AddScrConfig` and `SettingsView` (now `IConfigWriter<ConfigSettings>`)
  updated accordingly. `Microsoft.Extensions.Configuration`/`.Binder`/
  `.Json` package references moved from `EliteSharpLib.csproj`/
  `StuntCarRacerLib.csproj` to `Useful.csproj`, the only project that now
  uses them directly.

  Adding those packages to `Useful` exposed a latent naming collision:
  `Useful.Maths.Extensions` triggered CA1724 (type name conflicts with
  the newly-referenced `Microsoft.Extensions` namespace) under this
  repo's warnings-as-errors build. Renamed it to `MathsExtensions` (pure
  rename — it's an extension-method container, so no call site needed
  updating) to unblock the build; unrelated to the config unification
  itself but a direct consequence of it.

  Added generic coverage in `Useful.Tests/Config/ConfigFileTests.cs`
  (defaults-when-missing, round-trip, mistyped-value fallback, failing
  validation) covering the shared logic once; `EliteSharpLib.Tests`/
  `StuntCarRacerLib.Tests` keep their own `ConfigFileTests` but only for
  wiring specific to their real settings type/filename (Elite's also
  exercises the actual `IsValidConfig` predicate, now `internal` for
  testability). Full solution build and test suite pass; smoke-tested
  both built apps to confirm the DI wiring still resolves and runs.

### Fixed (Config parse failures crashed the game at startup, 2026-07-21)

- `ConfigFile.ReadConfig`/`ScrConfigFile.ReadConfig` only caught
  `IOException`/`UnauthorizedAccessException`/`FormatException` around
  `IConfiguration.Bind`, but `Microsoft.Extensions.Configuration.Binder`
  wraps type-conversion failures (e.g. a hand-edited `elitesharp.cfg`
  with a non-boolean string for `ShipWireframe`) in
  `InvalidOperationException`, not `FormatException` — so a corrupt or
  hand-edited config file crashed the whole game with an unhandled
  exception instead of falling back to defaults as intended. Both catch
  clauses now also match `InvalidOperationException`. Also tightened
  both `SDLProgram.Main`s: `GetRequiredService<EliteMain>()`/
  `<StuntCarRacerMain>()` (which triggers `ReadConfig` as part of DI
  composition) previously ran *before* the surrounding try/catch, so any
  composition-time exception bypassed the apps' own
  `LogMessages.CriticalAppTerminated` logging and crashed silently
  instead; moved the call inside the try block for both apps.

  Reproduced the exact reported crash (`elitesharp.cfg` containing
  `{"ShipWireframe": "hello!"}`) against the built app before and after
  the fix; added `ConfigFileTests`/`ScrConfigFileTests` regression cases
  covering the mistyped-value scenario, plus default-when-missing and
  write/read round-trip coverage that didn't exist for either config
  file before.

### Changed (Shared TheSharpKind user-data folder, 2026-07-21)

- Both apps' `userDataPath` (`SDLProgram.cs`) moved from their own
  per-game folder (`%AppData%\EliteSharp`, `%AppData%\StuntCarRacer`) to
  a shared `%AppData%\TheSharpKind`, since they're both part of the same
  project. Elite's config filename (`elitesharp.cfg`) and commander
  saves (`.cmdr`) already avoided any collision by name, so it needed no
  change; SCR's config filename gained a `sharp` suffix
  (`stuntcarracer.cfg` → `stuntcarracersharp.cfg`) to stay unambiguous
  now it sits next to Elite's files in the same folder. Docs
  (`elite-readme.md`, `scr-readme.md`) updated to match and cross-link
  each other's Configuration section, now that the folder is shared.

### Added (SCR persisted settings, 2026-07-21)

- Gave SCR its own settings file, mirroring Elite's `ConfigFile`/
  `ConfigSettings` pattern: a new internal `ScrConfigSettings`/
  `ScrConfigFile` (`StuntCarRacerLib/Config`) reads/writes
  `MusicOn`/`EffectsOn` to a JSON file (`stuntcarracersharp.cfg`) rooted
  at a user-data path, and a public
  `StuntCarRacerServiceCollectionExtensions.AddScrConfig(userDataPath)`
  (mirroring `AddEliteConfig`) registers it in DI and exposes the result
  as `Useful.Audio.AudioOptions` — the type `StuntCarRacerMain` already
  took at its `AudioController` construction site. `StuntCarRacerMain`
  gained a new public constructor overload accepting `AudioOptions` (the
  existing no-option constructors now default to `new()`, unchanged
  behaviour); `SDLProgram` wires `AddScrConfig` in ahead of the
  `StuntCarRacerMain` registration, same as Elite's `SDLProgram`.

  Skipped a settings-screen UI to write the file, following the same
  precedent as Elite's `ShipRenderMode` setting (2026-07-20): SCR has no
  settings screen at all today, so the config file remains the only way
  to change these values (still satisfies "toggle without code
  changes"). `FrameGap` and league selection stay out of scope, per the
  backlog item, as future candidates once a settings screen exists.

  Smoke-tested the built app: it starts, creates the shared user-data
  directory, and reads a hand-written config file
  (`MusicOn`/`EffectsOn` both `false`) without error. Added
  `ScrConfigFileTests` (default-when-missing, write/read round-trip) and
  two `StuntCarRacerMainTests` cases for the new constructor overload.

### Changed (Colour handling unified on FastColor, 2026-07-20)

- Phase 3 of unifying colour handling across Elite and SCR: `uint` →
  `FastColor` across the shared colour contract — `IGraphics` (and both
  implementations, `SoftwareGraphics`/`SDLGraphics`), `IPolygonRenderer`
  and its three strategies (`ZBufferRenderer`/`PainterRenderer`/
  `WireframeRenderer`), `IPaletteCollection`/`Palette`/`PaletteReader`
  (`Useful.Assets.Palettes`), `Face.Color` (`Useful.Assets.Models`), and
  SCR's `WorldPolygon.Colour`/`CarPalette`. Only the declared surfaces
  changed — internal call sites across Elite's ~110 `uint`-typed colour
  locals/fields (Views, Planets, Suns, Ships) and SCR's `ScrPalette`/
  `TrackRenderer` kept compiling unchanged via the implicit `uint`↔
  `FastColor` conversions added earlier the same day (see below), keeping
  the diff to 27 files. The exceptions needing edits were Moq test
  matchers (`It.IsAny<uint>()` → `It.IsAny<FastColor>()`), since Moq
  checks the literal parameter type rather than tolerating implicit
  conversions.

  Also resolves the open `[Useful.SDL] ToSDLColor decodes the colour as
  RGBA` defect: `ToSDLColor` now decodes via `FastColor.R`/`G`/`B`/`A`
  instead of hand-rolled bit-shifts, matching `SetRenderDrawColor` and
  every other colour in the codebase (ARGB).

### Changed (ScrPalette loaded from palette.json, 2026-07-20)

- Phase 2 of the colour-handling unification: `ScrPalette`'s hardcoded
  42-entry `uint[]` replaced with a JSON asset
  (`StuntCarRacerLib/Assets/Palette/palette.json`) loaded through
  `Useful.Assets.Palettes.PaletteReader`/`IPaletteCollection` — the same
  mechanism `EliteDraw` already used for its named palette. `Colour(int)`
  stays static with a lazily-loaded backing store and keeps addressing
  colours positionally (`Track.ScrBaseColour + offset`, matching the
  original's `SCR_BASE_COLOUR`-relative scheme) rather than converting to
  an injected instance: `RoadTextures.Textures` resolves colours in a
  static field initializer at type-load time, and ~20 call sites across 8
  files address colours by numeric offset, so DI injection here would
  have meant a much larger, riskier change for the same goal.

### Changed (FastColor moved into the base Useful library, 2026-07-20)

- Phase 1 of the colour-handling unification: `FastColor`/`BaseColors`
  relocated from `Useful.Graphics` into the base `Useful` library, so
  `Useful.Assets` (which sits below `Useful.Graphics` in the dependency
  graph and owns `IPaletteCollection`/`Face.Color`) can reference it in a
  later phase without a circular project reference. No call-site changes
  needed elsewhere — `Useful.Graphics`/`Useful.SDL` already see it via
  C#'s enclosing-namespace lookup. Added implicit `uint`↔`FastColor`
  conversions so existing ARGB literals and `uint`-typed fields kept
  compiling as call sites migrated to `FastColor` over the subsequent
  phases above.

### Changed (CarMesh converted to a car.obj asset, 2026-07-20)

- SCR's `CarMesh` (the opponent's wedge-body/wheel-quad geometry,
  previously hardcoded vertex/quad arrays) now loads from a Wavefront OBJ
  asset (`StuntCarRacerLib/Assets/Models/car.obj`), mirroring how Elite
  loads its ship models via `Useful.Assets.Models.ModelReader`. `CarMesh`
  changed from a static class to an instance constructed once (and
  injected into `OpponentRenderer`) instead of re-parsed on every track
  load; a new `CarPalette` resolves car.obj's five materials to
  track-palette colours since `Track.ScrBaseColour` is presently a fixed
  offset. Also fixed `.gitignore`'s blanket `*.obj` rule, which was
  silently excluding the new `car.obj` — only Elite's ship-model path was
  carved out as a genuine-OBJ exception; added SCR's models path
  alongside it.

### Changed (Polygon renderers moved to Useful.Graphics, 2026-07-20)

- `IShipRenderer`, `ShipRenderMode`, `PolygonData`, `WireframeRenderer`,
  `PainterRenderer` and `ZBufferRenderer` moved from
  `EliteSharpLib.Graphics` to `Useful.Graphics.Rendering`
  (`src/useful/libs/Useful.Graphics/Rendering/`) — after today's earlier
  work stripped their last Elite dependencies (`GameState`, the
  `ShipWireframe` check), none of the six referenced anything
  Elite-specific anymore; they only depend on `IGraphics`,
  `IAssetLocator` and `PaletteReader`, all already public in `Useful.*`.
  Renamed the two type names that said "ship": `IShipRenderer` →
  `IPolygonRenderer`, `ShipRenderMode` → `PolygonRenderMode`
  (`SubmitFace`/`faceColor` → `Submit`/`color` too, and
  `PolygonData.FaceColor` → `Color`); the three renderer classes
  (`WireframeRenderer`/`PainterRenderer`/`ZBufferRenderer`) didn't
  mention ships in their names already, so those stayed as-is. Elite's
  `ConfigSettings.ShipRenderMode` *property* keeps its name — that's
  Elite's own config key, just typed by the now-shared enum.
  `IShipRenderer`/the three renderer classes were `internal`; became
  `public` since Elite now consumes them from a different assembly
  (`PolygonData` stayed `internal`, used only inside the two chain-based
  renderers). Picked up two analyzer fixes moving to a fresh project
  (`CA1062` null-checks on the newly-public constructor/`Submit`
  parameters, `IDE0290` primary constructors) that hadn't fired while
  the types were `internal` to `EliteSharpLib`.

  Checked `StuntCarRacerLib` for anything that duplicates this
  chain/depth-sort logic and could reuse it: no — `TrackRenderer`/
  `OpponentRenderer` submit `WorldPolygon`s straight to
  `Graphics.DrawPolygonFilledDepth` with no buffering step, so there's
  nothing to swap over, though `ZBufferRenderer` is the closer relative
  if SCR ever wants the explicit start/submit/end lifecycle. Also
  checked the rest of this session's changes (`AudioOptions`,
  `ConfigSettings`/`ConfigFile`) for the same "no longer game-specific"
  smell — `AudioOptions` was already correctly in `Useful.Audio`;
  `ConfigSettings`/`ConfigFile` hold genuinely Elite-only fields
  (`PlanetStyle`, `SunStyle`, the `elitesharp.cfg` filename) and stay put.

### Added (Selectable ship render mode, 2026-07-20)

- Last of the ship-rendering-strategy items: `ZBufferRenderer` and
  `PainterRenderer` are now purely filled renderers — the `ShipWireframe`
  branch (and the `GameState`/`IAssetLocator` dependencies it needed for
  the white outline colour) is gone from both, so `ZBufferRenderer`'s
  constructor is just `(IGraphics)` now and `PainterRenderer`'s
  unchanged. A new `WireframeRenderer` (`Graphics/WireframeRenderer.cs`)
  handles outline mode instead: since line order doesn't affect the
  result, it draws each submitted face immediately in `SubmitFace`
  rather than buffering a depth-sort chain at all. A new
  `ConfigSettings.ShipRenderMode` (`ShipRenderMode.Painter`/`ZBuffer`,
  defaulting `ZBuffer` — the current behaviour) selects between the two
  filled strategies; `EliteServiceCollectionExtensions`'s `IShipRenderer`
  registration now picks `WireframeRenderer`, `PainterRenderer`, or
  `ZBufferRenderer` based on `ShipWireframe`/`ShipRenderMode` at
  composition time, same as `Enum.IsDefined` validation as the other mode
  enums in `ConfigFile.IsValid`.

  Skipped the literal `FilledRenderer` wrapper the backlog item named:
  `ZBufferRenderer`/`PainterRenderer`, once stripped of their wireframe
  branch, already fully implement `IShipRenderer` as pure filled
  renderers, so a forwarding decorator would have added a class with no
  behaviour of its own. Also skipped a `SettingsView` toggle for the new
  setting — `ShipWireframe` itself has no in-game UI today either, so the
  config file remains the only way to switch (still satisfies "toggle
  without code changes").

  Added `VisualDumpTests.PainterAndZBufferRenderIdenticallyForNonDecalGeometry`,
  rendering a decal-free `Asteroid` model through both filled strategies
  and asserting pixel-for-pixel equality (passes exactly, no tolerance
  needed) — confirms the split didn't change either algorithm's actual
  behaviour. Also visually spot-checked `WireframeRenderer`'s output
  (a clean white ship outline, as expected).

### Changed (Z-buffer ship renderer renamed, 2026-07-20)

- Third of the ship-rendering-strategy items: `ShipRenderer` (the
  combined depth-sort/fill/wireframe behaviour extracted from `EliteDraw`
  earlier today) renamed to `ZBufferRenderer`, matching the
  `PainterRenderer` sibling added alongside it — no behaviour change,
  same registration in `EliteServiceCollectionExtensions.AddEliteMain`
  and the same direct construction in `PlanetBenchmarks`,
  `SunBenchmarks` and `VisualDumpTests`. The backlog item also asked to
  carry `ShipBase`'s face-root decal-inheritance logic
  (`FindFaceRoots`/`FaceMeanZ`) into this class; that logic stayed in
  `ShipBase` instead, deliberately deviating from the item's literal
  text — it computes the `z` value every `IShipRenderer.SubmitFace` call
  receives regardless of which strategy is active (`PainterRenderer`
  depends on the exact same computation), so moving it into
  `ZBufferRenderer` specifically would make `ShipBase` need to know which
  renderer is currently selected to compute a face's depth key, which
  defeats the point of the `IShipRenderer` abstraction landed earlier
  today. The open decal-seam defect this logic has (some decals lose to
  their base face at certain angles, from the 2026-07-14 z-buffer spike)
  is unchanged and untouched by this rename.

### Added (Painter's-algorithm ship renderer, 2026-07-20)

- Second of the ship-rendering-strategy items: a new
  `PainterRenderer : IShipRenderer` (`Graphics/PainterRenderer.cs`)
  restores the pre-2026-07-14-spike behaviour — a plain
  (non-depth-tested) `Graphics.DrawPolygonFilled` fill in back-to-front
  `_polyChain` order, instead of `ShipRenderer`'s per-pixel z-buffer test
  (`Graphics.DrawPolygonFilledDepth`) — as its own selectable
  implementation, without touching `ShipBase`'s face-transform code.
  Deliberately duplicates `ShipRenderer`'s chain-management code rather
  than factoring out a shared base class now, since the z-buffer item
  (still open) may reshape `ShipRenderer` further; `PainterRenderer` also
  skips the per-vertex `Depths` array entirely since the painter's fill
  never reads it. Not yet registered in the DI container — `ShipRenderer`
  stays the sole active implementation until the wireframe/filled item
  wires up config-driven selection between them. Verified with a
  throwaway test rendering the same interpenetrating-hulls scene as
  `VisualDumpTests`; both single-ship and intersecting-hull frames looked
  correct on visual inspection (not a pixel diff against the z-buffer
  path — that comparison is the wireframe/filled item's job).

### Added (Ship-rendering strategy abstraction, 2026-07-20)

- First of the ship-rendering-strategy items (backlog): a new internal
  `IShipRenderer` (`SubmitFace`/`StartFrame`/`EndFrame`,
  `Graphics/IShipRenderer.cs`) isolates the depth-sort/fill algorithm
  from `EliteDraw`, which previously hardcoded the back-to-front
  `_polyChain` chain and the `ShipWireframe` render-mode branch directly
  in `DrawPolygonFilled`/`RenderStart`/`RenderEnd` — the same fields the
  2026-07-14 z-buffer spike edited in place rather than toggled. Today's
  exact combined behaviour (chain, z-buffer fill, wireframe branch) moved
  unmodified into a new `ShipRenderer : IShipRenderer`
  (`Graphics/ShipRenderer.cs`); `PolygonData` moved into the same
  `Graphics` namespace/folder since it's now `ShipRenderer`-only.
  `EliteDraw`'s constructor takes an injected `IShipRenderer` and its
  three methods just delegate to it now. Registered as a singleton in
  `EliteServiceCollectionExtensions.AddEliteMain`, resolved before
  `EliteDraw`. This is pure extraction, not a split — the actual
  painter's/z-buffer/wireframe/filled separation is the three remaining
  backlog items. `PlanetBenchmarks`, `SunBenchmarks` and
  `VisualDumpTests` construct `EliteDraw` directly (not via DI) and
  needed a `ShipRenderer` passed in too; visually spot-checked its
  `frame_interpenetrate.bmp` dump (the two-hulls-intersecting per-pixel
  depth test) and it renders correctly, though this wasn't a pixel diff
  against a pre-change baseline.

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
