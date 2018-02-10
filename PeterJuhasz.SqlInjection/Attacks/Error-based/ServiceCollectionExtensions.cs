using Microsoft.Extensions.DependencyInjection;
using System;

namespace PeterJuhasz.SqlInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static IErrorBasedSqlInjectionAttackBuilder UseErrorBased(
            this ISqlInjectionAttackBuilder builder,
            Func<InjectorBuilder, InjectionOptions> injectorBuilder,
            Func<IErrorBuilder, IErrorBuilder> errorBuilder,
            Func<IErrorDetectorBuilder, IErrorDetectorBuilder> errorDetector
        )
        {
            builder.Services.AddScoped<IHypothesisTester, ErrorBasedHypothesisTester>();
            builder.Services.AddScoped<InjectionOptions>(_ => injectorBuilder(new InjectorBuilder()));
            builder.Services.AddScoped<IErrorBasedInjector, Injector>();
            var b = new ErrorBasedSqlInjectionBuilder(builder.Services);
            errorBuilder(b);
            errorDetector(b);
            return b;
        }

        public static IErrorDetectorBuilder UsePhp(this IErrorDetectorBuilder builder)
        {
            builder.Services.AddSingleton<IErrorDetector, PhpErrorDetector>();
            return builder;
        }


        internal class ErrorBasedSqlInjectionBuilder : IErrorBasedSqlInjectionAttackBuilder, IErrorBuilder, IErrorDetectorBuilder
        {
            public ErrorBasedSqlInjectionBuilder(IServiceCollection services)
            {
                Services = services;
            }

            public IServiceCollection Services { get; }
        }
    }

    public interface IErrorBasedSqlInjectionAttackBuilder
    {
        IServiceCollection Services { get; }
    }

    public interface IErrorBuilder
    {
        IServiceCollection Services { get; }
    }

    public interface IErrorDetectorBuilder
    {
        IServiceCollection Services { get; }
    }
}
