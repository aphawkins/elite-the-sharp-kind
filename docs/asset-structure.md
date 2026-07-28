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

Only some asset categories vary by tier. Audio, models, TrueType fonts
and tracks are resolution-independent and are not duplicated. Images,
bitmap fonts and the palette are tier-specific, so those categories —
and only those — gain a tier subfolder:

```
Assets/
  AssetManifest.json
  Images/
    EightBit/     scanner.bmp, elitetext.bmp, ...
    SixteenBit/   scanner.bmp, elitetext.bmp, ...
  FontsBitmap/
    EightBit/     font1.bmp, font2.bmp
    SixteenBit/   font1.bmp, font2.bmp
  Palette/
    EightBit/     palette.json
    SixteenBit/   palette.json
  Models/  SFX/  Music/  SoundFonts/  FontsTrueType/   <- tier-neutral
```

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
  "Tiers": [ "EightBit", "SixteenBit" ],
  "TierOverrides": {
    "EightBit": { "Images": { "LaserBeam": "laser.bmp" } }
  }
}
```

* `Tiers` declares which tiers actually ship, so selecting an absent tier
  fails at startup rather than at first draw.
* `TierOverrides` is the escape hatch for the cases where a tier's set
  genuinely differs — a merged sprite sheet, or a bitmap with no
  equivalent at that tier.

## Resolution

`AssetLocator` is the only place asset paths are built, so tier
resolution lives there and nowhere else. The rule is:

1. `<Category>/<Tier>/<file>`
2. falling back to `<Category>/<file>`

The fallback is what keeps tier-neutral categories from needing a copy
per tier.

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

The cap is a count of distinct values only — any 32-bit RGB values are
permitted, they need not be quantised to a 12-bit or 9-bit colour space.

The cap applies to the **union across one game's whole asset set for the
active tier**, not per image: a game's tier corresponds to one machine's
palette. The union is per game, so Elite and Stunt Car Racer each get
their own budget.

Fully transparent pixels are excluded from the count. Alpha is enforced
to be either 0 or 255 on both tiers, matching the renderer, which
already treats transparency as binary.

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
