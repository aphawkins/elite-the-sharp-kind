// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls;

/// <summary>
/// Replays a <see cref="KeyScriptEvent"/> timeline into a real
/// <see cref="IKeyboardSink"/>, one tick at a time - the production
/// counterpart to the headless test harnesses' scripted <c>FakeKeyboard</c>
/// driving, for the rare live SDL run that needs reproducible input instead
/// of a human at the keyboard. <see cref="BeforeUpdate"/> runs before the
/// game's Update (so held/tapped keys are visible to it) and
/// <see cref="AfterUpdate"/> runs after (so single-tick taps release again),
/// mirroring <c>HeadlessGameHarnessBase.Step</c>.
/// </summary>
public sealed class KeyScriptPlayer
{
    private readonly IKeyboardSink _sink;
    private readonly IReadOnlyList<KeyScriptEvent> _script;
    private readonly List<(ConsoleKey Key, ConsoleModifiers Modifiers)> _pendingTaps = [];
    private bool _saveFramePending;

    public KeyScriptPlayer(IKeyboardSink sink, IReadOnlyList<KeyScriptEvent> script)
    {
        Guard.ArgumentNull(sink);
        Guard.ArgumentNull(script);

        _sink = sink;
        _script = script;
    }

    public int Tick { get; private set; }

    public void BeforeUpdate()
    {
        foreach (KeyScriptEvent scriptEvent in _script)
        {
            if (scriptEvent.Tick != Tick)
            {
                continue;
            }

            switch (scriptEvent.Action)
            {
                case KeyScriptAction.Tap:
                    _sink.KeyDown(scriptEvent.Key, scriptEvent.Modifiers);
                    _pendingTaps.Add((scriptEvent.Key, scriptEvent.Modifiers));
                    break;

                case KeyScriptAction.Hold:
                    _sink.KeyDown(scriptEvent.Key, scriptEvent.Modifiers);
                    break;

                case KeyScriptAction.Release:
                    _sink.KeyUp(scriptEvent.Key, scriptEvent.Modifiers);
                    break;

                case KeyScriptAction.SaveFrame:
                    _saveFramePending = true;
                    break;
            }
        }
    }

    /// <summary>
    /// Releases any keys tapped this tick and advances to the next tick.
    /// Returns whether a SaveFrame command fired this tick.
    /// </summary>
    public bool AfterUpdate()
    {
        foreach ((ConsoleKey key, ConsoleModifiers modifiers) in _pendingTaps)
        {
            _sink.KeyUp(key, modifiers);
        }

        _pendingTaps.Clear();
        Tick++;

        bool saveFrame = _saveFramePending;
        _saveFramePending = false;
        return saveFrame;
    }
}
