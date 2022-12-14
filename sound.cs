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

/*
 * sound.c
 */

#include <stdlib.h>
#include <allegro.h>
#include "sound.h"
#include "alg_data.h" 

namespace Elite
{
	using Elite.Enums;

	internal static class sound
    {
		const int NUM_SAMPLES = 14;

		extern DATAFILE *datafile;

		static bool sound_on;

		struct sound_sample
		{
 			internal SAMPLE *sample;
            internal string filename;
            internal int runtime;
            internal int timeleft;

			internal sound_sample(SAMPLE sample, string filename, int runtime, int timeleft)
			{
				this.sample = sample;
				this.filename = filename;
				this.runtime = runtime;
				this.timeleft = timeleft;
			}
		};

		static sound_sample[] sample_list = new sound_sample[NUM_SAMPLES]
		{
			new(null, "launch.wav",    32, 0),
			new(null, "crash.wav",      7, 0),
			new(null, "dock.wav",      36, 0),
			new(null, "gameover.wav",  24, 0),
			new(null, "pulse.wav",      4, 0),
			new(null, "hitem.wav",      4, 0),
			new(null, "explode.wav",   23, 0),
			new(null, "ecm.wav",       23, 0),
			new(null, "missile.wav",   25, 0),
			new(null, "hyper.wav",     37, 0),
			new(null, "incom1.wav",     4, 0),
			new(null, "incom2.wav",     5, 0),
			new(null, "beep.wav",       2, 0),
			new(null, "boop.wav",       7, 0),
		};
 
		static void snd_sound_startup()
		{
			int i;

 			/* Install a sound driver.. */
			sound_on = true;
	
			if (install_sound(DIGI_AUTODETECT, MIDI_AUTODETECT, ".") != 0)
			{
				sound_on = 0;
				return;
			}

			/* Load the sound samples... */

			for (i = 0; i < NUM_SAMPLES; i++)
			{
				sample_list[i].sample = load_sample(sample_list[i].filename);
			}
		}

		static void snd_sound_shutdown ()
		{
			int i;

			if (!sound_on)
			{
				return;
			}

			for (i = 0; i < NUM_SAMPLES; i++)
			{
				if (sample_list[i].sample != NULL)
				{
					destroy_sample (sample_list[i].sample);
					sample_list[i].sample = NULL;
				}
			}
		}

		internal static void snd_play_sample(SND sample_no)
		{
			if (!sound_on)
			{
				return;
			}

			if (sample_list[(int)sample_no].timeleft != 0)
			{
				return;
			}

			sample_list[(int)sample_no].timeleft = sample_list[(int)sample_no].runtime;

			play_sample(sample_list[(int)sample_no].sample, 255, 128, 1000, false);
		}

		static void snd_update_sound()
		{
			int i;
	
			for (i = 0; i < NUM_SAMPLES; i++)
			{
				if (sample_list[i].timeleft > 0)
					sample_list[i].timeleft--;
			}
		}

		static void snd_play_midi(SND midi_no, int repeat)
		{
			if (!sound_on)
			{
				return;
			}

			switch (midi_no)
			{
				case SND.SND_ELITE_THEME:
					play_midi(datafile[THEME].dat, repeat);
					break;
		
				case SND.SND_BLUE_DANUBE:
					play_midi(datafile[DANUBE].dat, repeat);
					break;
			}
		}

		static void snd_stop_midi()
		{
			if (sound_on)
			{
				play_midi(null, true);
			}
		}
	}
}