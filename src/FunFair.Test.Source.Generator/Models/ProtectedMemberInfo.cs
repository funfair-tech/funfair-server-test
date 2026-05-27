using System.Diagnostics;

namespace FunFair.Test.Source.Generator.Models;

[DebuggerDisplay("{Target}")]
internal readonly record struct ProtectedMemberInfo
{
    public ProtectedMemberInfo(string target)
    {
        this.Target = target;
    }

    public string Target { get; }
}
