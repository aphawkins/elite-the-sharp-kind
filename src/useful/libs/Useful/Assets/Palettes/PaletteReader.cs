// 'Useful Libraries' - Andy Hawkins 2025.

using System.Text.Json;

namespace Useful.Assets.Palettes;

public static class PaletteReader
{
    public static IPaletteCollection Read(string filePath)
    {
        string lines = File.ReadAllText(filePath);
        Dictionary<string, string> colors = JsonSerializer.Deserialize<Dictionary<string, string>>(lines) ?? [];
        return new Palette(
            colors.ToDictionary(x => x.Key, x => Convert.ToUInt32(x.Value.Replace("#", "0x", StringComparison.OrdinalIgnoreCase), 16)));
    }
}
