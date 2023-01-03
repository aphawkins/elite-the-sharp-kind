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
    using System.Diagnostics;
    using Commons.Music.Midi;
    using Elite.Enums;

    public class Sound : ISound, IDisposable
    {
        //const int NUM_SAMPLES = 14;

        //extern DATAFILE* datafile;

        MidiPlayer? _midiPlayer;

        bool sound_on;
        private bool disposedValue;

        //struct sound_sample
        //{
        //	internal SAMPLE* sample;
        //	internal string filename;
        //	internal int runtime;
        //	internal int timeleft;

        //	internal sound_sample(SAMPLE sample, string filename, int runtime, int timeleft)
        //	{
        //		this.sample = sample;
        //		this.filename = filename;
        //		this.runtime = runtime;
        //		this.timeleft = timeleft;
        //	}
        //};

        //static sound_sample[] sample_list = new sound_sample[NUM_SAMPLES]
        //{
        //	new(null, "launch.wav",    32, 0),
        //	new(null, "crash.wav",      7, 0),
        //	new(null, "dock.wav",      36, 0),
        //	new(null, "gameover.wav",  24, 0),
        //	new(null, "pulse.wav",      4, 0),
        //	new(null, "hitem.wav",      4, 0),
        //	new(null, "explode.wav",   23, 0),
        //	new(null, "ecm.wav",       23, 0),
        //	new(null, "missile.wav",   25, 0),
        //	new(null, "hyper.wav",     37, 0),
        //	new(null, "incom1.wav",     4, 0),
        //	new(null, "incom2.wav",     5, 0),
        //	new(null, "beep.wav",       2, 0),
        //	new(null, "boop.wav",       7, 0),
        //};

        public void SoundStartup()
        {
            Debug.WriteLine(nameof(SoundStartup));

            //int i;

            // Install a sound driver..
            sound_on = true;

            //if (install_sound(DIGI_AUTODETECT, MIDI_AUTODETECT, ".") != 0)
            //{
            //	sound_on = 0;
            //	return;
            //}

            ///* Load the sound samples... */

            //for (i = 0; i < NUM_SAMPLES; i++)
            //{
            //	sample_list[i].sample = load_sample(sample_list[i].filename);
            //}
        }

        public void SoundShutdown()
        {
            Debug.WriteLine(nameof(SoundShutdown));

            if (!sound_on)
            {
                return;
            }

            //for (int i = 0; i < NUM_SAMPLES; i++)
            //{
            //	if (sample_list[i].sample != NULL)
            //	{
            //		destroy_sample (sample_list[i].sample);
            //		sample_list[i].sample = NULL;
            //	}
            //}
        }

        public void PlaySample(SND sample_no)
        {
            Debug.WriteLine(nameof(PlaySample));

            if (!sound_on)
            {
                return;
            }

            //if (sample_list[(int)sample_no].timeleft != 0)
            //{
            //	return;
            //}

            //sample_list[(int)sample_no].timeleft = sample_list[(int)sample_no].runtime;

            //play_sample(sample_list[(int)sample_no].sample, 255, 128, 1000, false);
        }

        public void UpdateSound()
        {
            Debug.WriteLine(nameof(UpdateSound));

            //int i;

            //for (i = 0; i < NUM_SAMPLES; i++)
            //{
            //	if (sample_list[i].timeleft > 0)
            //	{
            //		sample_list[i].timeleft--;
            //	}
            //}
        }

        public void PlayMidi(SND midi_no, bool repeat)
        {
            //Debug.WriteLine(nameof(MidiPlay));

            if (!sound_on)
            {
                return;
            }

            string file;

            switch (midi_no)
            {
                case SND.SND_ELITE_THEME:
                    file = "theme.mid";
                    break;

                case SND.SND_BLUE_DANUBE:
                    file = "danube.mid";
                    break;

                default:
                    StopMidi();
                    return;
            }

            //TODO: Get repeat/loop working
            file = Path.Combine("music", file);
            IMidiAccess access = MidiAccessManager.Default;
            IMidiOutput output = access.OpenOutputAsync(access.Outputs.Last().Id).Result;
            MidiMusic music = MidiMusic.Read(File.OpenRead(file));
            _midiPlayer = new(music, output);
            if (repeat)
            {
                _midiPlayer.Finished += _midiPlayer_Finished;
                _midiPlayer.Play();
            }
            else
            {
                _midiPlayer.Play();
            }
        }

        private void _midiPlayer_Finished()
        {
            _midiPlayer.Seek(0);
            _midiPlayer.Play();
        }

        public void StopMidi()
        {
            //Debug.WriteLine(nameof(snd_stop_midi));

            if (!sound_on)
            {
                return;
            }

            _midiPlayer?.Stop();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _midiPlayer?.Stop();
                    _midiPlayer?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Sound()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}