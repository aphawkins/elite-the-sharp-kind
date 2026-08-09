// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Xunit;

namespace Useful.Security.Cryptography.Tests;

public class EnigmaSettingsTests
{
    [Fact]
    public void CtorDefault()
    {
        EnigmaSettings settings = new();
        Assert.Equal(EnigmaReflectorNumber.B, settings.Reflector.ReflectorNumber);
        Assert.Equal(EnigmaRotorNumber.I, settings.Rotors[EnigmaRotorPosition.Fastest].RotorNumber);
        Assert.Equal(EnigmaRotorNumber.II, settings.Rotors[EnigmaRotorPosition.Second].RotorNumber);
        Assert.Equal(EnigmaRotorNumber.III, settings.Rotors[EnigmaRotorPosition.Third].RotorNumber);
        Assert.Equal(1, settings.Rotors[EnigmaRotorPosition.Fastest].RingPosition);
        Assert.Equal(1, settings.Rotors[EnigmaRotorPosition.Second].RingPosition);
        Assert.Equal(1, settings.Rotors[EnigmaRotorPosition.Third].RingPosition);
        Assert.Equal('A', settings.Rotors[EnigmaRotorPosition.Fastest].CurrentSetting);
        Assert.Equal('A', settings.Rotors[EnigmaRotorPosition.Second].CurrentSetting);
        Assert.Equal('A', settings.Rotors[EnigmaRotorPosition.Third].CurrentSetting);
        Assert.Equal(0, settings.Plugboard.SubstitutionCount);
        Assert.Empty(settings.Plugboard.Substitutions());
    }

    [Fact]
    public void Reflector()
    {
        EnigmaSettings settings = new()
        {
            Reflector = new EnigmaReflector() { ReflectorNumber = EnigmaReflectorNumber.C },
        };

        Assert.Equal(EnigmaReflectorNumber.C, settings.Reflector.ReflectorNumber);
    }

    [Fact]
    public void Rotors()
    {
        EnigmaRotors rotors = new()
        {
            Rotors = new Dictionary<EnigmaRotorPosition, IEnigmaRotor>()
            {
                {
                    EnigmaRotorPosition.Fastest,
                    new EnigmaRotor()
                        {
                            RotorNumber = EnigmaRotorNumber.VI,
                            RingPosition = 2,
                            CurrentSetting = 'X',
                        }
                },
                {
                    EnigmaRotorPosition.Second,
                    new EnigmaRotor()
                        {
                            RotorNumber = EnigmaRotorNumber.VII,
                            RingPosition = 3,
                            CurrentSetting = 'Y',
                        }
                },
                {
                    EnigmaRotorPosition.Third,
                    new EnigmaRotor()
                        {
                            RotorNumber = EnigmaRotorNumber.VIII,
                            RingPosition = 4,
                            CurrentSetting = 'Z',
                        }
                },
            },
        };

        EnigmaSettings settings = new()
        {
            Rotors = rotors,
        };

        Assert.Equal(rotors.Rotors[EnigmaRotorPosition.Fastest].RotorNumber, settings.Rotors[EnigmaRotorPosition.Fastest].RotorNumber);
        Assert.Equal(rotors.Rotors[EnigmaRotorPosition.Second].RotorNumber, settings.Rotors[EnigmaRotorPosition.Second].RotorNumber);
        Assert.Equal(rotors.Rotors[EnigmaRotorPosition.Third].RotorNumber, settings.Rotors[EnigmaRotorPosition.Third].RotorNumber);

        Assert.Equal(
            rotors.Rotors[EnigmaRotorPosition.Fastest].RingPosition,
            settings.Rotors[EnigmaRotorPosition.Fastest].RingPosition);
        Assert.Equal(
            rotors.Rotors[EnigmaRotorPosition.Second].RingPosition,
            settings.Rotors[EnigmaRotorPosition.Second].RingPosition);
        Assert.Equal(
            rotors.Rotors[EnigmaRotorPosition.Third].RingPosition,
            settings.Rotors[EnigmaRotorPosition.Third].RingPosition);

        Assert.Equal(
            rotors.Rotors[EnigmaRotorPosition.Fastest].CurrentSetting,
            settings.Rotors[EnigmaRotorPosition.Fastest].CurrentSetting);
        Assert.Equal(
            rotors.Rotors[EnigmaRotorPosition.Second].CurrentSetting,
            settings.Rotors[EnigmaRotorPosition.Second].CurrentSetting);
        Assert.Equal(
            rotors.Rotors[EnigmaRotorPosition.Third].CurrentSetting,
            settings.Rotors[EnigmaRotorPosition.Third].CurrentSetting);
    }

    [Fact]
    public void Plugboard()
    {
        EnigmaSettings settings = new()
        {
            Plugboard = new EnigmaPlugboard(new List<EnigmaPlugboardPair>()
                {
                    { new EnigmaPlugboardPair() { From = 'A', To = 'B' } },
                }),
        };

        Assert.Equal(1, settings.Plugboard.SubstitutionCount);
        Assert.Equal('B', settings.Plugboard.Substitutions()['A']);
    }
}
