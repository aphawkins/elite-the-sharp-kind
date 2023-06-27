// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Ships;

namespace EliteSharp.Views
{
    internal interface IDraw
    {
        float Bottom { get; }

        Vector2 Centre { get; }

        float Left { get; }

        float Right { get; }

        float ScannerLeft { get; }

        float ScannerTop { get; }

        float Top { get; }

        void ClearDisplay();

        void DrawBorder();

        void DrawHyperspaceCountdown(int countdown);

        void DrawSun(IShip planet);

        void DrawTextPretty(Vector2 position, float width, string text);

        void DrawViewHeader(string title);

        Task LoadImagesAsync(CancellationToken token);

        void SetFullScreenClipRegion();

        void SetViewClipRegion();
    }
}
