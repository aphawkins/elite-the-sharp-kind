// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Useful.Abstraction;
using Useful.Assets;
using Useful.Fakes.Controls;
using Useful.Fakes.Harness;

namespace EliteSharpLib.Tests;

// Drives the real EliteMain (the same DI composition SDLProgram.Main
// builds, AddEliteConfig + AddEliteMain) against a real SoftwareGraphics
// with no SDL window, for tests that need several ticks of real gameplay
// and, occasionally, a rendered frame to eyeball. EliteMain.Run is unusable
// headlessly as-is - it hands off to GameHost.Run's real-time,
// wall-clock-waiting loop - so this calls Update()/Draw() directly per
// tick instead (via HeadlessGameHarnessBase).
internal sealed class HeadlessGameHarness : HeadlessGameHarnessBase<GameStateSummary>
{
    private readonly ServiceProvider _provider;
    private readonly string _configDirectory;

    // 512x512, matching SDLProgram's real ScreenWidth/ScreenHeight: EliteDraw
    // derives its layout (Centre, ScannerTop, ...) from these, and a 0x0
    // screen produces negative ranges that blow up star generation.
    public HeadlessGameHarness(int width = 512, int height = 512)
        : base(width, height, AssetLocator.Create())
    {
        FakeAbstraction abstraction = new(Graphics);
        Keyboard = (FakeKeyboard)abstraction.Keyboard;

        _configDirectory = Path.Combine(Path.GetTempPath(), "EliteHeadlessHarness_" + Guid.NewGuid().ToString("N"));

        ServiceCollection services = new();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddSingleton<IAbstraction>(abstraction);
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Graphics);
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Sound);
        services.AddSingleton(sp => sp.GetRequiredService<IAbstraction>().Keyboard);
        services.AddSingleton(_ => AssetLocator.Create());
        services.AddEliteConfig(_configDirectory);
        services.AddEliteMain();

        _provider = services.BuildServiceProvider();
        Game = _provider.GetRequiredService<EliteMain>();
    }

    public EliteMain Game { get; }

    public override GameStateSummary State => new(
        Game.State.CurrentScreen,
        Game.State.IsDocked,
        Game.State.IsGameOver);

    protected override void UpdateGame() => Game.Update();

    protected override void DrawGame() => Game.Draw();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _provider.Dispose();
            if (Directory.Exists(_configDirectory))
            {
                Directory.Delete(_configDirectory, recursive: true);
            }
        }

        base.Dispose(disposing);
    }
}
