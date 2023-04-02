using System;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace FunFair.Test.Common;

public static class TestServiceCollectionExtensions
{
    public static IServiceCollection AddMockedService<T>(this IServiceCollection serviceCollection)
        where T : class
    {
        T mock = Substitute.For<T>();

        return serviceCollection.AddSingleton(mock);
    }

    public static IServiceCollection AddMockedService<T>(this IServiceCollection serviceCollection, Action<T> init)
        where T : class
    {
        T mock = Substitute.For<T>();
        init(mock);

        return serviceCollection.AddSingleton(mock);
    }
}