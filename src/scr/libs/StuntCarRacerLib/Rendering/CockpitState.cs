// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

namespace StuntCarRacerLib.Rendering;

// The cockpit display values HudRenderer needs, decoupled from CarPhysics
// so the renderer can be tested without simulating exact physics states.
internal readonly record struct CockpitState(
    int LeftWheelFrame,
    int RightWheelFrame,
    int LeftWheelBounce,
    int RightWheelBounce,
    bool BoostActivated,
    int NewDamage,
    int SmashHoles,
    int DisplaySpeed,
    int LapNumber,
    int BoostReserve,
    int OpponentDistance,
    bool OnChains,
    bool WaitingToReleaseChains,
    int ChainSwingAngle);
