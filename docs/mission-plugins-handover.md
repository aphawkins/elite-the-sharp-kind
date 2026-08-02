# Mission plugins — work log and handover

Turning Elite's two hardcoded missions into discoverable plugin parts.
Started 2026-08-02. **Not finished** — this file is the state of play, so the
work can be picked up in a new session without re-deriving any of it.

T1 and T3 are landed. Nothing calls them yet, so the game behaves exactly as it
did before.

## Why

Adding a mission today means editing: two enums in
`src/elite/libs/EliteSharpLib/Types/`, two controllers, two `Screen` values and
four tier-specific views. The maintainer's objection, which started this: the
mission stages are hardcoded and that is not conducive to adding more missions.
It relates to the backlog's **[LARGE] Data-driven game content model** item,
with missions as the pilot.

A mission carries *behaviour*, not just data — spawning a Constrictor, fitting
an energy unit, ambushing while carrying plans — so a config file cannot
express one without inventing a scripting language. A plugin assembly can, which
is why this is a plugin model and not a `Missions.json`.

## Decisions already taken

These were signed off by the maintainer and should not be silently revisited.

| Decision | Choice | Why |
|---|---|---|
| Discovery mechanism | **MEF2** (`System.Composition`) | Maintainer's call. MEF is for *discovery only* — parts get registered into the existing `ServiceCollection`. `architecture-principles.md` requires exactly one composition root using M.E.DI, so a `CompositionHost` must not own lifetimes. |
| First pass scope | Missions end-to-end, **including** an external assembly | Proves the seam for real. |
| Stage type safety | `MissionProgress` guard type | Stages become strings (compile-time enums cannot survive plugins); the guard refuses a stage the mission does not declare, so invalid stages are unreachable at runtime instead. |
| System identity | **Planet number** | Seed bytes (`d`/`b`) were judged the one genuine leak in the public surface — two of six seed bytes, undiscoverable by a plugin author. |
| Equipment/portrait vocabularies | **Narrow to what exists** | No name-based equipment lookup exists in the game; a plugin cannot ship an asset. Small enums instead of invented string namespaces. |
| Save naming an unknown mission | **Reject the save, and log it** | Consistent with the existing save validation. Note the consequence: removing a plugin makes affected saves unloadable. |
| Briefing screens | **Collapse** `Screen.MissionOne` and `MissionTwo` into one | A plugin mission cannot draw itself otherwise. Touches `DockingView` and `EliteDraw`. |

## Progress

### T1 — Contracts assembly — **done**

`EliteSharp.Missions.Abstractions`: 14 files, 748 lines, builds clean (0
warnings under `AnalysisMode=All` + `TreatWarningsAsErrors`). No project or
package references. Nothing from `EliteSharpLib` in its public surface.

Public surface, in brief:

```csharp
public interface IMission
{
    string Name { get; }                  // save-file key; renaming breaks saves
    MissionStages Stages { get; }         // the only thing that can build a step
    MissionStep? Advance(IMissionContext context, string stage);
}

public interface IMissionContext          // read-only in its entirety
{
    int CombatScore { get; }
    int GalaxyNumber { get; }
    int CurrentPlanetNumber { get; }
    bool IsDocked { get; }
    string? StageOf(string missionName);  // null = no such mission installed
}
```

Plus segregated optional interfaces a mission implements only if it needs them:
`IMissionEncounters` (`Ambush` / `LoneWolfSubstitute`), `IMissionKills`
(`ShipDestroyed`), `IMissionPlanetDescriptions` (`DescribePlanet`).

Design points worth not undoing:

- **Awards ride on `MissionStep`**, and `IMissionContext` is read-only. An
  earlier draft paid through the context *before* the stage was committed, so a
  replayed screen entry could collect the Constrictor's 5000 credits twice.
- **`MissionStep`'s constructor is `internal`**; steps come only from
  `MissionStages.Step(current, next, ...)`, which throws unless both stages are
  declared and the move goes forwards. A step to itself, or backwards, cannot be
  built.
- **No `MissionEncounter` base type.** An earlier draft had an abstract record
  with a `private protected` constructor, believed closed. Records emit a
  `protected` copy constructor that any assembly can chain to, so an external
  plugin could derive a third kind that lied about its `Kind` and made the
  game's cast throw. Narrowing it is CS8878. It was deleted in favour of two
  methods returning two sealed types. **Do not reintroduce a shared base.**
- `ImmutableArray<string>` with hand-written equality, because records holding
  `List<string>` have no value semantics and let a plugin mutate a briefing
  after handing it over.

The guards were probed from outside the assembly, which is the only place the
claims mean anything. `EliteSharp.Missions.Abstractions.Tests` is that probe
made permanent: it references the contracts and nothing else, has no access to
their internals, and covers the runtime refusals — backwards and self steps,
steps naming an undeclared stage, a zero-chance ambush, a nameless ship, an
award worth nothing, a briefing with no paragraphs or a blank one, a blank
headline, `with` on a briefing re-running the checks, and handed-over lists that
go on being edited. 39 tests, all passing.

The rest is what the compiler refuses, which no test can assert. A throwaway
assembly compiled against the built DLL confirmed each of these, and they are
the reason the shapes are as they are:

- `new MissionStep(...)` — CS1729, the constructor is internal, so steps come
  only from `MissionStages`.
- `step with { Stage = ... }`, and the same on `AmbushEncounter`,
  `LoneWolfEncounter` and `MissionAward` — CS0200, every property is get-only,
  so `with` cannot route round a validating constructor.
