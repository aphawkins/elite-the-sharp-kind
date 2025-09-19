// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

[assembly: CLSCompliant(false)]

namespace EliteSharp.SDL;

internal static class SDLProgram
{
    private const string Title = "Elite - The Sharp Kind";

#if QHD
    private const int ScreenWidth = 960;
    private const int ScreenHeight = 540;
#else
    private const int ScreenWidth = 512;
    private const int ScreenHeight = 512;
#endif

#if SOFTWARERENDERER
    private static readonly SDLGameFactory s_gameFactory = new(ScreenWidth, ScreenHeight, Title, "SOFTWARE");
#else
    private static readonly SDLGameFactory s_gameFactory = new(ScreenWidth, ScreenHeight, Title, "SDL");
#endif

    public static void Main()
    {
        try
        {
            s_gameFactory.Game.Run();
        }
        catch //// (Exception ex)
        {
            //// Console.WriteLine(ex.ToString());
            Environment.Exit(-1);
            throw;
        }
    }
}
