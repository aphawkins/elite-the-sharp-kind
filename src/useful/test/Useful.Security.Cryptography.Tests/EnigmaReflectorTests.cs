// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Xunit;

namespace Useful.Security.Cryptography.Tests;

public class EnigmaReflectorTests
{
    [Fact]
    public void Defaults()
    {
        EnigmaReflector reflector = new();
        Assert.Equal(EnigmaReflectorNumber.B, reflector.ReflectorNumber);
    }

    [Theory]
    [InlineData(EnigmaReflectorNumber.B, "YRUHQSLDPXNGOKMIEBFZCWVJAT")]
    [InlineData(EnigmaReflectorNumber.C, "FVPJIAOYEDRZXWGCTKUQSBNMHL")]
    public void Reflect(EnigmaReflectorNumber reflectorNumber, string wiring)
    {
        ArgumentNullException.ThrowIfNull(wiring);

        const string characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        EnigmaReflector reflector = new()
        {
            ReflectorNumber = reflectorNumber,
        };
        Assert.Equal(reflectorNumber, reflector.ReflectorNumber);

        for (int i = 0; i < characterSet.Length; i++)
        {
            Assert.Equal(wiring[i], reflector.Reflect(characterSet[i]));
        }
    }
}
