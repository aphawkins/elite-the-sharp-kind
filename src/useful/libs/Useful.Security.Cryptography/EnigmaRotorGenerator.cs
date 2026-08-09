// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;

namespace Useful.Security.Cryptography;

/// <summary>
/// Enigma Reflector settings generator.
/// </summary>
internal static class EnigmaRotorGenerator
{
    public static IEnigmaRotors Generate()
    {
        int nextRandomNumber;
        Dictionary<EnigmaRotorPosition, IEnigmaRotor> rotors = [];
        List<int> usedRotorNumbers = [];

        foreach (EnigmaRotorPosition rotorPosition in EnigmaRotors.RotorPositions)
        {
            while (true)
            {
                nextRandomNumber = RandomNumberGenerator.GetInt32(0, EnigmaRotors.RotorSet.Count);
                if (!usedRotorNumbers.Contains(nextRandomNumber))
                {
                    usedRotorNumbers.Add(nextRandomNumber);
                    break;
                }
            }

            rotors[rotorPosition] = new EnigmaRotor()
            {
                RotorNumber = EnigmaRotors.RotorSet[nextRandomNumber],
                RingPosition = RandomNumberGenerator.GetInt32(1, 27),
                CurrentSetting = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[RandomNumberGenerator.GetInt32(0, 26)],
            };
        }

        return new EnigmaRotors()
        {
            Rotors = rotors,
        };
    }
}
