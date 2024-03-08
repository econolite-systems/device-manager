// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// namespace Econolite.Ode.Model.VehiclePriority.DeviceManager.Db
// {
//     public static class Extensions
//     {
//         public static DmConfigDto ToDto(this DeviceManager dmConfig)
//         {
//             if (dmConfig != null)
//             {
//                 //convert channelDB to channelDTO
//                 var channelsDTO = new List<ChannelDto>();
//                 if (dmConfig.Channels.Any())
//                 {
//                     foreach (var channel in dmConfig.Channels.Where(c => c != null))
//                     {
//                         var convertThis = channel.ToDto();
//                         channelsDTO.Add(convertThis);
//                     }
//                 }
//
//                 return new DmConfigDto
//                 {
//                     Id = dmConfig.Id,
//                     Name = dmConfig.Name,
//                     Location = dmConfig.Location,
//                     Port = dmConfig.Port,
//                     Type = dmConfig.Type,
//                     Channels = channelsDTO,
//                     Number = dmConfig.DmId
//                 };
//             }
//             return null;
//         }
//
//         public static DeviceManager ToDbModel(this DmConfigCreationDto dmConfigDto, Guid tenantid)
//         {
//             return new DeviceManager
//             {
//                 //Id
//                 DmId = dmConfigDto.Number ?? 1,
//                 Name = dmConfigDto.Name,
//                 Location = dmConfigDto.Location,
//                 Port = dmConfigDto.Port,
//                 Type = dmConfigDto.Type,
//                 TenantId = tenantid,
//                 Channels = new List<Channel>(),
//             };
//         }
//
//         public static ChannelDto ToDto(this Channel channel)
//         {
//             return new ChannelDto
//             {
//                 ChannelId = channel.ChannelId,
//                 ChannelType = channel.ChannelType,
//                 CommRequestTimeout = channel.CommRequestTimeout,
//                 Name = channel.Name,
//                 PrimaryPollRate = channel.PrimaryPollRate,
//                 PriorityPollRate = channel.PriorityPollRate,
//                 Protocol = channel.Protocol,
//                 SecondaryPollRate = channel.SecondaryPollRate,
//                 TertiaryPollRate = channel.TertiaryPollRate,
//                 AdaptivePollRate = channel.AdaptivePollRate,
//                 DeviceTimeout = channel.DeviceTimeout,
//                 MaxExpectedPacketSize = channel.MaxExpectedPacketSize,
//                 SourceIPAddress = channel.SourceIPAddress,
//                 SourcePort = channel.SourcePort,
//                 BroadcastIPAddress = channel.BroadcastIPAddress,
//                 DestinationIPAddress = channel.DestinationIPAddress,
//                 PollErrorThreshold = channel.PollErrorThreshold,
//                 Retries = channel.Retries,
//                 PollRetries = channel.PollRetries,
//                 TimeFormat = channel.TimeFormat,
//                 SignalControllers = channel.SignalControllers?.Where(sc => sc != null).Select(sc => sc.ToDto()).ToArray() ?? new ControllerDto[0],
//                 CheckTimeInterval = channel.CheckTimeInterval,
//                 AllowedTimeDrift = channel.AllowedTimeDrift,
//                 BaudRate = 0,
//                 DataBits = 0,
//                 InnerByteDelay = 0,
//                 StopBits = 0,
//                 CommPorts = new int[0],
//                 CtcOnDelay = 0,
//                 FlowControl = Handshake.None,
//                 InnerChannelDelay = false,
//                 InnerDeviceDelay = 0,
//                 RtsOnDelay = 0,
//                 Parity = Parity.None
//             };
//         }
//
//         public static Channel ToDbModel(this ChannelDto channelDto)
//         {
//             return new Channel
//             {
//                 ChannelId = channelDto.ChannelId ?? 0,
//                 ChannelType = channelDto.ChannelType,
//                 CommRequestTimeout = channelDto.CommRequestTimeout,
//                 Name = channelDto.Name,
//                 PrimaryPollRate = channelDto.PrimaryPollRate,
//                 PriorityPollRate = channelDto.PriorityPollRate,
//                 Protocol = channelDto.Protocol,
//                 SecondaryPollRate = channelDto.SecondaryPollRate,
//                 TertiaryPollRate = channelDto.TertiaryPollRate,
//                 AdaptivePollRate = channelDto.AdaptivePollRate,
//                 DeviceTimeout = channelDto.DeviceTimeout,
//                 SourceIPAddress = channelDto.SourceIPAddress,
//                 SourcePort = channelDto.SourcePort,
//                 BroadcastIPAddress = channelDto.BroadcastIPAddress,
//                 DestinationIPAddress = channelDto.DestinationIPAddress,
//                 PollErrorThreshold = channelDto.PollErrorThreshold,
//                 Retries = channelDto.Retries,
//                 TimeFormat = channelDto.TimeFormat,
//                 SignalControllers = channelDto.SignalControllers?.Where(sc => sc != null).Select(sc => sc.ToDbModel()).ToList() ?? new List<Controller>(),
//                 CheckTimeInterval = channelDto.CheckTimeInterval,
//                 AllowedTimeDrift = channelDto.AllowedTimeDrift,
//                 PollRetries = channelDto.PollRetries,
//                 MaxExpectedPacketSize = channelDto.MaxExpectedPacketSize
//                 //BaudRate = 0,
//                 //DataBits = 0,
//                 //InnerByteDelay = 0,
//                 //StopBits = 0,
//                 //CommPorts = new ushort[0],
//                 //CtcOnDelay = 0,
//                 //FlowControl = Handshake.None,
//                 //InnerChannelDelay = false,
//                 //InnerDeviceDelay = 0,
//                 //RtsOnDelay = 0,
//                 //Parity = Parity.None
//             };
//         }
//
//         public static ControllerDto ToDto(this Controller controller)
//         {
//             return new ControllerDto
//             {
//                 Id = controller.Id,
//                 Name = controller.Name,
//                 Type = controller.Type,
//                 SubType = controller.SubType,
//                 Communications = controller.Communications?.ToDto(),
//                 FTPCredentials = controller.FTPCredentials?.ToDto(),
//                 DiscoverDynamicObjects = controller.DiscoverDynamicObjects ?? false
//             };
//         }
//
//         public static Controller ToDbModel(this ControllerDto controller)
//         {
//             return new Controller
//             {
//                 Id=controller.Id,
//                 Name = controller.Name,
//                 Type = controller.Type,
//                 SubType = controller.SubType,
//                 Communications = controller.Communications?.ToDbModel(),
//                 FTPCredentials = controller.FTPCredentials?.ToDbModel(),
//                 DiscoverDynamicObjects = controller.DiscoverDynamicObjects
//             };
//         }
//
//         public static CommunicationsModelDto ToDto(this CommunicationsModel comms)
//         {
//             return new CommunicationsModelDto
//             {
//                 IPAddress = comms.IPAddress,
//                 Port = comms.Port,
//                 SSHPort = comms.SSHPort,
//                 SSHHostKey = comms.SSHHostKey,
//                 CommMode = comms.CommMode,
//                 FilteredCommBad = comms.FilteredCommBad,
//                 FilteredCommMarginal = comms.FilteredCommMarginal,
//                 FilteredCommWeightingFactor = comms.FilteredCommWeightingFactor,
//             };
//         }
//
//         public static CommunicationsModel ToDbModel(this CommunicationsModelDto comms)
//         {
//             return new CommunicationsModel
//             {
//                 IPAddress = comms.IPAddress,
//                 Port = comms.Port,
//                 SSHPort = comms.SSHPort,
//                 SSHHostKey = comms.SSHHostKey,
//                 CommMode = comms.CommMode,
//                 FilteredCommBad = comms.FilteredCommBad,
//                 FilteredCommMarginal = comms.FilteredCommMarginal,
//                 FilteredCommWeightingFactor = comms.FilteredCommWeightingFactor,
//             };
//         }
//
//         public static CredentialsModelDto ToDto(this CredentialsModel creds)
//         {
//             return new CredentialsModelDto
//             {
//                 Username = creds.Username,
//                 Password = creds.Password,
//                 SnmpCommunityName = creds.SnmpCommunityName
//             };
//         }
//
//         public static CredentialsModel ToDbModel(this CredentialsModelDto creds)
//         {
//             return new CredentialsModel
//             {
//                 Username = creds.Username,
//                 Password = creds.Password,
//                 SnmpCommunityName = creds.SnmpCommunityName
//             };
//         }
//
//         public static ScheduleCheckTime ToScheduleCheckTime(this ChannelDto channel, Guid tenantId)
//         {
//             var schedule = new ScheduleCheckTime(){
//                 TenantId = tenantId.ToString(),
//                 DeviceIds = channel.SignalControllers?.Select(c => c.Id.ToString()).ToList(),
//                 Cron = ToCron(channel.CheckTimeInterval),
//                 Payload = channel.ChannelId.ToString(),
//                 LastRun = new DateTime(),
//                 NextRun = DateTime.UtcNow
//             };
//
//             return schedule;
//         }
//
//         public static string ToCron(int intervalMinutes)
//         {
//             var cron = "0 * * * *";
//
//             if(intervalMinutes == 0) return cron;
//
//             if(intervalMinutes < 60)
//             {
//                 cron = $"*/{intervalMinutes} * * * *";
//             }
//             else if(intervalMinutes >= 60 && intervalMinutes < 1440 )
//             {
//                 cron = $"0 */{intervalMinutes/60} * * *";
//             }
//             else
//             {
//                 cron = "0 0 * * *";
//             }
//
//             return cron;
//         }
//     }
// }

