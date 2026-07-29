// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// The drawing half of one screen. Everything it needs arrives in
/// <typeparamref name="TModel"/>, so it holds no state and derives nothing:
/// one implementation per asset tier, differing only in layout.
/// </summary>
/// <typeparam name="TModel">The screen's view model, produced by its
/// <see cref="IScreenController"/>.</typeparam>
internal interface IView<in TModel>
{
    public void Draw(TModel model);
}
