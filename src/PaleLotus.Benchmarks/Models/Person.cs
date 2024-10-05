using PaleLotus.DataShaper;

namespace PaleLotus.Benchmarks.Models;

[ShapeAbleData]
public class Person
{
    public int Id { get; set; }
    public Guid Uuid { get; set; }
    public DateOnly JoinedDate { get; set; }
    public string? Name { get; set; }
    public Address? Address { get; set; }
}