// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

namespace StuntCarRacerLib.Cars;

// Coefficients needed for 3D rotation in order Y, X, Z, following the
// original CalcYXZTrigCoefficients (all rotations anti-clockwise).
internal sealed class TrigCoefficients
{
    internal short XX { get; private set; }

    internal short XY { get; private set; }

    internal short XZ { get; private set; }

    internal short YX { get; private set; }

    internal short YY { get; private set; }

    internal short YZ { get; private set; }

    internal short ZX { get; private set; }

    internal short ZY { get; private set; }

    internal short ZZ { get; private set; }

    internal void CalculateYXZ(int angleX, int angleY, int angleZ)
    {
        const int precision = AmigaTrig.Precision;

        int sinX = AmigaTrig.Sin(angleX);
        int sinY = AmigaTrig.Sin(angleY);
        int sinZ = AmigaTrig.Sin(angleZ);

        int cosX = AmigaTrig.Cos(angleX);
        int cosY = AmigaTrig.Cos(angleY);
        int cosZ = AmigaTrig.Cos(angleZ);

        XX = (short)(((cosY * cosZ) + (sinX * sinY / precision * sinZ)) / precision);
        XY = (short)(-(cosX * sinZ) / precision);
        XZ = (short)((-(sinY * cosZ) + (sinX * cosY / precision * sinZ)) / precision);

        YX = (short)(((cosY * sinZ) - (sinX * sinY / precision * cosZ)) / precision);
        YY = (short)(cosX * cosZ / precision);
        YZ = (short)((-(sinY * sinZ) - (sinX * cosY / precision * cosZ)) / precision);

        ZX = (short)(cosX * sinY / precision);
        ZY = (short)sinX;
        ZZ = (short)(cosX * cosY / precision);
    }
}
