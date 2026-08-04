# Maintainer Decisions — The Sharp Kind

Consolidated log of maintainer decisions for the repository, split out of
[backlog-roadmap.md](backlog-roadmap.md) so the backlog stays about work
items and this file stays about the calls that shape them. The backlog was
itself split on 2026-07-31 into [backlog-issues.md](backlog-issues.md)
(defects) and backlog-roadmap.md (features, refactors, cleanups) — a
decision may reshape items in either. Newest first. When a decision
reshapes or unblocks backlog items, those items are updated in the backlog
to reference the decision here rather than restating it.

## Resolved (2026-08-03) — a modifier that decides is held, not pressed

The Ctrl-M mission jump stopped M firing missiles. `IsPressed` is one-shot: it
reports a press and consumes it. The jump's guard read M first and Ctrl second,
so with `ELITE_DEBUG_MISSIONS` set every bare M was eaten by a cheat that then
declined to run, and the missile handler polled an empty keyboard.

- **A modifier that only decides what another key means is read with
  `IsHeld`, and read first.** `IKeyboard` gained `IsHeld(ConsoleModifiers)`
  for it. Consuming the key first steals it from the handler it belongs to;
  consuming the modifier steals it from the next combination in the tick
  (Ctrl-H's galactic hyperspace, which was reading it the same way and now
  does not).
- **The fake keyboard models the one-shot contract.** `FakeKeyboard.IsPressed`
  answered every call, so no test could have caught this - and four tests were
  leaning on it, holding one key-down across twenty ticks of menu navigation
  that the real keyboard would have moved one row. The fake consumes now and
  those tests press per tick.

## Resolved (2026-08-03) — a tier is a rendition, and a rendition draws everything

The entry below put the screens behind the plugin door and left the rest of
the drawing in the game. Finishing the job changed what the thing is called
and what it is allowed to be.

**Not a tier - a rendition.** "Tier" said these were classes of machine that
rank against each other. They are not: a rendition is one interpretation of
the game, and the next one need not be a machine at all - futuristic,
underwater, psychedelic, at any resolution. "Machine" was considered and lost
for the same reason, though the prose had been reaching for it (the 16-bit
decision below calls its tier a machine in its own title).

**So the name is a name, not an enum.** `SystemTier` is gone. A closed set of
two cannot name a rendition the game was never built against, which is the one
thing the plugin model exists to allow. It is an open string, as a mission's
is, with the same obligation to stay put across releases. Everything that
follows falls out of that:

- **A rendition declares what the game used to assume.** Its colour budget,
  its screen size and its coordinate scale were facts the game held about two
  known tiers, hardcoded in three different files. They are the rendition's
  now, which is what makes "any resolution" true rather than aspirational.
- **An unrecognised name is not repaired away.** Whether one by that name is
  installed is settled when it is looked for, by a failure that names it.
- **Config files keep working.** `engine.tier` is read, folded into
  `engine.rendition` and never written back, so a file upgrades itself the
  first time it is saved.

**Everything drawn is the rendition's, not just the screens.** The HUD, the
four planet styles, the three sun styles, the starfield and the colour a ship
is painted all moved out. The shape each time is the same: the game computes
and the rendition draws. A planet renderer is handed a centre, a radius and an
orientation and never sees a position in space; the scanner is handed a model
like any screen; the starfield is handed the marks the game decided to show.
What the game keeps is the universe.

- **Base in the contracts, subclass in the rendition.** Colour injection was
  tried first and rejected: it assumes the two draw identically forever and
  differ only in palette, which is an accident of the 8-bit layouts being
  first drafts. A subclass can override as far as the whole draw.
- **The entropy stays the game's.** A renderer is handed the seeded stream
  rather than making one - the fractal planet's seed, the sun's rim shimmer -
  or the same system would stop looking the same on every visit. The cockpit
  laser's aim rides on its model for the same reason.
- **The ship's beam is colours, not a renderer.** It draws through the
  depth-tested polygon path, which is deliberately not on `IViewSurface`;
  publishing that to give two colours a renderer would hand every rendition
  the z-buffer pipeline. One `ShipColours` per rendition serves both the
  scanner and the beam, which is what fixed a 16-bit Viper showing purple on
  the scanner and firing cyan.
- **A rendition brings its own artwork.** Art, palette, fonts and models
  travel with the assembly; the audio stays with the game. This reverses the
  tier-first layout rejected on 2026-07-30 - see
  [asset-structure.md](asset-structure.md) for why that objection expired.

