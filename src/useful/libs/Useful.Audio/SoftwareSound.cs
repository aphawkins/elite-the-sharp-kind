// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Useful.Assets;

namespace Useful.Audio;

public sealed class SoftwareSound : ISound, IDisposable
{
    private readonly MixingSampleProvider _mixer;
    private readonly IAssetLocator _assetLocator;
    private readonly WaveOutEvent _outputDevice;
    private Dictionary<int, SoundSampleProvider> _sfx = [];
    private Dictionary<int, SoundSampleProvider> _music = [];
    private bool _isDisposed;

    public SoftwareSound(IAssetLocator assetLocator)
    {
        Guard.ArgumentNull(assetLocator);

        _assetLocator = assetLocator;

        _outputDevice = new();
        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
        {
            ReadFully = true,
        };
        _mixer.MixerInputEnded += (_, args) => MixerInputEnded(args);
        _outputDevice.Init(_mixer);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Load()
    {
        _music = _assetLocator.MusicAssets.ToDictionary(
            x => x.Key,
            x => SoundSampleProvider.Create(x.Value));

        _sfx = _assetLocator.SfxAssets.ToDictionary(
            x => x.Key,
            x => SoundSampleProvider.Create(x.Value));
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

    private void MixerInputEnded(SampleProviderEventArgs e)
    {
        SoundSampleProvider provider = (SoundSampleProvider)e.SampleProvider;
        if (provider.Repeat)
        {
            AddMixerInput(provider);
        }
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
}
