using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xunit;

namespace FunFair.Test.Common;

// Converts the flat (name, action) case list built by a BuildDispatcherCases<TSelf>() chain into the
// TheoryData<string, Action<TSelf>> shape [MemberData] requires. Kept as a single, tiny conversion point so every
// base class's BuildDispatcherCases<TSelf>() can stay a plain array/collection-expression - composing with a
// parent's cases via "[.. Parent.BuildDispatcherCases<TSelf>(), Case<TSelf>(t => t.X()), ...]" - rather than a
// chain of TheoryData.Add(...) calls.
public static class DispatcherCaseData
{
    // Builds one (name, action) entry from a single-method-call lambda, capturing the called method's name from
    // the argument's own source text via CallerArgumentExpression - a compile-time-only mechanism, not runtime
    // reflection or expression-tree compilation, so this stays safe under AOT test discovery (see
    // EquatableObjectTestBase<TObject>.BuildDispatcherCases for why reflection/Expression.Compile are ruled out
    // here). Avoids repeating the method name in both a nameof(...) and the lambda body.
    public static (string Name, Action<TSelf> Action) Case<TSelf>(
        Action<TSelf> action,
        [CallerArgumentExpression(nameof(action))] string expressionText = ""
    )
    {
        int openParen = expressionText.LastIndexOf('(');
        int dot = expressionText.LastIndexOf('.', openParen);

        return (expressionText[(dot + 1)..openParen], action);
    }

    public static TheoryData<string, Action<TSelf>> ToTheoryData<TSelf>(
        this IEnumerable<(string Name, Action<TSelf> Action)> cases
    )
    {
        TheoryData<string, Action<TSelf>> data = [];

        foreach ((string name, Action<TSelf> action) in cases)
        {
            data.Add(name, action);
        }

        return data;
    }
}
