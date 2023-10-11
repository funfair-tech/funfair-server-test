using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace FunFair.Test.Common;

public static class TestServiceCollectionExtensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddMockedService<T>(this IServiceCollection serviceCollection)
        where T : class
    {
        T mock = Substitute.For<T>();

        return serviceCollection.AddSingleton(mock);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddMockedService<T>(this IServiceCollection serviceCollection, Action<T> init)
        where T : class
    {
        T mock = Substitute.For<T>();
        init(mock);

        return serviceCollection.AddSingleton(mock);
    }
}