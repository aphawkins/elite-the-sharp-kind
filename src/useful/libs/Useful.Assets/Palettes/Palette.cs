// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Assets.Palettes;

public class Palette(IDictionary<string, FastColor> dictionary) : Dictionary<string, FastColor>(dictionary), IPaletteCollection
{
}
