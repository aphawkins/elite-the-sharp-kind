// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharpLib.Config;
using EliteSharpLib.Renditions;
using Useful.Abstraction;
using Useful.Audio;
using Useful.Config;
using Useful.Controls;
using Useful.Graphics.Rendering;
using Useful.Widgets;

namespace EliteSharpLib.Views;

// The "engine" half of the config file: the settings that are not about Elite
// in particular. The game's own are on their own screen - see
// SettingsController.
internal sealed class EngineSettingsController : SettingsListController
{
    internal EngineSettingsController(
        GameState gameState,
        IKeyboard keyboard,
        Space space,
        AudioController audio,
        IConfigWriter<EliteConfig> configWriter,
        InstalledRenditions renditions,
        IBaseView baseView,
        IViewSurface surface,
        SettingsListStyle style)
        : base(
            gameState,
            keyboard,
            baseView,
            surface,
            style,
            "ENGINE SETTINGS",
            BuildSettings(gameState, space, audio, configWriter, renditions),
            "* Applies when the game is restarted")
    {
    }

    private static IReadOnlyList<ISetting> BuildSettings(
        GameState gameState,
        Space space,
        AudioController audio,
        IConfigWriter<EliteConfig> configWriter,
        InstalledRenditions renditions)
    {
        ArgumentNullException.ThrowIfNull(gameState);
        ArgumentNullException.ThrowIfNull(space);
        ArgumentNullException.ThrowIfNull(audio);
        ArgumentNullException.ThrowIfNull(configWriter);
        ArgumentNullException.ThrowIfNull(renditions);

        EliteConfig config = gameState.Config;
        void Save() => configWriter.WriteConfig(config);

        return
        [
            new SavedSetting(
                new EnumSetting<GraphicStyle>(
                    "Graphic Style:",
                    [(GraphicStyle.Wireframe, "Wireframe"), (GraphicStyle.Solid, "Solid")],
                    () => config.Engine.Graphics.GraphicStyle,
                    value =>
                    {
                        config.Engine.Graphics.GraphicStyle = value;

                        // The planet and sun styles only apply in a solid
                        // world, so both have to be rebuilt when that flips
                        // either way.
                        space.RefreshPlanetStyle();
                        space.RefreshSunStyle();
                    }),
                Save),
            new SavedSetting(
                new EnumSetting<DepthSort>(
                    "Depth Sort:",
                    [(DepthSort.Painter, "Painter"), (DepthSort.ZBuffer, "ZBuffer")],
                    () => config.Engine.Graphics.DepthSort,
                    value => config.Engine.Graphics.DepthSort = value),
                Save),
            new SavedSetting(
                new ToggleSetting(
                    "Music:",
                    "Off",
                    "On",
                    () => config.Engine.Sound.Music,
                    value =>
                    {
                        config.Engine.Sound.Music = value;
                        audio.MusicOn = value;

                        // Silence whatever is already playing rather than
                        // leaving it running until the next screen change.
                        if (!value)
                        {
                            audio.StopMusic();
                        }
                    }),
                Save),
            new SavedSetting(
                new ToggleSetting(
                    "Effects:",
                    "Off",
                    "On",
                    () => config.Engine.Sound.Effects,
                    value =>
                    {
                        config.Engine.Sound.Effects = value;
                        audio.EffectsOn = value;
                    }),
                Save),

            // Both of these are read before the game is built - the backend
            // picks the abstraction, the rendition the render resolution and
            // asset set - so they are saved now and taken up on the next
            // launch.
            new SavedSetting(
                new EnumSetting<Backend>(
                    "Backend *:",
                    [(Backend.Software, "Software"), (Backend.Hardware, "Hardware")],
                    () => config.Engine.Backend,
                    value => config.Engine.Backend = value),
                Save),

            // The renditions offered are the ones installed, so a commander
            // cannot select one that is not there. They are shown by the name
            // each calls itself: the game cannot prettify a name it has never
            // seen. With one installed the row simply stays put.
            new SavedSetting(
                new ChoiceSetting(
                    "Rendition *:",
                    renditions.Names,
                    () => config.Engine.Rendition,
                    value => config.Engine.Rendition = value),
                Save),
        ];
    }
}
