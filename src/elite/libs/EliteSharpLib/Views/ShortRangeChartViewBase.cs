// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using EliteSharpLib.Types;
using Useful.Controls;

namespace EliteSharpLib.Views;

/// <summary>
/// The short-range chart's shared behaviour: input, the planet layout pass and
/// the distance maths. Drawing is deliberately not here - each tier's subclass
/// owns its own <see cref="Draw"/>, since nothing that puts pixels on screen is
/// shared between tiers.
/// </summary>
/// <remarks>
/// This screen is still one class rather than the controller/model/view split
/// the other screens use: its layout pass emits positioned, named planets
/// rather than data, and blob size depends on <c>GameState.CarryFlag</c> as a
/// side effect of the <c>NamePlanet</c> call. See docs/backlog-roadmap.md.
/// </remarks>
internal abstract class ShortRangeChartViewBase(
    GameState gameState,
    IEliteDraw draw,
    IBaseView baseView,
    IKeyboard keyboard,
    PlanetController planet,
    PlayerShip ship) : IScreenController
{
    private readonly IKeyboard _keyboard = keyboard;
    private readonly PlanetController _planet = planet;

    private int _crossTimer;

    protected IBaseView BaseView { get; } = baseView;

    protected Vector2 Cross { get; private set; }

    protected IEliteDraw EliteDraw { get; } = draw;

    protected string FindName { get; private set; } = string.Empty;

    protected GameState GameState { get; } = gameState;

    protected bool IsFind { get; private set; }

    protected List<(Vector2 Position, string Name)> PlanetNames { get; } = [];

    protected List<(Vector2 Position, float Size)> PlanetSizes { get; } = [];

    protected PlayerShip Ship { get; } = ship;

    /// <summary>
    /// Gets the inclusive bounds the cross hairs may be moved within. Screen
    /// coordinates, so each tier states its own.
    /// </summary>
    protected abstract (float MinX, float MaxX, float MinY, float MaxY) CrossBounds { get; }

    public abstract void Draw();

    public void HandleInput()
    {
        if (IsFind)
        {
            HandleFindInput();
            return;
        }

        if (_keyboard.IsPressed(ConsoleKey.O))
        {
            Cross = EliteDraw.Layout.ViewportCentre;
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
            IsFind = true;
            FindName = string.Empty;
            _keyboard.ClearPressed();  // Clear the F so that it doesn't appear in the find word
        }
    }

    public void Reset()
    {
        IsFind = false;
        FindName = string.Empty;
        int[] row_used = new int[64];
        PlanetNames.Clear();
        PlanetSizes.Clear();

        for (int i = 0; i < 64; i++)
        {
            row_used[i] = 0;
        }

        GalaxySeed glx = new(GameState.Cmdr.Galaxy);
        float scale = EliteDraw.Layout.Scale;
        Vector2 centre = EliteDraw.Layout.ViewportCentre;

        for (int i = 0; i < 256; i++)
        {
            float dx = MathF.Abs(glx.D - GameState.DockedPlanet.D);
            float dy = MathF.Abs(glx.B - GameState.DockedPlanet.B);

            if ((dx >= 20) || (dy >= 38))
            {
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);

                continue;
            }

            float px = glx.D - GameState.DockedPlanet.D;

            // Convert to screen co-ords
            px = (px * 4 * scale) + centre.X;

            float py = glx.B - GameState.DockedPlanet.B;

            // Convert to screen co-ords
            py = (py * 2 * scale) + centre.Y;

            int row = (int)(py / (8 * scale));

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
                PlanetNames.Add((
                    new(px + (4 * scale), ((row * 8) - 5) * scale),
                    _planet.NamePlanet(glx)
                        .CapitaliseFirstLetter()));
            }

            // The next bit calculates the size of the circle used to represent
            // a planet.  The carry_flag is left over from the name generation.
            // Yes this was how it was done... don't ask :-(
            float blob_size = (glx.F & 1) + 2 + GameState.CarryFlag;
            blob_size *= scale;
            PlanetSizes.Add((new(px, py), blob_size));

            _planet.WaggleGalaxy(glx);
            _planet.WaggleGalaxy(glx);
            _planet.WaggleGalaxy(glx);
            _planet.WaggleGalaxy(glx);
        }

        _crossTimer = 0;
        CrossFromHyperspacePlanet();
        CalculateDistanceToPlanet();
    }

    public void Update()
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

    // Typing a planet name into the find prompt.
    private void HandleFindInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.Backspace) &&
            !string.IsNullOrEmpty(FindName))
        {
            FindName = FindName[..^1];
        }

        if (_keyboard.IsPressed(ConsoleKey.Enter))
        {
            IsFind = false;
            if (_planet.FindPlanetByName(FindName))
            {
                CrossFromHyperspacePlanet();
                CalculateDistanceToPlanet();
            }
            else
            {
                GameState.PlanetName = string.Empty;
            }
        }

        (ConsoleKey key, ConsoleModifiers _) = _keyboard.LastPressed();
        if (key is >= ConsoleKey.A and <= ConsoleKey.Z)
        {
            FindName += (char)key;
        }
    }

    private void CalculateDistanceToPlanet()
    {
        float scale = EliteDraw.Layout.Scale;
        Vector2 centre = EliteDraw.Layout.ViewportCentre;
        Vector2 location = new()
        {
            X = ((Cross.X - centre.X) / (4 * scale)) + GameState.DockedPlanet.D,
            Y = ((Cross.Y - centre.Y) / (2 * scale)) + GameState.DockedPlanet.B,
        };

        GameState.HyperspacePlanet = _planet.FindPlanet(GameState.Cmdr.Galaxy, location);
        GameState.PlanetName = _planet.NamePlanet(GameState.HyperspacePlanet);
        GameState.DistanceToPlanet = PlanetController.CalculateDistanceToPlanet(GameState.DockedPlanet, GameState.HyperspacePlanet);
        CrossFromHyperspacePlanet();
    }

    private void CrossFromHyperspacePlanet() => Cross = new(
        ((GameState.HyperspacePlanet.D - GameState.DockedPlanet.D) * 4 * EliteDraw.Layout.Scale) + EliteDraw.Layout.ViewportCentre.X,
        ((GameState.HyperspacePlanet.B - GameState.DockedPlanet.B) * 2 * EliteDraw.Layout.Scale) + EliteDraw.Layout.ViewportCentre.Y);

    /// <summary>
    /// Move the planet chart cross hairs to specified position.
    /// </summary>
    private void MoveCross(int dx, int dy)
    {
        _crossTimer = 5;
        (float minX, float maxX, float minY, float maxY) = CrossBounds;
        Cross = new(
            Math.Clamp(Cross.X + (dx * 4), minX, maxX),
            Math.Clamp(Cross.Y + (dy * 4), minY, maxY));
    }
}
