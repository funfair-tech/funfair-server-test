using System.Linq;
using FunFair.Test.Source.Generator.Builders;

namespace FunFair.Test.Source.Generator.Extensions;

internal static class CodeBuilderExtensions
{
    public static CodeBuilder AppendNamespaces(this CodeBuilder codeBuilder, params string[] namespaces)
    {
        return namespaces.Aggregate(seed: codeBuilder, func: (builder, item) => builder.AppendLine($"using {item};"));
    }

    public static CodeBuilder AppendSuppression(
        this CodeBuilder codeBuilder,
        string category,
        string checkId,
        string justification
    )
    {
        return codeBuilder.AppendLine(
            $"[assembly: SuppressMessage(category: \"{category}\", checkId: \"{checkId}\", Justification = \"{justification}\")]"
        );
    }
}
