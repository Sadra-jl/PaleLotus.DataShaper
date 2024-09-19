using System.Text;
using static PaleLotus.DataShaper.GeneratorHelpers;

namespace PaleLotus.DataShaper;

public abstract record TypeToGenerate
{
    public string TypeName { get; }
    protected string Namespace { get; set; }
    private string AccessModifiers { get; }
    private string DataType { get; }
    private EquatableArray<string> Properties { get; }
    protected bool IsStatic { get; set; }
    private string TypeDeclaration => $"{AccessModifiers}{(IsStatic ? " static" : "")} {DataType}";
    protected abstract string FullTypeName { get; }
    protected abstract IEnumerable<string> Usings { get; }
    
    
    protected readonly StringBuilder StringBuilder = new();
    private readonly bool _hasNameSpace;

    protected TypeToGenerate(string typeName,
        List<string> values,
        string accessModifiers,
        string dataType, string nameSpace)
    {
        TypeName = typeName;
        AccessModifiers = accessModifiers;
        DataType = dataType;
        Namespace = nameSpace;
        _hasNameSpace = !string.IsNullOrEmpty(Namespace);
        Properties = new EquatableArray<string>(values.Where(str => !str.Equals("EqualityContract")).ToArray());
    }

    public virtual string Generate()
    {
        StringBuilder.Clear();
        GenerateHead();
        
        var template = $$"""
                         {
                            {{GenerateBody()}}
                         }
                         """;
        
        GenerateTail(template);
        return StringBuilder.ToString();
    }

    protected abstract string? GenerateBody();

    protected virtual void GenerateHead()
    {
        StringBuilder.AppendLine(GeneratedNote);

        foreach (var @using in Usings)
            StringBuilder.AppendLine($"using {@using};");
        StringBuilder.AppendLine();
        
        if (_hasNameSpace)
            StringBuilder.AppendLine( $"namespace {Namespace}\n{{");

        StringBuilder.AppendLine($"{TypeDeclaration} {FullTypeName}");
    }
    

    protected virtual void GenerateTail(string template)
    {
        if (!_hasNameSpace)
        {
            StringBuilder.Append(template);
            return;
        }
        
        foreach (var str in template.Split('\n'))
        {
            StringBuilder.Append('\t');
            StringBuilder.AppendLine(str);
        }
        
        StringBuilder.Append('}');
    }
}