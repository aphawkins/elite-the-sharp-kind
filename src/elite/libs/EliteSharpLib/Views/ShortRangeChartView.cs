// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using EliteSharpLib.Types;
using Useful.Controls;

namespace EliteSharpLib.Views;

internal sealed class ShortRangeChartView : IView
{
    private readonly IEliteDraw _draw;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly PlanetController _planet;
    private readonly List<(Vector2 Position, string Name)> _planetNames = [];
    private readonly List<(Vector2 Position, float Size)> _planetSizes = [];
    private readonly PlayerShip _ship;
    private readonly uint _colorGold;
    private readonly uint _colorGreen;
    private readonly uint _colorLighterRed;
    private readonly uint _colorWhite;

    private int _crossTimer;
    private string _findName = string.Empty;
    private bool _isFind;

    internal ShortRangeChartView(GameState gameState, IEliteDraw draw, IKeyboard keyboard, PlanetController planet, PlayerShip ship)
    {
        _gameState = gameState;
        _draw = draw;
        _keyboard = keyboard;
        _planet = planet;
        _ship = ship;

        _colorGold = _draw.Palette["Gold"];
        _colorGreen = _draw.Palette["Green"];
        _colorLighterRed = _draw.Palette["LighterRed"];
        _colorWhite = _draw.Palette["White"];
    }

