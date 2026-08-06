// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Diagnostics;
using System.Text;

namespace Useful.Security.Cryptography;

/// <summary>
/// Simulates the Enigma encoding machine.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Enigma"/> class.
/// </remarks>
/// <param name="settings">The cipher's settings.</param>
public sealed class Enigma(IEnigmaSettings settings) : ICipher
{
    private const string CharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <inheritdoc />
    public string CipherName => "Enigma M3";

    /// <summary>
    /// Gets or sets settings.
    /// </summary>
    public IEnigmaSettings Settings { get; set; } = settings;

    /// <inheritdoc />
    public override string ToString() => CipherName;

    /// <inheritdoc />
    public string Decrypt(string ciphertext) => Encrypt(ciphertext);

    /// <inheritdoc />
    public string Encrypt(string plaintext)
    {
        ArgumentNullException.ThrowIfNull(plaintext);

        StringBuilder output = new();
        foreach (char inputChar in plaintext)
        {
            if (inputChar == ' ')
            {
                output.Append(' ');
            }
            else if (CharacterSet.Contains(inputChar, StringComparison.InvariantCultureIgnoreCase))
            {
                output.Append(Encrypt(char.ToUpperInvariant(inputChar)));
            }
        }

        return output.ToString();
    }

    /// <summary>
    /// Generates random settings.
    /// </summary>
    public void GenerateSettings() => Settings = EnigmaSettingsGenerator.Generate() with { };

    /// <summary>
    /// Encrypt a plaintext letter into an enciphered letter.  Decipher works in the same way as encipher.
    /// </summary>
    /// <param name="letter">The plaintext letter to encipher.</param>
    /// <returns>The encrypted letter.</returns>
    private char Encrypt(char letter)
    {
        // Advance the rotors one position
        Settings.Rotors.AdvanceRotors();

        // Plugboard
        char newLetter = Settings.Plugboard.GetSubstitution(letter);

        // Go thru the rotors forwards
        newLetter = Settings.Rotors[EnigmaRotorPosition.Fastest].Forward(newLetter);
        newLetter = Settings.Rotors[EnigmaRotorPosition.Second].Forward(newLetter);
        newLetter = Settings.Rotors[EnigmaRotorPosition.Third].Forward(newLetter);

        // Go thru the relector
        newLetter = Settings.Reflector.Reflect(newLetter);

        // Go thru the rotors backwards
        newLetter = Settings.Rotors[EnigmaRotorPosition.Third].Backward(newLetter);
        newLetter = Settings.Rotors[EnigmaRotorPosition.Second].Backward(newLetter);
        newLetter = Settings.Rotors[EnigmaRotorPosition.Fastest].Backward(newLetter);

        newLetter = Settings.Plugboard.GetSubstitution(newLetter);

        // Letter cannot encrypt to itself.
        Debug.Assert(letter != newLetter, "Letter cannot encrypt to itself.");
        return newLetter;
    }
}
