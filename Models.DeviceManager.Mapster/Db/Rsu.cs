// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.DeviceManager.Db;

public class Rsu
{
    public string IPAddress { get; set; } = "0.0.0.0";
    public ushort Port { get; set; }
    public string Username { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
}
