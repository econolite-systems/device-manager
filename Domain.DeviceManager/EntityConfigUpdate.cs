// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Domain.Configuration;
using Econolite.Ode.Models.DeviceManager.Db;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Interfaces;

namespace Econolite.Ode.Domain.DeviceManager;

public class EntityConfigUpdate : IEntityConfigUpdate
{
    private readonly IDeviceManagerService _deviceManagerService;

    public EntityConfigUpdate(IDeviceManagerService deviceManagerService)
    {
        _deviceManagerService = deviceManagerService;
    }
    
    public async Task Add(IEntityService service, EntityNode entity)
    {
        if (!entity.IsController()) return;

        await _deviceManagerService.AddControllerAsync( entity.DeviceManager!.Value, entity.Channel!.Value, entity.ToController());
    }

    public async Task Update(IEntityService service, EntityNode entity)
    {
        if (!entity.IsController()) return;
        await _deviceManagerService.UpdateControllerAsync(entity.DeviceManager!.Value, entity.Channel!.Value, entity.ToController());
    }

    public async Task Delete(IEntityService service, EntityNode entity)
    {
        if (!entity.IsController()) return;
        await _deviceManagerService.DeleteControllerAsync(entity.DeviceManager!.Value, entity.Channel!.Value, entity.Id);
    }
}

public static class EntityConfigUpdateExtensions
{
    public static Controller ToController(this EntityNode node)
    {
        var results = new Controller
        {
            Id = node.Id,
            Name = node.Name,
            Type = node.ControllerType ?? string.Empty,
            SubType = string.Empty,
            Communications = node.ToAddCommunicationsModel(),
            FTPCredentials = node.ToAddCredentialsModel(),
            DiscoverDynamicObjects = false
        };
        return results;
    }

    private static CommunicationsModel ToAddCommunicationsModel(this EntityNode node)
    {
        var results = new CommunicationsModel
        {
            IPAddress = node.IPAddress,
            Port = node.Port.HasValue ? Convert.ToUInt16(node.Port.Value) : (ushort) 0,
            SSHPort = node.SSHPort.HasValue ? Convert.ToUInt16(node.SSHPort.Value) : (ushort) 0,
            SSHHostKey = node.SSHHostKey ?? string.Empty,
            CommMode = (ushort?) node.CommMode,
            FilteredCommBad = node.FilteredCommBad,
            FilteredCommMarginal = node.FilteredCommMarginal,
            FilteredCommWeightingFactor = node.FilteredCommWeightingFactor
        };
        return results;
    }
    
    private static CredentialsModel ToAddCredentialsModel(this EntityNode node)
    {
        var results = new CredentialsModel
        {
            Username = node.Username,
            Password = node.Password,
            SnmpCommunityName = "public",
        };
        return results;
    }
    
    public static bool IsController(this EntityNode entity)
    {
        return !string.IsNullOrWhiteSpace(entity.ControllerType) &&
               entity.ControllerType != "SPAT" &&
               entity is {DeviceManager: not null, Channel: not null};
    }
}
