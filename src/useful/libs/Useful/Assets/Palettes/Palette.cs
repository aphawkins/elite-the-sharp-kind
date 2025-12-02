// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Assets.Palettes;

public class Palette(IDictionary<string, uint> dictionary) : Dictionary<string, uint>(dictionary), IPaletteCollection
{
}
