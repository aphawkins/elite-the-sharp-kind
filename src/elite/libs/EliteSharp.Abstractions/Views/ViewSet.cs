// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// The views a pack drew for one tier, held by the model each one draws. The
/// only way to put a view in is <see cref="Add{TModel}"/>, which takes the
/// model type off the view's own interface, so a pack cannot file a view under
/// a model it does not draw.
/// <para>
/// The game asks for every screen it has before the first frame, so a pack
/// that is a screen short is a startup failure naming the screen rather than a
/// missing draw call the commander finds later.
/// </para>
/// </summary>
public sealed class ViewSet
{
    private readonly Dictionary<Type, object> _views = [];

    /// <summary>
    /// Gets the model types this set holds a view for, which is what the game
    /// checks its screens against.
    /// </summary>
    public IReadOnlyCollection<Type> ModelTypes => _views.Keys;

    /// <summary>
    /// Adds the view for one screen. Returns the set so a pack can chain the
    /// lot in one expression.
    /// </summary>
    /// <typeparam name="TModel">The model the view draws.</typeparam>
    /// <param name="view">The view for that model.</param>
    /// <returns>This set.</returns>
    /// <exception cref="InvalidOperationException">
    /// The pack has already added a view for this model. Two views for one
    /// screen means one of them never draws, so it is refused rather than
    /// silently overwritten.
    /// </exception>
    public ViewSet Add<TModel>(IView<TModel> view)
    {
        ArgumentNullException.ThrowIfNull(view);

        return _views.TryAdd(typeof(TModel), view)
            ? this
            : throw new InvalidOperationException($"The view pack has two views for {typeof(TModel).Name}.");
    }

    /// <summary>
    /// Gets the view for one screen.
    /// </summary>
    /// <typeparam name="TModel">The model the view draws.</typeparam>
    /// <returns>The view for that model.</returns>
    /// <exception cref="KeyNotFoundException">
    /// The pack has no view for this model - which the game has already
    /// refused to start on, so reaching this means a screen the check does not
    /// know about.
    /// </exception>
    public IView<TModel> Get<TModel>() => (IView<TModel>)_views[typeof(TModel)];
}
