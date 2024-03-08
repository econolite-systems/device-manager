// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.DeviceManager.Db;
using Econolite.Ode.Models.DeviceManager.Models;

namespace Econolite.Ode.Domain.Configuration;

public interface IDeviceManagerService
{
    Task<IEnumerable<DmConfigDto>> GetAllAsync(Guid tenantId);
    Task<DmConfigDto?> GetByIdAsync(Guid id);
    Task<DmConfigDto?> AddDeviceManager(Guid tenantId, DmConfigAdd dmConfigAdd);
    Task<DmConfigDto?> UpdateDeviceManager(DmConfigUpdate update);
    Task<bool> DeleteDeviceManager(Guid id);
    Task<ChannelDto?> AddChannelAsync(Guid dmId, ChannelAdd channelAdd);
    Task<ChannelDto?> UpdateChannelAsync(Guid dmId, ChannelUpdate channelUpdate);
    Task<bool> DeleteChannelAsync(Guid dmId, Guid channelId);
    Task<ControllerDto?> AddControllerAsync(Guid dmId, Guid channelId, Controller controllerAdd);
    Task<ControllerDto?> UpdateControllerAsync(Guid dmId, Guid channelId, Controller controllerUpdate);
    Task<bool> DeleteControllerAsync(Guid dmId, Guid channelId, Guid signalId);
}
