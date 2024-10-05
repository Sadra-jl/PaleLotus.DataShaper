using PaleLotus.Benchmarks.Models.ModelEntity;

namespace PaleLotus.Benchmarks;

public interface IDataShaper<in T>
{
    IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString);
    ShapedEntity ShapeData(T entity, string fieldsString);
}