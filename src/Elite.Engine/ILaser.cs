using Elite.Engine.Enums;

namespace Elite.Engine
{
    public interface ILaser
    {
        string Name { get; }

        int Strength { get; }

        LaserType Type { get; }

        int Temperature { get; set;  }
    }
}
