using System;
using Meziantou.Xunit;

namespace FunFair.Test.Common.Helpers;

[DisableParallelization]
public static class ReferenceObjectHelpers
{
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

        if (left is null)
        {
            return false;
        }

        return eq(arg1: left, arg2: right);
    }

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

        if (left is null)
        {
            return 1;
        }

        return cmp(arg1: left, arg2: right);
    }
}