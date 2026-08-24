using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace FunFair.Test.Source.Generator.Builders;

public sealed class CodeBuilder
{
    private readonly StringBuilder _stringBuilder = new();

    public SourceText Text =>
        SourceText.From(
            this._stringBuilder.ToString(),
            encoding: Encoding.UTF8,
            checksumAlgorithm: SourceHashAlgorithm.Sha256
        );

    public CodeBuilder AppendBlankLine()
    {
        this._stringBuilder.AppendLine();

        return this;
    }

    public CodeBuilder AppendLine(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return this.AppendBlankLine();
        }

        this._stringBuilder.AppendLine(text);

        return this;
    }
}
