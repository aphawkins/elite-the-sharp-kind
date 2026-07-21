// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Tests;

public class RNGTests
{
    [Fact]
    public void GenerateRandomNumberIsDeterministicFromSeed()
    {
        RNG rngA = new(new Random(0)) { Seed = { A = 1, B = 2, C = 3, D = 4 } };
        RNG rngB = new(new Random(0)) { Seed = { A = 1, B = 2, C = 3, D = 4 } };

        int[] sequenceA = [rngA.GenerateRandomNumber(), rngA.GenerateRandomNumber(), rngA.GenerateRandomNumber()];
        int[] sequenceB = [rngB.GenerateRandomNumber(), rngB.GenerateRandomNumber(), rngB.GenerateRandomNumber()];

        Assert.Equal(sequenceA, sequenceB);
    }

    [Fact]
    public void GenerateRandomNumberStaysInByteRange()
    {
        RNG rng = new(new Random(0)) { Seed = { A = 17, B = 99, C = 200, D = 3 } };

        for (int i = 0; i < 500; i++)
        {
            Assert.InRange(rng.GenerateRandomNumber(), 0, 255);
        }
    }

    [Fact]
    public void GenMSXRandomNumberIsDeterministicFromSeed()
    {
        RNG rngA = new(new Random(0)) { Seed = { A = 5, B = 6, C = 7, D = 8 } };
        RNG rngB = new(new Random(0)) { Seed = { A = 5, B = 6, C = 7, D = 8 } };

        int[] sequenceA = [rngA.GenMSXRandomNumber(), rngA.GenMSXRandomNumber(), rngA.GenMSXRandomNumber()];
        int[] sequenceB = [rngB.GenMSXRandomNumber(), rngB.GenMSXRandomNumber(), rngB.GenMSXRandomNumber()];

        Assert.Equal(sequenceA, sequenceB);
    }

    [Fact]
    public void GenMSXRandomNumberStaysInRange()
    {
        RNG rng = new(new Random(0)) { Seed = { A = 250, B = 4, C = 128, D = 64 } };

        for (int i = 0; i < 500; i++)
        {
            Assert.InRange(rng.GenMSXRandomNumber(), 0, 4);
        }
    }

    [Fact]
    public void GaussianRandomStaysInRange()
    {
        RNG rng = new(new Random(0));

        for (int i = 0; i < 200; i++)
        {
            Assert.InRange(rng.GaussianRandom(-7, 8), -7, 7);
        }
    }
}
