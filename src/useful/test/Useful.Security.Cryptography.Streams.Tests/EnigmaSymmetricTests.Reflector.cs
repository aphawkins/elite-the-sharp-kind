// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Useful.Security.Cryptography.Tests;

public partial class EnigmaSymmetricTests
{
    [Fact]
    public void ReflectorCtor()
    {
        using SymmetricAlgorithm cipher = new EnigmaSymmetric();
        Assert.Equal(Encoding.Unicode.GetBytes("B|III II I|01 01 01|"), cipher.Key);
    }

    [Theory]
    [InlineData("1|III II I|01 01 01|")] // Reflector as number
    [InlineData("b|III II I|01 01 01|")] // Incorrect case
    [InlineData("Z|III II I|01 01 01|")] // Incorrect reflector letter
    [InlineData("BC|III II I|01 01 01|")] // Too many letters
    public void ReflectorInvalid(string key)
    {
        using SymmetricAlgorithm cipher = new EnigmaSymmetric();
        Assert.Throws<ArgumentException>(nameof(cipher.Key), () => cipher.Key = Encoding.Unicode.GetBytes(key));
    }

    [Theory]
    [InlineData("B|III II I|01 01 01|")]
    [InlineData("C|III II I|01 01 01|")]
    public void ReflectorValid(string key)
    {
        using SymmetricAlgorithm cipher = new EnigmaSymmetric
        {
            Key = Encoding.Unicode.GetBytes(key),
        };
        Assert.Equal(Encoding.Unicode.GetBytes(key), cipher.Key);
    }
}
