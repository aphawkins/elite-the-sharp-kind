// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Conflict;
using EliteSharp.Graphics;

namespace EliteSharp.Views;

internal sealed class LaunchView : IView
{
    private readonly AudioController _audio;
    private readonly BreakPattern _breakPattern;
    private readonly Combat _combat;
    private readonly GameState _gameState;
    private readonly Space _space;
    private readonly Universe _universe;

    internal LaunchView(GameState gameState, AudioController audio, Space space, Combat combat, Universe universe, IDraw draw)
    {
        _gameState = gameState;
        _audio = audio;
        _space = space;
        _combat = combat;
        _universe = universe;
        _breakPattern = new(draw);
    }

    public void Draw() => _breakPattern.Draw();

    public void HandleInput()
    {
    }

    public void Reset()
    {
        _combat.Reset();
        _universe.ClearUniverse();
        _breakPattern.Reset();
        _audio.PlayEffect(SoundEffect.Launch);
    }

    public void UpdateUniverse()
    {
        _breakPattern.Update();

        if (_breakPattern.IsComplete)
        {
            _space.LaunchPlayer();
            _gameState.SetView(Screen.FrontView);
        }
    }
}