    public void Draw()
    {
        // Header
        _draw.DrawViewHeader("SHORT RANGE CHART");

        // Fuel radius
        Vector2 centre = _draw.Centre;
        float radius = _ship.Fuel * 10 * _draw.Graphics.Scale;
        float cross_size = 16 * _draw.Graphics.Scale;
        _draw.Graphics.DrawCircle(centre, radius, _colorGreen);
        _draw.Graphics.DrawLine(new(centre.X, centre.Y - cross_size), new(centre.X, centre.Y + cross_size), _colorWhite);
        _draw.Graphics.DrawLine(new(centre.X - cross_size, centre.Y), new(centre.X + cross_size, centre.Y), _colorWhite);

        // Planets
        foreach ((Vector2 position, string name) in _planetNames)
        {
            _draw.Graphics.DrawTextLeft(position, name, (int)FontType.Small, _colorWhite);
        }

        foreach ((Vector2 position, float size) in _planetSizes)
        {
            _draw.Graphics.DrawCircleFilled(position, size, _colorGold);
        }

        // Cross
        centre = new(_gameState.Cross.X, _gameState.Cross.Y);
        _draw.Graphics.DrawLine(new(centre.X - 16, centre.Y), new(centre.X + 16, centre.Y), _colorLighterRed);
        _draw.Graphics.DrawLine(new(centre.X, centre.Y - 16), new(centre.X, centre.Y + 16), _colorLighterRed);

        // Text
        if (_isFind)
        {
            _draw.Graphics
                .DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 55), "Planet Name?", (int)FontType.Small, _colorGreen);
            _draw.Graphics
                .DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 40), _findName, (int)FontType.Small, _colorWhite);
        }
        else if (string.IsNullOrEmpty(_gameState.PlanetName))
        {
            _draw.Graphics
                .DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 55), "Unknown Planet", (int)FontType.Small, _colorGreen);
            _draw.Graphics
                .DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 40), _findName, (int)FontType.Small, _colorWhite);
        }
        else
        {
            _draw.Graphics.DrawTextLeft(
                new(16 + _draw.Offset, _draw.ScannerTop - 55),
                _gameState.PlanetName,
                (int)FontType.Small,
                _colorGreen);
            if (_gameState.DistanceToPlanet > 0)
            {
                _draw.Graphics.DrawTextLeft(
                    new(16 + _draw.Offset, _draw.ScannerTop - 40),
                    $"Distance: {_gameState.DistanceToPlanet:N1} Light Years ",
                    (int)FontType.Small,
                    _colorWhite);
            }
        }
    }

    public void HandleInput()
    {
        if (_isFind)
        {
            if (_keyboard.IsPressed(ConsoleKey.Backspace) &&
                !string.IsNullOrEmpty(_findName))
            {
                _findName = _findName[..^1];
            }

            if (_keyboard.IsPressed(ConsoleKey.Enter))
            {
                _isFind = false;
                if (_planet.FindPlanetByName(_findName))
                {
                    CrossFromHyperspacePlanet();
                    CalculateDistanceToPlanet();
                }
                else
                {
                    _gameState.PlanetName = string.Empty;
                }
            }

            (ConsoleKey key, ConsoleModifiers _) = _keyboard.LastPressed();
            if (key is >= ConsoleKey.A and <= ConsoleKey.Z)
            {
                _findName += (char)key;
            }

            return;
        }

        if (_keyboard.IsPressed(ConsoleKey.O))
        {
            _gameState.Cross = _draw.Centre;
            CalculateDistanceToPlanet();
        }

        if (_keyboard.IsPressed(ConsoleKey.D))
        {
            CalculateDistanceToPlanet();
        }

        if (_keyboard.IsPressed(ConsoleKey.S) || _keyboard.IsPressed(ConsoleKey.UpArrow))
        {
            MoveCross(0, -1);
        }

        if (_keyboard.IsPressed(ConsoleKey.X) || _keyboard.IsPressed(ConsoleKey.DownArrow))
        {
            MoveCross(0, 1);
        }

        if (_keyboard.IsPressed(ConsoleKey.OemComma) || _keyboard.IsPressed(ConsoleKey.LeftArrow))
        {
            MoveCross(-1, 0);
        }

        if (_keyboard.IsPressed(ConsoleKey.OemPeriod) || _keyboard.IsPressed(ConsoleKey.RightArrow))
        {
            MoveCross(1, 0);
        }

        if (_keyboard.IsPressed(ConsoleKey.F))
        {
            _isFind = true;
            _findName = string.Empty;
            _keyboard.ClearPressed();  // Clear the F so that it doesn't appear in the find word
        }
    }

    public void Reset()
    {
        _isFind = false;
        _findName = string.Empty;
        int[] row_used = new int[64];
        _planetNames.Clear();
        _planetSizes.Clear();

        for (int i = 0; i < 64; i++)
        {
            row_used[i] = 0;
        }

        GalaxySeed glx = new(_gameState.Cmdr.Galaxy);

        for (int i = 0; i < 256; i++)
        {
            float dx = MathF.Abs(glx.D - _gameState.DockedPlanet.D);
            float dy = MathF.Abs(glx.B - _gameState.DockedPlanet.B);

            if ((dx >= 20) || (dy >= 38))
            {
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);

                continue;
            }

            float px = glx.D - _gameState.DockedPlanet.D;

            // Convert to screen co-ords
            px = (px * 4 * _draw.Graphics.Scale) + _draw.Centre.X;

            float py = glx.B - _gameState.DockedPlanet.B;

            // Convert to screen co-ords
            py = (py * 2 * _draw.Graphics.Scale) + _draw.Centre.Y;

            int row = (int)(py / (8 * _draw.Graphics.Scale));

            if (row_used[row] == 1)
            {
                row++;
            }

            if (row_used[row] == 1)
            {
                row -= 2;
            }

            if (row <= 3)
            {
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);

                continue;
            }

            if (row_used[row] == 0)
            {
                row_used[row] = 1;
                _planetNames.Add((
                    new(px + (4 * _draw.Graphics.Scale), ((row * 8) - 5) * _draw.Graphics.Scale),
                    _planet.NamePlanet(glx)
                        .CapitaliseFirstLetter()));
            }

            // The next bit calculates the size of the circle used to represent
            // a planet.  The carry_flag is left over from the name generation.
            // Yes this was how it was done... don't ask :-(
            float blob_size = (glx.F & 1) + 2 + _gameState.CarryFlag;
            blob_size *= _draw.Graphics.Scale;
            _planetSizes.Add((new(px, py), blob_size));

            _planet.WaggleGalaxy(glx);
            _planet.WaggleGalaxy(glx);
            _planet.WaggleGalaxy(glx);
            _planet.WaggleGalaxy(glx);
        }

        _crossTimer = 0;
        CrossFromHyperspacePlanet();
        CalculateDistanceToPlanet();
    }

    public void UpdateUniverse()
    {
        if (_crossTimer > 0)
        {
            _crossTimer--;
            if (_crossTimer == 0)
            {
                CalculateDistanceToPlanet();
            }
        }
    }

    private void CalculateDistanceToPlanet()
    {
        Vector2 location = new()
        {
            X = ((_gameState.Cross.X - _draw.Centre.X) / (4 * _draw.Graphics.Scale)) + _gameState.DockedPlanet.D,
            Y = ((_gameState.Cross.Y - _draw.Centre.Y) / (2 * _draw.Graphics.Scale)) + _gameState.DockedPlanet.B,
        };

        _gameState.HyperspacePlanet = _planet.FindPlanet(_gameState.Cmdr.Galaxy, location);
        _gameState.PlanetName = _planet.NamePlanet(_gameState.HyperspacePlanet);
        _gameState.DistanceToPlanet = PlanetController.CalculateDistanceToPlanet(_gameState.DockedPlanet, _gameState.HyperspacePlanet);
        CrossFromHyperspacePlanet();
    }

    private void CrossFromHyperspacePlanet() => _gameState.Cross = new(
        ((_gameState.HyperspacePlanet.D - _gameState.DockedPlanet.D) * 4 * _draw.Graphics.Scale) + _draw.Centre.X,
        ((_gameState.HyperspacePlanet.B - _gameState.DockedPlanet.B) * 2 * _draw.Graphics.Scale) + _draw.Centre.Y);

    /// <summary>
    /// Move the planet chart cross hairs to specified position.
    /// </summary>
    private void MoveCross(int dx, int dy)
    {
        _crossTimer = 5;
        _gameState.Cross = new(Math.Clamp(_gameState.Cross.X + (dx * 4), 1, 510), Math.Clamp(_gameState.Cross.Y + (dy * 4), 37, 339));
    }
}
