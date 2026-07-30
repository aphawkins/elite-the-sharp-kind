## Architecture

* Old games from 8 and 16 bit computers ported over to dotnet and C#.
* Games should retain their original look and feel, while also taking advantage of modern hardware and software capabilities.
* Prefer dotnet framework intrinsics and libraries, over third party libraries.
* As much code as possible should be in the 'Useful' libraries and only game specific code should be in the game projects themselves.
* The highest possible coding standards should be used.
* The code focus should be on correctness, security, maintainability, readability, and performance.
* Code should adhere to DRY (Don't Repeat Yourself), YAGNI (You Aren't Gonna Need It), KISS (Keep It Simple, Stupid), and SOLID principles.
* Technical debt should be prioritised over new features.
* Development prioritisation technique should use the MoSCoW method
* Embrace continuous refactoring, continuous automated unit testing, and continuous integration.
* OOP principles should be used where appropriate, but not at the expense of performance or simplicity.
* The game engine should be designed to be as modular and extensible as possible, allowing for easy addition of new features and functionality.
* The game engine may differ from established engines in that it may be architected more like a business application, allowing developers from a business application background to more easily understand and contribute to the codebase.
* Unit testing, integration testing, and end-to-end testing should be used to ensure the quality and reliability of the codebase.
* Performance profiling and optimization should be an ongoing process throughout development, with a focus on identifying and addressing performance bottlenecks early in the development cycle.
* Code should be cross-platform, with support for Windows, Linux, x64, and ARM64 architectures. 32 bit architectures are not supported.

## Business application practices

The engine is architected like a business application; these are the house rules that follow from that.

### Composition and dependency injection

* Each executable has exactly one composition root: `Program.Main`, using `Microsoft.Extensions.DependencyInjection` (a plain `ServiceCollection`; the full Generic Host is not needed). What is identical between the executables — resolving the user-data path, standing up logging, running the game, reporting a failure — lives in `Useful.App`'s `GameApp.Run`; each `Main` supplies only its own title, log file and service registrations.
* Everything below the composition root receives its collaborators by constructor injection. Domain classes never `new` up their own dependencies (no `AssetLocator.Create()` or `new ConfigFile()` inside game classes) and never reach through a god object for them — a class's constructor signature is the honest list of what it uses.
* The container owns lifetimes and disposal: `IAbstraction`, graphics/sound/keyboard, configuration and asset services are singletons disposed by the provider in reverse order. Never call `Environment.Exit` to end the game; return from the loop and let `Main` unwind.
* No two-phase construction: an instance must never be observable half-built. Fold `Initialize(...)` methods and post-construction property mutation into constructors or static factories.

### Logging

* Libraries depend only on `Microsoft.Extensions.Logging.Abstractions` and accept `ILogger<T>` by constructor. The logging provider (Serilog) and sink configuration are app-level policy: they live in the app layer — `Program.Main` or the shared `Useful.App` host it delegates to — and no game or engine library references Serilog.
* Log messages use source-generated `[LoggerMessage]` partials (allocation-free, compile-checked) — the existing app `LogMessages.cs` pattern, replicated per library.
* Log at the edges, not per frame: startup and asset loading (Information), config/save read-write outcomes (Information/Warning), recoverable oddities (Warning), failures and top-level handlers (Error/Critical). The frame path stays logging-free.
* App sinks: console plus a rolling file (with a retained-file limit), minimum level Information, raisable via environment variable. A Debug-only sink is not acceptable as the sole sink — a crash on a player's machine must leave a trace.
* `System.Diagnostics` is not a logging channel: `Debug.Assert` is fine for internal invariants, but anything reporting an operational failure (`Debug.Fail`, `Debug.WriteLine`) must become a thrown exception or a logged Warning/Error — Debug calls compile away in the builds players run.

### Configuration and user data

* Settings are read via `Microsoft.Extensions.Configuration` (JSON provider) bound onto plain options classes, with defaults supplied by the POCO and validation at startup; an invalid or missing file logs a Warning and falls back to defaults, never crashes.
* Writing settings stays behind a small interface seam (M.E.Configuration is read-only), so views depend on the seam, not a file class.
* User data (settings, save files) lives under `Environment.SpecialFolder.ApplicationData`, not the current working directory, with the base path injected so it can be redirected in tests. Filenames derived from user input are sanitised before use.

### Error handling

