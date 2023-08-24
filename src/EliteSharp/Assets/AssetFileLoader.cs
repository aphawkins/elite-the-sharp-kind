// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Reflection;
using EliteSharp.Audio;
using EliteSharp.Graphics;
using NAudio.Vorbis;
using NAudio.Wave;

namespace EliteSharp.Assets
{
    internal sealed class AssetFileLoader : IAssets
    {
        public string AssetPath(ImageType image) => Path.Combine(GetAssetPath(), "Images", GetName(image));

        public async Task<byte[]> LoadAsync(SoundEffect effect, CancellationToken token)
        {
            using MemoryStream memStream = new();
            using FileStream stream = new(Path.Combine(GetAssetPath(), "SFX", GetName(effect)), FileMode.Open);
            await stream.CopyToAsync(memStream, token).ConfigureAwait(false);
            memStream.Position = 0;
            return memStream.ToArray();
        }

        public async Task<byte[]> LoadAsync(Music music, CancellationToken token) => await Task.Run(
            () =>
            {
                using MemoryStream memStream = new();
                using VorbisWaveReader vorbisStream = new(Path.Combine(GetAssetPath(), "Music", GetName(music)));
                WaveFileWriter.WriteWavFileToStream(memStream, vorbisStream);
                memStream.Position = 0;
                return memStream.ToArray();
            },
            token).ConfigureAwait(false);

        private static string GetAssetPath() => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty, "Assets");

        private static string GetName(ImageType image) => image switch
        {
            ImageType.GreenDot => "greendot.bmp",
            ImageType.DotRed => "reddot.bmp",
            ImageType.BigS => "safe.bmp",
            ImageType.EliteText => "elitetext.bmp",
            ImageType.BigE => "ecm.bmp",
            ImageType.MissileGreen => "missgrn.bmp",
            ImageType.MissileYellow => "missyell.bmp",
            ImageType.MissileRed => "missred.bmp",
            ImageType.Blake => "blake.bmp",
            ImageType.Scanner => "scanner.bmp",
            _ => throw new NotImplementedException(),
        };

        private static string GetName(SoundEffect effect) => effect switch
        {
            SoundEffect.Launch => "launch.wav",
            SoundEffect.Crash => "crash.wav",
            SoundEffect.Dock => "dock.wav",
            SoundEffect.Gameover => "gameover.wav",
            SoundEffect.Pulse => "pulse.wav",
            SoundEffect.HitEnemy => "hitem.wav",
            SoundEffect.Explode => "explode.wav",
            SoundEffect.Ecm => "ecm.wav",
            SoundEffect.Missile => "missile.wav",
            SoundEffect.Hyperspace => "hyper.wav",
            SoundEffect.IncomingFire1 => "incom1.wav",
            SoundEffect.IncomingFire2 => "incom2.wav",
            SoundEffect.Beep => "beep.wav",
            SoundEffect.Boop => "boop.wav",
            _ => throw new NotImplementedException(),
        };

        private static string GetName(Music music) => music switch
        {
            Music.EliteTheme => "theme.ogg",
            Music.BlueDanube => "danube.ogg",
            _ => throw new NotImplementedException(),
        };
    }
}
