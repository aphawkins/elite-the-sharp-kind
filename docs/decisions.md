# Maintainer Decisions — The Sharp Kind

Consolidated log of maintainer decisions for the repository, split out of
[backlog-roadmap.md](backlog-roadmap.md) so the backlog stays about work
items and this file stays about the calls that shape them. Newest first.
When a decision reshapes or unblocks backlog items, those items are
updated in backlog-roadmap.md to reference the decision here rather than
restating it.

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
