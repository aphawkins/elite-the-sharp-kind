// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Useful.Security.Cryptography.Tests;

public partial class EnigmaSymmetricTests
{
    [Fact]
    public void RotorsCtor()
    {
        using SymmetricAlgorithm cipher = new EnigmaSymmetric();
        Assert.Equal(Encoding.Unicode.GetBytes("B|III II I|01 01 01|"), cipher.Key);
        Assert.Equal(Encoding.Unicode.GetBytes("A A A"), cipher.IV);
    }

    [Theory]
    [InlineData("B||01 01 01|")] // No rotors
    [InlineData("B|II I|01 01 01|")] // Too few rotors
    [InlineData("B|IV III II I|01 01 01|")] // Too many rotors
    [InlineData("B|I I I|01 01 01|")] // Repeat rotors
    [InlineData("B|III II X|01 01 01|")] // Invalid rotor
    [InlineData("B|III II 1|01 01 01|")] // Rotor as number
    [InlineData("B| III II I |01 01 01|")] // Rotor spacing
    [InlineData("B|III  II  I|01 01 01|")] // Rotor spacing
    [InlineData("B|III II I||")] // Rings missing
    [InlineData("B|III II I|01 01|")] // Too few rings
    [InlineData("B|III II I|01 01 01 01|")] // Too many rings
    [InlineData("B|III II I|01 01 1|")] // Rings number padding
    [InlineData("B|III II I|01 01 00|")] // Rings number too small
    [InlineData("B|III II I|01 01 27|")] // Rings number too large
    [InlineData("B|III II I|01 01 0A|")] // Rings nmber as letter
    [InlineData("B|III II I| 01 01 01 |")] // Rings spacing
    [InlineData("B|III II I|01  01  01|")] // Rings spacing
    public void RotorsOrRingsInvalid(string key)
    {
        using SymmetricAlgorithm cipher = new EnigmaSymmetric();
        Assert.Throws<ArgumentException>(nameof(cipher.Key), () => cipher.Key = Encoding.Unicode.GetBytes(key));
    }

    [Theory]
    [InlineData("B|III II I|01 01 01|")]
    [InlineData("B|I II III|01 01 01|")]
    [InlineData("B|V IV III|01 01 01|")]
    [InlineData("B|III II I|26 13 01|")]
    public void RotorsOrRingsValid(string key)
    {
        using SymmetricAlgorithm cipher = new EnigmaSymmetric()
        {
            Key = Encoding.Unicode.GetBytes(key),
        };
        Assert.Equal(Encoding.Unicode.GetBytes(key), cipher.Key);
        Assert.Equal(Encoding.Unicode.GetBytes("A A A"), cipher.IV);
    }

    [Theory]
    [InlineData("A A")] // Too Few
    [InlineData("A A A A")] // Too many
    [InlineData(" A A A ")] // Spacing
    [InlineData("A  A  A")] // Spacing
    public void SettingInvalid(string iv)
    {
        using SymmetricAlgorithm cipher = new EnigmaSymmetric();
        Assert.Throws<ArgumentException>(nameof(cipher.IV), () => cipher.IV = Encoding.Unicode.GetBytes(iv));
    }
}
