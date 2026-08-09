// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Graphics.Rendering;

// How a computed colour is reduced to one the display can show. Only the
// method is a choice: what the display can show at all is the rendition's,
// since it is a fact about the machine being stood in for, not a preference.
public enum Quantisation
{
    // Take the closest colour available and accept the banding. One answer per
    // colour, so a whole face resolves at once.
    Nearest = 0,

    // Alternate per pixel between the two colours either side of the one
    // wanted, in the proportion that averages out to it. Trades banding for
    // texture, and buys back shades a limited palette cannot hold - which is
    // what the hardware these renditions stand in for actually did.
    Ordered = 1,
}
