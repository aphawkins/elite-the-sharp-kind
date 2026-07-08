// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Globalization;
using Useful;
using Useful.Graphics;

namespace StuntCarRacerLib.Rendering;

// The in-game cockpit overlay, ported from the ptitSeb/stuntcarremake
// DrawCockpit: the wheel-well/dashboard frame, front wheel sprites that
// bounce with the suspension and spin with road speed, the engine (with a
// boost flame animation), the damage crack image revealed progressively by
// damage, smash holes, the speed bar, and the lap/boost/opponent-distance
// read-outs. All sprites come from the single Atlas image (converted
// one-time from the original's Bitmap/atlas.png); layout is in the
// original's 640x480 standard-resolution coordinate space, scaled to the
// screen. The Super League ("2"-suffixed) atlas variants are not used yet.
internal sealed class HudRenderer
{
    private const string Atlas = "Atlas";

    private const float BaseWidth = 640f;
    private const float BaseHeight = 480f;

    private const int WheelWidth = 24;
    private const int WheelHeight = 56;
    private const int HoleWidth = 12;
    private const int HoleHeight = 8;
    private const int CrackingWidth = 238;
    private const int CrackingHeight = 8;

    // Cockpit layout, in the 640x480 virtual canvas (original DrawCockpit,
    // non-widescreen).
    private const float WheelLeftOffset = 31f;
    private const float WheelBottomGap = 20f;
    private const float EngineDestX = 84f;
    private const float EngineDestWidth = 235f * 2f;
    private const float EngineDestY = 123f * 2.4f;
    private const float EngineDestHeight = 35f * 2.4f;
    private const float TopDestX = 41f * 2f;
    private const float TopDestWidth = 238f * 2f;
    private const float TopDestHeight = 16f * 2.4f;
    private const float SideDestHeight = 153f * 2.4f;
    private const float DamageDestHeight = 8f * 2.4f;
    private const float HoleDestX = 47f * 2f;
    private const float HoleDestSpacing = 24f * 2f;
    private const float SpeedBarDestX = 196f;
    private const float SpeedBarDestY = BaseHeight - 61f;
    private const float SpeedBarDestWidth = 242f;
    private const float SpeedBarDestHeight = 3f;
    private const int SpeedBarMax = 240;
    private const float RightWheelDestX = BaseWidth - (WheelLeftOffset * 2f) - (WheelWidth * 2f);
    private const float FrameRightDestX = TopDestX + TopDestWidth;
    private const float BottomDestHeight = BaseHeight - SideDestHeight;

    // Amiga StuntCarRacer 1024x1024 atlas coordinates (InitAtlasCoord,
    // Windows/DirectX branch: top-down, matching our BMP loader).
    private static readonly int[] s_wheelSourceX = [160, 128, 96, 64, 32, 0];
    private static readonly AtlasRect s_hole = new(0, 64, HoleWidth, HoleHeight);
    private static readonly AtlasRect s_cracking = new(0, 128, CrackingWidth, CrackingHeight);
    private static readonly AtlasRect s_cockpitTop = new(41, 160, 238, 16);
    private static readonly AtlasRect s_cockpitLeft = new(0, 160, 41, 153);
    private static readonly AtlasRect s_cockpitRight = new(279, 160, 41, 153);
    private static readonly AtlasRect s_cockpitBottom = new(0, 313, 320, 47);
    private static readonly AtlasRect s_engine = new(42, 592, 235, 35);
    private static readonly AtlasRect s_engineFlames0 = new(42, 784, 235, 35);
    private static readonly AtlasRect s_engineFlames1 = new(42, 976, 235, 35);
    private static readonly AtlasRect s_engineFlames2 = new(362, 123, 235, 35);

    // The engine flame cycles through these frames every other tick while
    // boost is applied (the original's engineframes[8] table).
    private static readonly int[] s_engineFlameFrames = [0, 0, 0, 1, 2, 2, 2, 1];

    private readonly IGraphics _graphics;

    private int _engineFlameTick;

    internal HudRenderer(IGraphics graphics)
    {
        Guard.ArgumentNull(graphics);
        _graphics = graphics;
    }

    internal void Draw(in CockpitState state)
    {
        float scaleX = _graphics.ScreenWidth / BaseWidth;
        float scaleY = _graphics.ScreenHeight / BaseHeight;

        DrawWheel(scaleX, scaleY, WheelLeftOffset * 2f, mirrored: false, state.LeftWheelFrame, state.LeftWheelBounce);
        DrawWheel(scaleX, scaleY, RightWheelDestX, mirrored: true, state.RightWheelFrame, state.RightWheelBounce);

        DrawEngine(scaleX, scaleY, state.BoostActivated);
        DrawFrame(scaleX, scaleY);
        DrawDamage(scaleX, scaleY, state.NewDamage, state.SmashHoles);
        DrawSpeedBar(scaleX, scaleY, state.DisplaySpeed);
        DrawReadouts(scaleX, scaleY, state.LapNumber, state.BoostReserve, state.OpponentDistance);
    }

