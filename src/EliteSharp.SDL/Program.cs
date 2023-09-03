// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Controls;
using EliteSharp.Graphics;

namespace EliteSharp.SDL
{
    internal static class Program
    {
        public static void Main()
        {
            using ISound sound = new Sound();
            IKeyboard keyboard = new Keyboard();
            using CancellationTokenSource source = new();
            CancellationToken token = source.Token;

#pragma warning disable CA2000 // Dispose objects before losing scope
            IGraphics graphics = new SdlGraphics();
#pragma warning restore CA2000 // Dispose objects before losing scope

            EliteMain game = new(graphics, sound, keyboard);
            game.Run(token);
        }
    }
}
