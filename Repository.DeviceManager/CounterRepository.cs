// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Econolite.Ode.Repository.DeviceManager;

public class CounterRepository : DocumentRepositoryBase<Counter, string>, ICounterRepository
{
    public CounterRepository(IMongoContext context, ILogger<CounterRepository> logger) : base(context, logger)
    {
    }

    public async Task<int> GetCounterAsync(Guid tenant, string counterId)
    {
        var id = $"{tenant}-{counterId}";
        AddCommandFunc(collection =>
            async (cancellationToken) =>
            {
                var update = Builders<Counter>.Update.Inc(c => c.Seq, 1);
                var filter = Builders<Counter>.Filter.Eq(c => c.Id, id);
                var updated = await collection.FindOneAndUpdateAsync(filter, update,
                    new FindOneAndUpdateOptions<Counter> { IsUpsert = true }, cancellationToken);
            }
        );

        var (success, _) = await DbContext.SaveChangesAsync();

        if (!success) return -1;

        var counter = await GetByIdAsync(id);
        if (counter is null)
        {
            return -1;
        }

        return counter.Seq;
    }
}
