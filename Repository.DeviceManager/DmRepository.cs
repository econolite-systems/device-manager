// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.DeviceManager.Db;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Econolite.Ode.Repository.DeviceManager;

public class DmRepository : GuidDocumentRepositoryBase<DmConfig>, IDmRepository
{
    private readonly IMongoCollection<DmConfig> _dmConfigCollection;

    public DmRepository(IMongoContext mongoContext, ILogger<DmRepository> logger, IConfiguration configuration) : base(mongoContext, logger)
    {
        _dmConfigCollection = mongoContext.GetCollection<DmConfig>(configuration.GetValue("Collections:DeviceManagerConfig", "DmConfig")!);
    }

    public async Task<IEnumerable<DmConfig>> GetAllAsync(Guid tenantId)
    {
        var results = await ExecuteDbSetFuncAsync(collection => collection.FindAsync(i => i.TenantId == tenantId));
        return results?.ToList() ?? new List<DmConfig>();
    }

    public void DeleteChannel(Guid dmId, Guid channelId)
    {
        AddCommandFunc(collection =>
            async (cancellationToken) =>
            {
                var filter = Builders<DmConfig>.Filter.And(Builders<DmConfig>.Filter.Eq(c => c.Id, dmId));
                var update = Builders<DmConfig>.Update.PullFilter(n => n.Channels, Builders<Channel>.Filter.Eq(e => e.Id, channelId));
                var editResult = await collection.UpdateOneAsync(filter, update, default, cancellationToken);
                if (editResult is { IsModifiedCountAvailable: true } &&
                    editResult.MatchedCount != editResult.ModifiedCount)
                    throw new Exception(
                        $"Didn't remove {channelId} from {editResult.MatchedCount - editResult.ModifiedCount} of {editResult.ModifiedCount} parents");
            }
        );
    }

    public async Task<DmConfig?> GetByDmIdAsync(int dmId) 
    {
        var idMatch = Builders<DmConfig>.Filter.Eq(s => s.DmId, dmId);
        var cursor = await _dmConfigCollection.FindAsync(idMatch);
        var configs = await cursor.ToListAsync();
        return configs.FirstOrDefault();
    }
}
