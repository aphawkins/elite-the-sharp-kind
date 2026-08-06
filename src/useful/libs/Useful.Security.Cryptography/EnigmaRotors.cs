// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// Enigma rotor settings.
/// </summary>
public sealed class EnigmaRotors : IEnigmaRotors
{
    private IReadOnlyDictionary<EnigmaRotorPosition, IEnigmaRotor> _rotors = new Dictionary<EnigmaRotorPosition, IEnigmaRotor>();

    /// <summary>
    /// Initializes a new instance of the <see cref="EnigmaRotors"/> class.
    /// </summary>
    public EnigmaRotors() => _rotors = new Dictionary<EnigmaRotorPosition, IEnigmaRotor>
        {
            { EnigmaRotorPosition.Fastest, new EnigmaRotor() { RotorNumber = EnigmaRotorNumber.I } },
            { EnigmaRotorPosition.Second, new EnigmaRotor() { RotorNumber = EnigmaRotorNumber.II } },
            { EnigmaRotorPosition.Third, new EnigmaRotor() { RotorNumber = EnigmaRotorNumber.III } },
        };

    /// <summary>
    /// Initializes a new instance of the <see cref="EnigmaRotors"/> class.
    /// </summary>
    /// <param name="rotors">The rotors.</param>
    public EnigmaRotors(IReadOnlyDictionary<EnigmaRotorPosition, IEnigmaRotor> rotors) => Rotors = rotors;

    /// <summary>
    /// Gets the rotor positions.
    /// </summary>
    /// <returns>The rotor positions.</returns>
    public static IEnumerable<EnigmaRotorPosition> RotorPositions
        => [
            EnigmaRotorPosition.Fastest,
            EnigmaRotorPosition.Second,
            EnigmaRotorPosition.Third,
        ];

    /// <summary>
    /// Gets all the rotors.
    /// </summary>
    /// <returns>All the rotors.</returns>
    public static IList<EnigmaRotorNumber> RotorSet
        => [
            EnigmaRotorNumber.I,
            EnigmaRotorNumber.II,
            EnigmaRotorNumber.III,
            EnigmaRotorNumber.IV,
            EnigmaRotorNumber.V,
            EnigmaRotorNumber.VI,
            EnigmaRotorNumber.VII,
            EnigmaRotorNumber.VIII,
        ];

    /// <inheritdoc />
    public IReadOnlyDictionary<EnigmaRotorPosition, IEnigmaRotor> Rotors
    {
        get => _rotors;

        init
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value[EnigmaRotorPosition.Fastest].RotorNumber == value[EnigmaRotorPosition.Second].RotorNumber
                || value[EnigmaRotorPosition.Fastest].RotorNumber == value[EnigmaRotorPosition.Third].RotorNumber
                || value[EnigmaRotorPosition.Second].RotorNumber == value[EnigmaRotorPosition.Third].RotorNumber)
            {
                throw new ArgumentException("This rotor is already in use.", nameof(value));
            }

            _rotors = value;
        }
    }

    /// <inheritdoc />
    public IEnigmaRotor this[EnigmaRotorPosition position] => _rotors[position];

    /// <inheritdoc />
    public void AdvanceRotors()
    {
        Advance(EnigmaRotorPosition.Fastest);

        foreach (char notch in _rotors[EnigmaRotorPosition.Fastest].Notches)
        {
            if ((((_rotors[EnigmaRotorPosition.Fastest].CurrentSetting - 1 - 'A' + 26) % 26) + 'A') == notch)
            {
                StepMiddleRotor();
            }

            // Doublestep the middle rotor when the right rotor is 2 past a notch and the middle is on a notch
            if ((((_rotors[EnigmaRotorPosition.Fastest].CurrentSetting - 2) % 'A') + 'A') == notch)
            {
                DoublestepMiddleRotor();
                break;
            }
        }
    }

    private void Advance(EnigmaRotorPosition position)
        => _rotors[position].CurrentSetting
            = (char)(((_rotors[position].CurrentSetting + 1 - 'A' + 26) % 26) + 'A');

    private void StepMiddleRotor()
    {
        Advance(EnigmaRotorPosition.Second);

        if (_rotors[EnigmaRotorPosition.Second].Notches
            .Contains((char)(_rotors[EnigmaRotorPosition.Second].CurrentSetting - 1), StringComparison.Ordinal))
        {
            Advance(EnigmaRotorPosition.Third);
        }
    }

    private void DoublestepMiddleRotor()
    {
        if (!_rotors[EnigmaRotorPosition.Second].Notches
            .Contains(_rotors[EnigmaRotorPosition.Second].CurrentSetting, StringComparison.Ordinal))
        {
            return;
        }

        Advance(EnigmaRotorPosition.Second);
        Advance(EnigmaRotorPosition.Third);
    }
}
