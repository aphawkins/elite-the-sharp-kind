# Mission plugins — work log and handover

Turning Elite's two hardcoded missions into discoverable plugin parts.
Started 2026-08-02. **Not finished** — this file is the state of play, so the
work can be picked up in a new session without re-deriving any of it.

Nothing in this effort has been committed. `master` is untouched at
`dd1ede1a`, and the game behaves exactly as it did before.

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

### T1 — Contracts assembly — **built, one review round, rework unverified**

`EliteSharp.Missions.Abstractions`: 14 files, 748 lines, builds clean (0
warnings under `AnalysisMode=All` + `TreatWarningsAsErrors`). No project or
package references. Nothing from `EliteSharpLib` in its public surface.

**The code is in `docs/mission-plugins-contracts.patch`** (untracked). Apply
from the repo root:

```
git apply docs/mission-plugins-contracts.patch
```

It was produced in a throwaway agent worktree that may since have been pruned —
the patch is the durable copy. It was cut against `8a11e80f`, three commits
behind `master`, but touches only new files plus one line in `TheSharpKind.slnx`,
so it applies cleanly.

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

**Outstanding on T1:** the rework has had no independent review. The first
review found three high-severity problems; the second — checking the fixes hold
— died on a spend limit mid-run. Before trusting it, build an external probe
assembly against the built DLL and try: deriving from anything public, `with`
expressions bypassing validation, backwards/self steps, a zero-chance ambush, a
briefing with no paragraphs, mutating a handed-over list.

### T3 — Registry and discovery — **not started, blocked**

Blocked twice over: it needs T1 committed to `master` to be visible at all
(agent worktrees cannot see each other's uncommitted work — this cost a wasted
run), and the spend limit stopped further agent use.

Useful findings from the blocked attempt:

- `System.Composition` **10.0.x** matches the `Microsoft.Extensions.*` versions
  already in `Directory.Packages.props` (central package management is on, so
  the version goes there and the csproj reference carries none). Only
  `System.Composition.Hosting` and `System.Composition.TypedParts` are needed,
  not the metapackage.
- MEF2 has **no directory catalog** (unlike MEF1): enumerate the DLLs yourself
  and pass them to `ContainerConfiguration.WithAssemblies`.
- `src/elite/libs/EliteSharpLib/IMission.cs` — an `internal interface IMission
  { DrawBrief(); Update(); }` — is referenced by **nothing** and must be
  deleted. It sits in the `EliteSharpLib` namespace, so enclosing-namespace
  resolution would silently bind an unqualified `IMission` to it rather than the
  contracts one: a wrong-type bug with no compiler error. It was deleted only
  inside a discarded worktree, so **it is still there on master**.

## What is left

1. **T2 — `MissionProgress` + save wiring.** Stages become registry-validated
   strings; delete `Types/ConstrictorStage.cs`, `Types/ThargoidStage.cs`,
   `Types/MissionName.cs`. The save file's shape does **not** change —
   `"missions": { "Constrictor": { "stage": "Rewarded" } }` is already right —
   only its validation moves from the enums to the registry. A save naming a
   mission no plugin provides is rejected **and logged**.
2. **T3 — Registry and MEF2 discovery** (above). Plugin assemblies live in a
   `Missions` folder beside the executable; its absence is the normal case and
   must log cleanly, not throw. Duplicate mission names are a startup failure
   (Critical + throw with context); an unreadable DLL is a Warning and skipped.
   Take the base directory as an injected parameter so tests can point it
   elsewhere.
3. **T4 — One briefing screen.** `MissionBriefingController` plus one view per
   tier, keyed on what the briefing *contains* (headline? portrait? how many
   paragraphs?) rather than which mission it is. Collapses
   `Screen.MissionOne`/`MissionTwo`; `DockingView` and `EliteDraw` reference
   them.
4. **T5 — Port both missions to `IMission`**, still inside `EliteSharpLib`, to
   confirm the context suffices before publishing the assembly boundary.
5. **T6 — Extract to `EliteSharp.Missions.Classic`** and load it as a real
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
remaining tasks are ordinary sequential work. Suggested order — T1 (apply the
patch, probe it, commit), then T3, then T2, then T4, T5, T6, committing each
separately.

Two process notes that cost time here and need not cost it again: agent
worktrees are branched off whatever `master` was and cannot see each other's
uncommitted work; and `ELITE_DEBUG_COMMANDER` is set machine-wide on the
maintainer's box, so the game starts as Commander Max, not Jameson.
