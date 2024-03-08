// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Models.DeviceManager.Db;

public class DmConfig : IIndexedEntity<Guid>
{
    /// <summary>
    /// A user configurable device id integer.  Lines up to the DmId/Number field in Mobility.
    /// </summary>
    public int DmId { get; set; }

    public string Name { get; set; } = String.Empty;

    public string Location { get; set; } = String.Empty;

    public int Port { get; set; }

    public Guid TenantId { get; set; } = Guid.Empty;

    public List<Channel> Channels { get; set; } = new List<Channel>();

    public Guid Id { get; set; } = Guid.Empty;
}
