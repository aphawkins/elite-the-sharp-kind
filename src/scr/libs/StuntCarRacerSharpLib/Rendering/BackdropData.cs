// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

// Generated from the original Backdrop.cpp scenery tables. Do not hand-edit the arrays.
using StuntCarRacerSharpLib.Tracks;

namespace StuntCarRacerSharpLib.Rendering;

internal static class BackdropData
{
    internal static int[] SceneryPositions { get; } =
    [
        5, 15, 21, 31, 37, 47, 53, 63,
        69, 79, 85, 95, 101, 111, 117, 127,
        133, 143, 149, 159, 165, 175, 181, 191,
        197, 207, 213, 223, 229, 239, 245, 255,
    ];

    internal static int[] StandardNumbers { get; } =
    [
        0, 13, 10, 11, 12, 5, 2, 3,
        0, 1, 4, 5, 2, 1, 0, 5,
        2, 3, 4, 5, 0, 9, 6, 7,
        8, 5, 0, 3, 4, 1, 2, 5,
    ];

    internal static int[] TallerNumbers { get; } =
    [
        14, 15, 15, 14, 15, 14, 14, 15,
        14, 15, 14, 15, 15, 14, 15, 14,
        14, 15, 15, 14, 15, 15, 14, 14,
        15, 14, 15, 15, 14, 14, 14, 15,
    ];

    internal static int[] SnowcappedNumbers { get; } =
    [
        16, 17, 18, 19, 17, 16, 17, 18,
        18, 16, 19, 17, 19, 18, 18, 16,
        19, 17, 17, 18, 16, 16, 19, 18,
        17, 16, 19, 19, 17, 18, 16, 19,
    ];

    internal static int[] BuildingNumbers { get; } =
    [
        20, 21, 22, 23, 23, 21, 20, 20,
        21, 22, 22, 20, 20, 21, 23, 22,
        21, 20, 21, 21, 23, 22, 20, 21,
        23, 23, 22, 20, 21, 20, 21, 23,
    ];

    internal static int[] MixedNumbers { get; } =
    [
        16, 0, 18, 1, 17, 2, 17, 3,
        18, 4, 19, 5, 20, 24, 21, 24,
        23, 22, 12, 19, 6, 16, 7, 18,
        8, 16, 9, 19, 10, 18, 11, 19,
    ];

    internal static int[] StandardPolygons { get; } = [5, 4, 1, 0, 2, 3];

    internal static int[] TallerPolygons { get; } = [4, 3, 2, 0, 3, 5, 3, 1, 2, 3];

    internal static int[] SnowcappedPolygons { get; } = [4, 4, 1, 0, 5, 4, 5, 3, 2, 1, 4, 5, 4, 3, 2, 4, 6, 15, 3, 4, 5, 6];

    internal static int[] BuildingPolygons { get; } = [15, 4, 1, 0, 3, 4, 14, 4, 2, 1, 4, 5];

    internal static int[] LakePolygons { get; } = [6, 4, 2, 1, 0, 3];

    internal static Coord3D[] Standard1Coords { get; } =
    [
        new(0, 0, 65536),
        new(384, 0, 65536),
        new(75, 28, 65536),
        new(260, 16, 65536),
    ];

    internal static Coord3D[] Standard2Coords { get; } =
    [
        new(0, 0, 65536),
        new(256, 0, 65536),
        new(125, 18, 65536),
        new(192, 30, 65536),
    ];

    internal static Coord3D[] Standard3Coords { get; } =
    [
        new(0, 0, 65536),
        new(384, 0, 65536),
        new(100, 20, 65536),
        new(310, 37, 65536),
    ];

    internal static Coord3D[] Standard4Coords { get; } =
    [
        new(0, 0, 65536),
        new(256, 0, 65536),
        new(70, 24, 65536),
        new(216, 36, 65536),
    ];

    internal static Coord3D[] Standard5Coords { get; } =
    [
        new(0, 0, 65536),
        new(384, 0, 65536),
        new(200, 39, 65536),
        new(240, 31, 65536),
    ];

    internal static Coord3D[] Standard6Coords { get; } =
    [
        new(0, 0, 65536),
        new(256, 0, 65536),
        new(50, 12, 65536),
        new(168, 26, 65536),
    ];

    internal static Coord3D[] Standard7Coords { get; } =
    [
        new(0, 0, 65536),
        new(370, 0, 65536),
        new(112, 25, 65536),
        new(230, 20, 65536),
    ];

    internal static Coord3D[] Standard8Coords { get; } =
    [
        new(0, 0, 65536),
        new(250, 0, 65536),
        new(100, 12, 65536),
        new(187, 18, 65536),
    ];

    internal static Coord3D[] Standard9Coords { get; } =
    [
        new(0, 0, 65536),
        new(384, 0, 65536),
        new(198, 28, 65536),
        new(315, 24, 65536),
    ];

    internal static Coord3D[] Standard10Coords { get; } =
    [
        new(0, 0, 65536),
        new(256, 0, 65536),
        new(35, 40, 65536),
        new(110, 55, 65536),
    ];

