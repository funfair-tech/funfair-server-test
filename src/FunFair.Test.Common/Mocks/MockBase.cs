using System;
using System.Diagnostics;

namespace FunFair.Test.Common.Mocks;

[DebuggerDisplay("{_value}")]
internal sealed class MockBase<T>
    where T : notnull
{
    private readonly Func<T> _nextFactory;
    private readonly T _value;

    public MockBase(T value, Func<T> nextFactory)
    {
        this._value = value;
        this._nextFactory = nextFactory;
    }

    public static implicit operator T(MockBase<T> instance)
    {
        return instance._value;
    }

    public T Next()
    {
        return this._nextFactory();
    }

    public override string ToString()
    {
        return string.Concat(this._value);
    }
}
