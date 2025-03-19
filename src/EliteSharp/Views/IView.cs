// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Views;

internal interface IView
{
    public void Draw();

    public void HandleInput();

    public void Reset();

    public void UpdateUniverse();
}
