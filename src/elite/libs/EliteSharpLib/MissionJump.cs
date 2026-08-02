// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Types;
using EliteSharpLib.Views;

namespace EliteSharpLib;

/// <summary>
/// Jumps to the mission briefing screens, which are otherwise reachable only
/// by playing the missions - the Constrictor brief wants a combat rating of
/// Above Average, and the Thargoid sequence wants the third galaxy and two
/// specific planets. Checking those screens' layout meant hours of play or an
/// edited save; with <see cref="EnvVar"/> set, Ctrl-M cycles them (see
/// docs/elite-readme.md).
/// <para>
/// Each jump sets the state the screen's own <c>Reset</c> tests for and then
/// lets that <c>Reset</c> run, rather than forcing the screen straight in:
/// the briefings choose their text - and, for the Constrictor, spawn the ship
/// the screen shows - on the way through, so anything that skipped it would
/// be checking a screen the game never draws.
/// </para>
/// <para>
/// These are cheats, and they leave the commander mid-mission: the last two
/// stages also overwrite the docked planet's seed with the system the
/// briefing expects, so the planet name and data screens will disagree with
/// the chart afterwards. Restart the game rather than carrying on from one.
/// </para>
/// </summary>
internal static class MissionJump
{
    /// <summary>
    /// Set (to any value) to enable the Ctrl-M jump - a runtime opt-in rather
    /// than a debug build, following the same convention as
    /// <c>ELITE_DEBUG_COMMANDER</c> (issue #7 took Elite's last
    /// conditional-compilation site out for this reason), so the screens can
    /// be checked in a Release build too. Unset in normal play.
    /// </summary>
    internal const string EnvVar = "ELITE_DEBUG_MISSIONS";

    // The Thargoid briefings key off where the commander is docked: Ceerdi
    // for the second brief, Birera for the debrief (ThargoidMissionController).
    private const int CeerdiD = 215;
    private const int CeerdiB = 84;
    private const int BireraD = 63;
    private const int BireraB = 72;

    // The combat scores the two missions are offered at
    // (ConstrictorMissionController and ThargoidMissionController).
    private const int AboveAverageScore = 256;
    private const int DangerousScore = 1280;

    /// <summary>
    /// Gets a value indicating whether <see cref="EnvVar"/> is set.
    /// </summary>
    internal static bool IsEnabled => Environment.GetEnvironmentVariable(EnvVar) is not null;

    /// <summary>
    /// Gets the number of mission screens <see cref="To"/> can reach.
    /// </summary>
    internal static int Count => 5;

    /// <summary>
    /// Jumps to one of the mission screens, counted from 0 in the order the
    /// missions play out: Constrictor brief, Constrictor debrief, then the
    /// Thargoid sequence's two briefs and its debrief.
    /// </summary>
    internal static void To(GameState gameState, int stage)
    {
        ArgumentNullException.ThrowIfNull(gameState);

        switch (stage)
        {
            case 0:
                gameState.Cmdr.Constrictor = ConstrictorStage.None;
                gameState.Cmdr.Thargoid = ThargoidStage.None;
                gameState.Cmdr.Score = AboveAverageScore;
                gameState.Cmdr.GalaxyNumber = 0;
                gameState.SetView(Screen.MissionOne);
                break;

            case 1:
                gameState.Cmdr.Constrictor = ConstrictorStage.Destroyed;
                gameState.SetView(Screen.MissionOne);
                break;

            case 2:
                gameState.Cmdr.Constrictor = ConstrictorStage.Rewarded;
                gameState.Cmdr.Thargoid = ThargoidStage.None;
                gameState.Cmdr.Score = DangerousScore;
                gameState.Cmdr.GalaxyNumber = 2;
                gameState.SetView(Screen.MissionTwo);
                break;

            case 3:
                gameState.Cmdr.Constrictor = ConstrictorStage.Rewarded;
                gameState.Cmdr.Thargoid = ThargoidStage.Summoned;
                gameState.DockedPlanet.D = CeerdiD;
                gameState.DockedPlanet.B = CeerdiB;
                gameState.SetView(Screen.MissionTwo);
                break;

            default:
                gameState.Cmdr.Constrictor = ConstrictorStage.Rewarded;
                gameState.Cmdr.Thargoid = ThargoidStage.CarryingPlans;
                gameState.DockedPlanet.D = BireraD;
                gameState.DockedPlanet.B = BireraB;
                gameState.SetView(Screen.MissionTwo);
                break;
        }
    }
}
