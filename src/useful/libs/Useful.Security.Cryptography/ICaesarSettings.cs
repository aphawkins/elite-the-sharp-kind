// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// Caesar cipher settings.
/// </summary>
public interface ICaesarSettings
{
    /// <summary>
    /// Gets or sets the right shift of the cipher.
    /// </summary>
    public int RightShift { get; set; }
}
