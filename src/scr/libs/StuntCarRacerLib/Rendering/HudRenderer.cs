// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Globalization;
using Useful;
using Useful.Graphics;

namespace StuntCarRacerLib.Rendering;

// The in-game dashboard from the original Amiga game (StuntCarRacer.s):
// the chassis beam along the top of the screen carrying the damage crack
// (damage.line), the speed bar (display.speed.bar) and the lap ("L"),
// boost ("B") and opponent-distance read-outs on the bottom panel
// (print.lap.boost.text and friends). The original beam and panel artwork
// was bitmap data on the Amiga disk, so the panels here are flat shapes,
// but the layout and behaviour follow the assembler source. Layout is in
// the original's 320x200 coordinates, scaled to the screen.
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Security",
    "CA5394:Do not use insecure randomness",
    Justification = "Gameplay randomness only (the damage crack's random walk).")]
internal sealed class HudRenderer
{
    // The crack length at which the original wrecked the car ($f0).
    internal const int MaxCrackLength = 240;

    private const float AmigaWidth = 320f;
    private const float AmigaHeight = 200f;

    private readonly IGraphics _graphics;
    private readonly Random _random;

    // One entry per crack pixel: the row the random walk reached and
    // whether it moved down (which picks the other highlight colour).
    private readonly List<(int Row, bool MovedDown)> _crack = new(MaxCrackLength);

    private int _crackRow = 4;

    internal HudRenderer(IGraphics graphics)
        : this(graphics, new Random())
    {
    }

    internal HudRenderer(IGraphics graphics, Random random)
    {
        Guard.ArgumentNull(graphics);
        Guard.ArgumentNull(random);

        _graphics = graphics;
        _random = random;
    }

    internal IReadOnlyList<(int Row, bool MovedDown)> Crack => _crack;

    // The speed bar position from the original display.speed.bar: 183/256
    // of the speed above $1100, in 128ths. The original subtracted 128
    // rather than clamping, wrapping the bar around at very high speed.
    internal static int SpeedBarLength(int playerZSpeed)
    {
        int length = playerZSpeed - 0x1100;
        if (length < 0)
        {
            length = 0;
        }

        length = (int)(((long)length * 0xB700) >> 16) >> 7;
        if (length >= 128)
        {
            length -= 128;
        }

        return Math.Clamp(length, 0, 128);
    }

    internal void Draw(int lapNumber, int boostReserve, int newDamage, int playerZSpeed, int opponentDistance)
    {
        UpdateCrack(newDamage);

        float sx = _graphics.ScreenWidth / AmigaWidth;
        float sy = _graphics.ScreenHeight / AmigaHeight;

        DrawBeam(sx, sy);
        DrawPanel(sy);
        DrawSpeedBar(sx, sy, playerZSpeed);
        DrawReadouts(sx, sy, lapNumber, boostReserve, opponentDistance);
    }

    // Advance the crack to match the damage (the original damage.line
    // redrew incrementally into a persistent framebuffer; here the crack
    // path is remembered and redrawn each frame). Damage dropping below
    // the crack means a new race, so the crack starts over.
    private void UpdateCrack(int newDamage)
    {
        if (newDamage < _crack.Count)
        {
            _crack.Clear();
            _crackRow = 4;
        }

        while (_crack.Count < Math.Min(newDamage, MaxCrackLength))
        {
            AdvanceCrack();
        }
    }

    // One pixel of crack (the original dl2-dl7): a random walk that only
    // wanders on every other pixel, biased to stay within the beam.
    private void AdvanceCrack()
    {
        int drawn = _crack.Count + 1;
        int row = _crackRow;

        if ((drawn & 1) == 0 && RandomBit())
        {
            if (RandomBit())
            {
                if (row < 5)
                {
                    row++;
                }
                else if (!RandomBit())
                {
                    row = StepUp(row);
                }

                // else subq then addq: stays put
            }
            else
            {
                row = StepUp(row);
            }
        }

        bool movedDown = row > _crackRow;
        _crackRow = row;
        _crack.Add((row, movedDown));
    }

    // The original dl4/dl5: move up a row, unless already near the top of
    // the beam where the walk hesitates.
    private int StepUp(int row) => row >= 3 ? row - 1 : (RandomBit() ? row : row + 1);

