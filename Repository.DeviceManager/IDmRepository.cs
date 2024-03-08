// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.DeviceManager.Db;
using Econolite.Ode.Persistence.Common.Repository;

namespace Econolite.Ode.Repository.DeviceManager;

public interface IDmRepository : IRepository<DmConfig, Guid>
{
    Task<IEnumerable<DmConfig>> GetAllAsync(Guid tenantId);
    void DeleteChannel(Guid dmId, Guid channelId);
    Task<DmConfig?> GetByDmIdAsync(int dmId);
}
