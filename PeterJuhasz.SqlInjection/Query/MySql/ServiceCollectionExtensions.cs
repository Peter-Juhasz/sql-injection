using Microsoft.Extensions.DependencyInjection;

namespace PeterJuhasz.SqlInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static ISqlInjectionDatabaseBuilder UseMySql(this ISqlInjectionDatabaseBuilder builder)
        {
            builder.Services.AddTransient<SqlWriter, MySqlWriter>();
            builder.Services.AddTransient<IErrorExpressionProvider, ExponentFunctionErrorExpressionProvider>();
            builder.Services.AddTransient<IErrorDetector, PhpErrorDetector>();
            return builder;
        }

        public static IErrorBuilder UseExponentiationOverflow(this IErrorBuilder builder)
        {
            builder.Services.AddTransient<IErrorExpressionProvider, ExponentFunctionErrorExpressionProvider>();
            return builder;
        }
    }
}
