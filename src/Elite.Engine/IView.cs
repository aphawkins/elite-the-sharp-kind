// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp
{
    internal interface IView
    {
        void Draw();

        void HandleInput();

        void Reset();

        void UpdateUniverse();
    }
}
