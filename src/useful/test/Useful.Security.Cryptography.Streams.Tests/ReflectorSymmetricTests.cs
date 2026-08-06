// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;
using System.Text;
using Xunit;

#pragma warning disable CA5390 // Do not hard-code encryption key

namespace Useful.Security.Cryptography.Streams.Tests;

public class ReflectorSymmetricTests
{
    public static TheoryData<string, string, string> Data => new()
    {
        { "ABC|ABC", "ABC", "ABC" }, // Default settings
        { "ABC|BAC", "ABC", "BAC" }, // Simple substitution
        { "ABCD|BADC", "ABCD", "BADC" }, // All characters subtituted
        { "ABC|ABC", "Å", "Å" }, // Invalid character
        { "ABC|BAC", "AB C", "BA C" }, // Space
    };

    [Fact]
    public void CtorSettings()
    {
        byte[] defaultKey = Encoding.Unicode.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZ|ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        using ReflectorSymmetric cipher = new();
        Assert.Equal(defaultKey, cipher.Key);
        Assert.Equal([], cipher.IV);
    }

    [Fact]
    public void KeySet()
    {
        using ReflectorSymmetric cipher = new();
        byte[] key = Encoding.Unicode.GetBytes("ABC|ABC");
        cipher.Key = key;
        Assert.Equal(key, cipher.Key);
    }

    [Fact]
    public void IvSet()
    {
        using ReflectorSymmetric cipher = new();
        cipher.IV = Encoding.Unicode.GetBytes("A");
        Assert.Equal([], cipher.IV);
    }

    [Theory]
    [MemberData(nameof(Data))]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ|BADCFEHGIJKLMNOPQRSTUVWXYZ", "HELLOWORLD", "GfLlOwOrLc")]
    public void Decrypt(string key, string plaintext, string ciphertext)
    {
        using SymmetricAlgorithm cipher = new ReflectorSymmetric
        {
            Key = Encoding.Unicode.GetBytes(key),
        };
        Assert.Equal(plaintext, CipherMethods.SymmetricTransform(cipher, CipherTransformMode.Decrypt, ciphertext));
    }

    [Theory]
    [MemberData(nameof(Data))]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ|BADCFEHGIJKLMNOPQRSTUVWXYZ", "HeLlOwOrLd", "GFLLOWORLC")]
    public void Encrypt(string key, string plaintext, string ciphertext)
    {
        using SymmetricAlgorithm cipher = new ReflectorSymmetric
        {
            Key = Encoding.Unicode.GetBytes(key),
        };
        Assert.Equal(ciphertext, CipherMethods.SymmetricTransform(cipher, CipherTransformMode.Encrypt, plaintext));
    }

    [Fact]
    public void IvGenerateCorrectness()
    {
        using ReflectorSymmetric cipher = new();
        cipher.GenerateIV();
        Assert.Equal([], cipher.IV);
    }

    [Fact]
    public void KeyGenerateCorrectness()
    {
        using ReflectorSymmetric cipher = new();
        string keyString;
        for (int i = 0; i < 100; i++)
        {
            cipher.GenerateKey();
            keyString = Encoding.Unicode.GetString(cipher.Key);

            // How to test for correctness?
            Assert.NotNull(keyString);
        }
    }

    [Fact]
    public void KeyGenerateRandomness()
    {
        bool diff = false;

        using (ReflectorSymmetric cipher = new())
        {
            byte[] key;
            byte[] newKey;

            cipher.GenerateKey();
            newKey = cipher.Key;
            key = newKey;

            for (int i = 0; i < 10; i++)
            {
                if (!newKey.SequenceEqual(key))
                {
                    diff = true;
                    break;
                }

                key = newKey;
                cipher.GenerateKey();
                newKey = cipher.Key;
            }
        }

        Assert.True(diff);
    }

    [Fact]
    public void Name()
    {
        using ReflectorSymmetric cipher = new();
        Assert.Equal("Reflector", cipher.ToString());
    }
}
