// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.DeviceManager.Db;

public class CredentialsModel
{
    public string Username { get; set; } = String.Empty;

    public string Password { get; set; } = String.Empty;

    public string SnmpCommunityName { get; set; } = String.Empty;
}
