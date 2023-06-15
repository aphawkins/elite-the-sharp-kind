// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Media;
using EliteSharp.Audio;

namespace EliteSharp.WinForms
{
    internal sealed class Sound : ISound
    {
        private readonly ConcurrentDictionary<SoundEffect, SoundPlayer> _sfxs = new();
        private readonly ConcurrentDictionary<Music, SoundPlayer> _musics = new();
        private bool _disposedValue;

        public void Load(Music musicType, byte[] waveBytes)
        {
            Debug.Assert(waveBytes.Length > 0, "Music bytes missing");
            _musics[musicType] = new(new MemoryStream(waveBytes));
            _musics[musicType].Load();
            Debug.Assert(_musics[musicType].IsLoadCompleted, "Sound Effect failed to load");
        }

        public void Load(SoundEffect sfxType, byte[] waveBytes)
        {
            Debug.Assert(waveBytes.Length > 0, "Sound effects bytes missing");
            _sfxs[sfxType] = new(new MemoryStream(waveBytes));
            _sfxs[sfxType].Load();
            Debug.Assert(_sfxs[sfxType].IsLoadCompleted, "Sound effect failed to load");
        }

        public void Play(SoundEffect sfxType) => _sfxs[sfxType].Play();

        public void Play(Music musicType, bool repeat)
        {
            StopMusic();

            if (repeat)
            {
                _musics[musicType].PlayLooping();
            }
            else
            {
                _musics[musicType].Play();
            }
        }

        public void StopMusic()
        {
            foreach (KeyValuePair<Music, SoundPlayer> music in _musics)
            {
                music.Value.Stop();
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    foreach (KeyValuePair<Music, SoundPlayer> v in _musics)
                    {
                        v.Value.Dispose();
                    }

                    foreach (KeyValuePair<SoundEffect, SoundPlayer> v in _sfxs)
                    {
                        v.Value.Dispose();
                    }
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Sound()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}
