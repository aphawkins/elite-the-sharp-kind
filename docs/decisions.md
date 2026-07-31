# Maintainer Decisions — The Sharp Kind

Consolidated log of maintainer decisions for the repository, split out of
[backlog-roadmap.md](backlog-roadmap.md) so the backlog stays about work
items and this file stays about the calls that shape them. Newest first.
When a decision reshapes or unblocks backlog items, those items are
updated in backlog-roadmap.md to reference the decision here rather than
restating it.

## Resolved (2026-07-31) — `ViewLayout` is the viewport, and 8-bit text is on a grid

`ViewLayout` described its metrics partly against the border and partly
against the scanner, which made every position two subtractions away from
what it meant. It now describes **one region, the viewport** — everything
above the HUD — starting at the screen origin:

- **`BorderWidth`, `ScannerTop`, `ScannerLeft` and `ScannerRight` are
  gone.** The border's width belongs to the base view that draws the
  frame; the scanner's geometry belongs to `ScannerBase`. Both tiers'
  scanners are full-width, so `ScannerLeft` was always 0 and
  `ScannerRight` always `ScreenWidth - 1` — screen edges wearing the HUD's
  name. `Offset` went with them, having been an exact duplicate of
  `ScannerLeft` across ~90 call sites.
- **The border overlays the viewport's outer pixel rather than being
  reserved out of it.** Changing its width now moves nothing. The cost is
  that row 0 and the outermost columns are the border's, and most of the
  8-bit font's glyphs ink their top and edge pixels, so no text goes
  there.
- **The 8-bit viewport is therefore exactly 40x25 of its 8x8 cells**, and
  8-bit text is positioned with `Row(n)`/`Column(n)` on `BaseView8Bit`
  rather than pixel constants. Centring goes through
  `DrawTextCentreOnGrid`, since pixel-centring lands odd-length strings on
  half a cell; text the short range chart's packing places arbitrarily is
  put on the grid with `SnapToGrid`, which rounds **up** so a label clears
  the blob it names. The chrome band is row 1, the header rule row 2, and
  screen content starts at row 3. The 16-bit font is proportional, so no
  grid applies to that tier.

The clip region survives this: it is a single cheap guard at the pixel
level that everything drawn in a tick relies on, and making lasers,
planets, suns, stars and ships each police the viewport themselves would
reimplement in six places what `DrawPixel` already does once.

## Resolved (2026-07-30) — the 16-bit palette is web colour names too

The 8-bit decision below argued that a relative ramp name states nothing
about a colour. That argument does not stop at 8-bit, so the 16-bit
palette's 29 ramp names are now **29 CSS/HTML colour names at their exact
web values**, each the nearest web colour to the value it replaces:

```
White FFFFFF · Gainsboro DCDCDC · Gray 808080 · LightSlateGray 778899 · SlateGray 708090 · DimGray 696969 · Black 000000
Maroon 800000 · DarkRed 8B0000 · FireBrick B22222 · Crimson DC143C · Tomato FF6347 · Chocolate D2691E · SandyBrown F4A460
Goldenrod DAA520 · DarkGoldenrod B8860B · Khaki F0E68C · PaleGoldenrod EEE8AA · Yellow FFFF00
Green 008000 · YellowGreen 9ACD32 · LimeGreen 32CD32 · Teal 008080 · Cyan 00FFFF
Navy 000080 · DarkBlue 00008B · DarkSlateBlue 483D8B · MediumOrchid BA55D3 · Plum DDA0DD
```

- **Nearest by perceptual distance, not by name.** Each old value was
  matched against the CSS extended set with a low-cost redmean weighting,
  so the choices follow the colours the art already used rather than what
  the ramp name said they were: `Red` (800000) *is* `Maroon`, `Orange`
  (F07030) is `Tomato`, `Lilac` (E0A0E0) is `Plum`.
