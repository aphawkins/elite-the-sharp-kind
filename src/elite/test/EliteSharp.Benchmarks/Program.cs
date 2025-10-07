// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using BenchmarkDotNet.Running;

[assembly: CLSCompliant(false)]

namespace EliteSharp.Benchmarks;

internal static class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<PlanetBenchmarks>();
        BenchmarkRunner.Run<SunBenchmarks>();
        BenchmarkRunner.Run<SoftwareGraphicsBenchmarks>();
    }
}
