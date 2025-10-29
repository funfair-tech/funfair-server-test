using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FunFair.Test.Common.Mocks;

[SuppressMessage(
    category: "FunFair.CodeAnalysis",
    checkId: "FFS0029: Should be internal",
    Justification = "Infrastructure"
)]
[SuppressMessage(
    category: "FunFair.CodeAnalysis",
    checkId: "FFS0030: Should be internal",
    Justification = "Infrastructure"
)]
[DebuggerDisplay("{_value}")]
public abstract class MockBase<T>
    where T : notnull
{
    private readonly T _value;

    protected MockBase(T value)
    {
        this._value = value;
    }

    public static implicit operator T(MockBase<T> instance)
    {
        return instance._value;
    }

    public abstract T Next();

    [SuppressMessage(
        category: "ToStringWithoutOverrideAnalyzer",
        checkId: "ExplicitToStringWithoutOverrideAnalyzer: Calling ToString() on object of type 'T' but it does not override ToString()",
        Justification = "Valid in this case"
    )]
    public override string ToString()
    {
        return this._value.ToString() ?? string.Empty;
    }
}