- **Ties were broken to keep the ramps distinct and ordered.** Three greys
  and two reds each had the same nearest web colour, which a dictionary
  cannot hold twice. The assignment that minimises total distance while
  staying monotone was taken instead — so `DarkGrey` is `SlateGray` and
  `Grey` is `LightSlateGray`, both a shade cooler than the neutral they
  replace, and `LighterRed` is `FireBrick`. `LightBlue` (105090) landed on
  `Teal` once `DarkSlateBlue` went to the much closer `Purple`.
- **The tier is direct-colour, so nothing is enforced against this set.**
  Unlike 8-bit, 16-bit bitmaps may use any colour
  (`AssetColourBudget.PaletteNamesEveryColour` is false for the tier); the
  palette is only the set of names the geometry and views draw with, and
  the values moved by up to ~100 units without any asset needing repaint.
- **`FractalPlanet` had to become tier-aware.** It is shared code and
  looked up `Blue`/`LightBlue`/`LightGreen`, three names 16-bit no longer
  has. It now selects by role, which is the pattern the shared/tier split
  wants anyway.

## Resolved (2026-07-30) — the 8-bit palette is sixteen web colour names

The 8-bit palette was a set of 29 relative ramp names (`LighterGrey`,
`DarkerGrey`, `Gold`, `Lilac`, `RedOrange`…) mapped onto whatever values
the art happened to use, inherited from the 16-bit palette when the tier
was stood up. It is now **sixteen entries, each a CSS/HTML colour name at
its exact web value**:

```
White FFFFFF · LightGray D3D3D3 · DarkGray A9A9A9 · Gray 808080 · DimGray 696969 · Black 000000
Red FF0000 · Orange FFA500 · Yellow FFFF00 · Brown A52A2A
Green 008000 · LightGreen 90EE90 · Blue 0000FF · LightBlue ADD8E6 · Cyan 00FFFF · Purple 800080
```

- **Names are absolute, not relative.** A ramp name only means anything
  next to its neighbours, so it cannot survive a change of palette: the
  ramp had five greys because the 16-bit art wanted five, and the 8-bit
  tier has no obligation to supply them. A web name states a colour.
- **Sixteen entries is the tier's cap**, so the palette *is* the
  machine's colour set rather than an alias table over it. Every 8-bit
  bitmap colour is one of these sixteen, enforced at startup.
- **The two tiers no longer share a name set**, which is what forced the
  models to become tier-varying (see asset-structure.md). That cost —
  duplicating 31 `.obj` files per tier — was accepted as the price of
  the palette meaning what it says.
- **`DimGray` was added late, replacing `Magenta`.** Five neutral steps
  above black let `DarkerGrey`'s 48 model faces map to a dark grey rather
  than to pure black, which against black space reads as holes in the
  hull. `Magenta` was earning nothing once the mining laser moved to
  `Purple`.
- UK spelling is the repo standard, but these are **proper nouns from the
  CSS specification** and keep its spelling — `Gray`, not `Grey`. (The
  16-bit palette followed the same way the same day; see the decision
  above.)

## Resolved (2026-07-29) — `Focus` follows screen height

Revises one bullet of the 2026-07-28 tier presentation decision below,
which derived `Focus` from the screen *width*. Everything else in that
decision stands.

- **`Focus` is derived from the tier's screen height**, so the vertical
  field of view stays the same at every resolution and a wider screen
  shows more to the left and right rather than magnifying everything.
- The width-derived version holds the *horizontal* field of view
  constant instead, which is the wrong invariant once the width and
  height differ: with `Focus = width`, the vertical field of view is
  `2·atan(height / 2·width)`, so it narrows as the screen widens. That
  is why the trial widening of the 16-bit tier to 640x512 (2026-07-29,
  see the backlog) made the planet fill the front view — a 25% longer
  focal length and less visible above and below.
- It also fixes a live inconsistency rather than only a future one. The
  8-bit tier is 320x256, so today its `Focus` is 320 while its `Scale`
  is exactly half the 16-bit tier's — the 3D view is not the half-size
  rendering the rest of the tier is, and its vertical field of view is
  already narrower than 16-bit's. Height-derived `Focus` makes it 256,
  exactly half, consistent with `Scale`.
