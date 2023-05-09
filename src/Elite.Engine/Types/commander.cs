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
