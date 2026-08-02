// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Immutable;
using EliteSharp.Missions.Abstractions;

namespace EliteSharp.Missions.Classic;

/// <summary>
/// The Navy's hunt for the stolen Constrictor: briefed once the commander is
/// Above Average, hunted down in the second galaxy, paid for on the next
/// docking. Everything it does is in here, seen through
/// <see cref="IMissionContext"/> and said with a <see cref="MissionStep"/>, so
/// nothing about it needs the game's own types.
/// </summary>
public sealed class ConstrictorMission : IMission, IMissionEncounters, IMissionKills, IMissionPlanetDescriptions
{
    /// <summary>
    /// The name the save file records this mission under.
    /// </summary>
    internal const string Id = "Constrictor";

    /// <summary>
    /// The mission has not been offered.
    /// </summary>
    internal const string None = "None";

    /// <summary>
    /// The brief has been shown; the ship is out there to be found.
    /// </summary>
    internal const string Briefed = "Briefed";

    /// <summary>
    /// The Constrictor has been destroyed, but the reward not collected.
    /// </summary>
    internal const string Destroyed = "Destroyed";

    /// <summary>
    /// The debrief has been shown and the bounty paid.
    /// </summary>
    internal const string Rewarded = "Rewarded";

    /// <summary>
    /// The ship, by the name the game's ship list knows it by. It is both what
    /// poses behind the brief and what the commander is hunting.
    /// </summary>
    private const string Constrictor = "Constrictor";

    /// <summary>
    /// The combat score the Navy asks at, which is where Above Average starts.
    /// </summary>
    private const int AboveAverageScore = 256;

    /// <summary>
    /// The galaxy the stolen ship is hiding in, and the system in it where the
    /// commander finally meets it - Orarra, which is also the one system whose
    /// rumour says the pirate is right here.
    /// <para>
    /// This is a planet number rather than the seed bytes the game used to
    /// compare, because a mission cannot see seeds. The game's own MissionsTests
    /// is what keeps it honest: it checks this number still names Orarra.
    /// </para>
    /// </summary>
    private const int HuntingGalaxy = 1;

    /// <inheritdoc cref="HuntingGalaxy"/>
    private const int Orarra = 193;

    private const string BriefA =
        "Greetings Commander, I am Captain Curruthers of "
            + "Her Majesty's Space Navy and I beg a moment of your "
            + "valuable time. We would like you to do a little job "
            + "for us. The ship you see here is a new model, the "
            + "Constrictor, equiped with a top secret new shield "
            + "generator. Unfortunately it's been stolen.";

    private const string BriefFirstGalaxy =
        "It went missing from our ship yard on Xeer five months ago "
            + "and was last seen at Reesdice. Your mission should you decide "
            + "to accept it, is to seek and destroy this ship. You are "
            + "cautioned that only Military Lasers will get through the new "
            + "shields and that the Constrictor is fitted with an E.C.M. "
            + "System. Good Luck, Commander. ---MESSAGE ENDS.";

    private const string BriefLaterGalaxy =
        "It went missing from our ship yard on Xeer five months ago "
            + "and is believed to have jumped to this galaxy. "
            + "Your mission should you decide to accept it, is to seek and "
            + "destroy this ship. You are cautioned that only Military Lasers "
            + "will get through the new shields and that the Constrictor is "
            + "fitted with an E.C.M. System. Good Luck, Commander. ---MESSAGE ENDS.";

    private const string Debrief =
        "There will always be a place for you in Her Majesty's Space Navy. "
            + "And maybe sooner than you think... ---MESSAGE ENDS.";

    /// <summary>
    /// What is being said about the stolen ship, system by system. These are
    /// planet numbers already, so nothing about them had to be translated.
    /// </summary>
    /// <summary>
    /// The stages, in the order the commander passes through them. The names
    /// are the enum's names from before the missions became plugins, so that
    /// commander files written then still load.
    /// </summary>
    private static readonly MissionStages s_declared = new([None, Briefed, Destroyed, Rewarded]);

    private static readonly ImmutableArray<string> s_rumours =
    [
        "THE CONSTRICTOR WAS LAST SEEN AT REESDICE, COMMANDER.",
        "A STRANGE LOOKING SHIP LEFT HERE A WHILE BACK. LOOKED BOUND FOR AREXE.",
        "YEP, AN UNUSUAL NEW SHIP HAD A GALACTIC HYPERDRIVE FITTED HERE, USED IT TOO.",
        "I HEAR A WEIRD LOOKING SHIP WAS SEEN AT ERRIUS.",
        "THIS STRANGE SHIP DEHYPED HERE FROM NOWHERE, SUN SKIMMED AND JUMPED. I HEAR IT WENT TO INBIBE.",
        "ROGUE SHIP WENT FOR ME AT AUSAR. MY LASERS DIDN'T EVEN SCRATCH ITS HULL.",
        "OH DEAR ME YES. A FRIGHTFUL ROGUE WITH WHAT I BELIEVE YOU PEOPLE CALL A LEAD " +
            "POSTERIOR SHOT UP LOTS OF THOSE BEASTLY PIRATES AND WENT TO USLERI.",
        "YOU CAN TACKLE THE VICIOUS SCOUNDREL IF YOU LIKE. HE'S AT ORARRA.",
        "THERE'S A REAL DEADLY PIRATE OUT THERE.",
        "BOY ARE YOU IN THE WRONG GALAXY!",
    ];