- No effect at 512x512, where width and height are equal — which is
  also why the 16-bit tier reproduces unchanged.

## Resolved (2026-07-28) — tier presentation architecture

- **`Scale` magnifies the window, it does not scale the drawing.** A tier
  renders at its own native resolution and `Scale` only decides how large
  that framebuffer is presented: the 8-bit tier at 320x256 with a scale of
  2 fills a 640x512 window showing the same 320x256 pixels doubled. The
  graphics themselves do not change. This restates the pixel-doubling rule
  from the 2026-07-27 multi-resolution decision below, which had been
  misread as a coordinate multiplier — see the backlog item retiring
  `EliteDraw.Scale`'s current meaning.
- **Views are per tier, not shared and scaled.** A single scale factor
  cannot serve a Modern tier whose resolution is arbitrary and need not be
  a multiple of anything, so each tier gets its own view implementations,
  fine-tuned to its resolution, resolved through DI (the existing
  `PopulateScreens` map already keys `Screen` to `IView`, so the tier
  simply selects which implementation is registered).
- **Shared behaviour lives in an MVC split.** Duplicating whole views per
  tier would duplicate the data and formatting rules with them, so the
  per-screen model (game data, formatting) and controller (input,
  navigation) are shared and only the view — pure presentation, drawing
  only — varies per tier. **[LARGE]** — see the backlog items below.
- **Every screen gets a per-tier view**, not just the ones that look like
  they need it. They live in `Views/<Tier>/` with a tier-suffixed class
  name (`Views/EightBit/CommanderStatusView8Bit.cs`) to avoid clashes.
- **`WindowScale` is a config setting of its own**, integer only, and
  independent of `Tier`. The framebuffer stays at the tier's native size:
  both backends already render into a fixed-size texture and blit it to
  the window on every present, so magnifying means creating a larger
  window and sampling that blit nearest-neighbour. No game code changes.
- **3D projection needs its own float scale, separate from both.** The
  projection currently folds the view centre and the zoom into one
  integer `Scale`
  (`((x * 256 / z) + (Centre.X / 2)) * Scale`), which only lands the
  scene in the middle of the screen when `Scale` is exactly 2 — at 8-bit
  it centres the view a quarter of the way across. Decided: separate the
  two, so projection becomes `Centre + (x * Focus / z)` with `Centre` the
  real centre of the 3D viewport and `Focus` a float. `Focus` is derived
  from the tier's screen size times a per-tier factor, which reproduces
  16-bit exactly (512 x 1.0) and works at any Modern resolution without
  needing to be a multiple of anything. **Superseded 2026-07-29 on which
  dimension it follows**: this said width, holding the horizontal field
  of view constant; it is height, holding the vertical one constant —
  see the decision at the top of this file.
- **Elite first, then SCR.** SCR inherits the `Tier` setting from
  the shared `EngineConfigSettings` but nothing reads it yet; that is
  fine for now.

## Resolved (2026-07-28) — asset structure for the tier system

Follows on from the multi-resolution tier decision of 2026-07-27 below,
settling how assets are laid out and constrained per tier. Full design in
[asset-structure.md](asset-structure.md); implementation steps are in
[backlog-roadmap.md](backlog-roadmap.md)'s Should section.

- **Modern tier deferred**: it is predominantly vector-based and needs no
  bitmap set, so only the 8-bit and 16-bit tiers are modelled for now.
- **Layout**: shared category folders with a tier subfolder on the
  categories that actually vary (`Images`, `FontsBitmap`, `Palette`);
  audio, models, TrueType fonts and tracks stay tier-neutral and are not
  duplicated. Resolution is `<Category>/<Tier>/<file>` falling back to
  `<Category>/<file>`, in `AssetLocator` only.
