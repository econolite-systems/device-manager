// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Domain.DeviceManager;
using Econolite.Ode.Repository.DeviceManager;

namespace Worker.DeviceManager.ConfigRequestConsumer
{
    public static class ConfigRequestConsumerExtensions
    {
        public static IServiceCollection AddConfigRequestConsumerWorker(this IServiceCollection services)
        {

            services.AddDmRepo();

            services.AddDmService();

            services.AddHostedService<ConfigRequestConsumerWorker>();

            return services;
        }
    }
}
