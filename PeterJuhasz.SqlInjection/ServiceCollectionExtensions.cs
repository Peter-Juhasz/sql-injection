using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Net.Http;

namespace PeterJuhasz.SqlInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlInjection(this IServiceCollection services, Action<ISqlInjectionDatabaseBuilder> database, Action<ISqlInjectionAttackBuilder> attack)
        {
            services.AddTransient<BlindSqlInjection>();
            services.AddSingleton<SqlWriterOptions>(SqlWriterOptions.Default);
            services.AddTransient<SqlWriter>();
            services.AddSingleton<IPooledObjectPolicy<SqlWriter>, SqlWriterPooledObjectPolicy>();
            services.AddSingleton<ObjectPool<SqlWriter>, DefaultObjectPool<SqlWriter>>();
            services.AddScoped<HttpClient>();

            var builder = new SqlInjectionBuilder(services);
            database(builder);
            attack(builder);

            return services;
        }
        
        internal class SqlInjectionBuilder : ISqlInjectionDatabaseBuilder, ISqlInjectionAttackBuilder
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
