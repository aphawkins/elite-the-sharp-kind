// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SharpKind.Assets.Palettes;

namespace SharpKind.Fakes.Assets;

public sealed class FakePalette : Dictionary<string, FastColor>, IPaletteCollection
{
    public new FastColor this[string key] => FakeColor.TestColor;
}
