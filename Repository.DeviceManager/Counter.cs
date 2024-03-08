// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Repository.DeviceManager;

public class Counter : IIndexedEntity<string>
{
    public int Seq { get; set; }
    public string Id { get; set; } = string.Empty;
}
