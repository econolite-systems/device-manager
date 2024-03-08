// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Config.Messaging.Extensions;
using Econolite.Ode.Config.Messaging.Sink;
using Econolite.Ode.Domain.Configuration;
using Econolite.Ode.Helpers.Exceptions;
using Econolite.Ode.Models.DeviceManager.Db;
using Econolite.Ode.Models.DeviceManager.Models;
using Econolite.Ode.Repository.DeviceManager;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.DeviceManager;

public class DeviceManagerService : IDeviceManagerService
{
    private readonly ICounterRepository _counterRepository;
    private readonly IDmRepository _dmRepository;
    private readonly ILogger<DeviceManagerService> _logger;
    private readonly IDeviceManagerConfigResponseSink _deviceManagerConfigResponseSink;

    public DeviceManagerService(ICounterRepository counterRepository, IDmRepository dmRepository,
        ILogger<DeviceManagerService> logger, IDeviceManagerConfigResponseSink deviceManagerConfigResponseSink)
    {
        _deviceManagerConfigResponseSink = deviceManagerConfigResponseSink;
        _logger = logger;
        _dmRepository = dmRepository;
        _counterRepository = counterRepository;
    }

    public async Task<IEnumerable<DmConfigDto>> GetAllAsync(Guid tenantId)
    {
        var result = await _dmRepository.GetAllAsync(tenantId);
        return result.Select(r => r.AdaptToDto());
    }

    public async Task<DmConfigDto?> GetByIdAsync(Guid id)
    {
        var result = await _dmRepository.GetByIdAsync(id);
        return result.AdaptToDto();
    }

    public async Task<DmConfigDto?> AddDeviceManager(Guid tenantId, DmConfigAdd dmConfigAdd)
    {
        var existing = await _dmRepository.GetByDmIdAsync(dmConfigAdd.DmId);
        if (existing != null)
        {
            throw new Exception("Number already in use by a different device manager");
        }

        //don't save a null to the channel do an empty list; can't change it in the mapper extensions because those are auto generated
        dmConfigAdd.Channels ??= new List<ChannelAdd>();

        var dmConfig = dmConfigAdd.AdaptToDmConfig();
        dmConfig.TenantId = tenantId;

        _dmRepository.Add(dmConfig);
        var (success, errors) = await _dmRepository.DbContext.SaveChangesAsync();

        //Note: Per Jade, don't need to call this on an add.  They don't need the info until controllers are added.
        //if(success) await _dmConfigProducer.PublishUpdateDMConfigAsync(dmConfig);

        return dmConfig.AdaptToDto();
    }

    public async Task<DmConfigDto?> UpdateDeviceManager(DmConfigUpdate update)
    {
        var dbConfig = await _dmRepository.GetByIdAsync(update.Id);

        //don't save a null to the channel do an empty list; can't change it in the mapper extensions because those are auto generated
        update.Channels ??= new List<ChannelUpdate>();

        var toUpdate = update.Adapt(dbConfig);
        if (toUpdate is null)
        {
            return null;
        }

        _dmRepository.Update(toUpdate);

        var (success, errors) = await _dmRepository.DbContext.SaveChangesAsync();
        if (!success && !string.IsNullOrWhiteSpace(errors)) throw new UpdateException(errors);

        if (success) await _deviceManagerConfigResponseSink.SinkAsync(toUpdate.TenantId, toUpdate.ToConfigResponse().ToProtobuf(), CancellationToken.None);

        return toUpdate.AdaptToDto();
    }

    public async Task<bool> DeleteDeviceManager(Guid id)
    {
        try
        {
            _dmRepository.Remove(id);
            var (success, errors) = await _dmRepository.DbContext.SaveChangesAsync();

            //Note: Per Jade, don't need to call this on a delete because it gets blocked in the "don't send empty updates to the field" check
            //if (success) await _dmConfigProducer.PublishUpdateDMConfigAsync(dmConfig);

            return success;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return false;
        }
    }

    public async Task<ChannelDto?> AddChannelAsync(Guid dmId, ChannelAdd channelAdd)
    {
        var dmConfig = await _dmRepository.GetByIdAsync(dmId);
        if (dmConfig is null)
        {
            return null;
        }

        if (channelAdd.ChannelId <= 1)
            channelAdd.ChannelId = await _counterRepository.GetCounterAsync(dmConfig.TenantId, "channel");

        //don't save a null do an empty list; can't change it in the mapper extensions because those are auto generated
        channelAdd.SignalControllers ??= new List<ControllerAdd>();

        var toAdd = channelAdd.AdaptToChannel();
        toAdd.Id = Guid.NewGuid();

        dmConfig.Channels ??= new List<Channel>();

        dmConfig.Channels.Add(toAdd);
        _dmRepository.Update(dmConfig);
        var (success, errors) = await _dmRepository.DbContext.SaveChangesAsync();
        if (!success && !string.IsNullOrWhiteSpace(errors)) throw new UpdateException(errors);

        if (success) await _deviceManagerConfigResponseSink.SinkAsync(dmConfig.TenantId, dmConfig.ToConfigResponse().ToProtobuf(), CancellationToken.None);

        return toAdd.AdaptToDto();
    }

