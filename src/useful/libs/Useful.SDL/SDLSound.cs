// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Useful.Assets;
using Useful.Audio;
using static SDL2.SDL;
using static SDL2.SDL_mixer;

namespace Useful.SDL;

public sealed class SDLSound : ISound, IDisposable
{
    // Reserved so Mix_PlayChannel(-1, ...) (one-shot sfx) never picks this
    // channel; it is driven exclusively by the pitched loop below.
    private const int LoopChannel = 0;

    // Reserved for one-shot effects that need a pitch other than 1.0
    // (SDL_mixer has no built-in pitch control for one-shots either), using
    // the same resample-on-a-reserved-channel technique as the loop above,
    // but stopping after a single pass instead of wrapping.
    private const int OneShotPitchChannel = 1;

    private readonly Dictionary<string, float[]> _loopSamples = [];
    private readonly Dictionary<string, nint> _music;
    private readonly Dictionary<string, nint> _sfx;
    private bool _disposedValue;
    private Mix_EffectFunc_t? _loopEffect;
    private float[] _loopSampleData = [];
    private float[] _loopScratch = [];
    private string? _loopName;
    private double _loopPitch = 1.0;
    private double _loopPosition;
    private Mix_EffectFunc_t? _oneShotEffect;
    private float[] _oneShotSampleData = [];
    private float[] _oneShotScratch = [];
    private double _oneShotPitch = 1.0;
    private double _oneShotPosition;
    private bool _oneShotDone = true;

    public SDLSound(IAssetLocator assetLocator)
    {
        Guard.ArgumentNull(assetLocator);

        SDLGuard.Execute(() => SDL_Init(SDL_INIT_AUDIO));
        SDLGuard.Execute(() => Mix_Init(MIX_InitFlags.MIX_INIT_OGG));

        SDL_AudioSpec audioSpecDesired = default;
        audioSpecDesired.channels = 2;
        audioSpecDesired.format = AUDIO_F32SYS;
        audioSpecDesired.freq = 44100;
        audioSpecDesired.samples = 1024;

        SDLGuard.Execute(
            () => Mix_OpenAudio(audioSpecDesired.freq, audioSpecDesired.format, audioSpecDesired.channels, audioSpecDesired.samples));
        SDLGuard.Execute(() => Mix_AllocateChannels(16));
        SDLGuard.Execute(() => Mix_ReserveChannels(2));

        // Mix_QuerySpec uses the inverted (0 = error) convention. The
        // compiler can't see that the lambda runs synchronously, so these
        // need an initial value to satisfy definite-assignment analysis.
        int frequency = 0;
        ushort format = 0;
        int channels = 0;
        SDLGuard.Execute(() => Mix_QuerySpec(out frequency, out format, out channels), zeroIndicatesError: true);
        Debug.Assert(frequency == 44100, "Loop pitch-shifting assumes 44100Hz chunk data.");
        Debug.Assert(format == AUDIO_F32SYS, "Loop pitch-shifting assumes float32 chunk data.");
        Debug.Assert(channels == 2, "Loop pitch-shifting assumes stereo chunk data.");

        _music = assetLocator.MusicPaths.ToDictionary(
            x => x.Key,
            x =>
            {
                Debug.Assert(!string.IsNullOrWhiteSpace(x.Value), "Music is missing");
                return SDLGuard.Execute(() => Mix_LoadMUS(x.Value));
            });

        _sfx = assetLocator.SfxPaths.ToDictionary(
            x => x.Key,
            x => SDLGuard.Execute(() => Mix_LoadWAV(x.Value)));
    }

    // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~SDLSound()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Play(string sfxType, float volume, float pan, double pitch)
    {
        if (Math.Abs(pitch - 1.0) < 0.0001)
        {
            // Mix_PlayChannel returns -1 both on a real error and when every
            // channel is busy; either way, dropping this one-shot effect
            // beats crashing the game.
            int channel = Mix_PlayChannel(-1, _sfx[sfxType], 0);
            if (channel < 0)
            {
                return;
            }

            // Mix_Volume has no failure case (it always returns the previous
            // volume); wrapped for consistency with the rest of this class
            // routing every Mix_ call through SDLGuard.
            SDLGuard.Execute(() => Mix_Volume(channel, ToMixVolume(volume)));

            // Mix_SetPanning uses the inverted (0 = error) convention.
            SDLGuard.Execute(() => Mix_SetPanning(channel, ToLeftPan(pan), ToRightPan(pan)), zeroIndicatesError: true);
            return;
        }

        PlayOneShotPitched(sfxType, volume, pan, pitch);
    }

