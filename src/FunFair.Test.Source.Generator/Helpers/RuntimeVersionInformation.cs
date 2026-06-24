using System.Reflection;

namespace FunFair.Test.Source.Generator.Helpers;

internal static class RuntimeVersionInformation
{
    private static readonly AssemblyName AssemblyName = typeof(TestAssemblyCodeGenerator).Assembly.GetName();

    public static string ToolName { get; } = AssemblyName.Name;

    public static string GeneratorVersion { get; } = AssemblyName.Version.ToString();
}
