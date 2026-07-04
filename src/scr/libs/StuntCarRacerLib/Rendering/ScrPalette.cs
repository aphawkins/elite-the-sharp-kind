// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

namespace StuntCarRacerLib.Rendering;

// The 42-entry Stunt Car Racer palette from the original StuntCarRacer.cpp.
// Indices 10-17 are car colours 1, 18-25 are car colours 2 and 26 onwards
// are the track colours (Track.ScrBaseColour is 26).
public static class ScrPalette
{
    private static readonly uint[] s_colours =
    [
        0xff000000, 0xff000000, 0xff000000, 0xff000000, 0xff000000,
        0xff000000, 0xff000000, 0xff000000, 0xff000000, 0xff000000,

        // car colours 1
        0xff000000, 0xff880022, 0xffaa0033, 0xffcc0044,
        0xffee0055, 0xff222233, 0xff444444, 0xff333333,

        // car colours 2
        0xff000000, 0xff220088, 0xff3300aa, 0xff4400cc,
        0xff5500ee, 0xff222233, 0xff444444, 0xff333333,

        // track colours
        0xff000000, 0xff999977, 0xffbbbb99, 0xffffff00, 0xff99bb33,
        0xff557777, 0xff55bbff, 0xff5599ff, 0xff335577, 0xff550000,
        0xff773333, 0xff995555, 0xffdd9999, 0xff777755, 0xffbbbbbb,
        0xffffffff,
    ];

    public static uint Colour(int colourIndex) => s_colours[colourIndex];
}