    /// <inheritdoc/>
    public string Name => Id;

    /// <inheritdoc/>
    public MissionStages Stages => s_declared;

    /// <inheritdoc/>
    public MissionStep? Advance(IMissionContext context, string stage)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Offered to a commander who has proved they can fight, and only while
        // there is still somewhere for the ship to have run to.
        if (string.Equals(stage, None, StringComparison.Ordinal)
            && context.CombatScore >= AboveAverageScore
            && context.GalaxyNumber < 2)
        {
            return s_declared.Step(
                stage,
                Briefed,
                new MissionBriefing
                {
                    Paragraphs =
                    [
                        BriefA,
                        context.GalaxyNumber == 0 ? BriefFirstGalaxy : BriefLaterGalaxy,
                    ],

                    // The ship the brief is about, posing behind the text.
                    ShipName = Constrictor,
                });
        }

        // The bounty waits for a station: the kill was claimed mid-fight.
        return string.Equals(stage, Destroyed, StringComparison.Ordinal)
            ? s_declared.Step(
                stage,
                Rewarded,
                new MissionBriefing
                {
                    Headline = "Congratulations Commander!",
                    Paragraphs = [Debrief],
                },
                new MissionAward(AboveAverageScore, 5000))
            : null;
    }

    /// <inheritdoc/>
    public AmbushEncounter? Ambush(IMissionContext context, string stage) => null;

    /// <summary>
    /// The stolen ship itself, in place of the lone pirate the system was about
    /// to send - so it turns up as often as a pirate would, in the one system
    /// it is hiding in, and only ever one at a time.
    /// </summary>
    /// <param name="context">The game, seen through the mission facade.</param>
    /// <param name="stage">The stage the commander has reached.</param>
    /// <returns>The Constrictor, or null to let the pirate come.</returns>
    public LoneWolfEncounter? LoneWolfSubstitute(IMissionContext context, string stage)
    {
        ArgumentNullException.ThrowIfNull(context);

        return string.Equals(stage, Briefed, StringComparison.Ordinal)
            && context.GalaxyNumber == HuntingGalaxy
            && context.CurrentPlanetNumber == Orarra
            ? new LoneWolfEncounter(Constrictor, unique: true)
            : null;
    }

    /// <inheritdoc/>
    public MissionStep? ShipDestroyed(IMissionContext context, string stage, string shipName)
        => string.Equals(stage, Briefed, StringComparison.Ordinal)
            && string.Equals(shipName, Constrictor, StringComparison.Ordinal)
            ? s_declared.Step(stage, Destroyed)
            : null;

    /// <summary>
    /// What they are saying in this station about the stolen ship. Only while
    /// the commander is still looking for it, and only about the system they
    /// are standing on - a rumour is somebody here talking, not something the
    /// chart knows about a system light years away.
    /// </summary>
    /// <param name="context">The game, seen through the mission facade.</param>
    /// <param name="stage">The stage the commander has reached.</param>
    /// <param name="planetNumber">The system being described.</param>
    /// <returns>The rumour, or null.</returns>
    public string? DescribePlanet(IMissionContext context, string stage, int planetNumber)
    {
        ArgumentNullException.ThrowIfNull(context);

        return !string.Equals(stage, Briefed, StringComparison.Ordinal)
            || !context.IsDocked
            || planetNumber != context.CurrentPlanetNumber
            ? null
            : context.GalaxyNumber switch
            {
                0 => FirstGalaxyRumour(planetNumber),
                1 => SecondGalaxyRumour(planetNumber),
                2 => planetNumber == 101 ? s_rumours[9] : null,
                _ => null,
            };
    }

    private static string? FirstGalaxyRumour(int planetNumber) => planetNumber switch
    {
        150 => s_rumours[0],
        36 => s_rumours[1],
        28 => s_rumours[2],
        _ => null,
    };

    private static string? SecondGalaxyRumour(int planetNumber) => planetNumber switch
    {
        32 or 68 or 164 or 220 or 106 or 16 or 162 or 3 or 107 or 26 or 192 or 184 or 5 => s_rumours[3],
        253 => s_rumours[4],
        79 => s_rumours[5],
        53 => s_rumours[6],
        118 => s_rumours[7],
        Orarra => s_rumours[8],
        _ => null,
    };
}
