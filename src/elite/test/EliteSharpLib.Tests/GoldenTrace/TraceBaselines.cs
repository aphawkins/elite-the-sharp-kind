// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Tests.GoldenTrace;

// Where the committed baselines live.
//
// Reading goes through the build output, which the csproj copies them into,
// so a test run never depends on the source tree being where it was
// compiled. Regenerating writes back to the source tree instead, because the
// point of regenerating is to produce a diff to review and commit - a
// rewritten copy under bin/ would be thrown away by the next clean.
internal static class TraceBaselines
{
    // Set ELITE_REGENERATE_TRACES=1 to rewrite the baselines from the
    // current build instead of asserting against them. Deliberately not a
    // flag on the test: regenerating is how a genuine behaviour change is
    // accepted, and it should take a conscious act outside the test run.
    internal const string RegenerateEnvVar = "ELITE_REGENERATE_TRACES";

    private const string Folder = "Baselines";

    internal static bool Regenerating
        => Environment.GetEnvironmentVariable(RegenerateEnvVar) is "1" or "true";

    internal static string OutputPath(string scenarioName)
        => Path.Combine(AppContext.BaseDirectory, "GoldenTrace", Folder, scenarioName + TraceFile.Extension);

    internal static string SourcePath(string scenarioName)
        => Path.Combine(SourceFolder(), Folder, scenarioName + TraceFile.Extension);

    // Found by walking up from the build output to the project folder, not
    // by [CallerFilePath]: this repo builds with deterministic source paths,
    // so a caller path is the rewritten "/_/src/..." and regenerating through
    // it silently wrote the baselines to C:\_ instead of the working tree.
    private static string SourceFolder()
    {
        const string ProjectFile = "EliteSharpLib.Tests.csproj";

        DirectoryInfo? folder = new(AppContext.BaseDirectory);
        while (folder is not null && !File.Exists(Path.Combine(folder.FullName, ProjectFile)))
        {
            folder = folder.Parent;
        }

        return folder is null
            ? throw new InvalidOperationException(
                $"Cannot locate {ProjectFile} above {AppContext.BaseDirectory}; regenerate from a source build.")
            : Path.Combine(folder.FullName, "GoldenTrace");
    }
}
