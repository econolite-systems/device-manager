// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;
using Econolite.Ode.Repository.DeviceManager;
using Econolite.Ode.Config.Messaging.Sink;
using Econolite.Ode.Config.Messaging.Source;
using Econolite.Ode.Domain.DeviceManager;
using Econolite.Ode.Config.Messaging.Extensions;

namespace Worker.DeviceManager.ConfigRequestConsumer
{
    public class ConfigRequestConsumerWorker : BackgroundService
    {
        private readonly IDmRepository _dmRepository;
        private readonly ILogger<ConfigRequestConsumerWorker> _logger;
        private readonly IDeviceManagerConfigRequestSource _deviceManagerConfigRequestSource;
        private readonly IDeviceManagerConfigResponseSink _deviceManagerConfigResponseSink;

        public ConfigRequestConsumerWorker(
            IServiceProvider serviceProvider,
            IDeviceManagerConfigRequestSource deviceManagerConfigRequestSource,
            IDeviceManagerConfigResponseSink deviceManagerConfigResponseSink,
            ILogger<ConfigRequestConsumerWorker> logger)
        {
            _deviceManagerConfigResponseSink = deviceManagerConfigResponseSink;
            _deviceManagerConfigRequestSource = deviceManagerConfigRequestSource;
            _logger = logger;

            var serviceScope = serviceProvider.CreateScope();
            _dmRepository = serviceScope.ServiceProvider.GetRequiredService<IDmRepository>();
         }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                _logger.LogInformation("Starting...");
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            _logger.LogTrace("Waiting for Configuration request message...");
                            await _deviceManagerConfigRequestSource.ConsumeOn(async (consumed) =>
                            { 
                                _logger.LogInformation("Received request for configuration for {@}", consumed);
                                await RespondAsync(consumed.TenantId, consumed.DeviceManagerId, consumed.RequestVersion, stoppingToken);
                            }, stoppingToken);
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogTrace(ex, "Lost connection to topic");
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Unable to process request");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Stopping Upload / Download processing loop");
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "Upload / Download processing stopped unexpectedly, {@Exception}", e);
                    throw;
                }
                _logger.LogInformation("Upload / Download processing has ended.");
            });
        }

        private async Task RespondAsync(Guid tenantId, int deviceManagerId, int requestVersion, CancellationToken cancellationToken)
        {
            var done = false;
            do
            {
                try
                {
                    var config = await _dmRepository.GetByDmIdAsync(deviceManagerId);
                    if (config != null)
                    {
                        if (requestVersion >= 2)
                        {
                            _logger.LogInformation("Sending version 2 configuration to {@}", new { TenantId = tenantId, DevicemanagerId = deviceManagerId });
                            await _deviceManagerConfigResponseSink.SinkAsync(tenantId, config.ToConfigResponse().ToProtobuf(), cancellationToken);
                        }
                        else
                        {
                            _logger.LogInformation("Sending version 1 configuration to {@}", new { TenantId = tenantId, DevicemanagerId = deviceManagerId });
                            await _deviceManagerConfigResponseSink.SinkAsync(tenantId, config.ToConfigResponse(), cancellationToken);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Unable to find configuration for tenant {0}, devicemanager {1}", tenantId, deviceManagerId);
                        done = true;
                    }
                    done = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to send configuration to {@}", new { Tenant = tenantId, DeviceManagerId = deviceManagerId });
                }
            } while (!done);
        }
    }
}
