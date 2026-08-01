# Asset Structure — The Sharp Kind

How game assets are laid out, resolved, decoded and validated across the
8-bit and 16-bit system tiers. This is the design note behind the
multi-resolution tier decision in [decisions.md](decisions.md); the work
items that implement it live in
[backlog-roadmap.md](backlog-roadmap.md).

Scope: both games get both tiers. The Modern tier from the tier decision
is deliberately **out of scope here** — it is predominantly vector-based
and needs no bitmap set, so it is not modelled until it has content.

## Layout

Only some asset categories vary by tier. Audio, TrueType fonts and
tracks are resolution-independent and are not duplicated. Images,
bitmap fonts, the palette and the models are tier-specific, so those
categories — and only those — gain a tier subfolder:

```
Assets/
  AssetManifest.json
  Images/
    EightBit/     scanner.bmp, ...
    SixteenBit/   scanner.bmp, elitetext.bmp, ...
  FontsBitmap/
    EightBit/     font1.bmp, font2.bmp
    SixteenBit/   font1.bmp, font2.bmp
  Palette/
    EightBit/     palette.json
    SixteenBit/   palette.json
  Models/
    EightBit/     adder.obj, ..., palette.mtl
    SixteenBit/   adder.obj, ..., palette.mtl
  SFX/  Music/  SoundFonts/  FontsTrueType/   <- tier-neutral
```

Models joined the tier-varying categories on 2026-07-30. Geometry is
resolution-independent, so they were tier-neutral at first — but a
model's colours are not geometry: `ModelReader` resolves each
`usemtl <name>` straight through the active palette, and once the 8-bit
palette stopped being a subset of the 16-bit one (see below), 13 of the
21 material names the ships use no longer existed at 8-bit. A shared set
cannot name colours that only one tier has.

Each tier's folder carries its own `palette.mtl`, regenerated from that
tier's `palette.json`. The game never reads it — `ModelReader` resolves
`usemtl` through the palette directly — but the `.obj` files declare
`mtllib palette.mtl`, so without one per tier the models open with
missing materials in external tools.

The rejected alternative was tier-first (`Assets/EightBit/Images/...`,
the whole tree duplicated per tier). It makes adding or removing a tier a
single folder operation, but forces every `.obj` and `.ogg` to be
triplicated or a `Shared/` pseudo-tier to be invented — which is the
category-first layout again, with an extra concept.

The palette is tier-scoped because the colour budget below makes it so:
an 8-bit palette cannot be the 16-bit palette.

## Manifest

One manifest per game, keyed by logical name exactly as today. The same
logical name resolves to the same filename in every tier — the tier is a
*path* concern, not a naming one, so the ~50 logical-name entries are not
duplicated per tier and cannot drift apart.

Two additions:

```json
{
  "Tiers": [ "8Bit", "16Bit" ],
  "TierOverrides": {
    "8Bit": { "Images": { "LaserBeam": "laser.bmp" } }
  }
}
```

* `Tiers` declares which tiers actually ship, so selecting an absent tier
  fails at startup rather than at first draw. The digit spelling is the
  JSON one, matching the config file's `tier`; the directories on disk
  are named for the `SystemTier` members (`EightBit/`, `SixteenBit/`),
  because a C# identifier cannot start with a digit.
* `TierOverrides` is the escape hatch for the cases where a tier's set
  genuinely differs — a merged sprite sheet, or a bitmap with no
  equivalent at that tier.

## Resolution

`AssetLocator` is the only place asset paths are built, so tier
resolution lives there and nowhere else. The rule is:

1. `<Category>/<Tier>/<file>`
2. falling back to `<Category>/<file>`

The fallback is what keeps tier-neutral categories from needing a copy
per tier, and it is why adding a tier folder to a category needs no code
change beyond pointing that category at `TierPath`.

The tier is chosen once, at construction (`AssetLocator.Create(tier)`),
not per call. `IAssetLocator`'s members are unchanged, so no consumer of
`ImagePaths` and friends has to know a tier exists. The tier comes from
each game's configuration, alongside the resolution setting.

`SystemTier` is an enum in `Useful.Assets` carrying each tier's colour
budget.

## Decoding

Assets should not be restricted to one bitmap flavour. `Useful.Graphics`
gains an `ImageReader.Read` that sniffs magic bytes and dispatches to a
per-format decoder.

