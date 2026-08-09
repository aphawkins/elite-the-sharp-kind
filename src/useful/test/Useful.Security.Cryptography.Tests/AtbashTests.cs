// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Xunit;

namespace Useful.Security.Cryptography.Tests;

public class AtbashTests
{
    public static TheoryData<string, string> Data => new()
    {
        { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ZYXWVUTSRQPONMLKJIHGFEDCBA" },
        { ">?@ [\\]", ">?@ [\\]" },
        { "Å", "Å" },
    };

    [Theory]
    [MemberData(nameof(Data))]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "zyxwvutsrqponmlkjihgfedcba")]
    public void DecryptCipher(string plaintext, string ciphertext)
    {
        Atbash cipher = new();
        Assert.Equal(plaintext, cipher.Decrypt(ciphertext));
    }

    [Theory]
    [MemberData(nameof(Data))]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "ZYXWVUTSRQPONMLKJIHGFEDCBA")]
    public void EncryptCipher(string plaintext, string ciphertext)
    {
        Atbash cipher = new();
        Assert.Equal(ciphertext, cipher.Encrypt(plaintext));
    }

    [Fact]
    public void Name()
    {
        Atbash cipher = new();
        Assert.Equal("Atbash", cipher.CipherName);
        Assert.Equal("Atbash", cipher.ToString());
    }
}