Consequences:

- **The game no longer knows which renditions exist.** No switch anywhere
  names one. Adding a third is a folder.
- **`Useful` is free of tiers**, which is what lets Stunt Car Racer grow
  renditions later without forking `AssetLocator`. Its own vestigial
  `SixteenBit` folders were flattened in passing.
- **A rendition is now a large thing to write** - every screen, every world
  renderer, and a full asset set. That is the price of the two shipped ones
  going through the same door as a stranger's, and it is the right price:
  the door is known to work rather than assumed to.

## Resolved (2026-08-03) — the tiers are plugin assemblies too, behind one contracts assembly

> Superseded the same day by the entry above, which renamed all of this and
> carried it further. The names here - `Views` folder, view pack, `IViewPack`,
> tier - are the ones it was decided under; the calls it records still stand
> unless that entry says otherwise.

The mission work proved the door; this puts the presentation through it. Each
asset tier's screens are now **an assembly the game finds at startup**, in a
`Views` folder beside the executable: `EliteSharp.Views.EightBit` and
`EliteSharp.Views.SixteenBit`, each referencing the contracts and nothing else,
and neither referenced by the game. Adding a third tier is an assembly, and no
edit to the game's composition at all.

**One contracts assembly, not one per plugin family.** The mission contracts
assembly was renamed `EliteSharp.Abstractions` and subdivided by namespace —
`Missions`, `Views`, `Assets`, `Ships` — rather than growing a project per
family, which would be sprawl standing in for a folder. Note the price, which
was accepted knowingly: the mission contracts were dependency-free on their
own, and views need `IGraphics` and a palette to exist at all, so a mission
author now transitively carries a graphics assembly they never call. What a
mission may touch is a namespace boundary now rather than a compile error.

The calls that shaped it:

- **`IViewSurface`, not `IEliteDraw`.** Views only ever used `Graphics`,
  `Layout` and `Palette` — nothing called `DrawObject`, `Focus` or the
  clip/render members — so the published surface is those three and
  `IEliteDraw` implements it. Projecting a ship and starting a frame stay on
  the game's side and are unreachable from a pack.
- **A pack hands over a `ViewSet`, checked once at startup.** `Add<TModel>`
  takes the model type off the view's own interface, so a pack cannot file a
  view under a screen it does not draw, and `ViewRegistry` checks the set
  against the game's screens before a frame is composed. A pack that is a
  screen short is a startup failure naming every missing screen at once, not a
  blank screen the commander finds later.
- **The mission briefing is handed over separately.** It is the one screen that
  answers back — where its tier puts the ship posing behind a briefing — so it
  has its own factory method rather than being fished out of the set with a
  cast that could fail.
- **A missing pack is fatal, unlike a missing mission.** No `Missions` folder
  costs the commander some missions; no pack for the configured tier leaves the
  game with nothing to draw at all, so the loader throws and names the tier.
- **The laser's game state moved onto `PilotModel`.** `LaserDrawBase` read
  `GameState` for the wireframe setting and `RNG` for the beams' per-frame
  jitter. Both now arrive on the model — the roll happens game-side because the
  game owns the one source of entropy, and a view that rolled its own would not
  be reproducible. The geometry is already derived from the tier's scale, so
  the base sits with the contracts and only the beam colours are each pack's.

Consequences:

- **The `Views` folder is load-bearing, more so than `Missions`.** Both packs
  are copied on `Build` and `Publish` whichever tier is configured, because the
  tier is a setting the commander can change without reinstalling anything.
- **The tier branch is gone from the composition root.** `IsEightBit` no longer
  selects views; the registrations name a screen and take what the pack drew.
- **Trimming and AOT are now foreclosed for the presentation as well**, which
  the mission decision flagged as the thing to watch if the plugin model spread
  to views. It has. Still nothing configures either.
- The views and their models became public API. The views themselves stay
  internal to their pack — the game only ever sees an `IView` — with the tests
  let in, the same arrangement `EliteSharp.Missions.Classic` uses.

## Resolved (2026-08-02) — missions are plugin assemblies, not game code

The mission stages were hardcoded, and adding a mission meant editing two
enums, two controllers, two `Screen` values and four tier-specific views.
A mission is now **an assembly the game finds at startup**, in a `Missions`
folder beside the executable. The two the game has always had went out with
everything else, into `EliteSharp.Missions.Classic`, so the door a stranger
would come through is the one the game itself uses.

