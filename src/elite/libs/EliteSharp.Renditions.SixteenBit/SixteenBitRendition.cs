// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Renditions;
using EliteSharp.Abstractions.Views;
using Useful.Assets;

namespace EliteSharp.Renditions.SixteenBit;

/// <summary>
/// The 16-bit tier: a 640x512 canvas, a proportional font with no character
/// grid, and a 29-entry colour ramp whose names the 8-bit palette does not
/// share. Every screen here looks up only names this tier's palette defines.
/// </summary>
public sealed class SixteenBitRendition : IRendition
{
    public SystemTier Tier => SystemTier.SixteenBit;

    public IBaseView CreateBaseView(IViewSurface surface) => new BaseView16Bit(surface);

    public IMissionBriefingView CreateMissionBriefingView(IViewSurface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        return new MissionBriefingView16Bit(surface);
    }

    public ViewSet CreateViews(IViewSurface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        return new ViewSet()
            .Add<CommanderStatusModel>(new CommanderStatusView16Bit(surface))
            .Add<EquipmentModel>(new EquipmentView16Bit(surface))
            .Add<EscapeCapsuleModel>(new EscapeCapsuleView16Bit(surface))
            .Add<GalacticChartModel>(new GalacticChartView16Bit(surface))
            .Add<GameOverModel>(new GameOverView16Bit(surface))
            .Add<Intro1Model>(new Intro1View16Bit(surface))
            .Add<Intro2Model>(new Intro2View16Bit(surface))
            .Add<InventoryModel>(new InventoryView16Bit(surface))
            .Add<LoadCommanderModel>(new LoadCommanderView16Bit(surface))
            .Add<MarketModel>(new MarketView16Bit(surface))
            .Add<OptionsModel>(new OptionsView16Bit(surface))
            .Add<PilotModel>(new PilotView16Bit(surface))
            .Add<PlanetDataModel>(new PlanetDataView16Bit(surface))
            .Add<QuitModel>(new QuitView16Bit(surface))
            .Add<SaveCommanderModel>(new SaveCommanderView16Bit(surface))
            .Add<ScannerModel>(new ScannerView16Bit(surface))
            .Add<SettingsListModel>(new SettingsListView16Bit(surface))
            .Add<ShortRangeChartModel>(new ShortRangeChartView16Bit(surface));
    }
}
