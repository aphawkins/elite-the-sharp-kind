// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Views
{
    internal interface IDraw
    {
        float Bottom { get; }

        Vector2 Centre { get; }

        IGraphics Graphics { get; }

        float Left { get; }

        float Offset { get; }

        float Right { get; }

        float ScannerLeft { get; }

        float ScannerRight { get; }

        float ScannerTop { get; }

        float Top { get; }

        void ClearDisplay();

        void DrawBorder();

        void DrawHyperspaceCountdown(int countdown);

        void DrawObject(IShip ship);

        void DrawPolygonFilled(Vector2[] point_list, Colour face_colour, float zAvg);

        void DrawTextPretty(Vector2 position, float width, string text);

        void DrawViewHeader(string title);

        Task LoadImagesAsync(CancellationToken token);

        void RenderEnd();

        void RenderStart();

        void SetFullScreenClipRegion();

        void SetViewClipRegion();
    }
}
