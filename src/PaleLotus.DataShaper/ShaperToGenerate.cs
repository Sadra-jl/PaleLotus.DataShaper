using System.Text;
using static PaleLotus.DataShaper.EmbeddedSources;

namespace PaleLotus.DataShaper;

public record ShaperToGenerate : TypeToGenerate
{
    private readonly IEnumerable<List<string>> _combinations;
    private readonly string _defaultNamespace;

    public ShaperToGenerate(string typeName, List<string> values, string accessModifiers, string dataType,
        string nameSpace, IEnumerable<List<string>> combinations) : base(typeName, values, accessModifiers, dataType, $"{nameSpace}.Shapers")
    {
        _combinations = combinations;
        _defaultNamespace = nameSpace;
    }

    protected override string FullTypeName =>
        $" {TypeName}Shaper : Entity, {nameof(IDataShaper)}<{TypeName}>";

    protected override IEnumerable<string> Usings =>
    [
        "System",
        "System.Linq",
        "System.Collections.Generic",
        GeneratorHelpers.ProjectNameSpace,
        $"{_defaultNamespace}.Dtos"
    ];

    protected override string GenerateBody()
    {
        return $$"""
                 {{GenerateSelectorDictionary()}}
                 
                 public IQueryable<Entity> ShapeData(IQueryable<{{TypeName}}> source, string[] fields)
                 {
                    var orderedFields = GetSortedFields(fields);
                    return _selectors.TryGetValue(orderedFields, out var selector) ? selector(source) : source;
                 }
                 """;
    }
    
    private string GenerateSelectorDictionary()
    {
        var dictionaryEntries = new StringBuilder();

        foreach (var combination in _combinations)
        {
            var dtoName = $"{TypeName}{string.Join("", combination.Select(f => f.Replace(".", "_")))}Dto";
            var dictionaryKey = string.Join(",", combination);  // Comma-separated field names to act as the case key

            dictionaryEntries.AppendLine($$"""
                           
                                           { "{{dictionaryKey}}", src => src.Select(e => new {{dtoName}} { {{string.Join(", ", combination.Select(f => $"{f.Replace(".", "_")} = e.{f.Replace(".", ".")}"))}} }) },
                               """);
        }
        
        return $$"""
                 private readonly Dictionary<string, Func<IQueryable<{{TypeName}}>, IQueryable<Entity>>> _selectors = new()
                 {
                      {{dictionaryEntries.ToString().TrimEnd(',', ' ')}}
                 };
                 """;
    }
}