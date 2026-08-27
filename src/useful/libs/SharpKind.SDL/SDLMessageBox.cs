// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SDL;
using static SDL.SDL3;

namespace SharpKind.SDL;

/// <summary>
/// A standalone error dialog: the last resort for telling a player why the game
/// did not start. A failure already reaches the log file and stderr, and a
/// player who launched from a shortcut sees neither - to them the window simply
/// never appears.
/// <para>
/// SDL's simple message box is deliberately the whole implementation. It works
/// before <c>SDL_Init</c> and without a window, which is exactly the ground
/// this has to cover: the failures worth reporting are the ones that happen
/// before there is anything to show them in.
/// </para>
/// </summary>
public static unsafe class SDLMessageBox
{
    /// <summary>
    /// Shows the failure, or gives up quietly. This is itself the fallback, so
    /// it has none of its own: a dialog that cannot be shown leaves the console
    /// and the log file, and both have already been written by the time this is
    /// called. It does not go through <see cref="SDLGuard"/> for the same
    /// reason - reporting a failure must not raise one.
    /// </summary>
    /// <param name="title">The dialog's title, which is the game's.</param>
    /// <param name="message">What went wrong, over as many lines as it takes.</param>
    public static void ShowError(string title, string message)
    {
        try
        {
            _ = SDL_ShowSimpleMessageBox(SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR, title, message, null);
        }
        catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException or BadImageFormatException)
        {
            // The native library is the very thing that could not be loaded, so
            // there is no dialog to be had.
        }
    }
}
