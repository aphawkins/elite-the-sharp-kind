// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Renditions;
using EliteSharp.Abstractions.Ships;
using EliteSharp.Abstractions.Views;
using EliteSharp.Renditions.EightBit;
using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Graphics;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Views;
using SharpKind.Abstraction;
using SharpKind.Fakes.Input;
using SharpKind.Graphics.Fakes;
using SharpKind.Graphics.Rendering;

namespace EliteSharpLib.Tests.Renditions;

/// <summary>
/// Every screen of every rendition, drawn once from a filled-in model.
/// </summary>
/// <remarks>
/// The 16-bit tier got its cover incidentally, from the golden traces and the
/// headless harness, which draw whatever the game happens to reach. The 8-bit
/// tier is loaded by neither, so nothing had ever drawn a single one of its
/// screens. These tests draw both tiers from the same table, so neither can
/// be the one that is only tested by accident.
/// <para>
/// The palette is the real one off disk, which is what makes a smoke test
/// worth running: a view naming a colour its own tier's palette does not
/// define throws, and nothing else would catch it.
/// </para>
/// </remarks>
[Trait("Level", "Integration")]
public class RenditionViewTests
{
    public static TheoryData<string> Renditions => ["8-bit", "16-bit"];

    // One entry per screen the renditions offer. The models are filled in
    // rather than left at their defaults: an empty list draws nothing, which
    // would let a smoke test pass without reaching the code it is here for.
    private static (Type Model, Action<ViewSet> Draw)[] Screens => [
        (typeof(CommanderStatusModel), DrawCommanderStatus),
        (typeof(CreditsModel), DrawCredits),
        (typeof(EquipmentModel), DrawEquipment),
        (typeof(EscapeCapsuleModel), DrawEscapeCapsule),
        (typeof(GalacticChartModel), DrawGalacticChart),
        (typeof(GameOverModel), DrawGameOver),
        (typeof(Intro1Model), DrawIntro1),
        (typeof(Intro2Model), DrawIntro2),
        (typeof(LoadCommanderModel), DrawLoadCommander),
        (typeof(OptionsModel), DrawOptions),
        (typeof(PilotModel), DrawPilot),
        (typeof(PlanetDataModel), DrawPlanetData),
        (typeof(QuitModel), DrawQuit),
        (typeof(SaveCommanderModel), DrawSaveCommander),
        (typeof(ScannerModel), DrawScanner),
        (typeof(ShortRangeChartModel), DrawShortRangeChart),
    ];

    [Theory]
    [MemberData(nameof(Renditions))]
    public void EveryScreenDrawsWithoutFailing(string renditionName)
    {
        ViewSet views = CreateViews(renditionName, out _);

        foreach ((Type model, Action<ViewSet> draw) in Screens)
        {
            Exception? exception = Record.Exception(() => draw(views));
            Assert.True(exception is null, $"{renditionName} could not draw {model.Name}: {exception}");
        }
    }

    [Theory]
    [MemberData(nameof(Renditions))]
    public void EveryScreenTheRenditionOffersIsDrawnHere(string renditionName)
    {
        ViewSet views = CreateViews(renditionName, out _);

        // A screen added to a rendition and not added here would go on being
        // drawn by nothing, which is how the 8-bit tier got to nought.
        Assert.Equal(
            [.. views.ModelTypes.Select(t => t.Name).Order()],
            [.. Screens.Select(s => s.Model.Name).Order()]);
    }

    [Theory]
    [MemberData(nameof(Renditions))]
    public void TheWordsAScreenIsGivenReachThePage(string renditionName)
    {
        ViewSet views = CreateViews(renditionName, out RecordingGraphics graphics);

        DrawCommanderStatus(views);

        // Both tiers print the system the commander is at and the equipment
        // fitted, so finding them says the model reached the page rather than
        // that the page drew something.
        List<string> text = [.. AllText(graphics)];
        Assert.Contains("LAVE", text);
        Assert.Contains("Large Cargo Bay", text);
    }

    [Theory]
    [MemberData(nameof(Renditions))]
    public void AScreenOfTwoWordsPutsTwoWordsOnThePage(string renditionName)
    {
        ViewSet views = CreateViews(renditionName, out RecordingGraphics graphics);

        DrawQuit(views);

        List<string> text = [.. AllText(graphics)];
        Assert.Contains("QUIT", text);
        Assert.Contains("ARE YOU SURE?", text);
    }

    private static void DrawCommanderStatus(ViewSet views) => views.Get<CommanderStatusModel>().Draw(new(
        "COMMANDER JAMESON",
        "LAVE",
        "ZAONCE",
        "GREEN",
        "7.0 Light Years",
        "100.0 Credits",
        "Clean",
        "Harmless",
        ["Large Cargo Bay", "E.C.M. System"]));

