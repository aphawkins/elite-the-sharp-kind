// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Graphics;
using Useful.Audio;
using Useful.Config;
using Useful.Controls;

namespace EliteSharpLib.Views;

// The "engine" half of the config file: the settings shared by every game in
// the collection, as opposed to Elite's own on the Game Settings screen.
internal sealed class EngineSettingsView : SettingsListView
{
    private readonly AudioController _audio;
    private readonly Space _space;

    internal EngineSettingsView(
        GameState gameState,
        IEliteDraw draw,
        IKeyboard keyboard,
        Space space,
        AudioController audio,
        IConfigWriter<EliteConfig> configWriter)
        : base(
            gameState,
            draw,
            keyboard,
            configWriter,
            "ENGINE SETTINGS",
            [
                new("Graphic Style:", ["Wireframe", "Solid"]),
                new("Depth Sort:", ["Painter", "ZBuffer"]),
                new("Music:", ["Off", "On"]),
                new("Effects:", ["Off", "On"]),

                // Both of these are read before the game is built - the
                // backend picks the abstraction, the tier the render
                // resolution and asset set - so they are saved now and taken
                // up on the next launch.
                new("Backend *:", ["Software", "Hardware"]),
                new("Tier *:", ["8-Bit", "16-Bit"]),
            ],
            "* Applies when the game is restarted")
    {
        _space = space;
        _audio = audio;
    }

    protected override int SettingValue(int index) => index switch
    {
        0 => (int)State.Config.Engine.Graphics.GraphicStyle,
        1 => (int)State.Config.Engine.Graphics.DepthSort,
        2 => State.Config.Engine.Sound.Music ? 1 : 0,
        3 => State.Config.Engine.Sound.Effects ? 1 : 0,
        4 => (int)State.Config.Engine.Backend,
        5 => (int)State.Config.Engine.Tier,
        _ => 0,
    };

    protected override void ToggleSetting(int index)
    {
        switch (index)
        {
            case 0:
                State.Config.Engine.Graphics.GraphicStyle = Next(State.Config.Engine.Graphics.GraphicStyle);

                // The planet and sun styles only apply in a solid world, so
                // both have to be rebuilt when that flips either way.
                _space.RefreshPlanetStyle();
                _space.RefreshSunStyle();
                break;

            case 1:
                State.Config.Engine.Graphics.DepthSort = Next(State.Config.Engine.Graphics.DepthSort);
                break;

            case 2:
                State.Config.Engine.Sound.Music = !State.Config.Engine.Sound.Music;
                _audio.MusicOn = State.Config.Engine.Sound.Music;

                // Silence whatever is already playing rather than leaving it
                // running until the next screen change.
                if (!_audio.MusicOn)
                {
                    _audio.StopMusic();
                }

                break;

            case 3:
                State.Config.Engine.Sound.Effects = !State.Config.Engine.Sound.Effects;
                _audio.EffectsOn = State.Config.Engine.Sound.Effects;
                break;

            case 4:
                State.Config.Engine.Backend = Next(State.Config.Engine.Backend);
                break;

            case 5:
                State.Config.Engine.Tier = Next(State.Config.Engine.Tier);
                break;
        }
    }
}