- **Bitmap formats not restricted**: `BitmapReader`'s 32bpp-only,
  fixed-header assumption is replaced by a real BMP decoder (1/4/8/24/
  32bpp, real data offset, row padding, top-down), plus PNG support
  hand-rolled on `ZLibStream` — no third-party dependency, stays pure
  managed for the Software backend and headless tests.
- **Colour budgets**: 16 distinct opaque colours for the 8-bit tier,
  4096 for the 16-bit tier; modern is unrestricted 32bpp. Counted as
  distinct values only (no quantisation to a 12-bit or 9-bit space), and
  capped over the union of one game's whole asset set for the active
  tier — per game, not per image. Transparent pixels are excluded; alpha
  must be 0 or 255.
- **Enforced eagerly**: assets are never loaded on demand. A single
  eager `AssetSet` load in `Useful.Assets` replaces the separate loads in
  `SoftwareGraphics` and `SDLGraphics` (the SDL path currently decodes
  via SDL and would bypass validation entirely) and fails startup on an
  over-budget set.
- **Existing 16-bit assets are over budget**: measured unions are Elite
  2481 (passes) and SCR 5095 (fails), caused by anti-aliasing in
  `font2.bmp` (2431 alone) and `atlas.bmp` (2676). Decided to posterise
  those two assets rather than soften the cap; the validator ships
  warn-only and flips to hard-fail once both games comply.

## Resolved (2026-07-27)

- **Elite frame rate vs the 13.5Hz tick**: Elite currently composes each
  frame inside `Update` at a fixed 13.5Hz (authentic to The New Kind), so
  raising `Fps` today draws nothing new above the tick rate. Decided:
  Elite should compose (draw) frames at the configured `Fps` setting
  itself — not by interpolating between fixed 13.5Hz ticks. This means
  anything currently timed against the 13.5Hz update tick (tactics/AI
  pacing in `Space.UpdateUniverse`, animations, etc.) needs auditing and
  reworking so it stays correct when `Fps` differs from 13.5Hz.
  **[LARGE]** — not yet broken into scoped backlog items; see the new
  placeholder under [backlog-roadmap.md](backlog-roadmap.md)'s Could
  section.
- **UI/graphics scale model**: the existing `Scale` setting is untested
  and buggy. Decided: scale is **per-element and game-aware**, not a
  single discrete multiplier applied only at the framebuffer/present
  layer — game code needs to know about scale for individual elements.
  This aligns with the existing Scale/centring cleanup already in
  backlog-roadmap.md's Could section (move scale policy out of
  `Useful.Graphics` into the game).
- **Data-driven game content**: `EquipmentType`, `StockType`, ship
  definitions, and similar currently hardcoded/`AssetManifest.json`-driven
  game data (including `ShipFactory.CreateShipFromName`'s reflection-based
  construction) should move to a proper config-driven model. **[LARGE]** —
  design/scope the config shape in a follow-up session; this is now a
  commitment, not just a spike question.
