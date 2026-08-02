// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Equipment;
using EliteSharpLib.Missions;
using EliteSharpLib.Ships;
using Useful.Controls;

namespace EliteSharpLib.Views;

/// <summary>
/// The Thargoid mission's message sequence: advancing the mission's stage on
/// arrival at the right system, and selecting which briefing is shown.
/// </summary>
internal sealed class ThargoidMissionController : IScreenController
{
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

    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly PlayerShip _ship;
    private readonly IView<ThargoidMissionModel> _view;

    internal ThargoidMissionController(
        GameState gameState,
        IKeyboard keyboard,
        PlayerShip ship,
        IView<ThargoidMissionModel> view)
    {
        _gameState = gameState;
        _keyboard = keyboard;
        _ship = ship;
        _view = view;
    }

    public void Draw() => _view.Draw(BuildModel());

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.Spacebar))
        {
            _gameState.SetView(Screen.CommanderStatus);
        }
    }

    public void Reset()
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
        }
        else if (_gameState.Cmdr.Missions.IsAt(ThargoidMission.Id, ThargoidMission.Summoned)
            && _gameState.DockedPlanet.D == 215
            && _gameState.DockedPlanet.B == 84)
        {
            // Second brief
            _gameState.Cmdr.Missions.MoveTo(ThargoidMission.Id, ThargoidMission.CarryingPlans);
        }
        else if (_gameState.Cmdr.Missions.IsAt(ThargoidMission.Id, ThargoidMission.CarryingPlans)
            && _gameState.DockedPlanet.D == 63
            && _gameState.DockedPlanet.B == 72)
        {
            // Debrief
            _gameState.Cmdr.Missions.MoveTo(ThargoidMission.Id, ThargoidMission.Rewarded);
            _gameState.Cmdr.Score += 256;
            _ship.EnergyUnit = EnergyUnit.Naval;
        }
        else
        {
            _gameState.SetView(Screen.CommanderStatus);
        }
    }

    public void Update()
    {
    }

    // Exposed for tests: which briefing the current stage selects.
    internal ThargoidMissionModel BuildModel() => _gameState.Cmdr.Missions.StageOf(ThargoidMission.Id) switch
    {
        ThargoidMission.Summoned => new(
            ThargoidMission.Summoned,
            string.Empty,
            [Mission2BriefA],
            false),
        ThargoidMission.CarryingPlans => new(
            ThargoidMission.CarryingPlans,
            string.Empty,
            [Mission2BriefB, Mission2BriefC],
            true),
        ThargoidMission.Rewarded => new(
            ThargoidMission.Rewarded,
            "Well done Commander!",
            [Mission2Debrief],
            false),
        _ => new(ThargoidMission.None, string.Empty, [], false),
    };
}
