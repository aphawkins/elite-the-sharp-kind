// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Abstraction;

namespace EliteSharpLib.Views;

/// <summary>
/// The behavioural half of one screen: it takes input, advances the screen's
/// own state and produces what gets drawn. The drawing half is an
/// <see cref="IView{TModel}"/> it delegates to, so layout can vary per asset
/// tier while behaviour has a single home.
/// </summary>
internal interface IScreenController : IGameScreen
{
    public void HandleInput();
}
