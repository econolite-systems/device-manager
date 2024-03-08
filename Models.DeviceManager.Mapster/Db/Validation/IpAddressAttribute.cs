// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Econolite.Ode.Models.DeviceManager.Db.Validation;

public class IpAddressAttribute : ValidationAttribute
{
    public override string FormatErrorMessage(string name)
    {
        return "The field " + name + " must be a valid IP address.";
    }

    public override bool IsValid(object? value)
    {
        return value != null && IPAddress.TryParse((string) value, out _);
    }
}
