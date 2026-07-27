# The Sharp Kind

![Line coverage](docs/images/coverage-badge.svg)

Classic 8/16-bit computer games re-engineered in C# / .NET, sharing a common set of `Useful.*` game-engine libraries. The games are meant to look, feel and play like the originals while running cross-platform on modern hardware.

## Games

### [Elite - The Sharp Kind](docs/elite-readme.md)

![Elite - The Sharp Kind - Screenshot](docs/images/elite-screenshot.png)

Status: Playable, feature-complete.

### [Stunt Car Racer - The Sharp Kind](docs/scr-readme.md)

![Stunt Car Racer - The Sharp Kind - Screenshot](docs/images/scr-screenshot.png)

Status: Playable, preview.

## Getting started

Requires the [.NET SDK](https://dotnet.microsoft.com/download) (see `Directory.Build.props` for the target framework version).

```bash
# Elite
dotnet run --project src/elite/apps/EliteSharp

# Stunt Car Racer
dotnet run --project src/scr/apps/StuntCarRacerSharp
```

Both games can also be run and debugged directly from an IDE: open [TheSharpKind.slnx](TheSharpKind.slnx) in Visual Studio and set `EliteSharp` or `StuntCarRacerSharp` as the startup project, or open the repo root in VS Code and use the "Elite" / "Stunt Car Racer" launch configurations (`.vscode/launch.json`).

Tested platforms: Windows (x64), Ubuntu (x64), Raspberry Pi 4 (ARM64).

## SDL - Development Setup

Both games can be developed using either Visual Studio 2026 or VSCode.
For all platforms, ensure that the dotnet SDK is installed.
Start the `EliteSharp` or `StuntCarRacerSharp` project, which support most platforms, to get straight into the game.

The SDL3, SDL3_ttf and SDL3_mixer native libraries ship inside their respective NuGet packages (win-x64, linux-x64 and linux-arm64), so no manual install step is required on any platform — `dotnet run` or `dotnet build` is enough.

## Repository layout

- `src/useful/` — shared engine libraries (graphics, audio, input, assets, game loop) used by both games
- `src/elite/` — Elite: game library, app, tests, benchmarks
- `src/scr/` — Stunt Car Racer: game library, app, tests
- `docs/` — per-game readmes and project documentation

## Documentation

- [Architecture principles](docs/architecture-principles.md)
- [Backlog and roadmap](docs/backlog-roadmap.md) — the single consolidated backlog
- [Changelog](CHANGELOG.md)
- [Contributing](CONTRIBUTING.md)

## Licence

[MIT](LICENSE). Original game copyrights remain with their respective owners — see each game's readme for credits.
