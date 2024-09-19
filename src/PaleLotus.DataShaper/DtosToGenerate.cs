namespace PaleLotus.DataShaper;

public record DtosToGenerate : TypeToGenerate
{
    private readonly List<(string Name, string Type)> _combination;

    public DtosToGenerate(string typeName, List<string> values, string accessModifiers, string dataType,
        string nameSpace, List<(string Name, string Type)> combination) : base(typeName, values, "internal", dataType, $"{nameSpace}.Dtos")
    {
        _combination = combination;
    }

    public string GetFullTypeName() =>
        $"{TypeName}{string.Join("", _combination.Select(member => member.Name.Replace(".", "_")))}Dto";
    protected override string FullTypeName => $"{GetFullTypeName()} : Entity";

    protected override IEnumerable<string> Usings => [GeneratorHelpers.ProjectNameSpace];

    protected override string? GenerateBody() =>
        string.Join("\n", _combination.Select(member =>
                $"public {member.Type} {member.Name.Replace(".", "_")} {{ get; set; }}"));
}