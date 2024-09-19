using PaleLotus.DataShaper;

namespace ConsoleApp1;

[ShapeAbleData]
public class Person
{
    public int Id { get; set; }
    public Guid Uuid { get; set; }
    public DateOnly JoinedDate { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
}

[ShapeAbleData]
public class Address
{
    public string City { get; set; }
    public string Country { get; set; }
}