* **BMP** — the existing `BitmapReader` becomes a real decoder rather
  than a reader of one specific export. Today it hard-rejects anything
  that is not 32bpp and assumes pixel data begins at a hardcoded offset
  of 150 bytes (`54 + 96`), which happens to be true of every file
  currently committed. It must instead read the header's real data
  offset and handle 1/4/8/24/32bpp, 4-byte row padding, and top-down
  (negative height) images.
* **PNG** — hand-rolled on `System.IO.Compression.ZLibStream`: inflate,
  unfilter, then colour-type handling. No new dependency, and it stays
  pure managed so the Software backend and the headless tests keep
  working (per the architecture principle preferring framework
  intrinsics over third-party libraries). Non-interlaced only.

`BitmapWriter` stays BMP-only; nothing needs to write PNG.

## Colour budgets

Each tier caps the number of distinct opaque colours:

| Tier | Cap |
| --- | --- |
| EightBit | 16 |
| SixteenBit | 4096 |

The cap is a count of distinct values only; which values those may be is
the separate channel-depth rule below.

The cap applies to the **union across one game's whole asset set for the
active tier**, not per image: a game's tier corresponds to one machine's
palette. The union is per game, so Elite and Stunt Car Racer each get
their own budget.

Fully transparent pixels are excluded from the count. Alpha is enforced
to be either 0 or 255 on both tiers, matching the renderer, which
already treats transparency as binary.

## The palette as the whole colour set (2026-07-30)

On some tiers the cap is not the only palette constraint: a bitmap may
only use colours the palette names.

| Tier | Palette names every colour |
| --- | --- |
| EightBit | yes |
| SixteenBit | no |

The split follows the hardware each tier stands in for. 8-bit machines
were **indexed-colour** — a pixel *was* a palette entry, and one palette
served the whole display — so a bitmap colour the palette does not name
is a colour the machine could not have shown. 16-bit hardware is
direct-colour, where `palette.json` is only a set of names the geometry
draws with (`usemtl` in the models, `Palette["Gold"]` in the views) and
bitmaps are independent of it.

This is a **subset** test, not equality: the palette may name colours no
bitmap uses, which it must, since most named colours only ever reach the
screen as filled geometry rather than as pixels in a file. The check
excludes fully transparent pixels, for the same reason the cap does —
they carry no colour.

Applying it to 16-bit would fail immediately and for no good reason:
Elite's 16-bit set holds 134 distinct colours against a 29-entry
palette, so most of them are "unnamed" by construction.

`AssetColourBudget.OutsidePalette` records the offending colours per
asset on **every** tier so they can be logged, but only
`PaletteNamesEveryColour` tiers fail startup over them.

## Channel depth (2026-08-01)

The third rule, and the only one that says which colours a tier may use
rather than how many or which are named:

| Tier | Bits per channel | Levels per channel |
| --- | --- | --- |
| EightBit | 8 | 256 |
| SixteenBit | 4 | 16 |

The 16-bit machines the tier stands in for drove a **12-bit DAC** — four
bits each of red, green and blue, 4096 colours — so a channel there may
only hold one of sixteen levels. An n-bit channel widens to eight by
**replication**: `0xE` becomes `0xEE`. Not by a left shift, which gives
`0xE0` and tops out short of white.

The 8-bit tier is limited by its 16-entry palette instead, not by channel
depth, so it keeps all eight bits and the rule is a no-op there.

This is independent of the palette rule above. 16-bit is still
direct-colour, so a bitmap need not use a colour the palette *names* — it
just has to use one the tier could *produce*. Both the palette entries and
every opaque bitmap pixel are checked.

`AssetColourBudget.ChannelBits` gives the depth, `IsOnGrid` tests one
colour and `NearestLevel` snaps one channel; `AssetColourBudget.OffGrid`
records the strays per asset and `AssetSet.Load` throws with the asset and
the colour named. Alpha is not a DAC channel and is not checked here — it
is covered by `PartialAlphaCount`.

### Baseline, and how the assets were brought inside it (2026-07-28)

Counted over every committed `.bmp`, treating alpha 0 as transparent,
the sets started out at:

| Game | Distinct opaque colours | vs 4096 |
| --- | --- | --- |
| Elite | 2481 | passed |
| Stunt Car Racer | 5095 | **failed** |

