using Microsoft.Extensions.DependencyInjection;
using System;

namespace PeterJuhasz.SqlInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static ISqlInjectionAttackBuilder UseTimeBased(this ISqlInjectionAttackBuilder builder, Func<InjectorBuilder, InjectionOptions> injectorBuilder, TimeBasedBlindSqlInjectionOptions options)
        {
            builder.Services.AddSingleton(options);
            builder.Services.AddScoped<IHypothesisTester, TimeBasedHypothesisTester>();
            builder.Services.AddScoped<InjectionOptions>(_ => injectorBuilder(new InjectorBuilder()));
            builder.Services.AddScoped<ITimeBasedInjector, Injector>();
            return builder;
        }
    }
}
