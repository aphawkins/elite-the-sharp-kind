// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// An Enigma reflector.
/// </summary>
public sealed class EnigmaReflector : IEnigmaReflector
{
    /// <summary>
    /// The substitution of the rotor, effectively the wiring.
    /// </summary>
    private ReflectorSettings _wiring;

    private EnigmaReflectorNumber _reflectorNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnigmaReflector"/> class.
    /// </summary>
    public EnigmaReflector()
    {
        _reflectorNumber = EnigmaReflectorNumber.B;
        _wiring = GetWiring(_reflectorNumber);
    }

    /// <inheritdoc />
    public EnigmaReflectorNumber ReflectorNumber
    {
        get => _reflectorNumber;

        set
        {
            _reflectorNumber = value;
            _wiring = GetWiring(_reflectorNumber);
        }
    }

    /// <inheritdoc />
    public char Reflect(char letter) => _wiring.Reflect(letter);

    private static ReflectorSettings GetWiring(EnigmaReflectorNumber reflectorNumber)
    {
        Dictionary<EnigmaReflectorNumber, IList<char>> wiring = new()
        {
            { EnigmaReflectorNumber.B, "YRUHQSLDPXNGOKMIEBFZCWVJAT".ToCharArray() },
            { EnigmaReflectorNumber.C, "FVPJIAOYEDRZXWGCTKUQSBNMHL".ToCharArray() },
        };

        return new() { CharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(), Substitutions = wiring[reflectorNumber] };
    }
}
