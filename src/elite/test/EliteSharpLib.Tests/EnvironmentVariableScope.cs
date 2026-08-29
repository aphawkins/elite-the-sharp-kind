// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Tests;

/// <summary>
/// Sets an environment variable for as long as the scope lives, then puts back
/// whatever was there before. Environment variables belong to the process, not
/// the test, so a test that sets one and walks away has changed the answer for
/// every test that reads it afterwards - and xUnit runs test classes in
/// parallel, so which ones those are varies from run to run.
/// </summary>
internal sealed class EnvironmentVariableScope : IDisposable
{
    private readonly string _name;
    private readonly string? _original;

    private EnvironmentVariableScope(string name, string? value)
    {
        _name = name;
        _original = Environment.GetEnvironmentVariable(name);
        Environment.SetEnvironmentVariable(name, value);
    }

    public void Dispose() => Environment.SetEnvironmentVariable(_name, _original);

    /// <summary>
    /// Sets the variable until the scope is disposed.
    /// </summary>
    /// <param name="name">The environment variable to set.</param>
    /// <param name="value">The value to give it, or null to clear it.</param>
    /// <returns>The scope that restores the previous value.</returns>
    internal static EnvironmentVariableScope Set(string name, string? value) => new(name, value);
}
