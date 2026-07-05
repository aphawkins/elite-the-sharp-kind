// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using Useful.Audio;

namespace StuntCarRacerLib.Fakes;

public sealed class FakeSound : ISound
{
    public int PlayLoopCount { get; private set; }

    public int StopLoopCount { get; private set; }

    public string? LastLoopSample { get; private set; }

    public void Play(string musicType, bool repeat)
    {
    }

    public void Play(string sfxType)
    {
    }

    public void StopMusic()
    {
    }

    public void PlayLoop(string sfxType, double pitch)
    {
        PlayLoopCount++;
        LastLoopSample = sfxType;
    }

    public void StopLoop() => StopLoopCount++;
}
