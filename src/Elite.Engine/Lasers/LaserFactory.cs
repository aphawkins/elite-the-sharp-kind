namespace Elite.Engine
{
    using Elite.Engine.Enums;
    using Elite.Engine.Lasers;

    internal static class LaserFactory
    {
        internal static ILaser GetLaser(LaserType type)
        {
            return type switch
            {
                LaserType.Military => new MilitaryLaser(),
                LaserType.Mining => new MiningLaser(),
                LaserType.Pulse => new PulseLaser(),
                LaserType.Beam => new BeamLaser(),
                _ => new LaserNone(),
            };
        }
    }
}