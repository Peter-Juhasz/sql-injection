using Microsoft.Extensions.DependencyInjection;
using System;

namespace PeterJuhasz.SqlInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static ISqlInjectionAttackBuilder UseBooleanBased<THypothesisTester>(this ISqlInjectionAttackBuilder builder) where THypothesisTester : class, IHypothesisTester
        {
            builder.Services.AddScoped<IHypothesisTester, THypothesisTester>();
            return builder;
        }
    }
}
