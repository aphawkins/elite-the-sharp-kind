// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Conflict;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Useful.Controls;
using Useful.Maths;

namespace EliteSharpLib.Views;

/// <summary>
/// The Constrictor mission's message sequence: advancing the commander's
/// mission number when the brief is earned or the kill is claimed, and
/// selecting which briefing is shown.
/// </summary>
internal sealed class ConstrictorMissionController : IScreenController
{
    private const string Mission1BriefA =
        "Greetings Commander, I am Captain Curruthers of " +
            "Her Majesty's Space Navy and I beg a moment of your " +
            "valuable time.  We would like you to do a little job " +
            "for us.  The ship you see here is a new model, the " +
            "Constrictor, equiped with a top secret new shield " +
            "generator.  Unfortunately it's been stolen.";

    private const string Mission1BriefB =
        "It went missing from our ship yard on Xeer five months ago " +
            "and was last seen at Reesdice. Your mission should you decide " +
            "to accept it, is to seek and destroy this ship. You are " +
            "cautioned that only Military Lasers will get through the new " +
            "shields and that the Constrictor is fitted with an E.C.M. " +
            "System. Good Luck, Commander. ---MESSAGE ENDS.";

    private const string Mission1BriefC =
        "It went missing from our ship yard on Xeer five months ago " +
            "and is believed to have jumped to this galaxy. " +
            "Your mission should you decide to accept it, is to seek and " +
            "destroy this ship. You are cautioned that only Military Lasers " +
            "will get through the new shields and that the Constrictor is " +
            "fitted with an E.C.M. System. Good Luck, Commander. ---MESSAGE ENDS.";

    private const string Mission1Debrief =
        "There will always be a place for you in Her Majesty's Space Navy. " +
            "And maybe sooner than you think... ---MESSAGE ENDS.";

    private readonly Combat _combat;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly PlayerShip _ship;
    private readonly Trade _trade;
    private readonly Universe _universe;
    private readonly IShipFactory _shipFactory;
    private readonly ILogger<ConstrictorMissionController> _logger;
    private readonly IView<ConstrictorMissionModel> _view;

    internal ConstrictorMissionController(
        GameState gameState,
        IKeyboard keyboard,
        PlayerShip ship,
        Trade trade,
        Combat combat,
        Universe universe,
        IShipFactory shipFactory,
        IView<ConstrictorMissionModel> view,
        ILogger<ConstrictorMissionController>? logger = null)
    {
        _gameState = gameState;
        _keyboard = keyboard;
        _ship = ship;
        _trade = trade;
        _combat = combat;
        _universe = universe;
        _shipFactory = shipFactory;
        _view = view;
        _logger = logger ?? NullLogger<ConstrictorMissionController>.Instance;
    }

    public void Draw() => _view.Draw(BuildModel());

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.Spacebar))
        {
            _combat.Reset();
            _universe.ClearUniverse();
            _gameState.SetView(Screen.MissionTwo);
        }
    }

    public void Reset()
    {
        if (_gameState.Cmdr.Mission == 0 && _gameState.Cmdr.Score >= 256 && _gameState.Cmdr.GalaxyNumber < 2)
        {
            // Show brief
            _gameState.Cmdr.Mission = 1;

            _combat.Reset();
            _universe.ClearUniverse();
            IShip constrictor = _shipFactory.CreateShip("Constrictor");
            if (!_universe.AddNewShip(constrictor, new(200, 90, 600, 0), VectorMaths.GetLeftHandedBasisMatrix, -127, -127))
            {
                LogMessages.FailedToCreateShip(_logger, "Constrictor");
            }

            constrictor.Flags = ShipProperties.None;
            _ship.Roll = 0;
            _ship.Climb = 0;
            _ship.Speed = 0;
        }
        else if (_gameState.Cmdr.Mission == 2)
        {
            // Show debrief
            _gameState.Cmdr.Mission = 3;
            _gameState.Cmdr.Score += 256;
            _trade.Credits += 5000;
        }
        else
        {
            _gameState.SetView(Screen.MissionTwo);
        }
    }

    public void Update()
    {
    }

    // Exposed for tests: which briefing the current mission number selects,
    // and which of the two second paragraphs the galaxy chooses.
    internal ConstrictorMissionModel BuildModel() => _gameState.Cmdr.Mission switch
    {
        1 => new(
            1,
            string.Empty,
            [Mission1BriefA, _gameState.Cmdr.GalaxyNumber == 0 ? Mission1BriefB : Mission1BriefC]),
        3 => new(3, "Congratulations Commander!", [Mission1Debrief]),
        _ => new(0, string.Empty, []),
    };
}
