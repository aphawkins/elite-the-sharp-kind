// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Assets.Palettes;

namespace Useful.Fakes.Assets;

public sealed class FakePalette : Dictionary<string, FastColor>, IPaletteCollection
{
    public new FastColor this[string key] => FakeColor.TestColor;
}
