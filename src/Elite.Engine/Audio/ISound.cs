// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine.Audio
{
    public interface ISound
    {
        void Load(Music midiType, Stream midiStream);

        void Load(SoundEffect waveType, Stream waveStream);

        void PlayMidi(Music midiNo, bool repeat);

        void PlayWave(SoundEffect waveType);

        void StopMidi();
    }
}
