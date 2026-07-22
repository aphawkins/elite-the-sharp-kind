// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Types;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests;

public class PlanetControllerTests
{
    [Fact]
    public void CalculateDistanceToPlanetIsZeroForSamePlanet()
    {
        GalaxySeed seed = new() { A = 1, B = 2, C = 3, D = 4, E = 5, F = 6 };

        float distance = PlanetController.CalculateDistanceToPlanet(seed, seed);

        Assert.Equal(0, distance);
    }

    [Fact]
    public void CalculateDistanceToPlanetIsSymmetric()
    {
        GalaxySeed from = new() { A = 1, B = 10, C = 3, D = 20, E = 5, F = 6 };
        GalaxySeed to = new() { A = 1, B = 200, C = 3, D = 100, E = 5, F = 6 };

        float forward = PlanetController.CalculateDistanceToPlanet(from, to);
        float backward = PlanetController.CalculateDistanceToPlanet(to, from);

        Assert.Equal(forward, backward);
    }

    [Fact]
    public void GeneratePlanetDataIsDeterministicFromSeed()
    {
        GalaxySeed seedA = new() { A = 17, B = 99, C = 200, D = 3, E = 128, F = 64 };
        GalaxySeed seedB = new() { A = 17, B = 99, C = 200, D = 3, E = 128, F = 64 };

        PlanetData planetA = PlanetController.GeneratePlanetData(seedA);
        PlanetData planetB = PlanetController.GeneratePlanetData(seedB);

        Assert.Equal(planetA.Economy, planetB.Economy);
        Assert.Equal(planetA.Government, planetB.Government);
        Assert.Equal(planetA.Population, planetB.Population);
        Assert.Equal(planetA.Productivity, planetB.Productivity);
        Assert.Equal(planetA.Radius, planetB.Radius);
        Assert.Equal(planetA.TechLevel, planetB.TechLevel);
    }

    [Fact]
    public void WaggleGalaxyIsDeterministicFromSeed()
    {
        GalaxySeed glxA = new() { A = 1, B = 2, C = 3, D = 4, E = 5, F = 6 };
        GalaxySeed glxB = new() { A = 1, B = 2, C = 3, D = 4, E = 5, F = 6 };
        PlanetController controllerA = new(CreateGameState());
        PlanetController controllerB = new(CreateGameState());

        controllerA.WaggleGalaxy(glxA);
        controllerB.WaggleGalaxy(glxB);

        Assert.Equal(glxA.A, glxB.A);
        Assert.Equal(glxA.B, glxB.B);
        Assert.Equal(glxA.C, glxB.C);
        Assert.Equal(glxA.D, glxB.D);
        Assert.Equal(glxA.E, glxB.E);
        Assert.Equal(glxA.F, glxB.F);
    }

    [Fact]
    public void WaggleGalaxyKeepsFieldsInByteRange()
    {
        GalaxySeed glx = new() { A = 255, B = 255, C = 255, D = 255, E = 255, F = 255 };
        PlanetController controller = new(CreateGameState());

        for (int i = 0; i < 1024; i++)
        {
            controller.WaggleGalaxy(glx);

            Assert.InRange(glx.A, 0, 255);
            Assert.InRange(glx.B, 0, 255);
            Assert.InRange(glx.C, 0, 255);
            Assert.InRange(glx.D, 0, 255);
            Assert.InRange(glx.E, 0, 255);
            Assert.InRange(glx.F, 0, 255);
        }
    }

    [Fact]
    public void NamePlanetDoesNotMutateInputSeed()
    {
        GalaxySeed glx = new() { A = 1, B = 2, C = 3, D = 4, E = 5, F = 6 };
        PlanetController controller = new(CreateGameState());

        _ = controller.NamePlanet(glx);

        Assert.Equal(1, glx.A);
        Assert.Equal(2, glx.B);
        Assert.Equal(3, glx.C);
        Assert.Equal(4, glx.D);
        Assert.Equal(5, glx.E);
        Assert.Equal(6, glx.F);
    }

