// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Abstraction;

// Which concrete IAbstraction to construct. Software rasterises every frame
// into an off-screen bitmap and blits it through SDL once per frame, while
// Hardware issues SDL render calls directly.
public enum GraphicsBackend
{
    Software = 0,
    Hardware = 1,
}
