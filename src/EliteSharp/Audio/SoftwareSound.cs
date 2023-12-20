// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Assets;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace EliteSharp.Audio
{
    public sealed class SoftwareSound : ISound
    {
        private readonly MixingSampleProvider _mixer;

        private readonly Dictionary<SoundEffect, SoundSampleProvider> _sfx;
        private readonly Dictionary<MusicType, SoundSampleProvider> _music;
        private readonly WaveOutEvent _outputDevice;
        private bool _isDisposed;

        public SoftwareSound(SoftwareAssetLoader assetLoader)
        {
            Guard.ArgumentNull(assetLoader);

            _music = assetLoader.LoadMusic();
            _sfx = assetLoader.LoadSfx();

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

        public void Play(MusicType musicType, bool repeat)
        {
            StopMusic();
            SoundSampleProvider sampleProvider = _music[musicType];
            AddMixerInput(sampleProvider, repeat);
            _outputDevice.Play();
        }

        public void Play(SoundEffect sfxType)
        {
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
}
