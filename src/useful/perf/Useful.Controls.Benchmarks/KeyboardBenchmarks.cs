// 'Useful Libraries' - Andy Hawkins 2025.

using BenchmarkDotNet.Attributes;

namespace Useful.Controls.Benchmarks;

[MemoryDiagnoser]
public class KeyboardBenchmarks
{
    private const int EventBatchSize = 8;

    private TestInput _input = null!;
    private SoftwareKeyboard _keyboard = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _input = new TestInput();
        _keyboard = new SoftwareKeyboard(_input);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _keyboard.ClearPressed();
        _keyboard.Close = false;
        _input.Reset();
    }

    [Benchmark]
    public void KeyDown() => _keyboard.KeyDown(ConsoleKey.A, ConsoleModifiers.None);

    [Benchmark]
    public void KeyUp()
    {
        _keyboard.KeyDown(ConsoleKey.A, ConsoleModifiers.None);
        _keyboard.KeyUp(ConsoleKey.A, ConsoleModifiers.None);
    }

    [Benchmark]
    public bool IsPressedKey()
    {
        _keyboard.KeyDown(ConsoleKey.B, ConsoleModifiers.None);
        return _keyboard.IsPressed(ConsoleKey.B);
    }

    [Benchmark]
    public bool IsPressedModifier()
    {
        _keyboard.KeyDown(ConsoleKey.C, ConsoleModifiers.Shift);
        return _keyboard.IsPressed(ConsoleModifiers.Shift);
    }

    [Benchmark]
    public (ConsoleKey Key, ConsoleModifiers Modifiers) LastPressed()
    {
        _keyboard.KeyDown(ConsoleKey.D, ConsoleModifiers.Control);
        return _keyboard.LastPressed();
    }

    [Benchmark]
    public void ClearPressed() => _keyboard.ClearPressed();

    // TestInput will produce no events by default
    [Benchmark]
    public void PollNoEvents() => _keyboard.Poll();

    [Benchmark]
    public void PollWithEvents()
    {
        // Arrange TestInput to produce a small batch of events when polled
        _input.EnqueueBatch(EventBatchSize);
        _keyboard.Poll();
    }

    // Small test IInput implementation used by the benchmarks.
    // Register stores the keyboard instance; Poll will invoke keyboard methods to simulate input events.
    private sealed class TestInput : IInput
    {
        private readonly Queue<Action<IKeyboard>> _events = new();
        private IKeyboard? _registered;

        public void Register(IKeyboard keyboard) => _registered = keyboard;

        // Enqueue a deterministic batch of key down/up actions.
        public void EnqueueBatch(int count)
        {
            _events.Clear();
            for (int i = 0; i < count; i++)
            {
                // Cycle through some keys and modifiers to exercise different code paths.
                ConsoleKey key = (ConsoleKey)((int)ConsoleKey.A + (i % 26));
                ConsoleModifiers mods = (i % 3) switch
                {
                    0 => ConsoleModifiers.None,
                    1 => ConsoleModifiers.Shift,
                    _ => ConsoleModifiers.Control,
                };

                _events.Enqueue(k => k.KeyDown(key, mods));
                _events.Enqueue(k => k.KeyUp(key, mods));
            }
        }

        public void Reset() => _events.Clear();

        // Called by SoftwareKeyboard.Poll(); this method must be fast and deterministic.
        public void Poll()
        {
            if (_registered is null)
            {
                return;
            }

            // Drain at most one batch worth of actions per Poll call (keeps benchmark stable).
            int iterations = Math.Min(32, _events.Count);
            for (int i = 0; i < iterations && _events.Count > 0; i++)
            {
                Action<IKeyboard> action = _events.Dequeue();
                action(_registered);
            }
        }
    }
}
