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
      field, both mission briefings (`ThargoidMissionView16Bit`'s
      `new(116, 132)` and the Blake portrait at `new(352, 46)`,
      `ConstrictorMissionView16Bit`) and the save screen's name box, which
      sits left of the name it frames.
      Fix by **re-laying-out those screens against 640** — widening the
      tables and moving the columns out to use the width. Per
      [decisions.md](decisions.md) (2026-08-01) the tiers are fixed-width,
      so this is a one-off re-authoring against a known resolution, *not* a
      centred content band, a design-space origin, or any other mechanism
      inside `ViewLayout`. Reach the mission screens with Ctrl-M
      under `ELITE_DEBUG_MISSIONS` (see
      [elite-readme.md](elite-readme.md#environment-variables)); they are
      otherwise hours of play away.

## Should

- [ ] [EliteSharpLib] Number of stars is not proportional to screen area
      (issue #4): the 8-bit tier renders at 320x256, a quarter the area of
      512x512, with the same star count, so the starfield is four times as
      dense; the 16-bit tier's 640x512 changes it again. Scale star count by
      screen area. Sequence after the Elite non-512x512 audit in
      [backlog-roadmap.md](backlog-roadmap.md) so the coordinate space it
      scales against is settled.

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