    public async Task<ChannelDto?> UpdateChannelAsync(Guid dmId, ChannelUpdate channelUpdate)
    {
        var dmConfig = await _dmRepository.GetByIdAsync(dmId);
        if (dmConfig is null)
        {
            return null;
        }

        var index = dmConfig.Channels.FindIndex(c => c.ChannelId == channelUpdate.ChannelId);
        var channel = dmConfig.Channels[index];

        //don't save a null do an empty list; can't change it in the mapper extensions because those are auto generated
        channelUpdate.SignalControllers ??= new List<ControllerUpdate>();

        var updated = channelUpdate.Adapt(channel);
        dmConfig.Channels[index] = updated;

        _dmRepository.Update(dmConfig);

        var (success, errors) = await _dmRepository.DbContext.SaveChangesAsync();
        if (!success && !string.IsNullOrWhiteSpace(errors)) throw new UpdateException(errors);

        if (success) await _deviceManagerConfigResponseSink.SinkAsync(dmConfig.TenantId, dmConfig.ToConfigResponse().ToProtobuf(), CancellationToken.None);

        return updated.AdaptToDto();
    }

    public async Task<bool> DeleteChannelAsync(Guid dmId, Guid channelId)
    {
        try
        {
            _dmRepository.DeleteChannel(dmId, channelId);
            var (success, _) = await _dmRepository.DbContext.SaveChangesAsync();

            if (success)
            {
                var dmConfig = await _dmRepository.GetByIdAsync(dmId);
                if (dmConfig != null) await _deviceManagerConfigResponseSink.SinkAsync(dmConfig.TenantId, dmConfig.ToConfigResponse().ToProtobuf(), CancellationToken.None);
            }

            return success;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return false;
        }
    }

    public async Task<ControllerDto?> AddControllerAsync(Guid dmId, Guid channelId, Controller controllerAdd)
    {
        var dmConfig = await _dmRepository.GetByIdAsync(dmId);
        if (dmConfig is null)
        {
            return null;
        }

        var channelIndex = dmConfig.Channels.FindIndex(c => c.Id == channelId);
        var channel = dmConfig.Channels[channelIndex];
        var controller = controllerAdd;
        channel.SignalControllers ??= new List<Controller>();
        channel.SignalControllers.Add(controller);

        dmConfig.Channels[channelIndex] = channel;
        _dmRepository.Update(dmConfig);

        var (success, errors) = await _dmRepository.DbContext.SaveChangesAsync();
        if (!success && !string.IsNullOrWhiteSpace(errors)) throw new UpdateException(errors);

        if (success) await _deviceManagerConfigResponseSink.SinkAsync(dmConfig.TenantId, dmConfig.ToConfigResponse().ToProtobuf(), CancellationToken.None);

        return controller.AdaptToDto();
    }

    public async Task<ControllerDto?> UpdateControllerAsync(Guid dmId, Guid channelId, Controller controllerUpdate)
    {
        var dmConfig = await _dmRepository.GetByIdAsync(dmId);
        if (dmConfig is null)
        {
            return null;
        }

        var channel = dmConfig.Channels.Find(c => c.Id == channelId);
        if (channel is null)
        {
            return null;
        }

        if (channel is { SignalControllers: null } || channel.SignalControllers.All(c => c.Id != controllerUpdate.Id))
            return await AddControllerAsync(dmId, channelId, controllerUpdate);
        var channelIndex = dmConfig.Channels.FindIndex(c => c.Id == channel.Id);
        var controllerIndex = channel.SignalControllers.FindIndex(c => c.Id == controllerUpdate.Id);
        var controller = channel.SignalControllers[controllerIndex];
        var updated = controllerUpdate;
        channel.SignalControllers[controllerIndex] = updated;
        dmConfig.Channels[channelIndex] = channel;

        _dmRepository.Update(dmConfig);

        var (success, errors) = await _dmRepository.DbContext.SaveChangesAsync();
        if (!success && !string.IsNullOrWhiteSpace(errors)) throw new UpdateException(errors);

        if (success) await _deviceManagerConfigResponseSink.SinkAsync(dmConfig.TenantId, dmConfig.ToConfigResponse().ToProtobuf(), CancellationToken.None);

        return updated.AdaptToDto();
    }

    public async Task<bool> DeleteControllerAsync(Guid dmId, Guid channelId, Guid signalId)
    {
        var dmConfig = await _dmRepository.GetByIdAsync(dmId);
        if (dmConfig is null)
        {
            return false;
        }

        var index = dmConfig.Channels.FindIndex(c => c.Id == channelId);
        var channel = dmConfig.Channels[index];
        var controller = channel.SignalControllers.Find(s => s.Id == signalId);
        if (controller is null)
        {
            return false;
        }

        var removed = channel.SignalControllers.Remove(controller);
        if (!removed) return false;

        dmConfig.Channels[index] = channel;
        var dto = dmConfig.AdaptToDto();
        var update = dto.Adapt<DmConfigUpdate>();
        var result = await UpdateDeviceManager(update);

        await _deviceManagerConfigResponseSink.SinkAsync(dmConfig.TenantId, dmConfig.ToConfigResponse().ToProtobuf(), CancellationToken.None);

        return result != null;
    }
}
