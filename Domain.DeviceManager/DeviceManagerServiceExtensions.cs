// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Econolite.Ode.Config.Messaging;
using Econolite.Ode.Models.DeviceManager.Db;
using Econolite.Ode.Config.Channels;
using Econolite.Ode.Config.Devices;
using Econolite.Ode.Models.Entities.Interfaces;


namespace Econolite.Ode.Domain.DeviceManager;

public static class DeviceManagerServiceExtensions
{
    public static ConfigResponse ToConfigResponse(this DmConfig dmConfigDto)
    {
        return new ConfigResponse
        {
            Channels = dmConfigDto.Channels.Select(_ => _.ToConfig()).ToArray(),
            Devices = dmConfigDto.Channels.SelectMany(c => c.SignalControllers.Select(_ => _.ToConfig(c))).ToArray(),
            DeviceManagerId = dmConfigDto.DmId
        };
    }

    public static BaseChannelConfig ToConfig(this Channel channel)
    {
        //these fields aren't used in ODE so setting to defaults
        var innerChannelDelay = false;
        var innerByteDelay = 0;
        var innerDeviceDelay = 0;

        BaseChannelConfig result;
        switch (channel.ChannelType)
        {
            case ChannelType.Udp:
                result = new UdpConfig(id: channel.ChannelId, name: channel.Name, protocol: channel.Protocol, retries: channel.Retries,
                    pollRetries: channel.PollRetries, innerByteDelay: innerByteDelay, innerDeviceDelay: innerDeviceDelay,
                    innerChannelDelay: innerChannelDelay, commRequestTimeout: channel.DeviceTimeout, byteRxTimeout: channel.DeviceTimeout,
                    maxExpectedPacketSize: channel.MaxExpectedPacketSize, pollErrorThreshold: channel.PollErrorThreshold, failedPollRate: channel.AdaptivePollRate,
                    primaryPollRate: channel.PrimaryPollRate, secondaryPollRate: channel.SecondaryPollRate, priorityPollRate: channel.PriorityPollRate,
                    tertiaryPollRate: channel.TertiaryPollRate, broadcastIpAddress: channel.BroadcastIPAddress, timeFormat: channel.TimeFormat,
                    sourceIp: channel.SourceIPAddress, sourcePort: channel.SourcePort);
                break;

            case ChannelType.SharedUdp:
                result = new SharedUdpConfig(id: channel.ChannelId, name: channel.Name, protocol: channel.Protocol, retries: channel.Retries,
                    pollRetries: channel.PollRetries, innerByteDelay: innerByteDelay, innerDeviceDelay: innerDeviceDelay,
                    innerChannelDelay: innerChannelDelay, commRequestTimeout: channel.DeviceTimeout, byteRxTimeout: channel.DeviceTimeout,
                    maxExpectedPacketSize: channel.MaxExpectedPacketSize, pollErrorThreshold: channel.PollErrorThreshold, failedPollRate: channel.AdaptivePollRate,
                    primaryPollRate: channel.PrimaryPollRate, secondaryPollRate: channel.SecondaryPollRate, priorityPollRate: channel.PriorityPollRate,
                    tertiaryPollRate: channel.TertiaryPollRate, broadcastIpAddress: channel.BroadcastIPAddress, timeFormat: channel.TimeFormat,
                    sourceIp: channel.SourceIPAddress, sourcePort: channel.SourcePort);
                break;
            case ChannelType.SerialOverUdp:
                result = new SerialOverUdpConfig(id: channel.ChannelId, name: channel.Name, protocol: channel.Protocol, retries: channel.Retries,
                    pollRetries: channel.PollRetries, innerByteDelay: innerByteDelay, innerDeviceDelay: innerDeviceDelay,
                    innerChannelDelay: innerChannelDelay, commRequestTimeout: channel.DeviceTimeout, byteRxTimeout: channel.DeviceTimeout,
                    maxExpectedPacketSize: channel.MaxExpectedPacketSize, pollErrorThreshold: channel.PollErrorThreshold, failedPollRate: channel.AdaptivePollRate,
                    primaryPollRate: channel.PrimaryPollRate, secondaryPollRate: channel.SecondaryPollRate, priorityPollRate: channel.PriorityPollRate,
                    tertiaryPollRate: channel.TertiaryPollRate, broadcastIpAddress: channel.BroadcastIPAddress, timeFormat: channel.TimeFormat,
                    sourceIp: channel.SourceIPAddress, sourcePort: channel.SourcePort, channel.DestinationIPAddress, channel.DestinationPort);
                break;
            default:
                throw new ArgumentOutOfRangeException(paramName: channel.ChannelType.ToString());
        }

        return result;
    }

    public static BaseDeviceConfig ToConfig(this Controller controller, Channel channel)
    {
        //for now there's only one device type that we send to the device manager and that's ESS; signals are not using the device manager
        Enum.TryParse(controller.SubType, out EssType essType);
        BaseDeviceConfig result = new EssConfig(externalTag: controller.Name, deviceId: controller.Id, deviceSubType: (byte)essType,
            commMode: (CommMode)(controller.Communications.CommMode ?? 0), channelType: channel.ChannelType, protocol: channel.Protocol,
            deviceIp: controller.Communications.IPAddress, devicePort: controller.Communications.Port, deviceDropAddress: null, deviceRetries: channel.Retries, devicePollRetries: channel.PollRetries,
            primaryPollRate: channel.PrimaryPollRate, secondaryPollRate: channel.SecondaryPollRate, tertiaryPollRate: channel.TertiaryPollRate, priorityPollRate: channel.PriorityPollRate,
            failedPollRate: channel.AdaptivePollRate, channelId: channel.ChannelId, pollErrorThreshold: channel.PollErrorThreshold, commRequestTimeout: channel.DeviceTimeout,
            deviceTimeFormat: channel.TimeFormat, broadcastIpAddress: channel.BroadcastIPAddress,
            snmpCommunityName: controller.FTPCredentials.SnmpCommunityName, ftpUsername: controller.FTPCredentials.Username,
            ftpPassword: controller.FTPCredentials.Password, sshHostKey: controller.Communications.SSHHostKey, sshPort: controller.Communications.SSHPort, maxVbsPerPdu: 6, backupPreventPeriod: TimeSpan.FromMinutes(value: 1),
            filteredCommWeightingFactor: controller.Communications.FilteredCommWeightingFactor ?? 0,
            filteredCommMarginal: controller.Communications.FilteredCommMarginal ?? 0, filteredCommBad: controller.Communications.FilteredCommBad ?? 0,
            allowTimeDrift: TimeSpan.FromSeconds(channel.AllowedTimeDrift));

        return result;
    }

    public static IServiceCollection AddDmService(this IServiceCollection services) => services
        .AddScoped<IDeviceManagerService, DeviceManagerService>()
        .AddScoped<IEntityConfigUpdate, EntityConfigUpdate>();
}
