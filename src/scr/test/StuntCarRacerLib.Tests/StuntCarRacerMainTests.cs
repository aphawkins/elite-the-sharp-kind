// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Fakes;
using Xunit;

namespace StuntCarRacerLib.Tests;

public class StuntCarRacerMainTests
{
    [Fact]
    public void ConstructWithFakeAbstractionSucceeds()
    {
        // Arrange
        FakeAbstraction abstraction = new();

        // Act
        StuntCarRacerMain game = new(abstraction);

        // Assert
        Assert.NotNull(game);
    }

    [Fact]
    public void ConstructWithNullAbstractionThrows()
        => Assert.Throws<ArgumentNullException>(() => new StuntCarRacerMain(null!));
}
