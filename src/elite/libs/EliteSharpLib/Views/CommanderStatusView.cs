// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Equipment;
using EliteSharpLib.Graphics;
using EliteSharpLib.Lasers;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;

namespace EliteSharpLib.Views;

internal sealed class CommanderStatusView : IView
{
    private readonly string[] _conditionText =
    [
        "Docked",
        "Green",
        "Yellow",
        "Red",
    ];

    private readonly IEliteDraw _draw;
    private readonly int _equipmentMaxY = 290;
    private readonly int _equipmentStartY = 202;
    private readonly int _equipmentWidth = 200;
    private readonly GameState _gameState;
    private readonly PlanetController _planet;
    private readonly uint _colorGreen;
    private readonly uint _colorWhite;

    private readonly (int Score, string Title)[] _ratings =
    [
        new(0x0000, "Harmless"),
        new(0x0008, "Mostly Harmless"),
        new(0x0010, "Poor"),
        new(0x0020, "Average"),
        new(0x0040, "Above Average"),
        new(0x0080, "Competent"),
        new(0x0200, "Dangerous"),
        new(0x0A00, "Deadly"),
        new(0x1900, "- - - E L I T E - - -"),
    ];

    private readonly PlayerShip _ship;
    private readonly int _spacingY = 16;
    private readonly Trade _trade;
    private readonly Universe _universe;

    internal CommanderStatusView(
        GameState gameState,
        IEliteDraw draw,
        PlayerShip ship,
        Trade trade,
        PlanetController planet,
        Universe universe)
    {
        _gameState = gameState;
        _draw = draw;
        _ship = ship;
        _trade = trade;
        _planet = planet;
        _universe = universe;

        _colorGreen = draw.Palette["Green"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw()
    {
        DrawStatus(CurrentRating(), CurrentCondition());
        DrawEquipment();
    }

    public void HandleInput()
    {
    }

    public void Reset()
    {
    }

    public void Update()
    {
    }

    // The commander's combat rating, the highest one their score has reached.
    private string CurrentRating()
    {
        string rating = string.Empty;
        foreach ((int score, string title) in _ratings)
        {
            if (_gameState.Cmdr.Score >= score)
            {
                rating = title;
            }
        }

        return rating;
    }

    // Docked / Green / Yellow / Red, as an index into _conditionText.
    private int CurrentCondition()
    {
        if (_gameState.IsDocked)
        {
            return 0;
        }

        int condition = 1;

        foreach (IObject obj in _universe.GetAllObjects())
        {
            if (obj.Type is ShipType.Missile or (> ShipType.Rock and < ShipType.Dodec))
            {
                condition = 2;
                break;
            }
        }

        if (condition == 2 && _ship.Energy < 128)
        {
            condition = 3;
        }

        return condition;
    }

    private void DrawStatus(string rating, int condition)
    {
        _draw.DrawViewHeader($"COMMANDER {_gameState.Cmdr.Name}");

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 58), "Present System:", nameof(FontType.Small), _colorGreen);

        if (!_gameState.InWitchspace)
        {
            _draw.Graphics.DrawTextLeft(
                new(200 + _draw.Offset, 58),
                _planet.NamePlanet(_gameState.DockedPlanet).CapitaliseFirstLetter(),
                nameof(FontType.Small),
                _colorWhite);
        }

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 74), "Hyperspace System:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(
            new(200 + _draw.Offset, 74),
            _planet.NamePlanet(_gameState.HyperspacePlanet).CapitaliseFirstLetter(),
            nameof(FontType.Small),
            _colorWhite);

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 90), "Condition:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(200 + _draw.Offset, 90), _conditionText[condition], nameof(FontType.Small), _colorWhite);

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 106), "Fuel:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics
            .DrawTextLeft(new(200 + _draw.Offset, 106), $"{_ship.Fuel:N1} Light Years", nameof(FontType.Small), _colorWhite);

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 122), "Cash:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics
            .DrawTextLeft(new(200 + _draw.Offset, 122), $"{_trade.Credits:N1} Credits", nameof(FontType.Small), _colorWhite);

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 138), "Legal Status:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(
            new(200 + _draw.Offset, 138),
            _gameState.Cmdr.LegalStatus == 0 ? "Clean" : _gameState.Cmdr.LegalStatus > 50 ? "Fugitive" : "Offender",
            nameof(FontType.Small),
            _colorWhite);

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 154), "Rating:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(200 + _draw.Offset, 154), rating, nameof(FontType.Small), _colorWhite);

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 186), "EQUIPMENT:", nameof(FontType.Small), _colorGreen);
    }

    // The equipment list, filling the left column before wrapping to the right.
    private void DrawEquipment()
    {
        Vector2 position = new(50 + _draw.Offset, _equipmentStartY);

        foreach (string item in EquipmentFitted())
        {
            _draw.Graphics.DrawTextLeft(position, item, nameof(FontType.Small), _colorWhite);

            position.Y += _spacingY;
            if (position.Y > _equipmentMaxY)
            {
                position.Y = _equipmentStartY;
                position.X += _equipmentWidth;
            }
        }
    }

    // Every piece of equipment the ship is carrying, in display order.
    private IEnumerable<string> EquipmentFitted()
    {
        if (_ship.CargoCapacity > 20)
        {
            yield return "Large Cargo Bay";
        }

        if (_ship.HasEscapeCapsule)
        {
            yield return "Escape Capsule";
        }

        if (_ship.HasFuelScoop)
        {
            yield return "Fuel Scoops";
        }

        if (_ship.HasECM)
        {
            yield return "E.C.M. System";
        }

        if (_ship.HasEnergyBomb)
        {
            yield return "Energy Bomb";
        }

        if (_ship.EnergyUnit != EnergyUnit.None)
        {
            yield return _ship.EnergyUnit == EnergyUnit.Extra ? "Extra Energy Unit" : "Naval Energy Unit";
        }

        if (_ship.HasDockingComputer)
        {
            yield return "Docking Computers";
        }

        if (_ship.HasGalacticHyperdrive)
        {
            yield return "Galactic Hyperspace";
        }

        if (_ship.LaserFront.Type != LaserType.None)
        {
            yield return $"Front {_ship.LaserFront.Name} Laser";
        }

        if (_ship.LaserRear.Type != LaserType.None)
        {
            yield return $"Rear {_ship.LaserRear.Name} Laser";
        }

        if (_ship.LaserLeft.Type != LaserType.None)
        {
            yield return $"Left {_ship.LaserLeft.Name} Laser";
        }

        if (_ship.LaserRight.Type != LaserType.None)
        {
            yield return $"Right {_ship.LaserRight.Name} Laser";
        }
    }
}