    private void DrawWheel(float scaleX, float scaleY, float destX, bool mirrored, int frame, int bounce)
    {
        float destY1 = BaseHeight - (WheelHeight * 2.4f) - (WheelBottomGap * 2.4f) - bounce;
        float destY2 = BaseHeight - (WheelBottomGap * 2.4f) - bounce;
        AtlasRect source = new(s_wheelSourceX[frame], 0, WheelWidth, WheelHeight);

        DrawSprite(scaleX, scaleY, destX, destY1, WheelWidth * 2f, destY2 - destY1, source, mirrored);
    }

    private void DrawEngine(float scaleX, float scaleY, bool boostActivated)
    {
        AtlasRect source = s_engine;
        if (boostActivated)
        {
            _engineFlameTick = (_engineFlameTick + 1) % 16;
            int flame = s_engineFlameFrames[_engineFlameTick >> 1];
            source = flame switch
            {
                0 => s_engineFlames0,
                1 => s_engineFlames1,
                _ => s_engineFlames2,
            };
        }

        DrawSprite(scaleX, scaleY, EngineDestX, EngineDestY, EngineDestWidth, EngineDestHeight, source);
    }

    // The cockpit frame: top bar, both side pillars and the bottom
    // dashboard, drawn around the (transparent) view of the world.
    private void DrawFrame(float scaleX, float scaleY)
    {
        DrawSprite(scaleX, scaleY, TopDestX, 0, TopDestWidth, TopDestHeight, s_cockpitTop);
        DrawSprite(scaleX, scaleY, 0, 0, TopDestX, SideDestHeight, s_cockpitLeft);
        DrawSprite(scaleX, scaleY, FrameRightDestX, 0, BaseWidth - FrameRightDestX, SideDestHeight, s_cockpitRight);
        DrawSprite(scaleX, scaleY, 0, SideDestHeight, BaseWidth, BottomDestHeight, s_cockpitBottom);
    }

    // The damage crack image is revealed progressively (not stretched): the
    // sampled source width grows with damage at a constant 2x scale, so it
    // looks like more of the crack appearing rather than the art warping.
    // Smash holes appear side by side once damage passes the smash threshold.
    private void DrawDamage(float scaleX, float scaleY, int newDamage, int smashHoles)
    {
        int dam = Math.Clamp(newDamage, 0, CrackingWidth);
        if (dam > 0)
        {
            AtlasRect crack = s_cracking with { Width = dam };
            DrawSprite(scaleX, scaleY, TopDestX, 0, dam * 2f, DamageDestHeight, crack);
        }

        for (int i = 0; i < smashHoles; i++)
        {
            float holeDestX = HoleDestX + (HoleDestSpacing * i);
            DrawSprite(scaleX, scaleY, holeDestX, 0, HoleWidth * 2f, DamageDestHeight, s_hole);
        }
    }

    // The dashboard speed bar: yellow up to 240 display-speed units, then
    // orange for the wrapped remainder above that (the original's
    // COCKPIT_SPEEDBAR_MAX wrap, not a clamp).
    private void DrawSpeedBar(float scaleX, float scaleY, int displaySpeed)
    {
        int length = displaySpeed > SpeedBarMax ? displaySpeed - SpeedBarMax : displaySpeed;
        float width = Math.Clamp(length / (float)SpeedBarMax, 0f, 1f) * SpeedBarDestWidth;
        if (width <= 0)
        {
            return;
        }

        uint colour = displaySpeed > SpeedBarMax ? 0xFFFFCC00 : 0xFFFFFF00;
        System.Numerics.Vector2 position = new(SpeedBarDestX * scaleX, SpeedBarDestY * scaleY);
        _graphics.DrawRectangleFilled(position, width * scaleX, SpeedBarDestHeight * scaleY, colour);
    }

    // "L <lap>" / "B <boost>" and the signed opponent distance, printed in
    // black onto the dashboard's read-out panels (print.lap.boost.text
    // and friends, at the original's screen positions).
    private void DrawReadouts(float scaleX, float scaleY, int lapNumber, int boostReserve, int opponentDistance)
    {
        const string font = StuntCarRacerMain.SmallFont;
        const uint black = 0xFF000000;

        string lap = lapNumber > 0 ? lapNumber.ToString(CultureInfo.InvariantCulture) : " ";
        _graphics.DrawTextLeft(new(88 * scaleX, (BaseHeight - 48f) * scaleY), $"L{lap}", font, black);

        string boost = Math.Clamp(boostReserve, 0, 99).ToString("D2", CultureInfo.InvariantCulture);
        _graphics.DrawTextLeft(new(150 * scaleX, (BaseHeight - 48f) * scaleY), $"B{boost}", font, black);

        string sign = opponentDistance < 0 ? "-" : "+";
        string distance = Math.Clamp(Math.Abs(opponentDistance), 0, 9999).ToString("D4", CultureInfo.InvariantCulture);
        _graphics.DrawTextLeft(new(84 * scaleX, (BaseHeight - 25f) * scaleY), sign + distance, font, black);
    }

    private void DrawSprite(
        float scaleX, float scaleY, float destX, float destY, float destWidth, float destHeight, in AtlasRect source, bool mirrored = false)
        => _graphics.DrawImagePart(
            Atlas,
            new(destX * scaleX, destY * scaleY),
            new(destWidth * scaleX, destHeight * scaleY),
            new(source.X, source.Y),
            new(mirrored ? -source.Width : source.Width, source.Height));

    private readonly record struct AtlasRect(int X, int Y, int Width, int Height);
}