    public void Play(string musicType, bool repeat)
    {
        StopMusic();

        SDLGuard.Execute(() => Mix_PlayMusic(_music[musicType], repeat ? -1 : 1));
    }

    public void StopMusic() => SDLGuard.Execute(Mix_HaltMusic);

    // SDL_mixer has no built-in pitch control, so the loop is played on a
    // reserved channel whose output is entirely replaced, each callback, by
    // our own linearly-interpolated resample of the source chunk.
    // Mix_PlayChannel on that channel is only there to keep the mixer
    // invoking the effect callback; its own (unmodified) output is never
    // heard.
    public void PlayLoop(string sfxType, double pitch)
    {
        if (_loopName != sfxType)
        {
            StopLoop();

            _loopSampleData = GetLoopSamples(sfxType);
            _loopPosition = 0;
            _loopName = sfxType;
            _loopEffect = LoopEffect;

            // Channel 0 is reserved and used only here, so unlike the -1
            // (any free channel) case in Play(string), a failure here is a
            // real bug rather than expected contention — worth surfacing loudly.
            SDLGuard.Execute(() => Mix_PlayChannel(LoopChannel, _sfx[sfxType], -1));

            // Mix_RegisterEffect uses the inverted (0 = error) convention.
            SDLGuard.Execute(() => Mix_RegisterEffect(LoopChannel, _loopEffect, null, nint.Zero), zeroIndicatesError: true);
        }

        _loopPitch = pitch;
    }

    public void StopLoop()
    {
        if (_loopName is null)
        {
            return;
        }

        // Same inverted error convention as Mix_RegisterEffect above.
        SDLGuard.Execute(() => Mix_UnregisterEffect(LoopChannel, _loopEffect), zeroIndicatesError: true);

        // Mix_HaltChannel has no failure case at all (it always returns 0),
        // so wrapping it can never throw; kept for consistency with the rest
        // of this class routing every Mix_ call through SDLGuard.
        SDLGuard.Execute(() => Mix_HaltChannel(LoopChannel));
        _loopName = null;
        _loopEffect = null;
    }

    private static int ToMixVolume(float volume) => Math.Clamp((int)MathF.Round(volume * MIX_MAX_VOLUME), 0, MIX_MAX_VOLUME);

    private static byte ToLeftPan(float pan) => (byte)Math.Clamp(MathF.Round((1f - pan) * 127.5f), 0, 255);

    private static byte ToRightPan(float pan) => (byte)Math.Clamp(MathF.Round((1f + pan) * 127.5f), 0, 255);

    // Same resample technique as PlayLoop, but OneShotEffect stops after a
    // single pass through the sample instead of wrapping, so the reserved
    // channel can be left running indefinitely (silent between plays)
    // rather than started/stopped per play.
    private void PlayOneShotPitched(string sfxType, float volume, float pan, double pitch)
    {
        _oneShotSampleData = GetLoopSamples(sfxType);
        _oneShotPosition = 0;
        _oneShotPitch = pitch;
        _oneShotDone = false;

        if (_oneShotEffect is null)
        {
            _oneShotEffect = OneShotEffect;

            // Channel 1 is reserved and used only here, playing a
            // placeholder chunk on an infinite loop purely to keep the
            // mixer invoking the effect callback; its own (unmodified)
            // output is never heard - see PlayLoop above.
            SDLGuard.Execute(() => Mix_PlayChannel(OneShotPitchChannel, _sfx[sfxType], -1));

            // Mix_RegisterEffect uses the inverted (0 = error) convention.
            SDLGuard.Execute(() => Mix_RegisterEffect(OneShotPitchChannel, _oneShotEffect, null, nint.Zero), zeroIndicatesError: true);
        }

        // Mix_Volume has no failure case (it always returns the previous
        // volume); wrapped for consistency with the rest of this class
        // routing every Mix_ call through SDLGuard.
        SDLGuard.Execute(() => Mix_Volume(OneShotPitchChannel, ToMixVolume(volume)));

        // Mix_SetPanning uses the inverted (0 = error) convention.
        SDLGuard.Execute(() => Mix_SetPanning(OneShotPitchChannel, ToLeftPan(pan), ToRightPan(pan)), zeroIndicatesError: true);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _disposedValue = true;

            if (disposing)
            {
                // dispose managed state (managed objects)
            }

            StopLoop();

            if (_oneShotEffect is not null)
            {
                SDLGuard.Execute(() => Mix_UnregisterEffect(OneShotPitchChannel, _oneShotEffect), zeroIndicatesError: true);
                SDLGuard.Execute(() => Mix_HaltChannel(OneShotPitchChannel));
                _oneShotEffect = null;
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            if (Mix_HaltMusic() < 0)
            {
                // Ignore
            }

            foreach (KeyValuePair<string, nint> music in _music)
            {
                Mix_FreeMusic(music.Value);
            }

            foreach (KeyValuePair<string, nint> sfx in _sfx)
            {
                Mix_FreeChunk(sfx.Value);
            }

            Mix_CloseAudio();
        }
    }

