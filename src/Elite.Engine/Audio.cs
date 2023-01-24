namespace Elite.Engine
{
    using System.Collections.Generic;
    using System.Reflection;
    using Elite.Assets;
    using Elite.Common.Enums;
    using Elite.Enums;

    internal class Audio
    {
        private readonly bool _musicOn;
        private readonly bool _effectsOn;
        private ISound _sound;

        internal Audio(ISound sound)
        {
            _sound = sound;
#if DEBUG
            _musicOn = true;
            _effectsOn = true;
#else
            _musicOn = true;
            _effectsOn = true;
#endif
        }

        private readonly Dictionary<Sfx, SfxSample> _sfx = new()
        {
            { Sfx.Launch, new(32) },
            { Sfx.Crash, new(7) },
            { Sfx.Dock, new(36) },
            { Sfx.Gameover, new(24) },
            { Sfx.Pulse, new(4) },
            { Sfx.HitEnemy, new(4) },
            { Sfx.Explode, new(23) },
            { Sfx.Ecm, new(23) },
            { Sfx.Missile, new(25) },
            { Sfx.Hyperspace, new(37) },
            { Sfx.IncomingFire1, new(4) },
            { Sfx.IncomingFire2, new(5) },
            { Sfx.Beep, new(2) },
            { Sfx.Boop, new(7) },
        };

        internal void LoadSounds()
        {
            AssetLoader loader = new();

            foreach (Music music in Enum.GetValues<Music>())
            {
                Stream? stream = loader.Load(music);
                if (stream != null)
                {
                    _sound.Load(music, stream);
                }
            }

            foreach (Sfx effect in Enum.GetValues<Sfx>())
            {
                Stream? stream = loader.Load(effect);
                if (stream != null)
                {
                    _sound.Load(effect, stream);
                }
            }
        }



        internal void PlayEffect(Sfx effect)
        {
            if (!_effectsOn)
            {
                return;
            }

            if (_sfx[effect].HasTimeRemaining)
            {
                return;
            }

            _sfx[effect].ResetTime();
            _sound.PlayWave(effect);
        }

        internal void PlayMusic(Music music, bool loop)
        {
            if (!_musicOn)
            {
                return;
            }

            _sound.PlayMidi(music, loop);
        }

        internal void StopMusic()
        {
            if (!_musicOn)
            {
                return;
            }

            _sound.StopMidi();
        }

        internal void UpdateSound()
        {
            foreach (KeyValuePair<Sfx, SfxSample> sfx in _sfx)
            {
                sfx.Value.ReduceTimeRemaining();
            }
        }
    }
}