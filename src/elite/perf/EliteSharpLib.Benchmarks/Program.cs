// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

[assembly: CLSCompliant(false)]

namespace EliteSharpLib.Benchmarks;

internal static class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<PlanetBenchmarks>(
            ManualConfig
                .Create(DefaultConfig.Instance)
                .WithArtifactsPath("../../../reports"));

        BenchmarkRunner.Run<SunBenchmarks>(
            ManualConfig
                .Create(DefaultConfig.Instance)
                .WithArtifactsPath("../../../reports"));
    }
}
