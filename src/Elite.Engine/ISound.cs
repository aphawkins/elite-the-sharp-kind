/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

namespace Elite.Engine
{
    using Elite.Common.Enums;

    public interface ISound
    {
        void Load(Music midiType, Stream midiStream);

        void Load(SoundEffect waveType, Stream waveStream);

        void PlayMidi(Music midi_no, bool repeat);

        void PlayWave(SoundEffect waveType);

        void StopMidi();
    }
}