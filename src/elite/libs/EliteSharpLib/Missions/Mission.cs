// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Types;

namespace EliteSharpLib.Missions;

internal sealed class Mission
{
    private readonly string[] _mission1_pdesc =
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
        "COMING SOON: ELITE - DARKNESS FALLS.",
    ];

    private readonly PlanetController _planet;

    internal Mission(PlanetController planet) => _planet = planet;

    internal string? MissionPlanetDescription(GameState gameState, GalaxySeed planet)
    {
        if (!gameState.IsDocked)
        {
            return null;
        }

        if (planet.A != gameState.DockedPlanet.A ||
            planet.B != gameState.DockedPlanet.B ||
            planet.C != gameState.DockedPlanet.C ||
            planet.D != gameState.DockedPlanet.D ||
            planet.E != gameState.DockedPlanet.E ||
            planet.F != gameState.DockedPlanet.F)
        {
            return null;
        }

        int pnum = _planet.FindPlanetNumber(gameState.Cmdr.Galaxy, planet);

        return gameState.Cmdr.GalaxyNumber switch
        {
            0 => Galaxy0Description(pnum),
            1 => Galaxy1Description(pnum),
            2 => pnum == 101 ? _mission1_pdesc[9] : null,
            _ => null,
        };
    }

    private string? Galaxy0Description(int pnum) => pnum switch
    {
        150 => _mission1_pdesc[0],
        36 => _mission1_pdesc[1],
        28 => _mission1_pdesc[2],
        _ => null,
    };

    private string? Galaxy1Description(int pnum) => pnum switch
    {
        32 or 68 or 164 or 220 or 106 or 16 or 162 or 3 or 107 or 26 or 192 or 184 or 5 => _mission1_pdesc[3],
        253 => _mission1_pdesc[4],
        79 => _mission1_pdesc[5],
        53 => _mission1_pdesc[6],
        118 => _mission1_pdesc[7],
        193 => _mission1_pdesc[8],
        _ => null,
    };
}
