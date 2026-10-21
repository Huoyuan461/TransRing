// Version labels for Settings → Engines: the NuGet version of a library engine, TransRing's own version for engines
// built into it, and the pinned version of a bundled or downloaded program (engines.json).

using System.Reflection;

namespace TransRing.Engines.Infrastructure;

/// <summary>Builds the version text an engine reports in its <see cref="TransRing.Core.Conversion.EngineStatus"/>.</summary>
public static class EngineVersions
{
    /// <summary>"Markdig 1.4.0": the product name and version of the library that contains <paramref name="type"/>.</summary>
    public static string Library(string name, Type type) => $"{name} {Of(type.Assembly)}";

    /// <summary>"TransRing 1.1.0" — for engines written as part of TransRing.</summary>
    public static string BuiltIn { get; } = $"TransRing {Of(typeof(EngineVersions).Assembly)}";

    /// <summary>The version engines.json pins for a bundled or downloaded component ("0.8.6"), or null.</summary>
    public static string? Component(string componentId) =>
        EngineDownloads.Components.TryGetValue(componentId, out var component) ? component.Version : null;

    /// <summary>Informational version without the source revision ("14.17.2+a1b2c3" → "14.17.2").</summary>
    private static string Of(Assembly assembly)
    {
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                      ?? assembly.GetName().Version?.ToString(3)
                      ?? "?";
        return version.Split('+')[0];
    }
}
