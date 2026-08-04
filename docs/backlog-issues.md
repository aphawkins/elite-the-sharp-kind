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

## Should

- [ ] [EliteSharpLib] Number of stars is not proportional to screen area
      (issue #4): the 8-bit tier renders at 320x256, a quarter the area of
      512x512, with the same star count, so the starfield is four times as
      dense; the 16-bit tier's 640x512 changes it again. Scale star count by
      screen area. Sequence after the Elite non-512x512 audit in
      [backlog-roadmap.md](backlog-roadmap.md) so the coordinate space it
      scales against is settled.

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
      confirmed. **2026-08-04 attempt:** the mechanic is real and clearly
      missing, but TA35's own proximity test (`x_lo OR y_lo OR z_lo` of the
      missile's position) only examines each axis's *low* byte, ignoring
      the high byte/sign entirely — read literally that's satisfied on
      roughly 1-in-256-cubed ticks, which can't be what "just got destroyed
      near us" means for ordinary play. Left unfixed rather than guess a
      distance convention (e.g. reusing the direct-hit box a few lines
      above) that isn't actually what the original does — needs someone
      who can resolve the byte-layout reading before it's implemented.

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
- [ ] [EliteSharpLib] Equipment tech-level gating for E.C.M., Fuel Scoops,
      Escape Pod, Energy Bomb, Energy Unit, Docking Computer and Galactic
      Hyperdrive — **not a bug.** A 2026-08-04 re-derivation of the
      original's positional formula (item at `PRXS` position *N* shows once
      `planet_tech >= N - 2`, from the `EQL1` list loop at
      [elite-source-docked.asm:16371-16408](../../../markmoxon/elite-source-code-bbc-micro-disc/1-source-files/main-sources/elite-source-docked.asm))
      matches every one of these items' current `TechLevel` exactly. A
      prior sweep had mis-derived the formula and flagged E.C.M. as
      one-level-too-strict; that was wrong. Mining Laser and Military
      Laser were genuinely off by the same mis-derivation's mirror error
      (too lenient) and have been fixed — see CHANGELOG.
- [ ] [EliteSharpLib] Docking-bay roll-alignment threshold
      (`FlyToDockingBay`, `Pilot.cs:342`, `MathF.Abs(dir) >= 0.9166f`) —
      **not a bug.** A prior sweep computed the original's `CPX #66`
      threshold as `33/96 ≈ 0.344`, dividing by the *unit vector's* own
      magnitude (96). But `TAS4`'s dot product (like the unrelated `CNT`
      dot product used elsewhere in `TACTICS`) is right-shifted by 8 as
      part of the multiply, capping its own high byte at `96*96/256 = 36`,
      not 96 — the same-shaped error as the equipment tech-level item
      above. The correct normalization is `33/36 ≈ 0.9167`, which is
      exactly `0.9166f` — already in the port, already correct, and
      already symmetric (`MathF.Abs`) as the original's "positive or
      negative, just needs to be parallel" comment describes.
