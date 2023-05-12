// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine.Ships
{
    [Flags]
    internal enum ShipFlags
    {
        None = 0,
        Dead = 1,
        Remove = 2,
        Explosion = 4,
        Angry = 8,
        Firing = 16,
        HasECM = 32,
        Hostile = 64,
        Cloaked = 128,
        FlyToPlanet = 256,
        FlyToStation = 512,
        Inactive = 1024,
        Slow = 2048,
        Bold = 4096,
        Police = 8192,
    }
}
