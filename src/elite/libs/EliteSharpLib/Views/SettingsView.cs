// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Graphics;
using EliteSharpLib.Planets;
using Useful.Config;
using Useful.Controls;

namespace EliteSharpLib.Views;

// The "game" half of the config file: how Elite itself looks and plays. The
// shared engine settings have their own screen - see EngineSettingsView.
internal sealed class SettingsView : SettingsListView
{
    private readonly Space _space;

    internal SettingsView(
        GameState gameState,
        IEliteDraw draw,
        IKeyboard keyboard,
        Space space,
        IConfigWriter<EliteConfig> configWriter)
        : base(
            gameState,
            draw,
            keyboard,
            configWriter,
            "GAME SETTINGS",
            [
                new("Planet Style:", ["Solid", "Striped", "Fractal"]),
                new("Sun Style:", ["Solid", "Gradient"]),
                new("Planet Desc.:", ["BBC", "MSX"]),
                new("Instant Dock:", ["Off", "On"]),
            ])
        => _space = space;

    protected override int SettingValue(int index) => index switch
    {
        0 => (int)State.Config.Game.PlanetStyle,
        1 => (int)State.Config.Game.SunStyle,
        2 => State.Config.Game.PlanetDescriptions == PlanetDescriptions.HoopyCasinos ? 1 : 0,
        3 => State.Config.Game.InstantDock ? 1 : 0,
        _ => 0,
    };

    protected override void ToggleSetting(int index)
    {
        switch (index)
        {
            case 0:
                State.Config.Game.PlanetStyle = Next(State.Config.Game.PlanetStyle);
                _space.RefreshPlanetStyle();
                break;

            case 1:
                State.Config.Game.SunStyle = Next(State.Config.Game.SunStyle);
                _space.RefreshSunStyle();
                break;

            case 2:
                State.Config.Game.PlanetDescriptions = Next(State.Config.Game.PlanetDescriptions);
                break;

            case 3:
                State.Config.Game.InstantDock = !State.Config.Game.InstantDock;
                break;
        }
    }
}
