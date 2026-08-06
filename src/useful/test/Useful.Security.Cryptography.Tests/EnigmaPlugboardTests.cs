// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Xunit;

namespace Useful.Security.Cryptography.Tests;

public class EnigmaPlugboardTests
{
    [Fact]
    public void CtorEmpty()
    {
        EnigmaPlugboard settings = new();
        Assert.Equal(0, settings.SubstitutionCount);
        Assert.Empty(settings.Substitutions());
        Assert.Equal('A', settings.GetSubstitution('A'));
    }

    [Theory]
    [InlineData("AB BA")] // Repeat letters
    [InlineData("aB")] // Subs incorrect case
    [InlineData("AA")] // Same letter
    public void CtorSubstitutionsInvalid(string pairs)
        => Assert.Throws<ArgumentException>(nameof(pairs), () => new EnigmaPlugboard(ParsePairs(pairs)));

    [Theory]
    [InlineData("AB CD", 2)]
    public void CtorSubstitutionsValid(string pairs, int substitutionCount)
    {
        ArgumentNullException.ThrowIfNull(pairs);

        List<EnigmaPlugboardPair> plugs = ParsePairs(pairs);

        EnigmaPlugboard plugboard = new(plugs);
        Assert.Equal(substitutionCount, plugboard.SubstitutionCount);
        Assert.Equal(plugs[0].From, plugboard.GetSubstitution(plugs[0].To));
        Assert.Equal(plugs[0].To, plugboard.GetSubstitution(plugs[0].From));
    }

    /// <summary>
    /// Turns a space separated list of plugs, e.g. "AB CD", into plugboard pairs.
    /// </summary>
    private static List<EnigmaPlugboardPair> ParsePairs(string pairs)
        => [.. pairs.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => new EnigmaPlugboardPair { From = pair[0], To = pair[1] })];
}
