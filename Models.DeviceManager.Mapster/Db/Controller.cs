// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.ComponentModel.DataAnnotations;

namespace Econolite.Ode.Models.DeviceManager.Db;

public class Controller
{
    [Required] public Guid Id { get; set; }

    [Required] public string Name { get; set; } = String.Empty;

    [Required] public string Type { get; set; } = String.Empty;

    [Required] public string SubType { get; set; } = String.Empty;

    public CommunicationsModel Communications { get; set; } = new CommunicationsModel();

    public CredentialsModel FTPCredentials { get; set; } = new CredentialsModel();

    public bool? DiscoverDynamicObjects { get; set; }
}
