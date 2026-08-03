// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;

namespace EliteSharpLib.Views;

/// <summary>
/// What a view pack drew, checked against what the game actually has screens
/// for. The check happens once, here, before a frame is composed: a pack that
/// is a screen short is a startup failure naming the screen rather than a
/// screen that turns out to be blank when the commander opens it.
/// </summary>
internal sealed class ViewRegistry
{
    // Every screen with a controller of its own, the HUD among them - it is
    // drawn from a model by a controller like any other. The mission briefing
    // is not here because the pack hands it over separately: it is the one
    // screen that answers back, with where its tier puts the posing ship.
    private static readonly Type[] s_requiredModels =
    [
        typeof(CommanderStatusModel),
        typeof(EquipmentModel),
        typeof(EscapeCapsuleModel),
        typeof(GalacticChartModel),
        typeof(GameOverModel),
        typeof(Intro1Model),
        typeof(Intro2Model),
        typeof(InventoryModel),
        typeof(LoadCommanderModel),
        typeof(MarketModel),
        typeof(OptionsModel),
        typeof(PilotModel),
        typeof(PlanetDataModel),
        typeof(QuitModel),
        typeof(SaveCommanderModel),
        typeof(ScannerModel),
        typeof(SettingsListModel),
        typeof(ShortRangeChartModel),
    ];

    private readonly ViewSet _views;

    internal ViewRegistry(IViewPack pack, IViewSurface surface)
    {
        ArgumentNullException.ThrowIfNull(pack);

        BaseView = pack.CreateBaseView(surface);
        MissionBriefingView = pack.CreateMissionBriefingView(surface);
        _views = pack.CreateViews(surface);

        string[] missing = [.. s_requiredModels
            .Except(_views.ModelTypes)
            .Select(model => model.Name)
            .Order(StringComparer.Ordinal)];

        if (missing.Length > 0)
        {
            throw new InvalidOperationException(
                $"The {pack.Tier} view pack draws no screen for: {string.Join(", ", missing)}.");
        }
    }

    /// <summary>
    /// Gets the tier's shared chrome, which every screen draws its border
    /// through and EliteMain the hyperspace countdown.
    /// </summary>
    internal IBaseView BaseView { get; }

    /// <summary>
    /// Gets the screen every mission's messages are drawn on.
    /// </summary>
    internal IMissionBriefingView MissionBriefingView { get; }

    /// <summary>
    /// Gets the view for one screen.
    /// </summary>
    /// <typeparam name="TModel">The model the screen draws.</typeparam>
    /// <returns>The view for that model.</returns>
    internal IView<TModel> View<TModel>() => _views.Get<TModel>();
}
