// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Xunit;

namespace Useful.Security.Cryptography.Tests;

public class EnigmaRotorsTests
{
    [Fact]
    public void CurrentSettingDefaults()
    {
        EnigmaRotors settings = new();
        Assert.Equal('A', settings[EnigmaRotorPosition.Fastest].CurrentSetting);
        Assert.Equal('A', settings[EnigmaRotorPosition.Second].CurrentSetting);
        Assert.Equal('A', settings[EnigmaRotorPosition.Third].CurrentSetting);
    }

    [Fact]
    public void CurrentSettingInvalid()
    {
        EnigmaRotors settings = new();
        Assert.Throws<ArgumentOutOfRangeException>(() => settings[EnigmaRotorPosition.Fastest].CurrentSetting = 'Å');
    }

    [Fact]
    public void CurrentSettingSet()
    {
        EnigmaRotors settings = new();
        settings[EnigmaRotorPosition.Fastest].CurrentSetting = 'B';
        settings[EnigmaRotorPosition.Second].CurrentSetting = 'D';
        settings[EnigmaRotorPosition.Third].CurrentSetting = 'E';
        Assert.Equal('B', settings[EnigmaRotorPosition.Fastest].CurrentSetting);
        Assert.Equal('D', settings[EnigmaRotorPosition.Second].CurrentSetting);
        Assert.Equal('E', settings[EnigmaRotorPosition.Third].CurrentSetting);
    }

    [Fact]
    public void RingPosition()
    {
        EnigmaRotors settings = new();
        settings[EnigmaRotorPosition.Fastest].RingPosition = 02;
        settings[EnigmaRotorPosition.Second].RingPosition = 04;
        settings[EnigmaRotorPosition.Third].RingPosition = 05;
        Assert.Equal(2, settings[EnigmaRotorPosition.Fastest].RingPosition);
        Assert.Equal(4, settings[EnigmaRotorPosition.Second].RingPosition);
        Assert.Equal(5, settings[EnigmaRotorPosition.Third].RingPosition);
    }

    [Fact]
    public void RingPositionDefaults()
    {
        EnigmaRotors settings = new();
        Assert.Equal(1, settings[EnigmaRotorPosition.Fastest].RingPosition);
        Assert.Equal(1, settings[EnigmaRotorPosition.Second].RingPosition);
        Assert.Equal(1, settings[EnigmaRotorPosition.Third].RingPosition);
    }

    [Fact]
    public void RingPositionInvalid()
    {
        EnigmaRotors settings = new();
        Assert.Throws<ArgumentOutOfRangeException>(() => settings[EnigmaRotorPosition.Fastest].RingPosition = 'Å');
    }

    [Fact]
    public void RotorOrder()
    {
        EnigmaRotors settings = new()
        {
            Rotors = new Dictionary<EnigmaRotorPosition, IEnigmaRotor>()
            {
                { EnigmaRotorPosition.Fastest, new EnigmaRotor() { RotorNumber = EnigmaRotorNumber.IV } },
                { EnigmaRotorPosition.Second, new EnigmaRotor() { RotorNumber = EnigmaRotorNumber.II } },
                { EnigmaRotorPosition.Third, new EnigmaRotor() { RotorNumber = EnigmaRotorNumber.III } },
            },
        };

        Assert.Equal(EnigmaRotorNumber.IV, settings[EnigmaRotorPosition.Fastest].RotorNumber);
        Assert.Equal(EnigmaRotorNumber.II, settings[EnigmaRotorPosition.Second].RotorNumber);
        Assert.Equal(EnigmaRotorNumber.III, settings[EnigmaRotorPosition.Third].RotorNumber);
    }

    [Fact]
    public void RotorOrderDefaults()
    {
        EnigmaRotors settings = new();
        Assert.Equal(EnigmaRotorNumber.I, settings[EnigmaRotorPosition.Fastest].RotorNumber);
        Assert.Equal(EnigmaRotorNumber.II, settings[EnigmaRotorPosition.Second].RotorNumber);
        Assert.Equal(EnigmaRotorNumber.III, settings[EnigmaRotorPosition.Third].RotorNumber);
    }

    [Fact]
    public void RotorOrderInvalid() => Assert.Throws<ArgumentException>(
        () => new EnigmaRotors()
        {
            Rotors = new Dictionary<EnigmaRotorPosition, IEnigmaRotor>()
                    {
                        { EnigmaRotorPosition.Fastest, new EnigmaRotor() { RotorNumber = EnigmaRotorNumber.IV } },
                        { EnigmaRotorPosition.Second, new EnigmaRotor() { RotorNumber = EnigmaRotorNumber.IV } },
                        { EnigmaRotorPosition.Third, new EnigmaRotor() { RotorNumber = EnigmaRotorNumber.III } },
                    },
        });

    [Fact]
    public void RotorPositionsDefaults()
    {
        List<EnigmaRotorPosition> positions = [.. EnigmaRotors.RotorPositions];
        Assert.Equal(3, positions.Count);
        Assert.Equal(EnigmaRotorPosition.Fastest, positions[0]);
        Assert.Equal(EnigmaRotorPosition.Second, positions[1]);
        Assert.Equal(EnigmaRotorPosition.Third, positions[2]);
    }

    [Fact]
    public void RotorSet()
    {
        IList<EnigmaRotorNumber> rotors = [.. EnigmaRotors.RotorSet];
        Assert.Equal(8, rotors.Count);
        Assert.Equal(EnigmaRotorNumber.I, rotors[0]);
        Assert.Equal(EnigmaRotorNumber.II, rotors[1]);
        Assert.Equal(EnigmaRotorNumber.III, rotors[2]);
        Assert.Equal(EnigmaRotorNumber.IV, rotors[3]);
        Assert.Equal(EnigmaRotorNumber.V, rotors[4]);
        Assert.Equal(EnigmaRotorNumber.VI, rotors[5]);
        Assert.Equal(EnigmaRotorNumber.VII, rotors[6]);
        Assert.Equal(EnigmaRotorNumber.VIII, rotors[7]);
    }
}
