// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using EliteSharp.Types;
using Useful.Controls;

namespace EliteSharp.Views;

internal sealed class GalacticChartView : IView
{
    private readonly IEliteDraw _draw;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly PlanetController _planet;
    private readonly List<Vector2> _planetPixels = [];
    private readonly PlayerShip _ship;
    private int _crossTimer;
    private string _findName = string.Empty;
    private bool _isFind;

    internal GalacticChartView(GameState gameState, IEliteDraw draw, IKeyboard keyboard, PlanetController planet, PlayerShip ship)
    {
        _gameState = gameState;
        _draw = draw;
        _keyboard = keyboard;
        _planet = planet;
        _ship = ship;
    }

    public void Draw()
    {
        // Header
        _draw.DrawViewHeader($"GALACTIC CHART {_gameState.Cmdr.GalaxyNumber + 1}");

        _draw.Graphics.DrawLine(new(0 + _draw.Offset, 36 + 258), new(_draw.ScannerRight, 36 + 258), EliteColors.White);

        // Fuel radius
        Vector2 centre = new(
            (_gameState.DockedPlanet.D * _draw.Graphics.Scale) + _draw.Offset,
            (_gameState.DockedPlanet.B / (2 / _draw.Graphics.Scale)) + (18 * _draw.Graphics.Scale) + 1);
        float radius = _ship.Fuel * 2.5f * _draw.Graphics.Scale;
        float cross_size = 7 * _draw.Graphics.Scale;
        _draw.Graphics.DrawCircle(centre, radius, EliteColors.Green);
        _draw.Graphics.DrawLine(new(centre.X, centre.Y - cross_size), new(centre.X, centre.Y + cross_size), EliteColors.White);
        _draw.Graphics.DrawLine(new(centre.X - cross_size, centre.Y), new(centre.X + cross_size, centre.Y), EliteColors.White);

        // Planets
        foreach (Vector2 pixel in _planetPixels)
        {
            _draw.Graphics.DrawPixel(pixel, EliteColors.White);
        }

        // Cross
        centre = new(_gameState.Cross.X + _draw.Offset, _gameState.Cross.Y);

        _draw.Graphics.DrawLine(new(centre.X - 8, centre.Y), new(centre.X + 8, centre.Y), EliteColors.LighterRed);
        _draw.Graphics.DrawLine(new(centre.X, centre.Y - 8), new(centre.X, centre.Y + 8), EliteColors.LighterRed);

        // Text
        if (_isFind)
        {
            _draw.Graphics
                .DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 55), "Planet Name?", (int)FontType.Small, EliteColors.Green);
            _draw.Graphics
                .DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 40), _findName, (int)FontType.Small, EliteColors.White);
        }
        else if (string.IsNullOrEmpty(_gameState.PlanetName))
        {
            _draw.Graphics
                .DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 55), "Unknown Planet", (int)FontType.Small, EliteColors.Green);
            _draw.Graphics
                .DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 40), _findName, (int)FontType.Small, EliteColors.White);
        }
        else
        {
            _draw.Graphics.DrawTextLeft(
                new(16 + _draw.Offset, _draw.ScannerTop - 55),
                _gameState.PlanetName,
                (int)FontType.Small,
                EliteColors.Green);
            if (_gameState.DistanceToPlanet > 0)
            {
                _draw.Graphics.DrawTextLeft(
                    new(16 + _draw.Offset, _draw.ScannerTop - 40),
                    $"Distance: {_gameState.DistanceToPlanet:N1} Light Years ",
                    (int)FontType.Small,
                    EliteColors.White);
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
            _gameState.Cross = new(
                _gameState.DockedPlanet.D * _draw.Graphics.Scale,
                (_gameState.DockedPlanet.B / (2 / _draw.Graphics.Scale)) + (18 * _draw.Graphics.Scale) + 1);
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
        GalaxySeed glx = new(_gameState.Cmdr.Galaxy);
        _planetPixels.Clear();

        for (int i = 0; i < 256; i++)
        {
            Vector2 pixel = new()
            {
                X = (glx.D * _draw.Graphics.Scale) + _draw.Offset,
                Y = (glx.B / (2 / _draw.Graphics.Scale)) + (18 * _draw.Graphics.Scale) + 1,
            };

            _planetPixels.Add(pixel);

            if ((glx.E | 0x50) < 0x90)
            {
                _planetPixels.Add(new(pixel.X + 1, pixel.Y));
            }

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
            X = _gameState.Cross.X / _draw.Graphics.Scale,
            Y = (_gameState.Cross.Y - ((18 * _draw.Graphics.Scale) + 1)) * (2 / _draw.Graphics.Scale),
        };

        _gameState.HyperspacePlanet = _planet.FindPlanet(_gameState.Cmdr.Galaxy, location);
        _gameState.PlanetName = _planet.NamePlanet(_gameState.HyperspacePlanet);
        _gameState.DistanceToPlanet = PlanetController.CalculateDistanceToPlanet(_gameState.DockedPlanet, _gameState.HyperspacePlanet);
        CrossFromHyperspacePlanet();
    }

    private void CrossFromHyperspacePlanet() => _gameState.Cross = new(
        _gameState.HyperspacePlanet.D * _draw.Graphics.Scale,
        (_gameState.HyperspacePlanet.B / (2 / _draw.Graphics.Scale)) + (18 * _draw.Graphics.Scale) + 1);

    /// <summary>
    /// Move the planet chart cross hairs to specified position.
    /// </summary>
    private void MoveCross(int dx, int dy)
    {
        _crossTimer = 5;
        _gameState.Cross = new(Math.Clamp(_gameState.Cross.X + (dx * 2), 1, 510), Math.Clamp(_gameState.Cross.Y + (dy * 2), 37, 293));
    }
}
