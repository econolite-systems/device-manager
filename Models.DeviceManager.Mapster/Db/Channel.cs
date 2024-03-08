// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Config.Channels;
using Econolite.Ode.Config.Devices;
using System.ComponentModel.DataAnnotations;

namespace Econolite.Ode.Models.DeviceManager.Db;

public class Channel
{
    public Guid Id { get; set; }

    public int ChannelId { get; set; }

    public string Name { get; set; } = String.Empty;

    public ChannelType ChannelType { get; set; }

    public Protocol Protocol { get; set; }

    /// <summary>
    ///     Gets or sets the timeout for a device response.
    /// </summary>
    public int CommRequestTimeout { get; set; }

    [Range(0, 100000)] public int PrimaryPollRate { get; set; }

    [Range(0, 100000)] public int SecondaryPollRate { get; set; }

    [Range(0, 100000)] public int TertiaryPollRate { get; set; }

    [Range(0, 100000)] public int AdaptivePollRate { get; set; }

    public int PriorityPollRate { get; set; }

    [Range(0, 10000)] public int DeviceTimeout { get; set; }

    [Range(0, 30000)] public int MaxExpectedPacketSize { get; set; }

    /// <summary>
    ///     Gets or sets the IP Address of the channel.
    /// </summary>
    public string SourceIPAddress { get; set; } = String.Empty;

    /// <summary>
    ///     Gets or sets the port of the channel.
    /// </summary>
    public int SourcePort { get; set; }

    public string BroadcastIPAddress { get; set; } = String.Empty;

    public string DestinationIPAddress { get; set; } = String.Empty;

    public int DestinationPort { get; set; }

    [Range(0, 25)] public int PollErrorThreshold { get; set; }

    [Range(0, 25)] public int Retries { get; set; }

    [Range(0, 25)] public int PollRetries { get; set; }

    public DeviceTimeFormat TimeFormat { get; set; }

    public int CheckTimeInterval { get; set; } = 60;

    public int AllowedTimeDrift { get; set; } = 5;

    public List<Controller> SignalControllers { get; set; } = new List<Controller>();
}
