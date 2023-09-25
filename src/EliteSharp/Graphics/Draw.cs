// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Assets;
using EliteSharp.Ships;
using EliteSharp.Views;

namespace EliteSharp.Graphics
{
    internal sealed class Draw : IDraw
    {
        private const int MAXPOLYS = 100;
        private readonly GameState _gameState;
        private readonly Vector3[] _pointList = new Vector3[100];
        private readonly PolygonData[] _polyChain = new PolygonData[MAXPOLYS];
        private int _startPoly;
        private int _totalPolys;

        internal Draw(GameState gameState, IGraphics graphics)
        {
            _gameState = gameState;
            Graphics = graphics;
        }

        public float Bottom
            => _gameState.Config.IsViewFullFrame ? Graphics.ScreenHeight - BorderWidth : Graphics.ScreenHeight - ScannerHeight;

        public Vector2 Centre => new(Graphics.ScreenWidth / 2, (ScannerTop / 2) + BorderWidth);

        public IGraphics Graphics { get; }

        public bool IsWidescreen { get; }

        public float Left => BorderWidth;

        public float Offset => ScannerLeft;

        public float Right => Graphics.ScreenWidth - BorderWidth;

        public float ScannerLeft => Centre.X - (ScannerWidth / 2);

        public float ScannerRight => ScannerLeft + ScannerWidth - 1;

        public float ScannerTop => Graphics.ScreenHeight - ScannerHeight;

        public float Top => BorderWidth;

        internal float Height => Bottom - BorderWidth;

        internal float Width => Graphics.ScreenWidth - (2 * BorderWidth);

        private static float BorderWidth => 1;

        private static float ScannerHeight => 129;

        private static float ScannerWidth => 512;

        public void DrawBorder()
        {
            for (int i = 0; i < BorderWidth; i++)
            {
                Graphics.DrawRectangle(new(i, i), Graphics.ScreenWidth - 1 - (2 * i), Bottom - (2 * i), EColors.White);
            }
        }

        public void DrawHyperspaceCountdown(int countdown)
            => Graphics.DrawTextRight(new(Left + 21, Top + 4), $"{countdown}", EColors.White);

        public void DrawPolygonFilled(Vector2[] point_list, EColor face_colour, float zAvg)
        {
            int i;

            if (_totalPolys == MAXPOLYS)
            {
                return;
            }

            int x = _totalPolys;
            _totalPolys++;

            _polyChain[x].FaceColour = face_colour;
            _polyChain[x].Z = zAvg;
            _polyChain[x].Next = -1;
            _polyChain[x].PointList = new Vector2[point_list.Length];

            for (i = 0; i < point_list.Length; i++)
            {
                _polyChain[x].PointList[i].X = point_list[i].X;
                _polyChain[x].PointList[i].Y = point_list[i].Y;
            }

            if (x == 0)
            {
                return;
            }

            if (zAvg > _polyChain[_startPoly].Z)
            {
                _polyChain[x].Next = _startPoly;
                _startPoly = x;
                return;
            }

            for (i = _startPoly; _polyChain[i].Next != -1; i = _polyChain[i].Next)
            {
                int nx = _polyChain[i].Next;

                if (zAvg > _polyChain[nx].Z)
                {
                    _polyChain[i].Next = x;
                    _polyChain[x].Next = nx;
                    return;
                }
            }

            _polyChain[i].Next = x;
        }

        public void DrawTextPretty(Vector2 position, float width, string text)
        {
            int i = 0;
            float maxlen = width / 8;
            int previous = i;

            while (i < text.Length)
            {
                i += (int)maxlen;
                i = Math.Clamp(i, 0, text.Length - 1);

                while (text[i] is not ' ' and not ',' and not '.')
                {
                    i--;
                }

                i++;
                Graphics.DrawTextLeft(position, text[previous..i], EColors.White);
                previous = i;
                position.Y += 8 * Graphics.Scale;
            }
        }

        public void DrawViewHeader(string title)
        {
            Graphics.DrawTextCentre(Top + 10, title, FontSize.Large, EColors.Gold);
            Graphics.DrawLine(new(Left, 36), new(Right, 36), EColors.White);

            // Vertical lines
            Graphics.DrawLine(new(ScannerLeft, Top + 37), new(ScannerLeft, ScannerTop), EColors.Yellow);
            Graphics.DrawLine(new(ScannerRight, Top + 37), new(ScannerRight, ScannerTop), EColors.Yellow);
        }

        public void LoadImages()
        {
            AssetPaths loader = new();
            Parallel.ForEach(
                Enum.GetValues<ImageType>(),
                (img) => Graphics.LoadBitmap(img, loader.AssetPath(img)));
        }

        public void SetFullScreenClipRegion() => Graphics.SetClipRegion(new(0, 0), Graphics.ScreenWidth, Graphics.ScreenHeight);

        public void SetViewClipRegion() => Graphics.SetClipRegion(new(Left, Top), Width, Height);