**Not a config file.** A mission carries behaviour — spawning a Constrictor,
fitting an energy unit, being ambushed while carrying plans — so data alone
cannot express one without inventing a scripting language. This is the pilot
for the backlog's [LARGE] data-driven game content model item; the answer for
*content that acts* is an assembly, and only inert content (equipment, stock,
ship definitions) is a config-shaped problem.

The calls that shaped it:

- **MEF2 (`System.Composition`) for discovery only.** Parts are registered
  into the existing `ServiceCollection`; the `CompositionHost` is created,
  drained and disposed inside `MissionLoader` and owns no lifetimes, so the
  one composition root in `architecture-principles.md` stands. MEF2 has no
  directory catalogue, so the DLLs are enumerated by hand.
- **Exported by convention, not by attribute.** A `ConventionBuilder` over
  types implementing `IMission` is what lets a plugin reference the contracts
  assembly *and nothing else*; an `[Export]` attribute would have made every
  plugin author reference MEF. A mission is a public class with a constructor
  taking no arguments.
- **Stages are strings, guarded by `MissionProgress`.** A compile-time enum
  cannot cover missions the game was never built against. Every stage name
  going in is checked against what that mission declared, so a stage nobody
  declared is unreachable and everything reading one back can trust it.
- **Systems are identified by planet number, never by seed bytes.** The old
  code compared two of a system's six seed bytes, which a plugin author could
  not discover. `PlanetAt` is the inverse of `FindPlanetNumber`; the numbers
  are Orarra 193 in galaxy 1, Ceerdi 83 and Birera 36 in galaxy 2, and a test
  holds them to those names.
- **Equipment and portraits are small enums of what exists.** There is no
  name-based equipment lookup in the game and a plugin cannot ship artwork, so
  inventing string namespaces for either would only let a mission ask for
  something that cannot be delivered.
- **One briefing screen for every mission**, laid out from what a briefing
  contains — a headline, how many paragraphs, whether somebody is pictured —
  never from which mission sent it. A plugin's mission cannot draw itself.

Contract shapes that exist for a reason and should not be undone:

- **Awards ride on `MissionStep`; `IMissionContext` is read-only.** An earlier
  draft paid through the context before the stage was committed, so a replayed
  screen could collect the Constrictor's 5000 credits twice.
- **`MissionStep`'s constructor is internal.** Steps come only from
  `MissionStages.Step`, which refuses an undeclared stage or a move that does
  not go forwards.
- **No shared base type for encounters.** An abstract record cannot be closed:
  it emits a `protected` copy constructor any assembly can chain to, and
  narrowing that is CS8878, so a plugin could derive a kind that lied about
  itself. Two sealed types instead.
- Every public property is get-only and every public type sealed, so `with`
  cannot route round a validating constructor and nothing can be derived from.
  Verified by compiling a probe assembly against the built DLL.

Consequences:

- **The `Missions` folder is load-bearing.** Remove it and the game has no
  missions, and a commander part-way through one is refused — the reject-and-log
  rule for a save naming a mission nothing provides. The app's build copies the
  plugin in on both `Build` and `Publish`.
- **Commander files hold only the stages actually reached**, so a fresh
  commander writes none. A file cannot demand that every installed mission be
  present when the installed set varies.
- **Trimming and AOT are foreclosed**, since discovery is reflection over
  externally-supplied assemblies. Nothing configures either today, so this costs
  nothing now; it matters if the plugin model spreads to views or stock. (It
  spread to views on 2026-08-03.)
- **The 8-bit Constrictor debrief moved down the screen** to match the Thargoid
  one. The two had drifted into different layouts for the same shape of
  briefing, and one rule cannot draw both. 16-bit is unchanged.
- A live bug went with the port rather than being reproduced: the rumours about
  the stolen Constrictor were printed for whatever system was selected on the
  chart, so Reesdice was named from anywhere in galaxy 0. A rumour is somebody
  in the station talking, and is now only said about the system underfoot.

## Resolved (2026-08-01) — the 16-bit tier is a 12-bit, 4096-colour machine

The 2026-07-30 decision that renamed the 16-bit palette to web colours is
**backed out**. A web colour name is only worth having if the value really
is that colour, and the constraint that actually governs this tier is not
naming but **channel depth**: the 16-bit machines it stands in for drove a
12-bit DAC — four bits each of red, green and blue, 4096 colours. So the
29 ramp names return, and every one of them is now a colour that hardware
could produce:

