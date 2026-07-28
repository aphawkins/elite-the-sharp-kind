// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Text.Json.Serialization;

namespace Useful.Assets;

// The class of machine a game's look is being reproduced from. Each tier
// has its own bitmap set and its own colour budget; see
// docs/asset-structure.md.
[JsonConverter(typeof(JsonStringEnumConverter<SystemTier>))]
public enum SystemTier
{
    EightBit = 0,
    SixteenBit = 1,
}
