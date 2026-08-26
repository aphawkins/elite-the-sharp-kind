// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SharpKind.Fakes.Audio;

namespace SharpKind.Audio.Tests;

public class AudioControllerTests
{
    [Fact]
    public void PlayEffectIsThrottledWhileTheSampleIsStillPlaying()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample> { { "Smash", new(2) } }, new());

        // Act
        audio.PlayEffect("Smash");
        audio.PlayEffect("Smash");

        // Assert
        Assert.Equal(1, sound.PlayCount("Smash"));
    }

    [Fact]
    public void PlayEffectNoOpsWhenNoSampleIsRegistered()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample>(), new());

        // Act
        audio.PlayEffect("Missing");

        // Assert
        Assert.Equal(0, sound.PlayCount("Missing"));
    }

    [Fact]
    public void PlayEffectReplaysOnceTheCooldownHasElapsed()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample> { { "Smash", new(2) } }, new());

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
            new Dictionary<string, SfxSample> { { "OffRoad", shared }, { "Wreck", shared } },
            new());

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
            new Dictionary<string, SfxSample> { { "OffRoad", shared }, { "Wreck", shared } },
            new());

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
        AudioController audio = new(sound, new Dictionary<string, SfxSample> { { "Smash", new(2, volume: 0.5f, pan: -1f) } }, new());

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
        AudioController audio = new(sound, new Dictionary<string, SfxSample> { { "Creak", new(2, volume: 1f, pan: 1f) } }, new());

        // Act
        audio.PlayEffect("Creak", 0.25f, 2.0);

        // Assert
        Assert.Equal(0.25f, sound.LastVolume);
        Assert.Equal(1f, sound.LastPan);
        Assert.Equal(2.0, sound.LastPitch);
    }

    [Fact]
    public void PlayEffectDoesNothingWhenEffectsAreOff()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(
            sound,
            new Dictionary<string, SfxSample> { { "Smash", new(2) } },
            new() { EffectsOn = false });

        // Act
        audio.PlayEffect("Smash");

        // Assert
        Assert.Equal(0, sound.PlayCount("Smash"));
    }

    [Fact]
    public void PlayMusicDoesNothingWhenMusicIsOff()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample>(), new() { MusicOn = false });

        // Act
        audio.PlayMusic("Theme", loop: true);

        // Assert
        Assert.Equal(0, sound.PlayMusicCount);
    }

    // StopMusic is unconditional: turning music off in the settings screen
    // sets MusicOn false and then calls it to silence what is already playing,
    // so it cannot depend on MusicOn still being true.
    [Fact]
    public void StopMusicStopsEvenWhenMusicIsOff()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample>(), new() { MusicOn = false });

        // Act
        audio.StopMusic();

        // Assert
        Assert.Equal(1, sound.StopMusicCount);
    }

    [Fact]
    public void TurningMusicOffAtRuntimeStopsFurtherPlayback()
    {
        // Arrange
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample>(), new() { MusicOn = true });

        // Act
        audio.PlayMusic("Theme", loop: true);
        audio.MusicOn = false;
        audio.PlayMusic("Theme", loop: true);

        // Assert
        Assert.Equal(1, sound.PlayMusicCount);
    }

    [Fact]
    public void AFractionOfATickAgesASampleByThatFraction()
    {
        // Four quarter-ticks make a tick, so a two-tick sample is still
        // running after seven of them and lapses on the eighth.
        //
        // The no-argument overload - the one Stunt Car Racer calls, which
        // must keep counting lifetimes in whole updates - is covered by
        // PlayEffectReplaysOnceTheCooldownHasElapsed above; that it still
        // passes unchanged is what says SCR's pacing was left alone.
        FakeSound sound = new();
        AudioController audio = new(sound, new Dictionary<string, SfxSample> { { "Smash", new(2) } }, new());

        audio.PlayEffect("Smash");
        for (int update = 0; update < 7; update++)
        {
            audio.UpdateSound(0.25f);
        }

        audio.PlayEffect("Smash");
        Assert.Equal(1, sound.PlayCount("Smash"));

        audio.UpdateSound(0.25f);
        audio.PlayEffect("Smash");

        Assert.Equal(2, sound.PlayCount("Smash"));
    }
}
