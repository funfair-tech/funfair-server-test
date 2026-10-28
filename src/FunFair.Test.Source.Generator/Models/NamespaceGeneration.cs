using System.Diagnostics;

namespace FunFair.Test.Source.Generator.Models;

[DebuggerDisplay("{AssemblyName}")]
public readonly record struct NamespaceGeneration
{
    public NamespaceGeneration(string? assemblyName)
    {
        this.AssemblyName = assemblyName;
    }

    public string? AssemblyName { get; }
}
