// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.ComponentModel;
using System.Globalization;

namespace Useful.Assets;

// Config files are written by System.Text.Json but read back through
// Microsoft.Extensions.Configuration's binder, which knows nothing about
// [JsonStringEnumMemberName] and would fall back to Enum.Parse - failing on
// "16Bit", since that is not the member name. This teaches the binder the
// same digit spellings, so a file written by the game reads back.
public sealed class SystemTierConverter : EnumConverter
{
    public SystemTierConverter()
        : base(typeof(SystemTier))
    {
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        => value is string text && TryParseDigitForm(text, out SystemTier tier)
            ? tier
            : base.ConvertFrom(context, culture, value);

    public override object? ConvertTo(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object? value,
        Type destinationType)
        => destinationType == typeof(string) && value is SystemTier tier
            ? DigitForm(tier)
            : base.ConvertTo(context, culture, value, destinationType);

    // The member name is still accepted on the way in, so a config file
    // written by an older build keeps working.
    private static bool TryParseDigitForm(string text, out SystemTier tier)
    {
        switch (text.Trim())
        {
            case "8Bit":
                tier = SystemTier.EightBit;
                return true;

            case "16Bit":
                tier = SystemTier.SixteenBit;
                return true;

            default:
                tier = default;
                return false;
        }
    }

    private static string DigitForm(SystemTier tier) => tier switch
    {
        SystemTier.EightBit => "8Bit",
        SystemTier.SixteenBit => "16Bit",
        _ => tier.ToString(),
    };
}
