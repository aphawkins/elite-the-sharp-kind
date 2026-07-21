// 'Useful Libraries' - Andy Hawkins 2025.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

[assembly: CLSCompliant(false)]

namespace Useful.Graphics.Benchmarks;

internal static class Program
{
    // Plain `dotnet run -c Release` (no arguments) runs every benchmark under
    // the full DefaultConfig job, same as before. Pass CLI arguments to speed
    // up local iteration instead, e.g.:
    //   dotnet run -c Release -- --filter *DrawPixel* --job short
    //
    // Runs in-process (no separate throwaway project generated/built per
    // run): avoids BenchmarkDotNet's project-file lookup, which searches the
    // whole repo tree by project name and throws if it finds more than one
    // (e.g. when a git worktree checkout sits nested under the repo, as
    // Claude Code's isolated-worktree agents create). Slightly less
    // isolation between iterations than the default out-of-process
    // toolchain, which is an acceptable trade for a benchmark suite whose
    // numbers are read as relative comparisons rather than absolute ones.
    public static void Main(string[] args) => BenchmarkSwitcher
        .FromAssembly(typeof(Program).Assembly)
        .Run(
            args.Length == 0 ? ["--filter", "*"] : args,
            ManualConfig
                .Create(DefaultConfig.Instance)
                .WithArtifactsPath("../../../reports")
                .AddJob(Job.Default.WithToolchain(InProcessNoEmitToolchain.Instance)));
}
