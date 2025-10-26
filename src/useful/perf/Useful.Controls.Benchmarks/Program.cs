// 'Useful Libraries' - Andy Hawkins 2025.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

[assembly: CLSCompliant(false)]

namespace Useful.Controls.Benchmarks;

internal static class Program
{
    public static void Main() => BenchmarkRunner
        .Run<KeyboardBenchmarks>(
            ManualConfig
                .Create(DefaultConfig.Instance)
                .WithArtifactsPath("../../../reports"));
}
