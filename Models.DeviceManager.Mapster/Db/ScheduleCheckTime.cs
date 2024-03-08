// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.DeviceManager.Db;

public class ScheduleCheckTime
{
    public string Id { get; set; } = String.Empty;

    public string TenantId { get; set; } = String.Empty;

    public string Cron { get; set; } = "0 * * * *";

    public string Command { get; set; } = "CheckTime";

    public List<string> DeviceIds { get; set; } = new List<string>();

    public string Payload { get; set; } = string.Empty;

    public DateTime LastRun { get; set; }

    public DateTime NextRun { get; set; }
}
