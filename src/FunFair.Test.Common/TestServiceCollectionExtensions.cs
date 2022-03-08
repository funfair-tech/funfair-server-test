using System;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace FunFair.Test.Common;

/// <summary>
///     Extension methods on <see cref="IServiceCollection" />
/// </summary>
public static class TestServiceCollectionExtensions
{
    /// <summary>
    ///     Add a mocked service using NSubstitute
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the mock to</param>
    /// <typeparam name="T">The service.</typeparam>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddMockedService<T>(this IServiceCollection serviceCollection)
        where T : class
    {
        T mock = Substitute.For<T>();

        return serviceCollection.AddSingleton(mock);
    }

    /// <summary>
    ///     Add a mocked service using NSubstitute
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the mock to</param>
    /// <param name="init">Action to initialise the mock.</param>
    /// <typeparam name="T">The service.</typeparam>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddMockedService<T>(this IServiceCollection serviceCollection, Action<T> init)
        where T : class
    {
        T mock = Substitute.For<T>();
        init(mock);

        return serviceCollection.AddSingleton(mock);
    }
}