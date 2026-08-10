// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Globalization;
using EliteSharp.Abstractions.Views;
using EliteSharpLib.Config;
using EliteSharpLib.Renditions;
using SharpKind.Abstraction;
using SharpKind.Audio;
using SharpKind.Config;
using SharpKind.Graphics.Rendering;
using SharpKind.Input;
using SharpKind.UI;

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
                new EnumSetting<FillMode>(
                    "Fill Mode:",
                    [(FillMode.Wireframe, "Wireframe"), (FillMode.Solid, "Solid")],
                    () => config.Engine.Graphics.FillMode,
                    value =>
                    {
                        config.Engine.Graphics.FillMode = value;

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

            // Both shown whether or not the rendition in use shades - the same
            // way Depth Sort is shown in a wireframe world. A setting that
            // reads back what it was set to is honest; hiding the row would
            // leave the commander wondering where it went.
            new SavedSetting(
                new EnumSetting<ShadingModelKind>(
                    "Shading:",
                    [
                        (ShadingModelKind.Unlit, "Unlit"),
                        (ShadingModelKind.Lambert, "Lambert"),
                        (ShadingModelKind.Gouraud, "Gouraud"),
                    ],
                    () => config.Engine.Graphics.Shading,
                    value => config.Engine.Graphics.Shading = value),
                Save),
            new SavedSetting(
                new EnumSetting<Quantisation>(
                    "Quantisation:",
                    [(Quantisation.Nearest, "Nearest"), (Quantisation.Ordered, "Ordered")],
                    () => config.Engine.Graphics.Quantisation,
                    value => config.Engine.Graphics.Quantisation = value),
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

            // How far the rendered pixels are magnified into the window. The
            // scales offered are the chosen rendition's - a canvas that is
            // already 640 wide has less room to grow than one that is 320 -
            // so switching rendition changes what this row offers on the next
            // launch, which is when either takes effect anyway.
            new SavedSetting(
                new NumberSetting(
                    "Window Scale *:",
                    renditions.Chosen.WindowScales,
                    scale => scale.ToString(CultureInfo.InvariantCulture) + "x",
                    () => config.Engine.WindowScale ?? renditions.Chosen.DefaultWindowScale,
                    value => config.Engine.WindowScale = value),
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
