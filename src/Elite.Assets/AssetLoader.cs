// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Reflection;
using Elite.Common.Enums;
using Elite.Common.Interfaces;

namespace Elite.Assets
{
    public class AssetLoader : IAssets
    {
        private readonly Assembly? _assets = Assembly.GetAssembly(typeof(AssetLoader));

        public Stream? Load(Image image) => _assets?.GetManifestResourceStream("Elite.Assets.Images." + GetName(image));

        public Stream? Load(SoundEffect effect) => _assets?.GetManifestResourceStream("Elite.Assets.SoundEffects." + GetName(effect));

        public Stream? Load(Music music) => _assets?.GetManifestResourceStream("Elite.Assets.Music." + GetName(music));

        private static string GetName(Image image) => image switch
        {
            Image.GreenDot => "greendot.bmp",
            Image.DotRed => "reddot.bmp",
            Image.BigS => "safe.bmp",
            Image.EliteText => "elitetext.bmp",
            Image.BigE => "ecm.bmp",
            Image.MissileGreen => "missgrn.bmp",
            Image.MissileYellow => "missyell.bmp",
            Image.MissileRed => "missred.bmp",
            Image.Blake => "blake.bmp",
            Image.Scanner => "scanner.bmp",
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
            Music.EliteTheme => "theme.mid",
            Music.BlueDanube => "danube.mid",
            _ => throw new NotImplementedException(),
        };
    }
}