The cause was anti-aliasing in two files: `font2.bmp` alone contributed
2431 colours (Elite's entire set was only 2481, so that one font *was*
Elite's budget), and SCR added `atlas.bmp`'s 2676 on top with almost no
overlap. `atlas.bmp` also held all 2765 of the partial-alpha pixels.

A bitmap font with 2431 distinct colours is not a 16-bit-era asset by
any reading — those were effectively modern-tier assets occupying the
16-bit slot, so the fix was to posterise them, not to soften the rule.
Both files were quantised to the **12-bit RGB space real 16-bit hardware
used** (keep each channel's high nibble and replicate it, so 0xFF stays
0xFF and 0x00 stays 0x00), and `atlas.bmp`'s alpha snapped to 0 or 255
at a threshold of 128. That choice follows from the tier's own premise
rather than from picking a number that merely fits, and it caps what any
one 16-bit asset can contribute at 4096 by construction.

| Game | After quantising | Partial alpha |
| --- | --- | --- |
| Elite | 145 | 0 |
| Stunt Car Racer | 349 | 0 |

Only `font2.bmp` and `atlas.bmp` were touched; every other asset was
already well inside the budget and unchanged by quantisation. Because
the rest of the set is not quantised, the union staying under 4096 is
not structurally guaranteed — but at 145 and 349 the headroom is large.

Note this quantisation applies to *authoring* the 16-bit assets. The
validator itself still only counts distinct values, exactly as decided;
it does not require colours to sit in the 12-bit space.

## Eager loading and validation

Assets are loaded and validated up front, never on demand.

Images are already loaded eagerly, but in two separate places —
`SoftwareGraphics` decodes via `BitmapReader`, while `SDLGraphics`
decodes via SDL. The SDL path therefore bypasses the managed reader
entirely, and would bypass any validation added to it.

Both are replaced by a single eager `AssetSet` load that decodes every
image and bitmap font for the active tier, validates them, and hands
both graphics backends the same `FastBitmap` instances. One code path,
one place the tier rule is enforced.

`AssetSet` lives in `Useful.Graphics`, not `Useful.Assets` as first
sketched: it holds `FastBitmap`s, and `Useful.Graphics` already depends
on `Useful.Assets`, so putting it the other way round would invert that
reference. `IAssetLocator` gained a `Tier` property, since the validator
has to know which cap applies.

The bitmap fonts are decoded even for the SDL backend, which draws text
with TrueType fonts and never uses them: they are part of the tier's
set, so they count against its budget, and validation has to give the
same answer whichever backend is running.

Validation accumulates the set of distinct opaque ARGB values across the
whole tier's assets and fails startup if the count exceeds the tier's
cap, or if any pixel's alpha is neither 0 nor 255. A per-asset breakdown
is logged at Information first, so the failure names the files to look
at.

## Build

Both game `.csproj` files enumerate their assets as roughly 120
individual `<None Update>` entries. Adding a second tier makes that
untenable, so the lists are replaced by a glob before any assets move:

```xml
<None Update="Assets\**\*" CopyToOutputDirectory="PreserveNewest" />
<None Update="Assets\SFX\*.wav;Assets\Music\*.ogg"
      CopyToOutputDirectory="Never" />
```

The second line preserves the current intent of keeping the
uncompressed sources in source control but out of the build output.

## Sequencing

Each step is independently verifiable, and steps 1–5 do not depend on
the 8-bit art existing.

1. Glob the asset lists in both game `.csproj` files → verify: build
   both games, asset output directory is unchanged.
2. `ImageReader` with the reworked BMP decoder and the new PNG decoder
   → verify: unit tests per bit depth, padding case and top-down case;
   existing bitmaps still load identically.
3. Move the existing images, bitmap fonts and palette into
   `SixteenBit/`; add `SystemTier` and tier resolution to
   `AssetLocator`, defaulting to `SixteenBit` → verify: no behaviour
   change, both games smoke-test identically.
4. Eager `AssetSet` replacing the two backend-specific loads, with the
   colour validator warn-only → verify: both backends render
   identically; the warning reports SCR at 5095.
5. Posterise `font2.bmp` and `atlas.bmp` within budget; flip the
   validator to hard-fail → verify: both games start; visual
   smoke-test for acceptable font and cockpit quality.
6. Add the 8-bit bitmaps under `EightBit/` and declare the tier →
   verify: the tier loads and passes the 16-colour cap.

None of these steps require the resolution-tier rendering work in
[backlog-roadmap.md](backlog-roadmap.md); selecting a tier from
configuration meets that work at step 6.
