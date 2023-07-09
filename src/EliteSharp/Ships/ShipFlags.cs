// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Ships
{
    [Flags]
    internal enum ShipFlags
    {
        None = 0,
        Dead = 1 << 0,
        Remove = 1 << 1,
        Explosion = 1 << 2,
        Angry = 1 << 3,
        Firing = 1 << 4,
        HasECM = 1 << 5,
        Hostile = 1 << 6,
        Cloaked = 1 << 7,
        FlyToPlanet = 1 << 8,
        FlyToStation = 1 << 9,
        Inactive = 1 << 10,
        Slow = 1 << 11,
        Bold = 1 << 12,
        Police = 1 << 13,
        Station = 1 << 14,
        Missile = 1 << 15,
        Tharglet = 1 << 16,
        SpaceJunk = 1 << 17,
        Trader = 1 << 18,
        PackHunter = 1 << 19,
        LoneWolf = 1 << 20,
    }
}
