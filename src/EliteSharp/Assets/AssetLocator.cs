// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Reflection;
using EliteSharp.Assets.Fonts;
using EliteSharp.Audio;
using EliteSharp.Graphics;

namespace EliteSharp.Assets;

public class AssetLocator : IAssetLocator
{
    public IDictionary<ImageType, string> ImageAssets()
        => Enum.GetValues<ImageType>().ToDictionary(x => x, image => Path.Combine(GetAssetPath(), "Images", GetName(image)));

    public IDictionary<SoundEffect, string> SfxAssets()
        => Enum.GetValues<SoundEffect>().ToDictionary(x => x, effect => Path.Combine(GetAssetPath(), "SFX", GetName(effect)));

    public IDictionary<MusicType, string> MusicAssets()
        => Enum.GetValues<MusicType>().ToDictionary(x => x, music => Path.Combine(GetAssetPath(), "Music", GetName(music)));

    public IDictionary<FontType, string> FontAssets()
        => Enum.GetValues<FontType>().ToDictionary(x => x, font => Path.Combine(GetAssetPath(), "Fonts", GetName(font)));

    protected virtual string GetAssetPath()
        => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty, "Assets");

    protected virtual string GetName(ImageType image) => image switch
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
        _ => throw new EliteException(),
    };

    protected virtual string GetName(SoundEffect effect) => effect switch
    {
        SoundEffect.Launch => "launch.ogg",
        SoundEffect.Crash => "crash.ogg",
        SoundEffect.Dock => "dock.ogg",
        SoundEffect.Gameover => "gameover.ogg",
        SoundEffect.Pulse => "pulse.ogg",
        SoundEffect.HitEnemy => "hitem.ogg",
        SoundEffect.Explode => "explode.ogg",
        SoundEffect.Ecm => "ecm.ogg",
        SoundEffect.Missile => "missile.ogg",
        SoundEffect.Hyperspace => "hyper.ogg",
        SoundEffect.IncomingFire1 => "incom1.ogg",
        SoundEffect.IncomingFire2 => "incom2.ogg",
        SoundEffect.Beep => "beep.ogg",
        SoundEffect.Boop => "boop.ogg",
        _ => throw new EliteException(),
    };

    protected virtual string GetName(MusicType music) => music switch
    {
        MusicType.EliteTheme => "theme.mid",
        MusicType.BlueDanube => "danube.mid",
        _ => throw new EliteException(),
    };

    protected virtual string GetName(FontType font) => font switch
    {
        FontType.Small => "OpenSans-Regular.ttf",
        FontType.Large => "OpenSans-Regular.ttf",
        _ => throw new EliteException(),
    };
}
