// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.DeviceManager.Db;

public class CommunicationsModel
{
    public string IPAddress { get; set; } = "0.0.0.0";

    public ushort Port { get; set; }

    public ushort SSHPort { get; set; }

    public string SSHHostKey { get; set; } = String.Empty;

    public ushort? CommMode { get; set; }

    public byte? FilteredCommBad { get; set; }

    public byte? FilteredCommMarginal { get; set; }

    public byte? FilteredCommWeightingFactor { get; set; }
}
