// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Config.Messaging;
using Econolite.Ode.Config.Messaging.Extensions;
using Econolite.Ode.Persistence.Mongo;
using Worker.DeviceManager.ConfigRequestConsumer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builderContext, services) =>
    {
        services.AddMongo();
        services
            .AddRemoteConfig(options =>
            {
                options.DefaultChannel = builderContext.Configuration.GetValue(Consts.CONFIG_REQUEST_TOPIC, "configrequest")!;
            })
            .AddConfigRequestConsumerWorker();
    })
    .Build();

await host.RunAsync();
