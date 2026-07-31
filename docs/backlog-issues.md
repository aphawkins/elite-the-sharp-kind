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

### 3D pipeline correctness (from the 2026-07-31 modern-pipeline gap analysis)

Three defects found comparing both games' 3D pipelines against a modern
rasterisation pipeline. They are Musts because each one makes a *shipped,
selectable* feature produce output that is wrong on its own terms — unlike
the deliberate period-faithful omissions (no lighting, no texturing in Elite,
no alpha/fog/AA), which are recorded under Won't below and as roadmap items.

- [ ] [EliteSharpLib] **The z-buffer renderer never gets per-vertex depth, so
      z-buffered mode cannot do the one thing it exists for.**
      [`ZBufferRenderer.Submit`](../src/useful/libs/Useful.Graphics/Rendering/ZBufferRenderer.cs)
      fills *every* entry of the polygon's `Depths` array with the same value
      — the flat whole-face key `z` handed to it — so
      `DrawPolygonFilledDepth` interpolates a constant across the triangle
      and the per-pixel test resolves at whole-polygon granularity.
      Interpenetrating and steeply-angled faces, the exact cases a depth
      buffer exists to solve, come out no better than the painter's chain.
      The rasteriser already supports true per-vertex depth and SCR feeds it
      correctly ([TrackRenderer.cs:227](../src/scr/libs/StuntCarRacerSharpLib/Rendering/TrackRenderer.cs)
      passes `clipped[j].Z`); Elite is the side that doesn't.
      Fix: carry the near-plane-clipped camera-space `Z` per vertex out of
      `ShipBase.BuildFacePolygon` alongside the projected `Vector2`, widen
      `IPolygonRenderer.Submit` to take it, and submit that instead of the
      mean. Fold in the second half while there: with a real per-pixel depth
      test, `ZBufferRenderer`'s O(n²) insertion sort into a linked chain is
      dead work — it sorts back-to-front *and then* depth-tests. Drop the
      chain from the z-buffer path (keep `PainterRenderer`'s, which needs
      it). Note `FaceMeanZ`'s plane-rooted decal keys
      ([ShipBase.FindFaceRoots](../src/elite/libs/EliteSharpLib/Ships/ShipBase.cs))
      exist to make coplanar decals tie with their base face and draw over
      it; per-vertex depth makes them exactly coplanar rather than merely
      equal, so decals will need an explicit bias or a draw-order guarantee
      — check this in the live app (cockpit windows, engine plates) before
      calling the item done.

- [ ] [EliteSharpLib] **Backface culling decides on garbage coordinates for
      faces straddling the camera plane.** The winding test in
      [`ShipBase.DrawModelFaces`](../src/elite/libs/EliteSharpLib/Ships/ShipBase.cs)
      runs on `pointList`, which came from `ProjectPoint`'s
      `if (vec.Z <= 0) { vec.Z = 1; }` clamp. A face with points on both
      sides of the camera plane therefore has meaningless projected X/Y
      feeding the cull, so it can be culled or kept incorrectly — and the
      near-plane clip that runs *afterwards* cannot undo a decision already
      taken. The clamp's own comment acknowledges it exists to keep the
      winding test fed. Visible as faces flickering on/off when a ship
      passes close by. Fix: cull in camera space against the face normal
      (`dot(normal, pointOnFace) ` with the normal from the model's own
      geometry — `FindFaceRoots` already computes plane normals, so the
      per-face normal can be precomputed once per model rather than per
      frame), *then* clip, *then* project. That removes the cull's dependency
      on the clamp entirely; the clamp itself can then only affect the laser
      aim, where it is harmless.

- [ ] [Useful.Graphics] **Elite silently drops geometry past 100 polygons per
      frame.** Both `PainterRenderer` and `ZBufferRenderer` open `Submit`
      with `if (_totalPolys == MAXPOLYS) { return; }` — no log, no counter,
      nothing. The cap is inherited from The New Kind's `poly_chain`, but it
      is reachable here: `Universe.MaxUniverseObjects` is 20 and the ship
      models run to 29 faces (`transporter.obj`; `cobramk3.obj` 19,
      `coriolis.obj` 15), so four Cobras plus a station is already ~91
      polygons before lasers and explosion debris. Whatever arrives last just
      isn't drawn. Fix direction: measure the real per-frame peak first (a
      debug counter over a busy scene in both games), then either raise the
      cap to something the worst case can't reach or make the overflow
      loud rather than silent. Sequence after the z-buffer item above, which
      touches the same two classes.

### Layout

- [ ] [EliteSharpLib] **Screens using bare absolute coordinates drift out of
      alignment** with those laid out from the viewport, which stay centred.
      This is a latent bug at any width except the tier's own, independent of
      the resolution-tier scheme — which is why it sits here rather than with
      the widescreen work in [backlog-roadmap.md](backlog-roadmap.md).
      `ThargoidMissionView16Bit` (`new(116, 132)`, the Blake portrait at
      `new(352, 46)`), `ConstrictorMissionView16Bit`, `PlanetDataView16Bit`,
      `MarketView16Bit` and the commander save/load screens are the known
      cases — and their 8-bit counterparts have the same shape at 320. None
      have been checked live at 640 yet. (`Offset` is gone — it duplicated
      `ScannerLeft`, and both were removed when `ViewLayout` became
      viewport-only; screens now lay out from `ViewportLeft`, which is the
      screen origin.)

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
