// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class DodecStation : ShipBase
{
    internal DodecStation(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Dodec;
        Flags = ShipProperties.Station;
        EnergyMax = 240;
        MinDistance = 900;
        Name = "Dodec Space Station";
        Size = 32400;
        VanishPoint = 125;
    }
}
