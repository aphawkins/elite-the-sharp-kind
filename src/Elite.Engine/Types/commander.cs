// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine.Types
{
    internal sealed class Commander
    {
        internal string Name { get; set; } = string.Empty;
        internal int Mission { get; set; }
        internal GalaxySeed Galaxy { get; set; } = new();
        internal int GalaxyNumber { get; set; }
        internal int LegalStatus { get; set; }
        internal int Score { get; set; }
        internal int Saved { get; set; }
    }
}
