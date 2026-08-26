// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Ships;

namespace EliteSharpLib.Conflict;

/// <summary>
/// The laser's own state between shots: how hot it is, and how long until it
/// can fire again.
/// </summary>
/// <remarks>
/// Split from the main partial on 2026-08-26 only because converting these
/// two to rates took the file past the length the repo enforces. They belong
/// together either way - both count down towards the next shot.
/// </remarks>
internal sealed partial class Combat
{
    internal void CoolLaser()
    {
        _laserStrength = 0;
        _laserType = LaserType.None;
        _gameState.DrawLasers = false;

        float ticks = _gameState.Clock.Ticks;

        if (_gameState.LaserTemp > GameState.LaserTempMin)
        {
            _gameState.LaserTemp = Math.Max(
                _gameState.LaserTemp - (GameState.LaserTempStep * ticks),
                GameState.LaserTempMin);
        }

        // Counts down two a tick to the moment the laser can fire again.
        _laserCounter = Math.Max(_laserCounter - (2 * ticks), 0);
    }
}
