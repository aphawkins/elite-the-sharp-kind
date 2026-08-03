// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharpLib.Config;
using EliteSharpLib.Renditions;
using Useful.Audio;
using Useful.Config;
using Useful.Controls;

namespace EliteSharpLib.Views;

// The "engine" half of the config file: the settings shared by every game in
// the collection, as opposed to Elite's own on the Game Settings screen.
internal sealed class EngineSettingsController : SettingsListController
{
    private readonly AudioController _audio;
    private readonly Space _space;
    private readonly IReadOnlyList<string> _renditions;

    internal EngineSettingsController(
        GameState gameState,
        IKeyboard keyboard,
        Space space,
        AudioController audio,
        IConfigWriter<EliteConfig> configWriter,
        InstalledRenditions renditions,
        IView<SettingsListModel> view)
        : base(
            gameState,
            keyboard,
            configWriter,
            "ENGINE SETTINGS",
            [
                new("Graphic Style:", ["Wireframe", "Solid"]),
                new("Depth Sort:", ["Painter", "ZBuffer"]),
                new("Music:", ["Off", "On"]),
                new("Effects:", ["Off", "On"]),

                // Both of these are read before the game is built - the
                // backend picks the abstraction, the rendition the render
                // resolution and asset set - so they are saved now and taken
                // up on the next launch.
                //
                // The renditions offered are the ones installed, so a
                // commander cannot select one that is not there. They are
                // shown by the name each calls itself: the game cannot
                // prettify a name it has never seen.
                new("Backend *:", ["Software", "Hardware"]),
                new("Rendition *:", [.. Names(renditions)]),
            ],
            view,
            "* Applies when the game is restarted")
    {
        _space = space;
        _audio = audio;
        _renditions = Names(renditions);
    }

    protected override int SettingValue(int index) => index switch
    {
        0 => (int)State.Config.Engine.Graphics.GraphicStyle,
        1 => (int)State.Config.Engine.Graphics.DepthSort,
        2 => State.Config.Engine.Sound.Music ? 1 : 0,
        3 => State.Config.Engine.Sound.Effects ? 1 : 0,
        4 => (int)State.Config.Engine.Backend,
        5 => RenditionIndex(),
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
                // Wraps, so with one installed the row simply stays put.
                State.Config.Engine.Rendition = _renditions[(RenditionIndex() + 1) % _renditions.Count];
                break;
        }
    }

    private static IReadOnlyList<string> Names(InstalledRenditions renditions)
    {
        ArgumentNullException.ThrowIfNull(renditions);

        return renditions.Names;
    }

    // The configured rendition is always one of these - the game would not
    // have started otherwise - but the answer indexes a row's values, so it
    // falls back to the first rather than risking a crash on this screen.
    private int RenditionIndex()
    {
        for (int i = 0; i < _renditions.Count; i++)
        {
            if (string.Equals(_renditions[i], State.Config.Engine.Rendition, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return 0;
    }
}
