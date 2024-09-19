#if DEBUG
    using System.Diagnostics;
#endif
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static PaleLotus.DataShaper.EmbeddedSources;
using static PaleLotus.DataShaper.GeneratorHelpers;
 
namespace PaleLotus.DataShaper;

[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    public Generator()
    {
#if DEBUG
        if (!Debugger.IsAttached)
            Debugger.Launch();
#endif
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(incContext =>
        {
            incContext.AddSource(
                $"{nameof(ShapeAbleDataAttribute)}.g.cs",
                SourceText.From(ShapeAbleDataAttribute, Encoding.UTF8));
            
            incContext.AddSource($"{nameof(IDataShaper)}.g.cs",
                IDataShaper);
            
            incContext.AddSource(
                $"{nameof(Entity)}.g.cs",
                SourceText.From(Entity, Encoding.UTF8));
        });

        var typesToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName($"{ProjectNameSpace}.{nameof(ShapeAbleDataAttribute)}",
                predicate: static (_, _) => true,
                transform: static (attContext, _) =>
                    GetTypeToGenerate(attContext.SemanticModel, attContext.TargetNode))
            .Where(static source => source is not null);
        
        context.RegisterSourceOutput(typesToGenerate,
            static (spc, source) => Execute(source, spc));
    }

    private static void Execute((ShaperToGenerate shaperToGenerate, IEnumerable<DtosToGenerate> dtosToGenerate)? typeToGenerate, SourceProductionContext context)
    {
        if (typeToGenerate is null) return;
        
        var (shaperToGenerate, dtosToGenerate) = typeToGenerate.Value;

        context.AddSource($"Shapers/{shaperToGenerate.TypeName}Shaper.g.cs", SourceText.From(shaperToGenerate.Generate(), Encoding.UTF8));
        
        foreach (var dtoToGenerate in dtosToGenerate)
            context.AddSource($"Dtos/{dtoToGenerate.GetFullTypeName()}.g.cs", SourceText.From(dtoToGenerate.Generate(), Encoding.UTF8));
    }


    private static (ShaperToGenerate shaperToGenerate, IEnumerable<DtosToGenerate> dtosToGenerate)?  GetTypeToGenerate(SemanticModel semanticModel, SyntaxNode typeDeclarationSyntax)
    {
        if (semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) is not INamedTypeSymbol typeSymbol)
            return null;

        var name = typeSymbol.Name;

        var typeSyntax = typeDeclarationSyntax as TypeDeclarationSyntax;
        Debug.Assert(typeSyntax != null, nameof(typeSyntax) + " != null");
        
        var membersWithType = GetMembersNameAndType(typeSymbol);
        var members = membersWithType.Select(member => member.Name).ToList();
        
        var combinationsWithType = GetFieldCombinations(membersWithType).ToList();
        var combinations = combinationsWithType.Select(combination => combination.Select(state => state.Name).ToList()).ToList();

        var propertiesWithType = GetMembersNameAndType(typeSymbol, false);
        var properties = propertiesWithType.Select(member => member.Name).ToList(); 
        
        var shaper = new ShaperToGenerate(name, properties, typeSymbol.GetAccessModifier(),
            typeSymbol.GetTypeKind(), typeSyntax!.GetNamespace(), combinations);

        var dtos = new List<DtosToGenerate>(combinations.Count);
        
        dtos.AddRange(combinationsWithType.Select(combination => new DtosToGenerate(name, members,
            typeSymbol.GetAccessModifier(), typeSymbol.GetTypeKind(), typeSyntax!.GetNamespace(), combination)));

        return (shaper, dtos);
    }
    
    // Helper method to generate all possible field combinations
    
    private static IEnumerable<List<(string Name, string Type)>> GetFieldCombinations(IEnumerable<(string Name, string Type)> fields)
    {
        var fieldList = fields.ToList();
        var combinations = new List<List<(string Name, string Type)>>();

        GenerateCombinations([], 0);

        // Remove duplicates
        return combinations.Select(c => c.Distinct().ToList()).Distinct(new ListComparer()).ToList();

        void GenerateCombinations(List<(string Name, string Type)> currentCombination, int startIndex)
        {
            if (currentCombination.Count > 0)
                combinations.Add([..currentCombination]);

            for (var i = startIndex; i < fieldList.Count; i++)
            {
                var field = fieldList[i];
                var parent = field.Name.Contains(".")
                    ? field.Name.Substring(0, field.Name.IndexOf('.'))
                    : field.Name;

                if (currentCombination.Any(f => f.Name == parent || parent.StartsWith(f.Name + ".")))
                    continue;

                currentCombination.Add(field);
                GenerateCombinations(currentCombination, i + 1);
                currentCombination.RemoveAt(currentCombination.Count - 1);
            }
        }
    }

}
internal class ListComparer : IEqualityComparer<List<(string Name, string Type)>>
{
    public bool Equals(List<(string Name, string Type)> x, List<(string Name, string Type)> y)
    {
        if (x.Count != y.Count)
            return false;

        return !x.Where((t, i) => !t.Equals(y[i])).Any();
    }

    public int GetHashCode(List<(string Name, string Type)> obj) => obj.Aggregate(666, (current, item) => current * (1331 + item.GetHashCode()));
}