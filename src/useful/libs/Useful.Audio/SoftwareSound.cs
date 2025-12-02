// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Useful.Assets;

namespace Useful.Audio;

public sealed class SoftwareSound : ISound, IDisposable
{
    private readonly MixingSampleProvider _mixer;
    private readonly WaveOutEvent _outputDevice;
    private bool _isDisposed;
    private Dictionary<int, SoundSampleProvider> _music = [];
    private Dictionary<int, SoundSampleProvider> _sfx = [];

    private SoftwareSound()
    {
        _outputDevice = new();
        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
        {
            ReadFully = true,
        };
        _mixer.MixerInputEnded += (_, args) => MixerInputEnded(args);
        _outputDevice.Init(_mixer);
    }

    public static SoftwareSound Create(IAssetLocator assetLocator)
    {
        Guard.ArgumentNull(assetLocator);

        return new()
        {
            _music = assetLocator.MusicPaths.ToDictionary(
                x => x.Key,
                x => SoundSampleProvider.Create(x.Value)),

            _sfx = assetLocator.SfxPaths.ToDictionary(
                x => x.Key,
                x => SoundSampleProvider.Create(x.Value)),
        };
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Play(int musicType, bool repeat)
    {
        Debug.Assert(_music.ContainsKey(musicType), "Music has not been loaded");

        StopMusic();
        SoundSampleProvider sampleProvider = _music[musicType];
        AddMixerInput(sampleProvider, repeat);
        _outputDevice.Play();
    }

    public void Play(int sfxType)
    {
        Debug.Assert(_sfx.ContainsKey(sfxType), "Sound effect has not been loaded");

        SoundSampleProvider sampleProvider = _sfx[sfxType];
        AddMixerInput(sampleProvider);
        _outputDevice.Play();
    }

    public void StopMusic()
    {
        _mixer.RemoveAllMixerInputs();
        _outputDevice.Stop();
    }

    private void AddMixerInput(SoundSampleProvider input, bool repeat = false)
    {
        input.Repeat = repeat;
        input.Reset();
        _mixer.AddMixerInput(input);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                _outputDevice?.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _isDisposed = true;
        }
    }

    private void MixerInputEnded(SampleProviderEventArgs e)
    {
        SoundSampleProvider provider = (SoundSampleProvider)e.SampleProvider;
        if (provider.Repeat)
        {
            AddMixerInput(provider);
        }
    }
}
