// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Assets.Palettes;

namespace Useful.Fakes.Assets;

public sealed class FakePalette : Dictionary<string, uint>, IPaletteCollection
{
    public new uint this[string key] => FakeColor.TestColor;
}
