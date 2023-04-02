using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Bogus;
using FunFair.Test.Common.Helpers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Xunit.Sdk;

namespace FunFair.Test.Common;

public abstract class TestBase
{
    protected TestBase()
    {
        // Nothing to do here!
        Assert.False(condition: false, userMessage: "Because");
    }

    protected static Task<T?> FromOptionalResultAsync<T>(T? value)
        where T : class
    {
        return Task.FromResult(value);
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used by test classes")]
    protected static Task<T?> NullResultAsync<T>()
        where T : class
    {
        return FromOptionalResultAsync((T?)null);
    }

    protected virtual ILogger<T> GetTypedLogger<T>()
    {
        return GetSubstitute<ILogger<T>>();
    }

    protected static IReadOnlyList<T> MakeFake<T>(Func<Faker<T>, Faker<T>> rules, int itemCount)
        where T : class
    {
        Assert.True(itemCount > 0, userMessage: "Must generate at least one ");

        const bool enable = true;

        return rules(new Faker<T>().StrictMode(enable))
            .Generate(itemCount);
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used by test classes")]
    protected static T GetSubstitute<T>(params object[] constructorArguments)
        where T : class
    {
        return Substitute.For<T>(constructorArguments);
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used by test classes")]
    protected static T1 GetSubstitute<T1, T2>(params object[] constructorArguments)
        where T1 : class where T2 : class
    {
        return Substitute.For<T1, T2>(constructorArguments);
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used by test classes")]
    protected static T1 GetSubstitute<T1, T2, T3>(params object[] constructorArguments)
        where T1 : class where T2 : class where T3 : class
    {
        return Substitute.For<T1, T2, T3>(constructorArguments);
    }

    protected static T AssertReallyNotNull<T>([NotNull] T? value)
        where T : class
    {
        Assert.NotNull(value);

        return value;
    }

    [SuppressMessage(category: "Microsoft.Usage", checkId: "CA1801:ReviewUnusedParameters", Justification = "Needed for Unit Test")]
    [SuppressMessage(category: "codecracker.CSharp", checkId: "CC0057:ReviewUnusedParameters", Justification = "Needed for Unit Test")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used by test classes")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void UnusedVariable<T>(T value)
    {
        // Marking that the variable is unused.
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used by test classes")]
    protected static void IsNotAssignableFrom<T>(object obj)
    {
        IsNotAssignableFrom(typeof(T), obj: obj);
    }

    protected static void IsNotAssignableFrom(Type expectedType, object obj)
    {
        if (IsNull(obj) || expectedType.GetTypeInfo()
                                       .IsAssignableFrom(obj.GetType()
                                                            .GetTypeInfo()))
        {
            throw new IsAssignableFromException(expected: expectedType, actual: obj);
        }
    }

    private static bool IsNull(object? obj)
    {
        return obj == null;
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used by test classes")]
    protected static string FormatValue<T>(T value)
        where T : notnull
    {
        return value.FormatValue();
    }
}