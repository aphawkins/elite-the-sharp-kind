// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Fakes.Audio;

namespace Useful.Audio.Tests;

public class AudioControllerTests
{
    [Fact]
    public void PlayEffectIsThrottledWhileTheSampleIsStillPlaying()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample> { { "Smash", new(2) } });

        // Act
        audio.PlayEffect("Smash");
        audio.PlayEffect("Smash");

        // Assert
        Assert.Equal(1, sound.PlayCount("Smash"));
    }

    [Fact]
    public void PlayEffectReplaysOnceTheCooldownHasElapsed()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample> { { "Smash", new(2) } });

        // Act
        audio.PlayEffect("Smash");
        audio.UpdateSound();
        audio.PlayEffect("Smash");
        audio.UpdateSound();
        audio.PlayEffect("Smash");

        // Assert: blocked after one tick, replayed after two
        Assert.Equal(2, sound.PlayCount("Smash"));
    }

    [Fact]
    public void EffectsSharingASampleShareTheCooldown()
    {
        // Arrange
        FakeSound sound = new();
        SfxSample shared = new(2);
        AudioController audio = new(
            sound,
            new Dictionary<string, SfxSample> { { "OffRoad", shared }, { "Wreck", shared } });

        // Act
        audio.PlayEffect("OffRoad");
        audio.PlayEffect("Wreck");

        // Assert
        Assert.Equal(1, sound.PlayCount("OffRoad"));
        Assert.Equal(0, sound.PlayCount("Wreck"));
    }

    [Fact]
    public void SharedSamplesTickOncePerUpdate()
    {
        // Arrange
        FakeSound sound = new();
        SfxSample shared = new(2);
        AudioController audio = new(
            sound,
            new Dictionary<string, SfxSample> { { "OffRoad", shared }, { "Wreck", shared } });

        // Act & Assert: one update must not tick the shared cooldown twice
        audio.PlayEffect("OffRoad");
        audio.UpdateSound();
        audio.PlayEffect("Wreck");
        Assert.Equal(0, sound.PlayCount("Wreck"));

        audio.UpdateSound();
        audio.PlayEffect("Wreck");
        Assert.Equal(1, sound.PlayCount("Wreck"));
    }

    [Fact]
    public void PlayEffectUsesTheSamplesStaticVolumeAndPanWhenNotOverridden()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample> { { "Smash", new(2, volume: 0.5f, pan: -1f) } });

        // Act
        audio.PlayEffect("Smash");

        // Assert
        Assert.Equal(0.5f, sound.LastVolume);
        Assert.Equal(-1f, sound.LastPan);
        Assert.Equal(1.0, sound.LastPitch);
    }

    [Fact]
    public void PlayEffectOverridesVolumeAndPitchPerPlayButKeepsTheSamplesPan()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample> { { "Creak", new(2, volume: 1f, pan: 1f) } });

        // Act
        audio.PlayEffect("Creak", 0.25f, 2.0);

        // Assert
        Assert.Equal(0.25f, sound.LastVolume);
        Assert.Equal(1f, sound.LastPan);
        Assert.Equal(2.0, sound.LastPitch);
    }
}
