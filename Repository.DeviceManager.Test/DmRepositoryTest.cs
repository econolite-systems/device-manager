// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Econolite.Ode.Models.DeviceManager.Db;
using Econolite.Ode.Persistence.Mongo.Test.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Econolite.Ode.Repository.DeviceManager.Test;

public class DmRepositoryTest : IdRepositoryBaseTest<Guid, DmRepository, DmConfig>, IClassFixture<MongoFixture>
{
    private readonly ILogger<DmRepository> _logger = Mock.Of<ILogger<DmRepository>>();
    private readonly IConfiguration _configuration = Mock.Of<IConfiguration>();

    public DmRepositoryTest(MongoFixture fixture) : base(fixture)
    {
    }

    protected override Guid Id { get; } = Guid.NewGuid();

    protected override string ExpectedJsonIdFilter => "UUID(\"" + Id + "\")";

    protected override DmRepository CreateRepository()
    {
        var configurationSectionMock = new Mock<IConfigurationSection>();
        configurationSectionMock
            .Setup(x => x.Value)
            .Returns("DmConfig");
        Mock.Get(_configuration)
            .Setup(x => x.GetSection(It.IsAny<string>()))
            .Returns(configurationSectionMock.Object);
        return new DmRepository(Context, _logger, _configuration);
    }

    protected override DmConfig CreateDocument()
    {
        return new DmConfig
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Location = Guid.NewGuid().ToString(),
            DmId = 1,
            TenantId = Guid.NewGuid()
        };
    }
}
