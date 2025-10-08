// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Graphics;
using Useful.Audio;

namespace EliteSharp.Views;

internal sealed class HyperspaceView : IView
{
    private readonly AudioController _audio;
    private readonly BreakPattern _breakPattern;
    private readonly GameState _gameState;

    internal HyperspaceView(GameState gameState, AudioController audio, IEliteDraw draw)
    {
        _gameState = gameState;
        _audio = audio;
        _breakPattern = new(draw);
    }

    public void Draw() => _breakPattern.Draw();

    public void HandleInput()
    {
    }

    public void Reset()
    {
        _breakPattern.Reset();
        _audio.PlayEffect((int)SoundEffect.Hyperspace);
    }

    public void UpdateUniverse()
    {
        _breakPattern.Update();

        if (_breakPattern.IsComplete)
        {
            _gameState.SetView(Screen.FrontView);
        }
    }
}
