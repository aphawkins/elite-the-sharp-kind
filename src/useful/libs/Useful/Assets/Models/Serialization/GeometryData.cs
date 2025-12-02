// 'Useful Libraries' - Andy Hawkins 2025.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Useful.Assets.Models.Serialization;

[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Need to be writable for serialisation.")]
public class GeometryData
{
    public Collection<List<int>> FaceNormals { get; set; } = [];

    public Collection<JsonElement> Faces { get; set; } = [];

    public Collection<List<int>> Lines { get; set; } = [];

    public Collection<List<int>> Points { get; set; } = [];
}
