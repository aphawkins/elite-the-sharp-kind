# Backlog — Issues

Defects in shipped behaviour: things that are **wrong**, not things that are
missing. Prioritised with MoSCoW (per
[architecture-principles.md](architecture-principles.md)).

Both games work. The repo-wide priority is therefore to close what is broken
here before starting anything in [backlog-roadmap.md](backlog-roadmap.md) —
features, refactors and cleanups wait on this file being empty of Musts.

How to use this file:

- An item belongs here only if the current code produces a wrong result, drops
  work silently, or contradicts its own documented intent. Anything that is
  merely absent, slower than ideal, or nicer-if-restructured belongs in
  [backlog-roadmap.md](backlog-roadmap.md).
- Each item is one concern, small enough for a single focused session, and
  self-contained (paths, problem, fix direction). Items too big for one
  session are tagged **[LARGE]** — split them in a follow-up conversation
  before starting, not here.
- Definition of done: build the full solution, run the complete test suite,
  and smoke-test the affected app(s) live if the change touches shared code
  or either game loop. A defect fix should also arrive with a test that fails
  without it, wherever the seam allows one.
- When an item completes, delete it here and record it in
  [CHANGELOG.md](../CHANGELOG.md).

## Decisions

Maintainer decisions live in [decisions.md](decisions.md), not here — check
there before starting an item that mentions a decision.

## Must

### Layout

