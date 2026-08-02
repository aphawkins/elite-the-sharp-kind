// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Conflict;
using EliteSharpLib.Equipment;
using EliteSharpLib.Missions;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Useful.Controls;
using Useful.Maths;

namespace EliteSharpLib.Views;

/// <summary>
/// The one screen every mission speaks on. On docking it asks each mission in
/// turn whether it has anything to say; the first that does gets the screen,
/// and pressing space asks the ones after it, so a commander who has earned two
/// messages at once still sees both. When nobody has anything the screen is
/// left at once, which is what happens on almost every docking.
/// <para>
/// The message sequences still live here rather than in the missions
/// themselves. What has gone is the screen knowing whose message it is drawing:
/// it hands the view a briefing, and the view lays it out from what is in it.
/// </para>
/// </summary>
internal sealed class MissionBriefingController : IScreenController
{
    private const string Mission1BriefA =
        "Greetings Commander, I am Captain Curruthers of "
            + "Her Majesty's Space Navy and I beg a moment of your "
            + "valuable time. We would like you to do a little job "
            + "for us. The ship you see here is a new model, the "
            + "Constrictor, equiped with a top secret new shield "
            + "generator. Unfortunately it's been stolen.";

    private const string Mission1BriefB =
        "It went missing from our ship yard on Xeer five months ago "
            + "and was last seen at Reesdice. Your mission should you decide "
            + "to accept it, is to seek and destroy this ship. You are "
            + "cautioned that only Military Lasers will get through the new "
            + "shields and that the Constrictor is fitted with an E.C.M. "
            + "System. Good Luck, Commander. ---MESSAGE ENDS.";

    private const string Mission1BriefC =
        "It went missing from our ship yard on Xeer five months ago "
            + "and is believed to have jumped to this galaxy. "
            + "Your mission should you decide to accept it, is to seek and "
            + "destroy this ship. You are cautioned that only Military Lasers "
            + "will get through the new shields and that the Constrictor is "
            + "fitted with an E.C.M. System. Good Luck, Commander. ---MESSAGE ENDS.";

    private const string Mission1Debrief =
        "There will always be a place for you in Her Majesty's Space Navy. "
            + "And maybe sooner than you think... ---MESSAGE ENDS.";

    private const string Mission2BriefA =
        "Attention Commander, I am Captain Fortesque of Her Majesty's Space Navy. "
            + "We have need of your services again. If you would be so good as to go to "
            + "Ceerdi you will be briefed.If succesful, you will be rewarded."
            + "---MESSAGE ENDS.";

    private const string Mission2BriefB =
        "Good Day Commander. I am Agent Blake of Naval Intelligence. As you know, "
            + "the Navy have been keeping the Thargoids off your ass out in deep space "
            + "for many years now. Well the situation has changed. Our boys are ready "
            + "for a push right to the home system of those murderers.";

    private const string Mission2BriefC =
        "I have obtained the defence plans for their Hive Worlds. The beetles "
            + "know we've got something but not what. If I transmit the plans to our "
            + "base on Birera they'll intercept the transmission. I need a ship to "
            + "make the run. You're elected. The plans are unipulse coded within "
            + "this transmission. You will be paid. Good luck Commander. ---MESSAGE ENDS.";

    private const string Mission2Debrief =
        "You have served us well and we shall remember. "
            + "We did not expect the Thargoids to find out about you."
            + "For the moment please accept this Navy Extra Energy Unit as payment. "
            + "---MESSAGE ENDS.";

    private readonly Combat _combat;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly PlayerShip _ship;
    private readonly Trade _trade;
    private readonly Universe _universe;
    private readonly IShipFactory _shipFactory;
    private readonly ILogger<MissionBriefingController> _logger;
    private readonly IMissionBriefingView _view;

    /// <summary>
    /// The mission whose message is on screen, so that space asks the ones
    /// after it rather than starting again at the top and showing the same
    /// message for ever.
    /// </summary>
    private string? _speaking;

    internal MissionBriefingController(
        GameState gameState,
        IKeyboard keyboard,
        PlayerShip ship,
        Trade trade,
        Combat combat,
        Universe universe,
        IShipFactory shipFactory,
        IMissionBriefingView view,
        ILogger<MissionBriefingController>? logger = null)
    {
        _gameState = gameState;
        _keyboard = keyboard;
        _ship = ship;
        _trade = trade;
        _combat = combat;
        _universe = universe;
        _shipFactory = shipFactory;
        _view = view;
        _logger = logger ?? NullLogger<MissionBriefingController>.Instance;
    }

    // Exposed for tests: what the screen is showing, which is the whole of what
    // the view is given.
    internal MissionBriefingModel Briefing { get; private set; } = MissionBriefingModel.Nothing;

    /// <summary>
    /// Gets the missions in the order they get the screen. The Constrictor
    /// comes first because the Thargoid run is only offered once it has been
    /// paid for, so one docking can earn both.
    /// </summary>
    private IEnumerable<(string Mission, Func<MissionBriefingModel?> Turn)> Sequence
        => [(ConstrictorMission.Id, ConstrictorTurn), (ThargoidMission.Id, ThargoidTurn)];

