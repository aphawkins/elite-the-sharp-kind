// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Concurrent;
using EliteSharp.Audio;

namespace EliteSharp.Graphics
{
    internal class SoftwareSound : ISound
    {
        private readonly ConcurrentDictionary<SoundEffect, EWave> _sfxs = new();
        private readonly ConcurrentDictionary<MusicType, EWave> _musics = new();

        public void Dispose() => throw new NotImplementedException();

        public void Load(MusicType musicType, string filePath)
        {
            using MemoryStream memStream = new();
            using FileStream stream = new(filePath, FileMode.Open);
            stream.CopyToAsync(memStream).ConfigureAwait(false);
            memStream.Position = 0;
            _musics[musicType] = new(memStream.ToArray());
        }

        public void Load(SoundEffect sfxType, string filePath)
        {
            using MemoryStream memStream = new();
            using FileStream stream = new(filePath, FileMode.Open);
            stream.CopyToAsync(memStream).ConfigureAwait(false);
            memStream.Position = 0;
            _sfxs[sfxType] = new(memStream.ToArray());
        }

        public void Play(MusicType musicType, bool repeat) => throw new NotImplementedException();

        public void Play(SoundEffect sfxType) => throw new NotImplementedException();

        public void StopMusic() => throw new NotImplementedException();
    }
}
