namespace Elite.Engine
{
    using System.Collections.Generic;
    using System.Reflection;
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
            Assembly assets = Assembly.LoadFrom("Elite.Assets.dll");

            Load(assets, Sfx.Launch, "launch.wav");
            Load(assets, Sfx.Crash, "crash.wav");
            Load(assets, Sfx.Dock, "dock.wav");
            Load(assets, Sfx.Gameover, "gameover.wav");
            Load(assets, Sfx.Pulse, "pulse.wav");
            Load(assets, Sfx.HitEnemy, "hitem.wav");
            Load(assets, Sfx.Explode, "explode.wav");
            Load(assets, Sfx.Ecm, "ecm.wav");
            Load(assets, Sfx.Missile, "missile.wav");
            Load(assets, Sfx.Hyperspace, "hyper.wav");
            Load(assets, Sfx.IncomingFire1, "incom1.wav");
            Load(assets, Sfx.IncomingFire2, "incom2.wav");
            Load(assets, Sfx.Beep, "beep.wav");
            Load(assets, Sfx.Boop, "boop.wav");

            Load(assets, Music.EliteTheme, "theme.mid");
            Load(assets, Music.BlueDanube, "danube.mid");
        }

        private void Load(Assembly assets, Sfx effect, string name)
        {
            _sound.Load(effect, assets.GetManifestResourceStream("Elite.Assets.sfx." + name));
        }

        private void Load(Assembly assets, Music music, string name)
        {
            _sound.Load(music, assets.GetManifestResourceStream("Elite.Assets.music." + name));
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