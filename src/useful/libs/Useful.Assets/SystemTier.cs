// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Useful.Assets;

// The class of machine a game's look is being reproduced from. Each tier
// has its own bitmap set and its own colour budget; see
// docs/asset-structure.md.
//
// The JSON names are the digit forms - "8Bit", "16Bit" - because that is how
// a config file reads naturally. The member names stay spelled out, because
// an identifier cannot start with a digit, and AssetLocator builds asset
// directory paths from the member name: the folders are Assets/Images/
// SixteenBit and friends, not the JSON spelling.
// SystemTierConverter carries the same spellings for the config binder,
// which reads the file back and does not go through System.Text.Json.
[JsonConverter(typeof(JsonStringEnumConverter<SystemTier>))]
[TypeConverter(typeof(SystemTierConverter))]
public enum SystemTier
{
    [JsonStringEnumMemberName("8Bit")]
    EightBit = 0,

    [JsonStringEnumMemberName("16Bit")]
    SixteenBit = 1,
}
