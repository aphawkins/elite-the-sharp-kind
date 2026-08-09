// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// Settings for the Caesar cipher.
/// </summary>
public sealed record CaesarSettings : ICaesarSettings
{
    /// <summary>
    /// The right shift.
    /// </summary>
    private int _rightShift;

    /// <summary>
    /// Initializes a new instance of the <see cref="CaesarSettings"/> class.
    /// </summary>
    public CaesarSettings() => _rightShift = 1;

    /// <summary>
    /// Gets or sets the right shift of the cipher.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Value must be between 0 and 25.</exception>
    public int RightShift
    {
        get => _rightShift;

        set
        {
            if (value is < 0 or > 25)
            {
                throw new ArgumentOutOfRangeException(nameof(RightShift), "Value must be between 0 and 25.");
            }

            _rightShift = value;
        }
    }
}
