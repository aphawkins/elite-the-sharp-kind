// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Renditions;
using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Planets;
using EliteSharp.Abstractions.Views.Stars;
using EliteSharp.Abstractions.Views.Suns;
using EliteSharpLib.Fakes;
using EliteSharpLib.Renditions;

namespace EliteSharpLib.Tests.Views;

public class RenditionRegistryTests
{
    [Fact]
    public void RefusesAPackThatIsAScreenShort()
    {
        // Arrange: a rendition that draws everything but the market. The commander
        // must not find that out by opening the market.
        ShortRendition rendition = new(typeof(MarketModel));

        // Act & Assert
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
            () => new RenditionRegistry(rendition, new FakeEliteDraw()));

        Assert.Contains(nameof(MarketModel), ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NamesEveryMissingScreenAtOnce()
    {
        // Arrange: naming one at a time would mean starting the game once per
        // missing screen to find out what is needed.
        ShortRendition rendition = new(typeof(MarketModel), typeof(QuitModel));

        // Act
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
            () => new RenditionRegistry(rendition, new FakeEliteDraw()));

        // Assert
        Assert.Contains(nameof(MarketModel), ex.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(QuitModel), ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HandsBackTheViewTheScreenAskedFor()
    {
        // Arrange
        RenditionRegistry registry = new(new ShortRendition(), new FakeEliteDraw());

        // Act
        IView<MarketModel> view = registry.View<MarketModel>();

        // Assert
        Assert.NotNull(view);
    }

    [Fact]
    public void RefusesTwoViewsForOneScreen()
    {
        // Arrange: one of the two would never draw, so it is a mistake rather
        // than an override.
        ViewSet views = new();
        views.Add<MarketModel>(new NothingView<MarketModel>());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => views.Add<MarketModel>(new NothingView<MarketModel>()));
    }

    // Draws every screen except the ones it was told to leave out.
    private sealed class ShortRendition(params Type[] omitted) : IRendition
    {
        public string Name => "Short";

        public int ScreenWidth => 320;

        public int ScreenHeight => 256;

        public int Scale => 1;

        public IBaseView CreateBaseView(IViewSurface surface) => new NothingBaseView();

        public IMissionBriefingView CreateMissionBriefingView(IViewSurface surface) => new NothingBriefingView();

        public IPlanetRenderer CreatePlanetRenderer(IViewSurface surface, PlanetLook look)
            => throw new NotSupportedException();

        public ISunRenderer CreateSunRenderer(IViewSurface surface, SunLook look)
            => throw new NotSupportedException();

        public IStarfieldRenderer CreateStarfieldRenderer(IViewSurface surface)
            => new NothingStarfield();

        public ShipColours CreateShipColours(IViewSurface surface)
            => new(default, default, default, default, default);

        public ViewSet CreateViews(IViewSurface surface)
        {
            ViewSet views = new();
            Add<CommanderStatusModel>(views);
            Add<EquipmentModel>(views);
            Add<EscapeCapsuleModel>(views);
            Add<GalacticChartModel>(views);
            Add<GameOverModel>(views);
            Add<Intro1Model>(views);
            Add<Intro2Model>(views);
            Add<InventoryModel>(views);
            Add<LoadCommanderModel>(views);
            Add<MarketModel>(views);
            Add<OptionsModel>(views);
            Add<PilotModel>(views);
            Add<PlanetDataModel>(views);
            Add<QuitModel>(views);
            Add<SaveCommanderModel>(views);
            Add<ScannerModel>(views);
            Add<SettingsListModel>(views);
            Add<ShortRangeChartModel>(views);

            return views;
        }

        private void Add<TModel>(ViewSet views)
        {
            if (!omitted.Contains(typeof(TModel)))
            {
                views.Add<TModel>(new NothingView<TModel>());
            }
        }
    }

    private sealed class NothingStarfield : IStarfieldRenderer
    {
        public void Draw(IReadOnlyList<StarMark> stars)
        {
        }
    }

    private sealed class NothingView<TModel> : IView<TModel>
    {
        public void Draw(TModel model)
        {
        }
    }

    private sealed class NothingBriefingView : IMissionBriefingView
    {
        public Vector4 ShipLocation => Vector4.Zero;

        public void Draw(MissionBriefingModel model)
        {
        }
    }

    private sealed class NothingBaseView : IBaseView
    {
        public Useful.Graphics.IGraphics Graphics => throw new NotSupportedException();

        public ViewLayout Layout => throw new NotSupportedException();

        public void DrawBorder()
        {
        }

        public void DrawFps(int fps)
        {
        }

        public void DrawHyperspaceCountdown(int countdown)
        {
        }

        public void DrawInfoMessage(string message)
        {
        }

        public void DrawTextPretty(Vector2 position, float width, string text)
        {
        }

        public void DrawViewHeader(string title)
        {
        }
    }
}
