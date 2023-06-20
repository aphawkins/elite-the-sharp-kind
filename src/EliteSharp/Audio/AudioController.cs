// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Assets;

namespace EliteSharp.Audio
{
    internal sealed class AudioController
    {
        private readonly bool _musicOn;
        private readonly bool _effectsOn;
        private readonly ISound _sound;

        private readonly Dictionary<SoundEffect, SfxSample> _sfx = new()
        {
            { SoundEffect.Launch, new(32) },
            { SoundEffect.Crash, new(7) },
            { SoundEffect.Dock, new(36) },
            { SoundEffect.Gameover, new(24) },
            { SoundEffect.Pulse, new(4) },
            { SoundEffect.HitEnemy, new(4) },
            { SoundEffect.Explode, new(23) },
            { SoundEffect.Ecm, new(23) },
            { SoundEffect.Missile, new(25) },
            { SoundEffect.Hyperspace, new(37) },
            { SoundEffect.IncomingFire1, new(4) },
            { SoundEffect.IncomingFire2, new(5) },
            { SoundEffect.Beep, new(2) },
            { SoundEffect.Boop, new(7) },
        };

        internal AudioController(ISound sound)
        {
            _sound = sound;
#if DEBUG
            _musicOn = false;
            _effectsOn = true;
#else
            _musicOn = true;
            _effectsOn = true;
#endif
        }

        internal async Task LoadSoundsAsync(CancellationToken token)
        {
            AssetFileLoader loader = new();
            ParallelOptions options = new()
            {
                CancellationToken = token,
            };

            await Parallel.ForEachAsync(
                Enum.GetValues<Music>(),
                options,
                async (music, token) => _sound.Load(music, await loader.LoadAsync(music, token).ConfigureAwait(false)))
                .ConfigureAwait(false);

            await Parallel.ForEachAsync(
                Enum.GetValues<SoundEffect>(),
                options,
                async (effect, token) => _sound.Load(effect, await loader.LoadAsync(effect, token).ConfigureAwait(false)))
                .ConfigureAwait(false);
        }

        internal void PlayEffect(SoundEffect effect)
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
            _sound.Play(effect);
        }

        internal void PlayMusic(Music music, bool loop)
        {
            if (!_musicOn)
            {
                return;
            }

            _sound.Play(music, loop);
        }

        internal void StopMusic()
        {
            if (!_musicOn)
            {
                return;
            }

            _sound.StopMusic();
        }

        internal void UpdateSound()
        {
            foreach (KeyValuePair<SoundEffect, SfxSample> sfx in _sfx)
            {
                sfx.Value.ReduceTimeRemaining();
            }
        }
    }
}
