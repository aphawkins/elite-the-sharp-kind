// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Xunit;

namespace Useful.Security.Cryptography.Tests;

/// <summary>
/// Pins the contract documented on <see cref="ICipher"/>: text round trips through the cipher,
/// with letters coming back upper cased and everything else untouched.
/// </summary>
public class CipherRoundTripTests
{
    public static TheoryData<string> CipherNames => [
        "Atbash",
        "ROT13",
        "Caesar",
        "Vigenere",
        "MonoAlphabetic",
        "Reflector",
    ];

    [Theory]
    [MemberData(nameof(CipherNames))]
    public void RoundTripUpperCases(string cipherName)
    {
        const string plaintext = "The quick brown fox jumps over the lazy dog!";

        ICipher cipher = Create(cipherName);
        string ciphertext = cipher.Encrypt(plaintext);

        Assert.Equal(plaintext.ToUpperInvariant(), cipher.Decrypt(ciphertext));
    }

    [Theory]
    [MemberData(nameof(CipherNames))]
    public void NonLettersPassThrough(string cipherName)
    {
        const string punctuation = "1234567890 !\"$%^&*()";

        Assert.Equal(punctuation, Create(cipherName).Encrypt(punctuation));
    }

    [Theory]
    [MemberData(nameof(CipherNames))]
    public void EncryptRejectsNull(string cipherName)
    {
        ICipher cipher = Create(cipherName);

        Assert.Throws<ArgumentNullException>(() => cipher.Encrypt(null!));
        Assert.Throws<ArgumentNullException>(() => cipher.Decrypt(null!));
    }

    private static ICipher Create(string cipherName)
        => cipherName switch
        {
            "Atbash" => new Atbash(),
            "ROT13" => new Rot13(),
            "Caesar" => new Caesar(new CaesarSettings() { RightShift = 7 }),
            "Vigenere" => new Vigenere(new VigenereSettings() { Keyword = "LEMON" }),
            "MonoAlphabetic" => new MonoAlphabetic(
                new MonoAlphabeticSettings() { Substitutions = "BADCFEHGJILKNMPORQTSVUXWZY" }),
            "Reflector" => new Reflector(new ReflectorSettings()),
            _ => throw new ArgumentException($"Unknown cipher '{cipherName}'.", nameof(cipherName)),
        };
}
