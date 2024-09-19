using System.Reflection;
using System.Text;

namespace PaleLotus.DataShaper;

internal class EmbeddedSources
{
    private static readonly Assembly ThisAssembly = typeof(EmbeddedSources).Assembly;
    
    // ReSharper disable once InconsistentNaming
    internal static readonly string IDataShaper = LoadEmbedded("IDataShaper.txt");
    internal static readonly string Entity = LoadEmbedded("Entity.txt");
    internal static readonly string ShapeAbleDataAttribute = LoadEmbedded("ShapeAbleDataAttribute.txt");

    private static string LoadEmbedded(string templateName)
        => LoadEmbeddedResource($"PaleLotus.DataShaper.Templates.{templateName}");

    private static string LoadEmbeddedResource(string resourceName)
    {
        var resourceStream = ThisAssembly.GetManifestResourceStream(resourceName);
        if (resourceStream is null)
        {
            var existingResources = ThisAssembly.GetManifestResourceNames();
            throw new ArgumentException(
                $"Could not find embedded resource {resourceName}. Available names: {string.Join(", ", existingResources)}");
        }

        using var reader = new StreamReader(resourceStream, Encoding.UTF8);

        return reader.ReadToEnd();
    }
}