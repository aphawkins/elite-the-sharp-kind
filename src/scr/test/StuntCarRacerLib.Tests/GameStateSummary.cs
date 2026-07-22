// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Screens;

namespace StuntCarRacerLib.Tests;

// A minimal, image-free snapshot of the running game, so most harness
// assertions never need to inspect a rendered frame.
internal readonly record struct GameStateSummary(
    GameMode Screen,
    bool RaceStarted,
    int PlayerPiece,
    int OpponentPiece,
    int DistanceToOpponent);
