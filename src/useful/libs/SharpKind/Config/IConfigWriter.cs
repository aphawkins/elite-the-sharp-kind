// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Config;

public interface IConfigWriter<in T>
{
    public void WriteConfig(T config);
}
