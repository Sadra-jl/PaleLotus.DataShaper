using System.Diagnostics;
using System.Reflection;
using PaleLotus.Benchmarks.Models;
using PaleLotus.Benchmarks.Models.ModelEntity;

namespace PaleLotus.Benchmarks;

public sealed class DataShaper<TEntity>(string idName) :IDataShaper<TEntity> where TEntity : class
{
    private PropertyInfo[] Properties { get; set; } =
        typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

    public IEnumerable<ShapedEntity> ShapeData(IEnumerable<TEntity> entities, string fieldsString)
    {
        var requiredProperties = GetRequiredProperty(fieldsString);
        return FetchData(entities, requiredProperties);
    }

    public ShapedEntity ShapeData(TEntity entity, string fieldsString)
    {
        var requiredProperties = GetRequiredProperty(fieldsString);
        return FetchDataForEntity(entity, requiredProperties);
    }

    private List<PropertyInfo> GetRequiredProperty(string fieldsString)
    {
        if (string.IsNullOrWhiteSpace(fieldsString))
            return Properties.ToList();

        var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        
        var requiredProperties = fields
            .Select(field => Properties.FirstOrDefault(info =>
                info.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            .OfType<PropertyInfo>()
            .ToList();

        return requiredProperties;
    }

    private ShapedEntity FetchDataForEntity(TEntity entity, IEnumerable<PropertyInfo> requiredProperties)
    {
        ShapedEntity shapedObject = new();

        foreach (var property in requiredProperties)
        {
            var objectPropertyValue = property.GetValue(entity);
            shapedObject.Entity!.TryAdd(property.Name, objectPropertyValue);
        }

        var objectProperty = entity.GetType().GetProperty(idName);
        
        Debug.Assert(objectProperty != null, nameof(objectProperty) + " != null");
        shapedObject.Id = (Guid) objectProperty.GetValue(entity)!;
        
        return shapedObject;
    }

    private List<ShapedEntity>
        FetchData(IEnumerable<TEntity> entities, IEnumerable<PropertyInfo> requiredProperties) =>
        entities.Select(entity => FetchDataForEntity(entity, requiredProperties)).ToList();
}