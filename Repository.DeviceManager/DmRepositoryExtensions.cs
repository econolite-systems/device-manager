// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Repository.DeviceManager;

public static class DmRepositoryExtensions
{
    public static IServiceCollection AddDmRepo(this IServiceCollection services)
    {
        services.AddScoped<IDmRepository, DmRepository>();
        services.AddScoped<ICounterRepository, CounterRepository>();

        return services;
    }
}
