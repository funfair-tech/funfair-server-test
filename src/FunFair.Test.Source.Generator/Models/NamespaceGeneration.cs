using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace FunFair.Test.Source.Generator.Models;

[DebuggerDisplay("{Namespace}")]
public readonly record struct NamespaceGeneration
{
    public NamespaceGeneration(AssemblyIdentity assembly)
    {
        this.Assembly = assembly;
    }

    public string Namespace => this.Assembly.Name;

    public AssemblyIdentity Assembly { get; }
}