```
White FFFFFF · LighterGrey EEEEEE · LightGrey 888888 · Grey 777777 · DarkGrey 666666 · DarkerGrey 555555 · Black 000000
Red 880000 · LightRed 991111 · LighterRed CC0000 · RedOrange FF3333 · Orange FF7733 · DarkOrange EE6622 · LightOrange FFBB77
Gold FFBB33 · DarkYellow DD9911 · PaleYellow FFFF55 · LightYellow FFFFBB · Yellow FFFF00
Green 008800 · LightGreen 88CC00 · LighterGreen 66EE22 · LightBlue 115599 · Cyan 00FFFF
Blue 000088 · DarkBlue 111199 · Purple 444488 · BrightPurple BB55EE · Lilac EEAAEE
```

- **Widen a channel by replication, never a left shift.** A 4-bit `0xE`
  becomes `0xEE`, not `0xE0`. The shift is the tempting one-liner and it is
  wrong: it can never reach `0xFF`, so a tier using it has no true white.
  The art settled this on its own — 109 of the 145 colours in the 16-bit
  bitmaps were already replicated values and only 2 were shifted ones.
- **The old ramp values were shifted, so the names cost nothing to
  restore.** 23 of the 29 were `E0`/`B0`/`70`-style truncations, meaning
  the author had already picked a 4-bit nibble and expanded it badly.
  Restoring them as `EE`/`BB`/`77` changes the expansion, not the choice,
  so the tier looks as it did before either commit.
- **Three needed a judgement call.** `BrightPurple` (B855F6) and
  `PaleYellow` (FFFF5C) went to their nearest level. `Grey` (727272) and
  `DarkGrey` (707070) both round to `777777`, and 12 bits has nothing
  between 7 and 8, so the neutral ramp was re-spaced one step down —
  `DarkerGrey` 555555, `DarkGrey` 666666, `Grey` 777777 — to keep four
  distinct greys.
- **Enforced at startup, for the palette and the art alike.**
  `AssetColourBudget.ChannelBits`/`IsOnGrid` and `AssetSet.Load` fail the
  game on any colour between the tier's levels, naming the asset and the
  value. This is a separate rule from `PaletteNamesEveryColour`, which
  stays false for 16-bit: the tier is still direct-colour, so a bitmap need
  not use a *named* colour — it just has to use a *producible* one.
- **36 bitmap colours were repainted to comply**, mostly `elitetext.bmp`'s
  anti-aliased grey ramp and `scanner.bmp`. The ramp loses a few steps
  where two of its shades now round together, which is inherent to 12 bits.
  Stunt Car Racer shares the tier and its `menu.bmp` was repainted the same
  way; its palette was already on the grid.
- **The 8-bit tier is unaffected.** Its limit is the 16-entry palette, not
  channel depth, so `ChannelBits` is 8 there and its web colour names — the
  2026-07-30 decision below — stand.

## Resolved (2026-08-01) — widescreen is a modern-tier concern only

The 8-bit and 16-bit tiers are **fixed-width**: each has one resolution
(320x256 and 640x512) and its screens are laid out for it. The widescreen
variants sketched in the 2026-07-24 multi-resolution decision above are
**dropped for those two tiers** — only the modern tier, being vector-based
rather than tied to a bitmap set, supports widescreen.

Consequences:

- A screen may position content by absolute coordinates measured from the
  viewport's left edge. There is no width but the tier's own to stay
  aligned at, so no centred content band, no design-space origin and no
  scaling layer is wanted between a screen and `ViewLayout`. `ViewLayout`
  stays the viewport and nothing else.
- What is left of the old "screens drift out of alignment" issue is
  narrower and concrete: the 16-bit screens are still laid out for the
  512-wide tier they were authored at, on a tier that has been 640 wide
  since 2026-07-30. The fix is to re-lay-out those screens at 640, not to
  centre a 512 band inside it. Tracked in
  [backlog-issues.md](backlog-issues.md).
- Widescreen layout for the modern tier is designed when that tier is,
  against vector chrome, and inherits nothing from these two.

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
  CSS specification** and keep its spelling — `Gray`, not `Grey`. The
  16-bit palette keeps its ramp names and its `Grey` spelling unchanged.

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
