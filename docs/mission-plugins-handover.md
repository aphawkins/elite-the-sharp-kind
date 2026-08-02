# Mission plugins — work log and handover

Turning Elite's two hardcoded missions into discoverable plugin parts.
Started and **finished** 2026-08-02. This file is the record of what was
decided and why; the design points in it are worth keeping, the task list is
history.

All six pieces landed. The missions are in `EliteSharp.Missions.Classic`, an
assembly the game does not reference, found in a `Missions` folder beside the
executable at startup. A normal run now logs `Loaded 2 mission(s) from 1 plugin
assemblies.`

## Why

Adding a mission used to mean editing: two enums in
`src/elite/libs/EliteSharpLib/Types/`, two controllers, two `Screen` values and
four tier-specific views. The maintainer's objection, which started this: the
mission stages are hardcoded and that is not conducive to adding more missions.
It relates to the backlog's **[LARGE] Data-driven game content model** item,
with missions as the pilot.

A mission carries *behaviour*, not just data — spawning a Constrictor, fitting
an energy unit, ambushing while carrying plans — so a config file cannot
express one without inventing a scripting language. A plugin assembly can, which
is why this is a plugin model and not a `Missions.json`.

## Decisions taken

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

## What was built

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

Registered in `AddEliteCore`, off `AppContext.BaseDirectory`, and resolved for
real since T2 — `GameState` needs it to build the commander, so a normal startup
now logs how many missions it found, or that there is no plugin folder at all.

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

### T2 — `MissionProgress` and save wiring — **done**

The three enums are gone. `Commander.Missions` is a `MissionProgress`: stage
names as strings, guarded by the registry, so `MoveTo` refuses a mission nobody
installed and a stage that mission never declared. `IsAt` is what nearly every
caller wants and keeps stage names from being compared the wrong way.

**T2 could not be done before the missions existed as registry entries.** The
handover's order had T2 before T5, but deleting the enums empties the stage
vocabulary, and with an empty registry every save is rejected. So
`ConstrictorMission` and `ThargoidMission` landed here as built-ins that declare
their stages — using the old enum's names, so existing saves still load — and
whose `Advance` returns null for now. Their behaviour is still in the two
controllers and is T5's job. `ClassicMissions` is the list, and the registry is
built-ins plus whatever the plugin folder holds.

`GameState`'s constructor takes the registry, because it builds the commander.
That is why ~25 test and benchmark call sites changed; they pass
`ClassicMissions.Registry()`.

One deliberate change of shape: **the save file now holds only the stages that
have been reached.** A fresh commander writes `"missions": {}`. It was going to
need this anyway — a save written with a plugin installed and loaded without it
(or the reverse) cannot require every installed mission to be present. Absence
now reads as not started; a mission the save *names* that nothing provides is
still rejected and logged, as decided.

### T4 — One briefing screen — **done**

`Screen.MissionOne` and `MissionTwo` are one `Screen.MissionBriefing`, drawn by
one `MissionBriefingView8Bit`/`16Bit` from one `MissionBriefingModel` — a
headline (or none), the paragraphs, and whether somebody is pictured. The view
lays out from that and never from which mission or stage it came, which is what
lets a plugin's mission draw itself.

`MissionBriefingController` replaces both controllers. On docking it asks each
mission in turn for a briefing and the first with one gets the screen; space
asks the ones after it, which is how the two screens used to chain into each
other and how one docking can still earn both messages. The message *sequences*
are still in the controller — that is T5's to move — but the screen no longer
knows whose message it is drawing.

Two things fell out of the collapse:

- **The universe had to be emptied rather than not drawn.** `EliteDraw` used to
  let the universe through on `MissionOne` and not on `MissionTwo`, which is
  what kept ships off the Thargoid screens. With one screen there is no such
  distinction, so a briefing with nothing posing behind it clears the universe
  as it is built.
- **One deliberate visual change, on the 8-bit tier only.** The two debriefs had
  drifted apart — the Constrictor's headline sat at row 6 with its text at row
  8, the Thargoid's at row 3 with its text at row 13. One rule cannot produce
  both, so both now use the Thargoid's, and the Constrictor debrief's text
  starts lower. The 8-bit layouts are first-draft pending the roadmap's "Author
  the 8-bit view layouts" item, so this is within what that item will revisit.
  **16-bit is unchanged**, verified screen by screen: its two views were already
  laying out identically, since the `ViewportLeft` one of them added is always
  zero.

