// 'Useful Libraries' - Andy Hawkins 2025.

using static SDL2.SDL;

namespace Useful.SDL.Tests;

public class SDLHelperTests
{
    // Regression test: SDLK_RIGHT was previously mapped to ConsoleKey
    // .OemPeriod (and ConsoleKey.RightArrow was only reachable via
    // SDLK_RIGHTBRACKET), so the physical Right Arrow key never produced
    // ConsoleKey.RightArrow at all. Elite's views masked this because they
    // check "OemPeriod || RightArrow", but anything checking RightArrow
    // alone (e.g. SCR's steering) silently never saw it.
    [Theory]
    [InlineData(SDL_Keycode.SDLK_LEFT, ConsoleKey.LeftArrow)]
    [InlineData(SDL_Keycode.SDLK_RIGHT, ConsoleKey.RightArrow)]
    [InlineData(SDL_Keycode.SDLK_UP, ConsoleKey.UpArrow)]
    [InlineData(SDL_Keycode.SDLK_DOWN, ConsoleKey.DownArrow)]
    [InlineData(SDL_Keycode.SDLK_COMMA, ConsoleKey.OemComma)]
    [InlineData(SDL_Keycode.SDLK_PERIOD, ConsoleKey.OemPeriod)]
    [InlineData(SDL_Keycode.SDLK_SPACE, ConsoleKey.Spacebar)]
    public void KeyConverterMapsPhysicalKeyToExpectedConsoleKey(SDL_Keycode sdlKey, ConsoleKey expected)
    {
        (ConsoleKey key, ConsoleModifiers modifiers) = SDLHelper.KeyConverter(sdlKey);

        Assert.Equal(expected, key);
        Assert.Equal(ConsoleModifiers.None, modifiers);
    }
}