    private float[] GetLoopSamples(string sfxType)
    {
        if (!_loopSamples.TryGetValue(sfxType, out float[]? samples))
        {
            MIX_Chunk chunk = Marshal.PtrToStructure<MIX_Chunk>(_sfx[sfxType]);
            int sampleCount = (int)(chunk.alen / sizeof(float));
            samples = new float[sampleCount];
            Marshal.Copy(chunk.abuf, samples, 0, sampleCount);
            _loopSamples[sfxType] = samples;
        }

        return samples;
    }

    // Called by SDL_mixer once per audio buffer fill for the loop channel;
    // overwrites its entire output with a resample of _loopSampleData at
    // _loopPitch (1.0 = recorded rate), looping back to the start.
    private void LoopEffect(int channel, nint stream, int len, nint userData)
    {
        int floatCount = len / sizeof(float);
        if (_loopScratch.Length < floatCount)
        {
            _loopScratch = new float[floatCount];
        }

        int frames = _loopSampleData.Length / 2;
        if (frames < 2)
        {
            Array.Clear(_loopScratch, 0, floatCount);
            Marshal.Copy(_loopScratch, 0, stream, floatCount);
            return;
        }

        double pitch = _loopPitch;
        for (int i = 0; i < floatCount; i += 2)
        {
            int frame0 = (int)_loopPosition;
            int frame1 = frame0 + 1 >= frames ? 0 : frame0 + 1;
            float fraction = (float)(_loopPosition - frame0);

            _loopScratch[i] = _loopSampleData[frame0 * 2]
                + ((_loopSampleData[frame1 * 2] - _loopSampleData[frame0 * 2]) * fraction);
            _loopScratch[i + 1] = _loopSampleData[(frame0 * 2) + 1]
                + ((_loopSampleData[(frame1 * 2) + 1] - _loopSampleData[(frame0 * 2) + 1]) * fraction);

            _loopPosition += pitch;
            while (_loopPosition >= frames)
            {
                _loopPosition -= frames;
            }
        }

        Marshal.Copy(_loopScratch, 0, stream, floatCount);
    }

    // Called by SDL_mixer once per audio buffer fill for the one-shot pitch
    // channel; overwrites its entire output with a resample of
    // _oneShotSampleData at _oneShotPitch, stopping (and emitting silence)
    // once it has played through the sample once, unlike LoopEffect which
    // wraps back to the start.
    private void OneShotEffect(int channel, nint stream, int len, nint userData)
    {
        int floatCount = len / sizeof(float);
        if (_oneShotScratch.Length < floatCount)
        {
            _oneShotScratch = new float[floatCount];
        }

        int frames = _oneShotSampleData.Length / 2;
        if (_oneShotDone || frames < 2)
        {
            Array.Clear(_oneShotScratch, 0, floatCount);
            Marshal.Copy(_oneShotScratch, 0, stream, floatCount);
            return;
        }

        double pitch = _oneShotPitch;
        int i = 0;
        for (; i < floatCount; i += 2)
        {
            int frame0 = (int)_oneShotPosition;
            if (frame0 >= frames - 1)
            {
                _oneShotDone = true;
                break;
            }

            int frame1 = frame0 + 1;
            float fraction = (float)(_oneShotPosition - frame0);

            _oneShotScratch[i] = _oneShotSampleData[frame0 * 2]
                + ((_oneShotSampleData[frame1 * 2] - _oneShotSampleData[frame0 * 2]) * fraction);
            _oneShotScratch[i + 1] = _oneShotSampleData[(frame0 * 2) + 1]
                + ((_oneShotSampleData[(frame1 * 2) + 1] - _oneShotSampleData[(frame0 * 2) + 1]) * fraction);

            _oneShotPosition += pitch;
        }

        if (i < floatCount)
        {
            Array.Clear(_oneShotScratch, i, floatCount - i);
        }

        Marshal.Copy(_oneShotScratch, 0, stream, floatCount);
    }
}
