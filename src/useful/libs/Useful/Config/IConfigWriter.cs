// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Config;

public interface IConfigWriter<in T>
{
    public void WriteConfig(T config);
}
