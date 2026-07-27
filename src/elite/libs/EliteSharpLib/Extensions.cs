// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;

namespace EliteSharpLib;

internal static class Extensions
{
    internal static string CapitaliseFirstLetter(this string text) => string.Create(
        text.Length,
        text,
        static (span, source) =>
        {
            span[0] = char.ToUpperInvariant(source[0]);
            for (int i = 1; i < source.Length; i++)
            {
                span[i] = char.ToLowerInvariant(source[i]);
            }
        });

    internal static void CopyTo(this IShip from, IShip to)
    {
        from.CopyTo((IObject)to);

        to.Bounty = from.Bounty;
        to.EnergyMax = from.EnergyMax;
        to.LaserFront = from.LaserFront;
        to.LaserStrength = from.LaserStrength;
        to.LootMax = from.LootMax;
        to.MissilesMax = from.MissilesMax;
        to.Model = from.Model;
        to.Name = from.Name;
        to.ScoopedType = from.ScoopedType;
        to.Size = from.Size;
        to.VanishPoint = from.VanishPoint;
        to.VelocityMax = from.VelocityMax;
        to.ExpDelta = from.ExpDelta;
        to.Flags = from.Flags;
        to.Type = from.Type;
        to.Location = from.Location;
        to.Energy = from.Energy;
        to.Velocity = from.Velocity;
        to.Acceleration = from.Acceleration;
        to.Missiles = from.Missiles;
        to.Target = from.Target;
        to.Bravery = from.Bravery;
        to.MinDistance = from.MinDistance;
    }

    internal static void CopyTo(this IObject from, IObject to)
    {
        to.Flags = from.Flags;
        to.Rotmat = from.Rotmat;
        to.RotX = from.RotX;
        to.RotZ = from.RotZ;
        to.Type = from.Type;
        to.Location = from.Location;
    }
}
