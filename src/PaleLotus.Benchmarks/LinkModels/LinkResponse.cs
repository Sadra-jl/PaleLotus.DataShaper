using PaleLotus.Benchmarks.Models;
using PaleLotus.Benchmarks.Models.ModelEntity;

namespace PaleLotus.Benchmarks.LinkModels;

public class LinkResponse
{
    public bool HasLinks { get; set; }
    public List<Entity> ShapedEntities { get; set; } = [];
    public LinkCollectionWrapper<Entity> LinkedEntities { get; set; } = new();
}