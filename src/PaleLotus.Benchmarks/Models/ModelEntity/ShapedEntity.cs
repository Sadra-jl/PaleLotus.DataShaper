namespace PaleLotus.Benchmarks.Models.ModelEntity;

public class ShapedEntity
{
    public Guid Id { get; set; }
    public ModelEntity.Entity Entity { get; set; } = new();
}