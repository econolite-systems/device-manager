// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Repository;

namespace Econolite.Ode.Repository.DeviceManager;

public interface ICounterRepository : IRepository
{
    Task<int> GetCounterAsync(Guid tenant, string counterId);
}