* Fail fast at startup with a logged Critical and an actionable message (missing assets, SDL init failure).
* Never catch-all on the frame path; catch the narrowest type that can actually occur and always log what was caught.
* Degrade gracefully for user data: config and save load failures return defaults/false with a Warning, they do not throw past the caller.
* Exceptions always carry context (`throw new EliteException($"Unknown ship roll {rnd}")`, never a bare `throw new EliteException()`).

### Testability seams

* The three ambient dependencies are always injectable: time (`TimeProvider`, as `GameLoop` already does), the file system (an injected base path at minimum), and randomness (an injected, seedable random source — never a static mutable RNG).
* Every library has a corresponding Tests project and, where consumers need doubles, a Fakes project; new logic lands with tests against those seams.

### Interfaces and API surface

* Keep producer and consumer roles on separate interfaces (interface segregation): the code that feeds input events and the code that polls key state should not share one interface.
* An interface member that a primary implementation cannot honour (silent no-op) is a design error: implement it or remove it.
* Prefer framework intrinsics over home-grown equivalents (`ArgumentNullException.ThrowIfNull` over a custom `Guard`), per the third-party-libraries principle above.

### Screens: controllers, models and per-tier views

The rules the 2026-07-29/30 controller/view split settled on. They apply to
every Elite screen and to any new one.

* Each screen is a `<Screen>Controller` (behaviour: `Reset`/`Update`/`HandleInput`, and all formatting), a `<Screen>Model` record (everything the screen draws, including its fixed wording), and one drawing-only view per tier. `ScreenManager` holds controllers, never views.
* One view interface: `IView<TModel> { void Draw(TModel model); }`. **Do not reintroduce a parameterless `IView`** — it was tried and removed on 2026-07-29: two screens needed it at once and a single registration can only serve one, with the last silently winning. Give a screen a model even when the model is only its wording.
* The three properties to preserve: behaviour has one home per screen, both tiers provably render the same data, and views are stateless. This is not MVC, MVP or MVVM and shouldn't be described as any of them (MVVM was considered and rejected: its value is binding and change notification, and Elite is immediate-mode with a full redraw per tick and polled input). The `Controller` suffix matches `PlanetController`/`AudioController` house style.
* Views live in `Views/EightBit/<Screen>View8Bit.cs` and `Views/SixteenBit/<Screen>View16Bit.cs`, one per tier, and the `IView<TModel>` registration picks between them on `IAssetLocator.Tier`. The tier contract is a signature, not a convention: both views take the same model, so they can differ only in coordinates and font, and the formatting stays testable without a renderer.
* Label text is chrome the view supplies, not model content, so each tier picks its own wording ("System:" at 8-bit against "Present System:" at 16-bit). Column-alignment format specifiers (`{qty,2}`) also stay in the view; a number the original drew as one indivisible string is content and belongs in the model.
* The 8-bit tier is a 320x256 canvas with a fixed 8x8 font — 40 characters per row, full stop — so its layouts are authored fresh rather than scaled down from the 16-bit one. Check a layout against the *shape* of its content (the fully-equipped ship, the longest credit line), not against whatever one screenshot happens to show.
* Screenshot diffing cannot prove "pixel-identical" on this app: two runs of the same build differ by ~2,000 pixels (window occlusion and the animated dashboard). Verify layout equivalence by algebra plus renderer-free controller tests, and use screenshots only to confirm nothing gross broke.
* Screens with nothing to extract stay plain `IScreenController` implementations rather than being split for consistency — `DockingView`/`HyperspaceView`/`LaunchView` all draw a shared, resolution-independent `BreakPattern` and have no formatting and no tier variance.
* The per-tier DI registrations live in `EliteSplitScreensServiceCollectionExtensions` and `EliteSplitAnimatedScreensServiceCollectionExtensions`, subdivided into `AddSplit*Screens` methods. Expect CA1506's class-coupling limit to force another method — or another static class, since the metric is per class — every few screens as views are added; that is the intended way to absorb it, not a suppression.

### Solution hygiene

* NuGet package versions are managed centrally (`Directory.Packages.props`); shared `PackageReference` blocks (analyzers) live in `Directory.Build.props`/`.targets`, not copy-pasted per project.
* Generated artifacts (benchmark reports, build output) are not committed.
* Avoid `#pragma warning disable` and `[SuppressMessage]`: fix the underlying code first. If a diagnostic is a genuine, verified false positive for this codebase (not just this call site), configure its severity in `.editorconfig` instead — repo-wide, or project-scoped via a nested `.editorconfig` when the false positive is specific to one assembly (e.g. `unsafe` interop code) — rather than scattering local suppressions. Reach for a local suppression only when neither a code fix nor an `.editorconfig` change is possible, and justify it with a comment.

