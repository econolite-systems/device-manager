// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Reflection;
using Econolite.Ode.Models.DeviceManager.Db;
using Mapster;

namespace Econolite.Ode.Models.DeviceManager;

public class MapsterRegister : ICodeGenerationRegister
{
    public void Register(CodeGenerationConfig config)
    {
        config.AdaptTo("[name]Dto")
            .ApplyDefaultRule();

        config.AdaptFrom("[name]Add", MapType.Map)
            .ApplyDefaultRule()
            .ForType<DmConfig>(cfg => { cfg.Ignore(dmConfig => dmConfig.Id); })
            .ForType<Channel>(cfg => { cfg.Ignore(channel => channel.Id); });

        config.AdaptTo("[name]Update", MapType.MapToTarget)
            .ApplyDefaultRule();

        config.GenerateMapper("[name]Mapper")
            .ForType<DmConfig>()
            .ForType<Channel>()
            .ForType<Controller>()
            .ForType<CommunicationsModel>()
            .ForType<CredentialsModel>();
    }
}

internal static class RegisterExtensions
{
    public static AdaptAttributeBuilder ApplyDefaultRule(this AdaptAttributeBuilder builder)
    {
        return builder
            .ForAllTypesInNamespace(Assembly.GetExecutingAssembly(), "Econolite.Ode.Models.DeviceManager.Db")
            .ExcludeTypes(type => type.IsEnum)
            .AlterType(type => type.IsEnum || Nullable.GetUnderlyingType(type)?.IsEnum == true, typeof(string))
            .ShallowCopyForSameType(true)
            .ForType<DmConfig>(cfg =>
            {
                cfg.Ignore(poco => poco.TenantId);
            });
    }

    // public static AdaptAttributeBuilder IgnoreNoModifyProperties(this AdaptAttributeBuilder builder)
    // {
    //     return builder
    //         .ForType<Enrollment>(cfg => cfg.Ignore(it => it.Student));
    // }
}
