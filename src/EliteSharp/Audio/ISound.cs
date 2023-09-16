﻿// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Audio
{
    public interface ISound : IDisposable
    {
        void Load(MusicType musicType, string filePath);

        void Load(SoundEffect sfxType, string filePath);

        void Play(MusicType musicType, bool repeat);

        void Play(SoundEffect sfxType);

        void StopMusic();
    }
}
