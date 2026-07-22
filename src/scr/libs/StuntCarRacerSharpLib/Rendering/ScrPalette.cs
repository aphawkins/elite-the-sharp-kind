// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Globalization;
using Useful;
using Useful.Assets.Palettes;

namespace StuntCarRacerSharpLib.Rendering;

// The 42-entry Stunt Car Racer palette from the original StuntCarRacer.cpp,
// loaded from palette.json (see PaletteReader) and addressed positionally,
// matching the original's SCR_BASE_COLOUR-relative indexing. Indices 10-17
// are car colours 1, 18-25 are car colours 2 and 26 onwards are the track
// colours (Track.ScrBaseColour is 26).
public sealed class ScrPalette
{
    private readonly IPaletteCollection _palette;

    public ScrPalette()
        : this(PaletteReader.Read(Path.Combine(AppContext.BaseDirectory, "Assets", "Palette", "palette.json")))
    {
    }

    internal ScrPalette(IPaletteCollection palette)
    {
        Guard.ArgumentNull(palette);
        _palette = palette;
    }

    public FastColor Colour(int colourIndex) => _palette[colourIndex.ToString(CultureInfo.InvariantCulture)];
}
