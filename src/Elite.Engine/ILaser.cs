namespace Elite.Engine
{
    using Elite.Engine.Enums;

    public interface ILaser
    {
        string Name { get; }

        int Strength { get; }

        LaserType Type { get; }

        int Temperature { get; set;  }
    }
}