    private static void DrawCredits(ViewSet views)
        => views.Get<CreditsModel>().Draw(new("1.0.0", ["ELITE", "BY IAN BELL", "AND DAVID BRABEN"]));

    private static void DrawEquipment(ViewSet views) => views.Get<EquipmentModel>().Draw(new(
        [
            new("Fuel", false, true, true, "14.0"),
            new("Front", true, false, false, "400.0"),
        ],
        "Cash: 100.0 Credits"));

    private static void DrawEscapeCapsule(ViewSet views)
        => views.Get<EscapeCapsuleModel>().Draw(new("ESCAPE CAPSULE LAUNCHED", true));

    private static void DrawGalacticChart(ViewSet views) => views.Get<GalacticChartModel>().Draw(new(
        "GALACTIC CHART 1",
        [new(new Vector2(20, 30), true), new(new Vector2(120, 200), false)],
        new Vector2(20, 30),
        7,
        new Vector2(120, 200),
        "LAVE",
        "Distance: 4.7 Light Years"));

    private static void DrawGameOver(ViewSet views) => views.Get<GameOverModel>().Draw(new("GAME OVER"));

    private static void DrawIntro1(ViewSet views)
        => views.Get<Intro1Model>().Draw(new(["ELITE", "BY IAN BELL"], "PRESS SPACE"));

    private static void DrawIntro2(ViewSet views)
        => views.Get<Intro2Model>().Draw(new("PRESS SPACE", "COBRA MK 3"));

    private static void DrawLoadCommander(ViewSet views)
        => views.Get<LoadCommanderModel>().Draw(new("JAMESON", "No such commander"));

    private static void DrawOptions(ViewSet views) => views.Get<OptionsModel>().Draw(new(
        [
            new("Save Commander", true),
            new("Quit", true),
            new("Load Commander", false),
        ],
        1));

    private static void DrawPilot(ViewSet views) => views.Get<PilotModel>().Draw(new(
        "FRONT VIEW",
        "HYPERSPACE 4",
        LaserType.Beam,
        true,
        new Vector2(4, -3),
        false));

    private static void DrawPlanetData(ViewSet views) => views.Get<PlanetDataModel>().Draw(new(
        "DATA ON LAVE",
        "4.7 Light Years",
        "Rich Agricultural",
        "Dictatorship",
        "5",
        "2.5 Billion",
        "7000 M CR",
        "4970 km",
        "This planet is a tedious little planet."));

    private static void DrawQuit(ViewSet views) => views.Get<QuitModel>().Draw(new("QUIT", "ARE YOU SURE?"));

    private static void DrawSaveCommander(ViewSet views)
        => views.Get<SaveCommanderModel>().Draw(new("JAMESON", "Saved"));

    private static void DrawScanner(ViewSet views) => views.Get<ScannerModel>().Draw(new(
        false,
        0.8f,
        0.6f,
        5.5f,
        0.2f,
        0.3f,
        0.9f,
        [1f, 0.9f, 0.8f, 0.7f],
        20,
        true,
        3,
        -2,
        [MissileIndicator.Locked, MissileIndicator.Armed, MissileIndicator.Stowed, MissileIndicator.Stowed],
        true,
        true,
        new CompassReading(new Vector2(0.3f, -0.4f), false),
        [new(10, 4, 8, ShipClass.Hostile), new(-20, -6, -12, ShipClass.Station)]));

    private static void DrawShortRangeChart(ViewSet views) => views.Get<ShortRangeChartModel>().Draw(new(
        "SHORT RANGE CHART",
        [new(new Vector2(100, 90), 4)],
        [new(new Vector2(104, 85), "Lave")],
        7,
        new Vector2(100, 90),
        "LAVE",
        "Distance: 0.0 Light Years"));

    private static IEnumerable<string> AllText(RecordingGraphics graphics) => [
        .. graphics.LeftTexts.Select(t => t.Text),
        .. graphics.RightTexts.Select(t => t.Text),
        .. graphics.CentredTexts.Select(t => t.Text),
    ];

    private static ViewSet CreateViews(string renditionName, out RecordingGraphics graphics)
    {
        IRendition rendition = renditionName == "8-bit"
            ? new EightBitRendition()
            : new SixteenBitRendition();

        graphics = new RecordingGraphics(rendition.ScreenWidth, rendition.ScreenHeight);
        GameState gameState = new(
            new ScreenManager<Screen, IScreenController>(new FakeKeyboard()),
            TestMissions.Registry());

        EliteDraw draw = new(
            gameState,
            graphics,
            graphics.Layout,
            TestAssets.Locator(renditionName),
            rendition,
            new ZBufferRenderer(graphics),
            new RenderRandom(new Random(0)));

        return rendition.CreateViews(draw);
    }
}
