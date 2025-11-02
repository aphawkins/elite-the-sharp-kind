// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Audio;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using Useful.Audio;

namespace EliteSharpLib.Views;

internal sealed class DockingView : IView
{
    private readonly AudioController _audio;
    private readonly BreakPattern _breakPattern;
    private readonly Combat _combat;
    private readonly GameState _gameState;
    private readonly Space _space;
    private readonly Universe _universe;

    internal DockingView(GameState gameState, AudioController audio, Space space, Combat combat, Universe universe, IEliteDraw draw)
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
        _audio.PlayEffect((int)SoundEffect.Dock);
    }

    public void UpdateUniverse()
    {
        _breakPattern.Update();

        if (_breakPattern.IsComplete)
        {
            _space.DockPlayer();
            _gameState.SetView(Screen.MissionOne);
        }
    }
}