    [Fact]
    public void NamePlanetIsDeterministicFromSeed()
    {
        GalaxySeed seedA = new() { A = 7, B = 8, C = 9, D = 10, E = 11, F = 12 };
        GalaxySeed seedB = new() { A = 7, B = 8, C = 9, D = 10, E = 11, F = 12 };
        PlanetController controller = new(CreateGameState());

        string nameA = controller.NamePlanet(seedA);
        string nameB = controller.NamePlanet(seedB);

        Assert.Equal(nameA, nameB);
        Assert.NotEmpty(nameA);
    }

    [Fact]
    public void FindPlanetNumberMatchesTheGalaxyWaggledThatManyTimes()
    {
        GalaxySeed galaxy = new() { A = 1, B = 2, C = 3, D = 4, E = 5, F = 6 };
        PlanetController controller = new(CreateGameState());
        GalaxySeed planet = new(galaxy);

        for (int i = 0; i < 4; i++)
        {
            controller.WaggleGalaxy(planet);
        }

        int foundIndex = controller.FindPlanetNumber(galaxy, planet);

        Assert.Equal(1, foundIndex);
    }

    [Fact]
    public void FindPlanetNumberReturnsMinusOneWhenPlanetIsNotInTheGalaxy()
    {
        GalaxySeed galaxy = new() { A = 1, B = 2, C = 3, D = 4, E = 5, F = 6 };
        PlanetController controller = new(CreateGameState());
        GalaxySeed notInGalaxy = new() { A = 255, B = 255, C = 255, D = 255, E = 255, F = 255 };

        int foundIndex = controller.FindPlanetNumber(galaxy, notInGalaxy);

        Assert.Equal(-1, foundIndex);
    }

    [Fact]
    public void FindPlanetByNameFindsTheFirstPlanetInTheGalaxyByItsOwnName()
    {
        GameState gameState = CreateGameState();
        gameState.Cmdr.Galaxy = new() { A = 1, B = 2, C = 3, D = 4, E = 5, F = 6 };
        PlanetController controller = new(gameState);
        string firstPlanetName = controller.NamePlanet(gameState.Cmdr.Galaxy);

        bool found = controller.FindPlanetByName(firstPlanetName);

        Assert.True(found);
        Assert.Equal(firstPlanetName, gameState.PlanetName);
        Assert.Equal(gameState.Cmdr.Galaxy.A, gameState.HyperspacePlanet.A);
        Assert.Equal(gameState.Cmdr.Galaxy.B, gameState.HyperspacePlanet.B);
    }

    [Fact]
    public void FindPlanetByNameReturnsFalseForAnUnknownName()
    {
        GameState gameState = CreateGameState();
        PlanetController controller = new(gameState);

        bool found = controller.FindPlanetByName("!!!Not A Real Planet!!!");

        Assert.False(found);
    }

    [Fact]
    public void DescribeInhabitantsIsHumanColonialWhenSeedEIsBelow128()
    {
        GalaxySeed planet = new() { E = 0 };
        PlanetController controller = new(CreateGameState());

        string description = controller.DescribeInhabitants(planet);

        Assert.Equal("(Human Colonials)", description);
    }

    [Fact]
    public void DescribeInhabitantsDescribesAlienSpeciesWhenSeedEIsAtLeast128()
    {
        GalaxySeed planet = new() { B = 1, D = 2, E = 128, F = 40 };
        PlanetController controller = new(CreateGameState());

        string description = controller.DescribeInhabitants(planet);

        Assert.StartsWith("(", description, StringComparison.Ordinal);
        Assert.EndsWith("s)", description, StringComparison.Ordinal);
        Assert.NotEqual("(Human Colonials)", description);
    }

    private static GameState CreateGameState()
    {
        ScreenManager<Screen, IView> views = new(new FakeKeyboard());
        return new(views);
    }
}