- Deriving from `MissionStep`, `MissionStages`, `MissionBriefing`,
  `AmbushEncounter` or `LoneWolfEncounter` — CS0509, all sealed.

### T3 — Registry and discovery — **done**

Two types in `EliteSharpLib/Missions/`:

- **`MissionLoader`** — `LoadFrom(baseDirectory, logger)`. Everything MEF
  touches happens inside it and is finished with before it returns, so the
  composition host owns no lifetimes and the game keeps its one M.E.DI
  composition root. An absent `Missions` folder logs Information and returns
  nothing; a file that will not load logs Warning and is skipped.
- **`MissionRegistry`** — the missions by name, `All` and `Find`. Two missions
  of one name log Critical (naming both assemblies) and throw, because a save
  file naming it could mean either.

Registered in `AddEliteCore`, off `AppContext.BaseDirectory`. Nothing resolves
it yet, so a test builds the game's real composition and asks for it, or the
registration would go untried until T2.

Two things worth keeping:

- **Missions are exported by convention, not by attribute.** A
  `ConventionBuilder` exporting types derived from `IMission` is what lets a
  plugin reference the contracts assembly *and nothing else* — an
  `[Export]` attribute would have made every plugin author reference MEF. A
  mission is a public class implementing `IMission` with a constructor taking
  no arguments.
- **`AssemblyLoadContext.Default.LoadFromAssemblyPath`, not
  `Assembly.LoadFrom`** — the latter trips Sonar S3885, and the former is the
  right API for loading by absolute path anyway.

`EliteSharp.Missions.TestPlugin` is a real plugin, built like a third party's:
contracts only, no reference to the game or to MEF. `EliteSharpLib.Tests`
builds it, does **not** reference it (`ReferenceOutputAssembly="false"`), copies
the DLL into a temp plugin folder and makes the loader find it. That is the only
way to know the seam works, and it is the template for T6.

The dead `EliteSharpLib/IMission.cs` is gone. It was referenced by nothing and
sat in the `EliteSharpLib` namespace, where enclosing-namespace resolution would
have silently bound an unqualified `IMission` to it rather than the contracts
one — a wrong-type bug with no compiler error.

Also from the earlier blocked attempt, still true: `System.Composition`
**10.0.x** matches the `Microsoft.Extensions.*` versions already in
`Directory.Packages.props`, and MEF2 has **no directory catalog** (unlike MEF1),
so the DLLs are enumerated by hand and passed to
`ContainerConfiguration.WithAssemblies`.

## What is left

1. **T2 — `MissionProgress` + save wiring.** Stages become registry-validated
   strings; delete `Types/ConstrictorStage.cs`, `Types/ThargoidStage.cs`,
   `Types/MissionName.cs`. The save file's shape does **not** change —
   `"missions": { "Constrictor": { "stage": "Rewarded" } }` is already right —
   only its validation moves from the enums to the registry. A save naming a
   mission no plugin provides is rejected **and logged**.
2. **T4 — One briefing screen.** `MissionBriefingController` plus one view per
   tier, keyed on what the briefing *contains* (headline? portrait? how many
   paragraphs?) rather than which mission it is. Collapses
   `Screen.MissionOne`/`MissionTwo`; `DockingView` and `EliteDraw` reference
   them.
3. **T5 — Port both missions to `IMission`**, still inside `EliteSharpLib`, to
   confirm the context suffices before publishing the assembly boundary.
4. **T6 — Extract to `EliteSharp.Missions.Classic`** and load it as a real
   plugin.

## Consequences to plan for

- **Planet numbers must be translated.** The port cannot copy the old literals:
  Ceerdi (215, 84) and Birera (63, 72) in galaxy 2, and the Constrictor's
  hunting ground (144, 33) in galaxy 1 are seed bytes. `FindPlanetNumber` gives
  the numbers game-side. `FindPlanetNumber` is a linear scan, so cache the
  current planet's number when it changes rather than computing it per encounter
  check.
- **`MissionJump` needs rewriting.** It currently fakes Ceerdi by overwriting
  the docked planet's seed; it will have to set a real system.
- **A live bug to fix on the way.** `Missions/Mission.cs` returns rumour text
  only when docked *and* describing the docked system, but
  `PlanetDataController` calls it with the **hyperspace target**. So the
  Reesdice rumour appears whenever planet 150 is selected on the chart from
  anywhere in galaxy 0. The contract expresses the correct rule; the port should
  fix the behaviour rather than reproduce it.
- **Trimming and AOT are foreclosed.** MEF2's discovery is reflection over
  externally-supplied assemblies. Nothing configures trimming or AOT today so
  this costs nothing now, but plugin discovery is fundamentally incompatible
  with it. Relevant if the plugin model later spreads to views and stock.
- **Unused public API in the tree.** Landing T1 and T3 alone leaves a contracts
  assembly and a registry that nothing calls, which cuts against the "nothing
  speculative" rule in `AGENTS.md`. Deliberate, but do not leave it parked
  indefinitely.

## Continuing without agents

The tiered pipeline (`.claude/skills/tier/SKILL.md`) was being followed by hand
because the `/tier` skill is not registered as invocable. It is not needed: the
remaining tasks are ordinary sequential work.

Two process notes that cost time here and need not cost it again: agent
worktrees are branched off whatever `master` was and cannot see each other's
uncommitted work; and `ELITE_DEBUG_COMMANDER` is set machine-wide on the
maintainer's box, so the game starts as Commander Max, not Jameson.

Remaining order: T2, then T4, T5, T6, committing each separately.
