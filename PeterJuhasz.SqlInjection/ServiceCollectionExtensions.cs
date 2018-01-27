using Microsoft.Extensions.DependencyInjection;
using System;

namespace PeterJuhasz.SqlInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlInjection(this IServiceCollection services, Action<ISqlInjectionDatabaseBuilder> database, Action<ISqlInjectionAttackBuilder> attack)
        {
            services.AddTransient<BlindSqlInjection>();
            services.AddSingleton<SqlWriterOptions>(SqlWriterOptions.Default);
            services.AddTransient<SqlWriter>();

            var builder = new SqlInjectionBuilder(services);
            database(builder);
            attack(builder);

            return services;
        }

        public static ISqlInjectionAttackBuilder UseTimeBased(this ISqlInjectionAttackBuilder builder, TimeBasedBlindSqlInjectionOptions options)
        {
            builder.Services.AddSingleton(options);
            builder.Services.AddScoped<IHypothesisTester, TimeBasedHypothesisTester>();
            return builder;
        }

        public static ISqlInjectionAttackBuilder UseBooleanBased<THypothesisTester>(this ISqlInjectionAttackBuilder builder) where THypothesisTester : class, IHypothesisTester
        {
            builder.Services.AddScoped<IHypothesisTester, THypothesisTester>();
            return builder;
        }

        public class SqlInjectionBuilder : ISqlInjectionDatabaseBuilder, ISqlInjectionAttackBuilder
        {
            public SqlInjectionBuilder(IServiceCollection services)
            {
                Services = services;
            }

            public IServiceCollection Services { get; }
        }
    }
    
    public interface ISqlInjectionDatabaseBuilder
    {
        IServiceCollection Services { get; }
    }

    public interface ISqlInjectionAttackBuilder
    {
        IServiceCollection Services { get; }
    }
}
