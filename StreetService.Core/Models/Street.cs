namespace StreetService.Core.Models;

using NetTopologySuite.Geometries;

public class Street
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public Geometry? Geometry { get; set; }

    public byte[]? Version { get; set; } // For concurrency control (Optimistic Concurrency)
}
