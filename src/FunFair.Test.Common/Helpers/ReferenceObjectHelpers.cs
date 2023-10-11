using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Meziantou.Xunit;

namespace FunFair.Test.Common.Helpers;

[DisableParallelization]
public static class ReferenceObjectHelpers
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreEqual<T>(T? left, T? right, Func<T, T, bool> eq)
        where T : class
    {
        if (ReferenceEquals(objA: left, objB: right))
        {
            return true;
        }

        if (right is null)
        {
            return false;
        }

        return left is not null && eq(arg1: left, arg2: right);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare<T>(T? left, T? right, Func<T, T, int> cmp)
        where T : class
    {
        if (ReferenceEquals(objA: left, objB: right))
        {
            return 0;
        }

        if (right is null)
        {
            return -1;
        }

        return left is null
            ? 1
            : cmp(arg1: left, arg2: right);
    }
}