    public void Draw() => _view.Draw(Briefing);

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.Spacebar))
        {
            ShowNext(after: _speaking);
        }
    }

    public void Reset() => ShowNext(after: null);

    public void Update()
    {
    }

    /// <summary>
    /// Gives the screen to the first mission after <paramref name="after"/>
    /// with something to say, or leaves the screen when none has.
    /// </summary>
    /// <param name="after">The mission already shown, or null to ask them all.</param>
    private void ShowNext(string? after)
    {
        bool reached = after is null;

        foreach ((string mission, Func<MissionBriefingModel?> turn) in Sequence)
        {
            if (!reached)
            {
                reached = string.Equals(mission, after, StringComparison.Ordinal);
                continue;
            }

            if (turn() is { } briefing)
            {
                _speaking = mission;
                Briefing = briefing;
                return;
            }
        }

        Briefing = MissionBriefingModel.Nothing;
        _speaking = null;
        _gameState.SetView(Screen.CommanderStatus);
    }

    private MissionBriefingModel? ConstrictorTurn()
    {
        if (_gameState.Cmdr.Missions.IsAt(ConstrictorMission.Id, ConstrictorMission.None)
            && _gameState.Cmdr.Score >= 256
            && _gameState.Cmdr.GalaxyNumber < 2)
        {
            // Show brief, with the ship it is about posing behind the text.
            _gameState.Cmdr.Missions.MoveTo(ConstrictorMission.Id, ConstrictorMission.Briefed);

            _combat.Reset();
            _universe.ClearUniverse();
            IShip constrictor = _shipFactory.CreateShip("Constrictor");
            if (!_universe.AddNewShip(constrictor, _view.ShipLocation, VectorMaths.GetLeftHandedBasisMatrix, -127, -127))
            {
                LogMessages.FailedToCreateShip(_logger, "Constrictor");
            }

            constrictor.Flags = ShipProperties.None;
            _ship.Roll = 0;
            _ship.Climb = 0;
            _ship.Speed = 0;

            return new(
                string.Empty,
                [Mission1BriefA, _gameState.Cmdr.GalaxyNumber == 0 ? Mission1BriefB : Mission1BriefC],
                ShowPortrait: false);
        }

        if (_gameState.Cmdr.Missions.IsAt(ConstrictorMission.Id, ConstrictorMission.Destroyed))
        {
            // Show debrief
            _gameState.Cmdr.Missions.MoveTo(ConstrictorMission.Id, ConstrictorMission.Rewarded);
            _gameState.Cmdr.Score += 256;
            _trade.Credits += 5000;

            return OnAnEmptyStage("Congratulations Commander!", [Mission1Debrief], showPortrait: false);
        }

        return null;
    }

    private MissionBriefingModel? ThargoidTurn()
    {
        // The Navy only calls once the Constrictor has been paid for, and only
        // once: the old single mission number said that by being 3 and then
        // moving on, where the Constrictor's own stage stays Rewarded for good.
        if (_gameState.Cmdr.Missions.IsAt(ThargoidMission.Id, ThargoidMission.None)
            && _gameState.Cmdr.Missions.IsAt(ConstrictorMission.Id, ConstrictorMission.Rewarded)
            && _gameState.Cmdr.Score >= 1280
            && _gameState.Cmdr.GalaxyNumber == 2)
        {
            // First brief
            _gameState.Cmdr.Missions.MoveTo(ThargoidMission.Id, ThargoidMission.Summoned);

            return OnAnEmptyStage(string.Empty, [Mission2BriefA], showPortrait: false);
        }

        if (_gameState.Cmdr.Missions.IsAt(ThargoidMission.Id, ThargoidMission.Summoned)
            && _gameState.DockedPlanet.D == 215
            && _gameState.DockedPlanet.B == 84)
        {
            // Second brief
            _gameState.Cmdr.Missions.MoveTo(ThargoidMission.Id, ThargoidMission.CarryingPlans);

            return OnAnEmptyStage(string.Empty, [Mission2BriefB, Mission2BriefC], showPortrait: true);
        }

        if (_gameState.Cmdr.Missions.IsAt(ThargoidMission.Id, ThargoidMission.CarryingPlans)
            && _gameState.DockedPlanet.D == 63
            && _gameState.DockedPlanet.B == 72)
        {
            // Debrief
            _gameState.Cmdr.Missions.MoveTo(ThargoidMission.Id, ThargoidMission.Rewarded);
            _gameState.Cmdr.Score += 256;
            _ship.EnergyUnit = EnergyUnit.Naval;

            return OnAnEmptyStage("Well done Commander!", [Mission2Debrief], showPortrait: false);
        }

        return null;
    }

    /// <summary>
    /// A briefing with nothing posing behind it, which is all of them but the
    /// Constrictor's. There used to be a screen the universe was never drawn
    /// on; now there is one screen for every briefing, so keeping the stage
    /// empty is what keeps the universe out of the picture.
    /// </summary>
    private MissionBriefingModel OnAnEmptyStage(string headline, IReadOnlyList<string> paragraphs, bool showPortrait)
    {
        _combat.Reset();
        _universe.ClearUniverse();

        return new(headline, paragraphs, showPortrait);
    }
}
