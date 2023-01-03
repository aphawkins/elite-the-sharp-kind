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

namespace Elite
{
    using Elite.Enums;

    public interface ISound
    {
        void PlayMidi(SND midi_no, bool repeat);

        void PlaySample(SND sample_no);

        void SoundShutdown();

        void SoundStartup();

        void StopMidi();

        void UpdateSound();
    }
}