    private bool RandomBit() => _random.Next(2) != 0;

    // The chassis beam across the top of the screen with the damage crack:
    // two black pixels per column with a highlight pixel above, at
    // x = 40 + damage (the original plotted at rows row, row-1 and row-2).
    private void DrawBeam(float sx, float sy)
    {
        uint beam = ScrPalette.Colour(Tracks.Track.ScrBaseColour + 14);
        uint shadow = ScrPalette.Colour(Tracks.Track.ScrBaseColour + 13);
        uint black = ScrPalette.Colour(Tracks.Track.ScrBaseColour);

        _graphics.DrawRectangleFilled(new(0, 0), _graphics.ScreenWidth, 8 * sy, beam);
        _graphics.DrawRectangleFilled(new(0, 8 * sy), _graphics.ScreenWidth, sy, shadow);

        for (int i = 0; i < _crack.Count; i++)
        {
            (int row, bool movedDown) = _crack[i];
            float x = (41 + i) * sx;
            uint highlight = ScrPalette.Colour(Tracks.Track.ScrBaseColour + (movedDown ? 12 : 11));

            _graphics.DrawRectangleFilled(new(x, (row - 2) * sy), sx, sy, highlight);
            _graphics.DrawRectangleFilled(new(x, (row - 1) * sy), sx, 2 * sy, black);
        }
    }

    // The dashboard panel across the bottom of the screen (the read-outs
    // and speed bar sat on the cockpit artwork from y=170 down).
    private void DrawPanel(float sy)
    {
        uint panel = ScrPalette.Colour(Tracks.Track.ScrBaseColour + 8);
        uint edge = ScrPalette.Colour(Tracks.Track.ScrBaseColour + 14);

        _graphics.DrawRectangleFilled(new(0, 170 * sy), _graphics.ScreenWidth, 30 * sy, panel);
        _graphics.DrawRectangleFilled(new(0, 170 * sy), _graphics.ScreenWidth, sy, edge);
    }

    // The speed bar at (96,174), up to 128 pixels long and two rows tall.
    private void DrawSpeedBar(float sx, float sy, int playerZSpeed)
    {
        uint black = ScrPalette.Colour(Tracks.Track.ScrBaseColour);
        uint white = ScrPalette.Colour(Tracks.Track.ScrBaseColour + 15);

        _graphics.DrawRectangleFilled(new(96 * sx, 174 * sy), 128 * sx, 2 * sy, black);

        int length = SpeedBarLength(playerZSpeed);
        if (length > 0)
        {
            _graphics.DrawRectangleFilled(new(96 * sx, 174 * sy), length * sx, 2 * sy, white);
        }
    }

    // "L <lap>" and "B <boost>" on character row 22 and the signed
    // opponent distance on row 23, as the original print.lap.boost.text,
    // boost.print and display.opponent.distance laid them out.
    private void DrawReadouts(float sx, float sy, int lapNumber, int boostReserve, int opponentDistance)
    {
        const string font = StuntCarRacerMain.SmallFont;
        uint white = ScrPalette.Colour(Tracks.Track.ScrBaseColour + 15);

        string lap = lapNumber > 0 ? lapNumber.ToString(CultureInfo.InvariantCulture) : " ";
        _graphics.DrawTextLeft(new(42 * sx, 178 * sy), "L", font, white);
        _graphics.DrawTextLeft(new(50 * sx, 178 * sy), lap, font, white);

        string boost = Math.Clamp(boostReserve, 0, 99).ToString("D2", CultureInfo.InvariantCulture);
        _graphics.DrawTextLeft(new(66 * sx, 178 * sy), "B", font, white);
        _graphics.DrawTextLeft(new(76 * sx, 178 * sy), boost, font, white);

        // the original printed a space or '-' then four digits
        string sign = opponentDistance < 0 ? "-" : " ";
        string distance = Math.Clamp(Math.Abs(opponentDistance), 0, 9999).ToString("D4", CultureInfo.InvariantCulture);
        _graphics.DrawTextLeft(new(49 * sx, 188 * sy), sign + distance, font, white);
    }
}
