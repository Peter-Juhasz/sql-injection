using Microsoft.Extensions.DependencyInjection;

namespace PeterJuhasz.SqlInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static ISqlInjectionDatabaseBuilder UseMySql(this ISqlInjectionDatabaseBuilder builder)
        {
            builder.Services.AddTransient<SqlWriter, MySqlWriter>();
            return builder;
        }
    }
}