- [ ] [EliteSharpLib] **The 16-bit screens are still laid out for 512 on a
      640-wide tier.** Their absolute coordinates were authored against
      512x512 and never revisited when the tier widened to 640x512 on
      2026-07-30, while headers, prompts and the scanner are drawn about the
      viewport's centre — so the two are 64 pixels apart at the resolution
      the game actually ships. Visible on the market table, planet data,
      equipment list, inventory, commander status, the galactic chart's star
      field, the mission briefing screen (`MissionBriefingView16Bit`'s
      single-block position at `new(116, 132)` and the portrait at
      `new(352, 46)`) and the save screen's name box, which sits left of the
      name it frames.
      Fix by **re-laying-out those screens against 640** — widening the
      tables and moving the columns out to use the width. Per
      [decisions.md](decisions.md) (2026-08-01) the tiers are fixed-width,
      so this is a one-off re-authoring against a known resolution, *not* a
      centred content band, a design-space origin, or any other mechanism
      inside `ViewLayout`. Reach the mission screens with Ctrl-M
      under `ELITE_DEBUG_MISSIONS` (see
      [elite-readme.md](elite-readme.md#environment-variables)); they are
      otherwise hours of play away.

- [ ] [EliteSharpLib] **Pack-hunter spawns are missing Cobra Mk III and use
      the wrong probability shape** — original `mt1`
      ([elite-source-flight.asm:24917-24974](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      picks from 8 ship types (Sidewinder, Mamba, Krait, Adder, Gecko,
      Cobra Mk I, Worm, Cobra Mk III) via `CPIR`, the AND of two random
      bytes reduced to 0-7 — explicitly documented as biased toward smaller
      indices, so Sidewinder is common and Cobra Mk III rare.
      `ShipFactory.CreatePackHunter()` (`ShipFactory.cs:115-129`) has only 7
      options (no Cobra Mk III at all) chosen via a flat `_rng.Random(7)`.
      Two separate problems in the same spot: a ship type missing entirely,
      and a uniform distribution where the original is deliberately skewed.
      (Note: `CreateLoneWolf`'s inclusion of the Moray, which looked similar
      at first glance, is *not* a bug — the original's own disassembly
      comments at asm:24838-24862 call the Moray's unreachability via that
      path "presumably a bug" and suggest almost exactly the fix
      `CreateLoneWolf` implements, so that one is a deliberate improvement,
      not a divergence.)

- [ ] [EliteSharpLib] **A legal status of exactly 50 shows "Offender"
      instead of "Fugitive"** — original status screen
      ([elite-source-docked.asm:6270-6277](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-docked.asm))
      does `CPY #50` then branches on carry, so **50 and above** is
      Fugitive (0 = Clean, 1-49 = Offender, 50+ = Fugitive, per the
      original's own comment at asm:1419-1428). `LegalStatusBand.For`
      (`LegalStatusBand.cs:21-23`) uses `bounty > FugitiveBounty` (i.e.
      `> 50`), so a bounty of exactly 50 is misclassified as "Offender".
      One-character fix (`>` to `>=`) once confirmed.

- [ ] [EliteSharpLib] **A full cargo hold shouldn't damage the ship when
      scooping, but the port applies full collision damage anyway** — the
      original has two distinct outcomes: can't scoop at all (canister
      above the ship, or no fuel scoop fitted — `MA58`,
      [elite-source-flight.asm:3157-3176](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      takes full collision damage (`128 + energy/2` via `OOPS`); tried to
      scoop but the hold is full (`tnpr1` fails → `MA59`, asm:3123-3129)
      only plays a destruction sound and removes the canister — **no damage
      at all**. `ScoopItem` (`Combat.cs:346-353`) collapses all three
      conditions (`!HasFuelScoop || obj.Location.Y >= 0 ||
      TotalCargoTonnage() == CargoCapacity`) into one branch that always
      applies `128 + obj.Energy/2` damage, so a full hold now deals the
      same heavy damage as a genuine bad-angle collision. The scooped-item
      type mapping elsewhere in the same method was checked and is correct
      (cargo canisters, escape capsules → Slaves, Tharglets → Alien Items
      all match the original's per-ship-type values).

- [ ] [EliteSharpLib] **Fuel scooping gains roughly 40x too much fuel per
      tick** — original's scoop-and-temperature block
      ([elite-source-flight.asm:3855-3867](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      adds `speed/8` to the ×10-scaled fuel counter, i.e. **`speed/80`**
      actual light years per scoop tick — explicitly documented as
      "between 0.1 and 0.5 light years" across the speed range 1-40.
      `UpdateCabinTemp` (`Space.cs:380`) does `_ship.Fuel += _ship.Speed / 2`
      directly in real light-year units, i.e. **`speed/2`** per tick — 40x
      the original rate (0.5 LY vs 20 LY at max speed, against a 7 LY tank).
      The calling cadence was checked and matches exactly
      (`EliteMain.cs:255`, `(State.MCount & 31) == 20`, same as the
      original's gating), so this isn't a frequency mismatch — it's the
      per-tick formula. Everything else in the same routine (ambient temp
      30, scoop threshold 224, overheat/death behaviour, max fuel cap of 7)
      was checked and matches.

- [ ] [EliteSharpLib] **Witchspace should always spawn exactly 4 Thargoids,
      not a random 1-4** — original `MJP1`
      ([elite-source-flight.asm:17372-17383](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      loops spawning Thargoids via `GTHG` until the count exceeds 3, always
      ending with exactly 4 — deterministic, not random. `EnterWitchspace`
      (`Space.cs:827`) does `int nthg = _rng.Random(1, 5);` and spawns that
      many, giving a random 1-4 instead. A misjump should reliably be a
      "four Thargoids" ambush every time; the port sometimes gives only
      one. (Same code independently confirms the stardust-count finding
      already on this list: the original sets `NOSTM = 3` for witchspace
      right after this loop.)

- [ ] [EliteSharpLib] **Asteroid loot drops are wrong on two counts: Pulse
      lasers shouldn't yield splinters, and asteroids should still be able
      to drop alloy/cargo** — original kill-loot logic
      ([elite-source-flight.asm:3291-3314](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm)):
      splinters only drop if the kill shot's laser power exactly equals the
      mining laser's value (`CMP #Mlas`) — Pulse-laser kills get none. Then,
      unconditionally, *every* kill (including asteroids) falls through to
      `nosp` and attempts to spawn alloy plates and cargo canisters
      (`SPIN PLT`/`SPIN OIL`) regardless of ship type — the splinter check
      is an addition on top of that, not an alternative to it.
      `DestroyTarget` (`Combat.cs:566-582`) instead does `if (Asteroid) {
      splinters if Mining OR Pulse } else { alloy + cargo }` — an asteroid
      kill in the port can only ever yield splinters (never alloy/cargo),
      and any other ship kill can never yield splinters, when the original
      has no such exclusivity and gates splinters on Mining specifically.

- [ ] [EliteSharpLib] **Police-spawn chance factors in legal status under
      an inverted condition** — original
      ([elite-source-flight.asm:24683-24726](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      ORs the player's legal status into the spawn-chance roll only when
      police are *already present* in the local bubble (`LDX MANY+COPS;
      BEQ P%+5` skips the OR when the count is zero) — modelling "they've
      already scanned you." `CheckForPolice` (`Combat.cs:902-908`) has
      `if (_universe.PoliceCount == 0) { offense |= LegalStatus; }` — the
      exact opposite condition, applying legal status only when *no*
      police are nearby instead. One-word fix (`==` to `!=`) once
      confirmed.

## Should

- [ ] [EliteSharpLib] Number of stars is not proportional to screen area
      (issue #4): the 8-bit tier renders at 320x256, a quarter the area of
      512x512, with the same star count, so the starfield is four times as
      dense; the 16-bit tier's 640x512 changes it again. Scale star count by
      screen area. Sequence after the Elite non-512x512 audit in
      [backlog-roadmap.md](backlog-roadmap.md) so the coordinate space it
      scales against is settled.

- [ ] [EliteSharpLib] **Firing-cone thresholds vs original `TACTICS` may be
      off** — `AttackTactics` (`Combat.cs:659-664`) gates laser consideration
      at `direction <= -0.833`, looser than the 6502 original's `CPX #160`
      gate (X <= -32 of a max magnitude 36, i.e. ≈ -0.889 normalized —
      [elite-source-flight.asm:9780](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm)).
      `FiringTactics` (`Combat.cs:714-734`) then sets the `Firing` flag at
      `-0.917`, a third threshold with no counterpart in the original, which
      uses the *same* gate for "may fire" and "sets firing flag." Only the
      final hit threshold (`-0.972`, matching original `CPX #163`/X<=-35) is
      confirmed faithful. Net effect if real: enemy ships enter "about to
      fire" state across a wider cone than the original. Not yet confirmed —
      needs someone to re-derive the exact normalized thresholds and trace
      `AttackTactics`'s callers.
- [ ] [EliteSharpLib] **Possible missing 80-point splash damage when a
      missile self-destructs near the player** — original `TA35`/`TA87`
      ([elite-source-flight.asm:9219-9245](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      applies 80 damage to the player whenever a missile is destroyed *for
      any reason* while near the player (not just a direct hit — this is
      separate from the 250-damage direct-hit case). `TryGetMissileHeading`
      (`Combat.cs:1145-1192`) only has the 250-damage direct-hit path and a
      no-damage ECM-jam path; no equivalent near-miss splash path was found
      in a quick read. May be handled elsewhere (missile removal/collision
      code was not traced) — needs verification before treating as
      confirmed.

- [ ] [EliteSharpLib] **Sun may rotate with player roll/pitch when the
      original explicitly prevents it** — original `MVEIT` part 6
      ([elite-source-flight.asm:32549-32570](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      returns early for `TYPE == 129` (the sun) specifically to skip parts 7
      and 8, which rotate an object's own orientation vectors by our
      roll/pitch and apply its own spin — comment: "we don't need to rotate
      the sun around its origin." `MoveUniverseObject`
      (`Space.cs:905-944`) has no such early-out:
      `VectorMaths.RotateVector(obj.Rotmat, alpha, beta)` runs
      unconditionally and `beta` is only zeroed for `ShipType.Planet`, not
      `Sun`; `SpinUniverseObject` also runs for the sun. **Not yet
      confirmed** — planets and suns in the original route through a
      separate `MV40` routine (not read during this sweep) before rejoining
      `MVEIT` at `MV45`, so `MV40`'s own position-update logic needs
      checking before treating this as a real bug: it may already handle
      orientation differently than the generic ship path this comparison
      was made against.

- [ ] [EliteSharpLib] **Docking-bay roll-alignment threshold looks about
      2.7x too strict** — original `DOCKIT` part `PH32`
      ([elite-source-flight.asm:10356-10387](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      checks `|roofv(station) . sidev(ship)| >= 33/96 ≈ 0.344` (doubled dot
      product >= 66 against a max unit-vector magnitude of 96) before
      committing to the final docking-bay approach — documented as
      deliberately loose ("it just needs to be parallel-ish"). `FlyToDockingBay`
      (`Pilot.cs:340-347`) checks the equivalent dot product against
      `0.9166f` instead. If real, the ported autopilot demands near-perfect
      roll alignment where the original was forgiving, likely causing
      stalling or slow docking approaches for both the player's docking
      computer and NPC ships (both go through `AutoPilotShip`). Needs
      someone to re-derive the exact normalized threshold and verify against
      actual docking behaviour before treating as confirmed.

- [ ] [EliteSharpLib] **Equipment tech-level gating may be systematically
      off by one for several items** — the original has no per-item tech
      level; availability is positional: the item at list position *N* in
      `PRXS` shows once `system_tech + 3 >= N`
      ([elite-source-docked.asm:16343-16357](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-docked.asm)).
      Working that formula out for E.C.M. (position 3) gives `tech >= 0`,
      i.e. **available at every tech level** — but `EquipmentController.cs:28`
      gives it `TechLevel = 2`, checked as `(planetTech+1) >= TechLevel`
      (`ListPrices`, `EquipmentController.cs:356-364`), which requires
      `tech >= 1` and locks E.C.M. out of the lowest-tech systems where the
      original allows it. Re-deriving the same original formula for the
      other gated items (Fuel Scoops, Escape Pod, Energy Bomb, Energy Unit,
      Docking Computer, Galactic Hyperdrive, the Pulse/Beam laser upgrades)
      mostly showed the same one-level-too-strict pattern, but Mining Laser
      and Military Laser came out matching or one level too lenient instead
      — inconsistent enough that a full re-derivation by hand is needed
      before treating this as confirmed beyond the E.C.M. case, which is
      the one unambiguous example.

- [ ] [EliteSharpLib] **Bounty hunters turning hostile at legal status 40
      may be entirely missing** — original `TACTICS` part 3
      ([elite-source-flight.asm:9479-9486](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      re-checks every tick, for any ship flagged as a bounty hunter
      (`NEWB` bit 1): once the player's `FIST` (legal status) reaches 40 —
      "Offender," not yet the 50-threshold "Fugitive" — the bounty hunter
      sets itself hostile. `ShipTactics` (`Combat.cs:504-533`) has only one
      legal-status hostility check, `Cmdr.LegalStatus >= 64` gated on
      `ShipProperties.Police` (a different, higher threshold, police-only).
      `ShipProperties` defines a `LoneWolf` flag but `CreateLoneWolf`
      (`Combat.cs:971-997`) never sets it on the ship it spawns, and no
      combat code reads a legal-status threshold against it. If real, this
      drops a real gameplay mechanic (moderately bad reputation drawing
      bounty-hunter aggression before police get involved). Only searched
      `Combat.cs` for this — needs a check of the rest of the ship-behaviour
      code before treating as confirmed missing rather than differently
      named.

- [ ] [EliteSharpLib] **Energy bomb shouldn't spare the Constrictor mission
      ship** — original's bomb-kill sweep (main flight loop part 5,
      [elite-source-flight.asm:2781-2794](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      excludes only the space station and already-exploding ships; it does
      not check ship type otherwise, so a detonated energy bomb kills a
      nearby Constrictor outright, bypassing its "only military lasers
      penetrate" shield gimmick entirely (confirmed real: asm:3265-3278
      shows non-military lasers do zero damage to it, matching
      `ApplyLaserDamage` in `Combat.cs` exactly — that part of the port is
      faithful). `IsDestroyedByBomb` (`Space.cs:598-605`) explicitly excludes
      `ShipType.Constrictor` from bomb kills (also excludes `Cougar`, but
      that ship doesn't exist in the base 6502 game so it isn't a
      divergence). If the Constrictor exclusion is deliberate rather than a
      guess-based "protect the special ship" addition, it removes a tactic
      that worked in the original.

- [ ] [EliteSharpLib] Stardust count doesn't vary by location, and doesn't
      match either of the original's two values — `NOSTM`
      ([elite-source-flight.asm:1645-1648](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-flight.asm))
      is 18 particles (`NOST`) in normal space, dropped to 3 in witchspace
      for a visibly emptier void. `Stars.cs` allocates a fixed
      `Vector4[20]` and always simulates/draws all 20, in every location —
      neither count matches, and there's no witchspace-specific reduction
      at all. Purely cosmetic (atmosphere, not scoring/physics), so low
      priority.

## Could

- [ ] [Useful.Graphics] No frustum side-plane clipping: `NearPlaneClip`
      clips one plane, and left/right/top/bottom are handled *after*
      rasterisation, per pixel, by `SetClipRegion` plus the `Math.Max/Min`
      span clamps in `DrawTriangleFilled`. Output is correct — this is a
      cost defect, not a wrongness one — but a face projected far off-screen
      still runs a full scanline loop over a hugely clamped span. Clip (or
      guard-band) in camera space before rasterising. Related to, and
      bounded by, the rasteriser-throughput Won't in
      [backlog-roadmap.md](backlog-roadmap.md): both games are frame-capped
      by design and nothing here is a measured bottleneck, so only take this
      if a profile says so.

## Won't

Investigated and deliberately not fixed. Kept so the same ground isn't
re-covered.

- [ ] [Useful.Graphics] Fan triangulation of concave polygons — **not
      reachable with the current assets.** `DrawPolygonFilled` fans from
      vertex 0, which is wrong for a concave polygon. A 2026-07-31 sweep of
      every face with four or more vertices across both model sets
      (`Assets/Models/EightBit` and `SixteenBit`, faces up to 8 vertices)
      found zero concave faces, and `NearPlaneClip`'s Sutherland-Hodgman
      output of a convex input is convex, so the clipped polygons are convex
      too. Revisit only if hand-authored models are added.
- [ ] [StuntCarRacerSharpLib] Road texture shimmer: `SampleTexture` is
      nearest-neighbour with edge clamping — no bilinear filtering, no
      mipmaps — so distant road aliases. Authentic to the Amiga original,
      which had neither. Any change here belongs with the texture-filtering
      roadmap item, not as a fix.
- [ ] [EliteSharpLib] Buying more than 255g of Gold/Platinum doesn't work —
      authentic to the original ("broken as designed"); documented, not fixed.
- [ ] [EliteSharpLib] Elite Intro2 parade shows 29 of ~33 ship models
      ([ShipFactory.cs:80-111](../src/elite/libs/EliteSharpLib/Ships/ShipFactory.cs))
      — Cougar, Constrictor and the Lone variants are mission-specific ships,
      deliberately excluded from the parade; confirmed intentional, not a bug.
- [ ] [Assets] Selecting the `EightBit` tier in SCR throws at startup — SCR's
      manifest lists `[ "SixteenBit" ]` only. Closing it is an asset-authoring
      job, not a code fix; see the SCR 8-bit asset set item under Won't in
      [backlog-roadmap.md](backlog-roadmap.md) for what it would take.
