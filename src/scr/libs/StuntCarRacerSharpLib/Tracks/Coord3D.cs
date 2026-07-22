// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

namespace StuntCarRacerSharpLib.Tracks;

// Fixed-point world coordinate (PRECISION units, see the original 3D Engine.h).
public record struct Coord3D(int X, int Y, int Z);
