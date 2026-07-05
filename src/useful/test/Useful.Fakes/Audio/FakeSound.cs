// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Audio;

namespace Useful.Fakes.Audio;

// Minimal in-test fake implementation of ISound that counts what was played.
public sealed class FakeSound : ISound
{
    private readonly Dictionary<string, int> _playCounts = [];

    public int PlayLoopCount { get; private set; }

    public int StopLoopCount { get; private set; }

    public string? LastLoopSample { get; private set; }

    public int PlayCount(string sfxType) => _playCounts.GetValueOrDefault(sfxType);

    public void Play(string musicType, bool repeat)
    {
    }

    public void Play(string sfxType) => _playCounts[sfxType] = PlayCount(sfxType) + 1;

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
