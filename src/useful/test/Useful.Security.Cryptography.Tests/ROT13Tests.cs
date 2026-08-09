// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Xunit;

namespace Useful.Security.Cryptography.Tests;

public class ROT13Tests
{
    public static TheoryData<string, string> Data => new()
    {
        { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "NOPQRSTUVWXYZABCDEFGHIJKLM" },
        { ">?@ [\\]", ">?@ [\\]" },
        { "Å", "Å" },
    };

    [Theory]
    [MemberData(nameof(Data))]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "nopqrstuvwxyzabcdefghijklm")]
    public void DecryptCipher(string plaintext, string ciphertext)
    {
        Rot13 cipher = new();
        Assert.Equal(plaintext, cipher.Decrypt(ciphertext));
    }

    [Theory]
    [MemberData(nameof(Data))]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "NOPQRSTUVWXYZABCDEFGHIJKLM")]
    public void EncryptCipher(string plaintext, string ciphertext)
    {
        Rot13 cipher = new();
        Assert.Equal(ciphertext, cipher.Encrypt(plaintext));
    }

    [Fact]
    public void Name()
    {
        Rot13 cipher = new();
        Assert.Equal("ROT13", cipher.CipherName);
        Assert.Equal("ROT13", cipher.ToString());
    }
}