    internal static Coord3D[] Standard11Coords { get; } =
    [
        new(0, 0, 65536),
        new(345, 0, 65536),
        new(92, 42, 65536),
        new(240, 30, 65536),
    ];

    internal static Coord3D[] Standard12Coords { get; } =
    [
        new(0, 0, 65536),
        new(250, 0, 65536),
        new(45, 15, 65536),
        new(128, 11, 65536),
    ];

    internal static Coord3D[] Standard13Coords { get; } =
    [
        new(0, 0, 65536),
        new(380, 0, 65536),
        new(136, 43, 65536),
        new(210, 35, 65536),
    ];

    internal static Coord3D[] Standard14Coords { get; } =
    [
        new(0, 0, 65536),
        new(256, 0, 65536),
        new(75, 41, 65536),
        new(155, 55, 65536),
    ];

    internal static Coord3D[] Taller1Coords { get; } =
    [
        new(0, 0, 65536),
        new(380, 0, 65536),
        new(136, 0, 65536),
        new(43, 210, 65536),
    ];

    internal static Coord3D[] Taller2Coords { get; } =
    [
        new(0, 0, 65536),
        new(256, 0, 65536),
        new(75, 0, 65536),
        new(41, 155, 65536),
    ];

    internal static Coord3D[] Snowcapped1Coords { get; } =
    [
        new(0, 0, 65536),
        new(250, 0, 65536),
        new(420, 0, 65536),
        new(595, 0, 65536),
        new(385, 46, 65536),
        new(280, 52, 65536),
        new(415, 115, 65536),
    ];

    internal static Coord3D[] Snowcapped2Coords { get; } =
    [
        new(0, 0, 65536),
        new(75, 0, 65536),
        new(295, 0, 65536),
        new(500, 0, 65536),
        new(175, 50, 65536),
        new(135, 60, 65536),
        new(255, 72, 65536),
    ];

    internal static Coord3D[] Snowcapped3Coords { get; } =
    [
        new(0, 0, 65536),
        new(135, 0, 65536),
        new(197, 0, 65536),
        new(250, 0, 65536),
        new(150, 70, 65536),
        new(105, 80, 65536),
        new(170, 95, 65536),
    ];

    internal static Coord3D[] Snowcapped4Coords { get; } =
    [
        new(0, 0, 65536),
        new(135, 0, 65536),
        new(275, 0, 65536),
        new(425, 0, 65536),
        new(145, 42, 65536),
        new(60, 50, 65536),
        new(140, 77, 65536),
    ];

    internal static Coord3D[] Building1Coords { get; } =
    [
        new(0, 0, 65536),
        new(16, 0, 65536),
        new(24, 0, 65536),
        new(0, 80, 65536),
        new(16, 80, 65536),
        new(24, 80, 65536),
    ];

    internal static Coord3D[] Building2Coords { get; } =
    [
        new(0, 0, 65536),
        new(16, 0, 65536),
        new(24, 0, 65536),
        new(0, 60, 65536),
        new(16, 60, 65536),
        new(24, 60, 65536),
    ];

    internal static Coord3D[] Building3Coords { get; } =
    [
        new(0, 0, 65536),
        new(40, 0, 65536),
        new(60, 0, 65536),
        new(0, 57, 65536),
        new(40, 57, 65536),
        new(60, 57, 65536),
    ];

    internal static Coord3D[] Building4Coords { get; } =
    [
        new(0, 0, 65536),
        new(105, 0, 65536),
        new(125, 0, 65536),
        new(0, 42, 65536),
        new(105, 42, 65536),
        new(125, 42, 65536),
    ];

    internal static Coord3D[] LakeCoords { get; } =
    [
        new(0, 8, 65536),
        new(50, 0, 65536),
        new(650, 0, 65536),
        new(700, 8, 65536),
    ];

    // scenery objects indexed by scenery number (coords + polygon list)
    internal static (Coord3D[] Coords, int[] Polygons)[] SceneryObjects { get; } =
    [
        (Standard1Coords, StandardPolygons),
        (Standard2Coords, StandardPolygons),
        (Standard3Coords, StandardPolygons),
        (Standard4Coords, StandardPolygons),
        (Standard5Coords, StandardPolygons),
        (Standard6Coords, StandardPolygons),
        (Standard7Coords, StandardPolygons),
        (Standard8Coords, StandardPolygons),
        (Standard9Coords, StandardPolygons),
        (Standard10Coords, StandardPolygons),
        (Standard11Coords, StandardPolygons),
        (Standard12Coords, StandardPolygons),
        (Standard13Coords, StandardPolygons),
        (Standard14Coords, StandardPolygons),
        (Taller1Coords, TallerPolygons),
        (Taller2Coords, TallerPolygons),
        (Snowcapped1Coords, SnowcappedPolygons),
        (Snowcapped2Coords, SnowcappedPolygons),
        (Snowcapped3Coords, SnowcappedPolygons),
        (Snowcapped4Coords, SnowcappedPolygons),
        (Building1Coords, BuildingPolygons),
        (Building2Coords, BuildingPolygons),
        (Building3Coords, BuildingPolygons),
        (Building4Coords, BuildingPolygons),
        (LakeCoords, LakePolygons),
    ];
}
