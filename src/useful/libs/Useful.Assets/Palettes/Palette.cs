// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Assets.Palettes;

public class Palette(IDictionary<string, FastColor> dictionary) : Dictionary<string, FastColor>(dictionary), IPaletteCollection
{
}
