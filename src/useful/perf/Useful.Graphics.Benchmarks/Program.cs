// 'Useful Libraries' - Andy Hawkins 2025.

using BenchmarkDotNet.Running;

[assembly: CLSCompliant(false)]

namespace Useful.Graphics.Benchmarks;

internal static class Program
{
    public static void Main() => BenchmarkRunner.Run<SoftwareGraphicsBenchmarks>();
}
