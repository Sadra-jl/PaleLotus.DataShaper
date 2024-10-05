using PaleLotus.DataShaper;

namespace PaleLotus.Benchmarks.Models;

[ShapeAbleData]
public class Address
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}