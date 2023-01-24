namespace Elite.Assets
{
    using System.Reflection;
    using Elite.Common.Enums;
    using Elite.Common.Interfaces;

    public class AssetLoader : IAssets
    {
        Assembly? assets = Assembly.GetAssembly(typeof(AssetLoader));

        public Stream? Load(IMG image)
        {
            return assets?.GetManifestResourceStream("Elite.Assets.Images." + GetName(image));
        }

        public Stream? Load(Sfx effect)
        {
            return assets?.GetManifestResourceStream("Elite.Assets.Effects." + GetName(effect));
        }

        public Stream? Load(Music music)
        {
            return assets?.GetManifestResourceStream("Elite.Assets.Music." + GetName(music));
        }

        private string GetName(IMG image)
        {
            return image switch
            {
                IMG.IMG_GREEN_DOT => "greendot.bmp",
                IMG.IMG_RED_DOT => "reddot.bmp",
                IMG.IMG_BIG_S => "safe.bmp",
                IMG.IMG_ELITE_TXT => "elitetx3.bmp",
                IMG.IMG_BIG_E => "ecm.bmp",
                IMG.IMG_MISSILE_GREEN => "missgrn.bmp",
                IMG.IMG_MISSILE_YELLOW => "missyell.bmp",
                IMG.IMG_MISSILE_RED => "missred.bmp",
                IMG.IMG_BLAKE => "blake.bmp",
                IMG.IMG_SCANNER => "scanner.bmp",
                _ => throw new NotImplementedException(),
            };
        }

        private string GetName(Sfx effect)
        {
            return effect switch
            {
                Sfx.Launch => "launch.wav",
                Sfx.Crash => "crash.wav",
                Sfx.Dock => "dock.wav",
                Sfx.Gameover => "gameover.wav",
                Sfx.Pulse => "pulse.wav",
                Sfx.HitEnemy => "hitem.wav",
                Sfx.Explode => "explode.wav",
                Sfx.Ecm => "ecm.wav",
                Sfx.Missile => "missile.wav",
                Sfx.Hyperspace => "hyper.wav",
                Sfx.IncomingFire1 => "incom1.wav",
                Sfx.IncomingFire2 => "incom2.wav",
                Sfx.Beep => "beep.wav",
                Sfx.Boop => "boop.wav",
                _ => throw new NotImplementedException(),
            };
        }

        private string GetName(Music music)
        {
            return music switch
            {
                Music.EliteTheme => "theme.mid",
                Music.BlueDanube => "danube.mid",
                _ => throw new NotImplementedException(),
            };
        }
    }
}