// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Graphics.Rendering;

// Backing storage for the renderers' per-frame polygon lists. The New Kind
// capped its poly_chain at 100 and dropped whatever arrived after that
// silently; the cap is reachable here (20 universe objects against models
// running to 29 faces), so the buffer grows instead and nothing is dropped.
// It is reused frame to frame, so the doubling settles at the busiest scene
// seen and stops allocating.
internal static class PolygonBuffer
{
    // Covers an ordinary scene without a resize; the old fixed cap was 100.
    internal const int InitialCapacity = 128;

    internal static void EnsureCapacity(ref PolygonData[] polys, int required)
    {
        if (required <= polys.Length)
        {
            return;
        }

        int capacity = polys.Length;

        while (capacity < required)
        {
            capacity *= 2;
        }

        Array.Resize(ref polys, capacity);
    }
}
