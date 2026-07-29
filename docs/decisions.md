# Maintainer Decisions — The Sharp Kind

Consolidated log of maintainer decisions for the repository, split out of
[backlog-roadmap.md](backlog-roadmap.md) so the backlog stays about work
items and this file stays about the calls that shape them. Newest first.
When a decision reshapes or unblocks backlog items, those items are
updated in backlog-roadmap.md to reference the decision here rather than
restating it.

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
  from the tier's screen width times a per-tier factor, which keeps the
  field of view constant across tiers, reproduces 16-bit exactly
  (512 x 1.0), and works at any Modern resolution without needing to be a
  multiple of anything.
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
