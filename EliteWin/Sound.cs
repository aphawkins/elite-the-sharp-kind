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
    using Commons.Music.Midi;
    using Elite.Enums;

    public partial class Sound : ISound, IDisposable
    {
        private MidiPlayer? _midiPlayer;
        private readonly IMidiAccess? _access;
        private readonly IMidiOutput? _output;
        private readonly bool _midiOn;
        private readonly bool _sfxOn;
        private bool _disposedValue;

        private readonly Dictionary<Sfx, SfxSample> _sfx = new()
        {
            { Sfx.Launch, new("launch.wav", 32) },
            { Sfx.Crash, new("crash.wav", 7) },
            { Sfx.Dock, new("dock.wav", 36) },
            { Sfx.Gameover, new("gameover.wav", 24) },
            { Sfx.Pulse, new("pulse.wav", 4) },
            { Sfx.HitEnemy, new("hitem.wav", 4) },
            { Sfx.Explode, new("explode.wav", 23) },
            { Sfx.Ecm, new("ecm.wav", 23) },
            { Sfx.Missile, new("missile.wav", 25) },
            { Sfx.Hyperspace, new("hyper.wav", 37) },
            { Sfx.IncomingFire1, new("incom1.wav", 4) },
            { Sfx.IncomingFire2, new("incom2.wav", 5) },
            { Sfx.Beep, new("beep.wav", 2) },
            { Sfx.Boop, new("boop.wav", 7) },
        };

        private readonly Dictionary<Music, string> _music = new()
        {
            { Music.EliteTheme, Path.Combine("music", "theme.mid") },
            { Music.BlueDanube, Path.Combine("music", "danube.mid") },
        };

        public Sound()
        {
#if DEBUG
            _midiOn = false;
            _sfxOn = true;
#else
            _midiOn = true;
            _sfxOn = true;
#endif

            _access = MidiAccessManager.Default;
            _output = _access.OpenOutputAsync(_access.Outputs.Last().Id).Result;
        }

        public void PlaySample(Sfx sample_no)
        {
            if (!_sfxOn)
            {
                return;
            }

            if (_sfx[sample_no].HasTimeRemaining)
            {
                return;
            }

            _sfx[sample_no].ResetTime();
            _sfx[sample_no].Play();
        }

        public void UpdateSound()
        {
            foreach (KeyValuePair<Sfx, SfxSample> sfx in _sfx)
            {
                if (sfx.Value.HasTimeRemaining)
                {
                    sfx.Value.ReduceTimeRemaining();
                }
            }
        }

        public void PlayMidi(Music midi_no, bool repeat)
        {
            if (!_midiOn)
            {
                return;
            }

            StopMidi();

            //TODO: Get repeat/loop working
            MidiMusic music = MidiMusic.Read(File.OpenRead(_music[midi_no]));
            _midiPlayer = new(music, _output);
            if (repeat)
            {
                //_midiPlayer.Finished += _midiPlayer_Finished;
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
            if (!_midiOn)
            {
                return;
            }

            if (_midiPlayer != null)
            {
                _midiPlayer.Stop();
                _midiPlayer.Dispose();
                _midiPlayer = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _midiPlayer?.Stop();
                    _midiPlayer?.Dispose();

                    foreach (KeyValuePair<Sfx, SfxSample> v in _sfx)
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}