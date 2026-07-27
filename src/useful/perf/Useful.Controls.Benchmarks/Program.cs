// 'Useful Libraries' - Andy Hawkins 2023-2026.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

[assembly: CLSCompliant(false)]

namespace Useful.Controls.Benchmarks;

internal static class Program
{
    public static void Main(string[] args) => BenchmarkSwitcher
        .FromAssembly(typeof(Program).Assembly)
        .Run(
            args.Length == 0 ? ["--filter", "*"] : args,
            ManualConfig
                .Create(DefaultConfig.Instance)
                .WithArtifactsPath("../../../reports"));
}
