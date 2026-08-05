// 'Useful Libraries' - Andy Hawkins 2023-2026.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

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

                // Relative to the project directory, which is where both
                // `dotnet run` and the benchmarks workflow start from. Kept
                // in step with .gitignore's src/*/perf/*/reports/ and with
                // the workflow's output-file-path.
                .WithArtifactsPath("reports")

                // Runs in-process rather than generating and building a
                // throwaway project per run: that generation looks the project
                // up by name across the whole repo tree and throws if it finds
                // more than one, which it does whenever a git worktree
                // checkout sits nested under the repo (as Claude Code's
                // isolated-worktree agents create). Slightly less isolation
                // between iterations, which these numbers are read relatively
                // enough to absorb.
                .AddJob(Job.Default.WithToolchain(InProcessNoEmitToolchain.Instance)));
}
