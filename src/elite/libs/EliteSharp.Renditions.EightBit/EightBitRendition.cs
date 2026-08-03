// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Renditions;
using EliteSharp.Abstractions.Views;

namespace EliteSharp.Renditions.EightBit;

/// <summary>
/// The 8-bit tier: a 320x256 canvas, a fixed 8x8 font and sixteen colours
/// under their web names. Every screen is authored fresh for that canvas
/// rather than scaled down from the 16-bit one, so this rendition shares no layout
/// with the other and needs none.
/// </summary>
public sealed class EightBitRendition : IRendition
{
    public string Name => "EightBit";

    public int ScreenWidth => 320;

    public int ScreenHeight => 256;

    public int Scale => 1;

    public IBaseView CreateBaseView(IViewSurface surface) => new BaseView8Bit(surface);

    public IMissionBriefingView CreateMissionBriefingView(IViewSurface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        return new MissionBriefingView8Bit(surface);
    }

    public ViewSet CreateViews(IViewSurface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        return new ViewSet()
            .Add<CommanderStatusModel>(new CommanderStatusView8Bit(surface))
            .Add<EquipmentModel>(new EquipmentView8Bit(surface))
            .Add<EscapeCapsuleModel>(new EscapeCapsuleView8Bit(surface))
            .Add<GalacticChartModel>(new GalacticChartView8Bit(surface))
            .Add<GameOverModel>(new GameOverView8Bit(surface))
            .Add<Intro1Model>(new Intro1View8Bit(surface))
            .Add<Intro2Model>(new Intro2View8Bit(surface))
            .Add<InventoryModel>(new InventoryView8Bit(surface))
            .Add<LoadCommanderModel>(new LoadCommanderView8Bit(surface))
            .Add<MarketModel>(new MarketView8Bit(surface))
            .Add<OptionsModel>(new OptionsView8Bit(surface))
            .Add<PilotModel>(new PilotView8Bit(surface))
            .Add<PlanetDataModel>(new PlanetDataView8Bit(surface))
            .Add<QuitModel>(new QuitView8Bit(surface))
            .Add<SaveCommanderModel>(new SaveCommanderView8Bit(surface))
            .Add<ScannerModel>(new ScannerView8Bit(surface))
            .Add<SettingsListModel>(new SettingsListView8Bit(surface))
            .Add<ShortRangeChartModel>(new ShortRangeChartView8Bit(surface));
    }
}
