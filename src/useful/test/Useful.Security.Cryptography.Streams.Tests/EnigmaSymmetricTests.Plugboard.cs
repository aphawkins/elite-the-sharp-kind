// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Useful.Security.Cryptography.Tests;

public partial class EnigmaSymmetricTests
{
    [Fact]
    public void PlugboardCtor()
    {
        using SymmetricAlgorithm cipher = new EnigmaSymmetric();
        Assert.Equal(Encoding.Unicode.GetBytes("B|III II I|01 01 01|"), cipher.Key);
    }

    [Theory]
    [InlineData("B|III II I|01 01 01|ABCD")] // Too many subs
    [InlineData("B|III II I|01 01 01| AB")] // Subs spacing
    [InlineData("B|III II I|01 01 01|AB ")] // Subs spacing
    [InlineData("B|III II I|01 01 01|AB  CD")] // Subs spacing
    [InlineData("B|III II I|01 01 01|aB")] // Subs incorrect case
    [InlineData("B|III II I|01 01 01|AA")] // Incorrect subs letters
    public void PairsInvalid(string key)
    {
        using SymmetricAlgorithm cipher = new EnigmaSymmetric();
        Assert.Throws<ArgumentException>(nameof(cipher.Key), () => cipher.Key = Encoding.Unicode.GetBytes(key));
    }

    [Theory]
    [InlineData("B|III II I|01 01 01|AB CD")]
    public void PairsValid(string key)
    {
        using SymmetricAlgorithm cipher = new EnigmaSymmetric
        {
            Key = Encoding.Unicode.GetBytes(key),
        };
        Assert.Equal(Encoding.Unicode.GetBytes(key), cipher.Key);
    }
}
