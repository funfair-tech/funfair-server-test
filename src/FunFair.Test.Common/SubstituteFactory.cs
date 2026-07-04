using NSubstitute;

namespace FunFair.Test.Common;

internal static class SubstituteFactory
{
    internal static T Create<T>(params object[] constructorArguments)
        where T : class
    {
        return Substitute.For<T>(constructorArguments);
    }

    internal static T1 Create<T1, T2>(params object[] constructorArguments)
        where T1 : class
        where T2 : class
    {
        return Substitute.For<T1, T2>(constructorArguments);
    }

    internal static T1 Create<T1, T2, T3>(params object[] constructorArguments)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        return Substitute.For<T1, T2, T3>(constructorArguments);
    }
}