- **Multi-resolution tier system** (supersedes the "Elite at non-512x512
  resolutions" scope question): both Elite and SCR should eventually
  support three resolution "system" tiers, each with its own bitmap set
  (standard and widescreen variants):
  - **8-bit tier**: e.g. 320x256 standard, 512x256 widescreen
  - **16-bit tier**: e.g. 640x512 standard, 1024x512 widescreen
  - **Modern tier**: predominantly vector-graphics-based HUD etc., not
    tied to a fixed bitmap resolution

  Both games get all tiers (not one tier per game). Scaling between
  tiers is integer-based pixel-doubling (e.g. 320x256 → 640x512 is
  exactly 2x, rendered blocky using the 8-bit bitmaps scaled up — not
  the native 16-bit bitmaps). `AssetManager` needs to support multiple
  asset "systems"; config needs a per-game system-type + scale setting.
  **[LARGE]** — supersedes and expands the existing "Elite at
  non-512x512 resolutions" and SCR widescreen items in
  backlog-roadmap.md's Could section; re-scope those items against this
  scheme before starting either.

## Resolved (2026-07-24) — benchmark history tracking

Decided: use [benchmark-action/github-action-benchmark](https://github.com/benchmark-action/github-action-benchmark)
against BenchmarkDotNet's `[JsonExporterAttribute.FullCompressed]` output,
storing history on a `gh-pages` branch (one chart per benchmark class, kept
apart by the action's `name` input), triggered manually via
`workflow_dispatch` only — not on every push/PR, since shared GitHub-hosted
runners are too noisy for benchmark numbers to gate CI, and this repo has no
regression-alerting need yet. Implemented in
[.github/workflows/benchmarks.yml](../.github/workflows/benchmarks.yml).
One-time manual follow-up outside of code: after the workflow's first run
creates the `gh-pages` branch, enable it under repo Settings → Pages →
Source, to get a browsable dashboard (the history is recorded either way;
this only makes it visible).

## Resolved (2026-07-19) — ptitSeb parity audit

A full feature-by-feature comparison of `C:\code\github\ptitSeb\stuntcarremake`
(the definitive conversion source) against `StuntCarRacerSharpLib` was run on
2026-07-19, prompted by concerns that earlier analysis had missed
functionality. Conclusions:

- **Verified ported and faithful**: the four-mode screen flow, orbiting
  menu camera + menu.png art with named tracks, preview camera (incl.
  the Draw Bridge high viewpoint), race/game-over flow with the
  six-second flashing RACE WON/LOST then GAME OVER text, opponent-name
  announcement, the full atlas cockpit (wheels/bounce/spin frames,
  engine + boost-flame animation, crack + smash holes, speed bar with
  over-max colour, lap/boost/distance read-outs), backdrop horizon +
  all five scenery types, track rendering with road lines around the
  player, opponent shadow, drawbridge animation incl. its opponent
  speed tables, opponent AI (random selection, names, attribute flags,
  wheelies, steering randomisation, obstruction/push interaction,
  car-to-car collision), lap/damage/boost/engine-revs models, the
  pitch-shifted engine loop, and effect-sound triggers.
- **Chains**: neither C++ reference implements them (`on_chains` is
  hardcoded FALSE, "won't implement chains at first"); ptitSeb instead
  drops the car above its current piece after 64 off-track frames. The
  port's crane chain-recovery (commit 7039a11) deliberately goes beyond
  ptitSeb toward the Amiga original — keep it.
- **racewin/racelost/wrecked/heads/menu bitmaps**: present in ptitSeb's
  `Bitmap/` but never loaded by its code (upstream commit 7ad79f7 "More
  bitmaps, but not pluged in yet"). menu.png is already wired up in the
  port; the rest are now discrete items under Could.
- **Remaining genuine gaps** are all tracked as items in
  [backlog-roadmap.md](backlog-roadmap.md): Super League, pause, 'R'
  turn-around, mid-race 'M', F9/F10, gamepad, outside view, art screens,
  widescreen/resolution work.
  (`Opponent_Speed_Value` was ported 2026-07-19; wreck-at-full-damage,
  the cockpit wheel-spin rate fix, a (simplified) lap-time clock, and
  per-effect sound volume/pitch/pan all landed 2026-07-20, see
  CHANGELOG.)

## Resolved (2026-07-11)

- **v1 release scope**: Elite + SCR, with SCR labelled preview given its
  open defects list (see Release engineering in backlog-roadmap.md).
- **First tag**: `v1.0.0`.
- **Claimed platforms**: win-x64, linux-x64, linux-arm64. macOS stays
  unclaimed (untested).
- **Coverage visibility**: add a badge (see Could in backlog-roadmap.md).
- **NuGet packaging of `Useful.*`**: defer until an external consumer
  exists.
- **Elite Intro2 parade**: keep mission ships (Cougar, Constrictor, Lone
  variants) out of the parade — status quo confirmed intentional, not a bug
  (see Won't in backlog-roadmap.md).