### T5 — Missions carry their own behaviour — **done**

`ConstrictorMission` and `ThargoidMission` now implement `Advance` and the
optional interfaces they need, and read the game only through
`IMissionContext`. **The context turned out to be enough** — nothing had to be
added to it, which was the question T5 existed to answer.

`MissionRunner` is the one seam. The briefing screen, `Combat` and
`PlanetDataController` all go through it, and applying a step (record the
stage, pay the award) is written once, so a reward cannot be collected for a
stage that was not taken.

What moved, and what it cost:

- **Planet numbers, at last.** The missions name systems by number:
  Orarra 193 in galaxy 1, Ceerdi 83 and Birera 36 in galaxy 2. `PlanetAt` is
  the new inverse of `FindPlanetNumber`, and `MissionsTests` checks the three
  numbers still name the right systems.
- **`MissionJump` is a real jump now.** It used to fake Ceerdi by overwriting
  two of the docked planet's six seed bytes, which no longer identifies
  anything. It sets the galaxy seed and the docked planet properly, so the
  chart, the planet name and the data screen agree afterwards — visible in the
  short-range chart after the Thargoid debrief, which now reads BIRERA.
- **`CurrentPlanetNumber` is memoised.** Finding a planet's number is a scan of
  all 256, and encounter checks ask on every tick, so `MissionContext` keeps the
  answer until the seed or galaxy changes.
- **The rumour bug is fixed.** The mission answers only when docked *and* asked
  about the system underfoot, so Reesdice is no longer named from anywhere in
  galaxy 0. There is a test for it.
- **The Thargoid systems are now galaxy-checked.** The old seed-byte comparison
  ignored the galaxy; planet numbers only mean anything within one, so the
  mission checks it. That is a tightening, not a regression.
- **The ambush roll flipped from `>= 200` to `< 56`.** Identical odds and the
  same one RNG draw, but a seeded run picks different outcomes.
- **`Combat` is `partial`.** It was already at the analyzers' 1000-line limit,
  so the mission-facing spawn lives in `Combat.Missions.cs`.

### T6 — The missions become a plugin — **done**

`EliteSharp.Missions.Classic` holds both missions and references the contracts
and nothing else. The game does **not** reference it: the app's build drops the
DLL into `$(OutDir)Missions`, and `MissionLoader` finds it there at startup like
any other. There is no built-in mission list left — `ClassicMissions` is gone,
and the registry is exactly what the plugin folder holds.

- **The stage and mission names went `internal`**, with `InternalsVisibleTo` for
  the tests. Sonar objects to public constants, and nothing outside needs them:
  the game reads names off `IMission`.
- **`MissionJump` spells the names out**, since it can no longer reference the
  types, and does nothing for a mission nobody installed. It is the only place
  in the game that names a specific mission, and it is a debug cheat.
- **`EliteSharpLib.Tests` copies the plugin into its own `Missions` folder**, so
  the tests that build the game's real composition find their missions the way
  the game does rather than being handed them.

**The consequence to remember: delete the `Missions` folder and the game has no
missions at all**, and any commander file part-way through one is refused. That
is the "removing a plugin makes affected saves unloadable" line in the decisions
table, now real. The build puts the folder there for both `Build` and `Publish`.

## Consequences to plan for

- **Trimming and AOT are foreclosed.** MEF2's discovery is reflection over
  externally-supplied assemblies. Nothing configures trimming or AOT today so
  this costs nothing now, but plugin discovery is fundamentally incompatible
  with it. Relevant if the plugin model later spreads to views and stock.

## Notes for next time

The rest of this was done by hand rather than through the tiered pipeline, and
did not need it: once the contracts were settled the work was ordinary and
sequential.

Two process notes that cost time and need not cost it again: agent worktrees are
branched off whatever `master` was and cannot see each other's uncommitted work;
and `ELITE_DEBUG_COMMANDER` is set machine-wide on the maintainer's box, so the
game starts as Commander Max, not Jameson.