        /// <summary>
        /// Draws an object in the universe. (Ship, Planet, Sun etc).
        /// </summary>
        public void DrawObject(IObject obj)
        {
            if (_gameState.CurrentScreen is not Screen.FrontView and not Screen.RearView and
                not Screen.LeftView and not Screen.RightView and
                not Screen.IntroOne and not Screen.IntroTwo and
                not Screen.GameOver and not Screen.EscapeCapsule and
                not Screen.MissionOne)
            {
                return;
            }

            if (obj.Flags.HasFlag(ShipProperties.Dead) && !obj.Flags.HasFlag(ShipProperties.Explosion))
            {
                obj.Flags |= ShipProperties.Explosion;
                ((IShip)obj).ExpDelta = 18;
            }

            if (obj.Flags.HasFlag(ShipProperties.Explosion))
            {
                DrawExplosion((IShip)obj);
                return;
            }

            // Only display ships in front of us.
            if (obj.Location.Z <= 0)
            {
                return;
            }

            if (obj.Type == ShipType.Planet)
            {
                obj.Draw();
                return;
            }

            if (obj.Type == ShipType.Sun)
            {
                obj.Draw();
                return;
            }

            // Check for field of vision.
            if (MathF.Abs(obj.Location.X) > obj.Location.Z ||
                MathF.Abs(obj.Location.Y) > obj.Location.Z)
            {
                return;
            }

            obj.Draw();
        }

        public void RenderEnd()
        {
            if (_totalPolys == 0)
            {
                return;
            }

            for (int i = _startPoly; i != -1; i = _polyChain[i].Next)
            {
                EColor colour = _gameState.Config.ShipWireframe ? EColors.White : _polyChain[i].FaceColour;

                if (_polyChain[i].PointList.Length == 2)
                {
                    Graphics.DrawLine(_polyChain[i].PointList[0], _polyChain[i].PointList[1], colour);
                    continue;
                }

                if (_gameState.Config.ShipWireframe)
                {
                    Graphics.DrawPolygon(_polyChain[i].PointList, colour);
                }
                else
                {
                    Graphics.DrawPolygonFilled(_polyChain[i].PointList, colour);
                }
            }
        }

        public void RenderStart()
        {
            _startPoly = 0;
            _totalPolys = 0;
        }

        private void DrawExplosion(IShip ship)
        {
            Vector3[] trans_mat = new Vector3[3];
            bool[] visible = new bool[32];

            if (ship.ExpDelta > 251)
            {
                ship.Flags |= ShipProperties.Remove;
                return;
            }

            ship.ExpDelta += 4;

            if (ship.Location.Z <= 0)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                trans_mat[i] = ship.Rotmat[i];
            }

            Vector3 camera_vec = VectorMaths.MultiplyVector(ship.Location, trans_mat);
            camera_vec = VectorMaths.UnitVector(camera_vec);

            ShipFaceNormal[] ship_norm = ship.FaceNormals;

            for (int i = 0; i < ship.FaceNormals.Length; i++)
            {
                Vector3 vec = VectorMaths.UnitVector(ship_norm[i].Direction);
                float cos_angle = VectorMaths.VectorDotProduct(vec, camera_vec);
                visible[i] = cos_angle < -0.13;
            }

            (trans_mat[1].X, trans_mat[0].Y) = (trans_mat[0].Y, trans_mat[1].X);
            (trans_mat[2].X, trans_mat[0].Z) = (trans_mat[0].Z, trans_mat[2].X);
            (trans_mat[2].Y, trans_mat[1].Z) = (trans_mat[1].Z, trans_mat[2].Y);
            int np = 0;

            for (int i = 0; i < ship.Points.Length; i++)
            {
                if (visible[ship.Points[i].Face1]
                    || visible[ship.Points[i].Face2] ||
                    visible[ship.Points[i].Face3]
                    || visible[ship.Points[i].Face4])
                {
                    Vector3 vec = VectorMaths.MultiplyVector(ship.Points[i].Point, trans_mat);
                    Vector3 r = vec + ship.Location;
                    Vector2 position = new(r.X, -r.Y);
                    position *= 256 / r.Z;
                    position += Centre / 2;
                    position *= Graphics.Scale;
                    _pointList[np].X = position.X;
                    _pointList[np].Y = position.Y;
                    np++;
                }
            }

            float z = ship.Location.Z;
            float q = z >= 0x2000 ? 254 : (int)(z / 32) | 1;
            float pr = ship.ExpDelta * 256 / q;

            ////  if (pr > 0x1C00)
            ////      q = 254;
            ////  else
            q = pr / 32;

            for (int cnt = 0; cnt < np; cnt++)
            {
                float sx = _pointList[cnt].X;
                float sy = _pointList[cnt].Y;

                for (int i = 0; i < 16; i++)
                {
                    Vector2 position = new(RNG.Random(-128, 128), RNG.Random(-128, 128));

                    position.X = position.X * q / 256;
                    position.Y = position.Y * q / 256;

                    position.X = position.X + position.X + sx;
                    position.Y = position.Y + position.Y + sy;

                    int sizex = RNG.Random(1, 3);
                    int sizey = RNG.Random(1, 3);

                    for (int psy = 0; psy < sizey; psy++)
                    {
                        for (int psx = 0; psx < sizex; psx++)
                        {
                            Graphics.DrawPixel(new(position.X + psx, position.Y + psy), EColors.White);
                        }
                    }
                }
            }
        }
    }
}
