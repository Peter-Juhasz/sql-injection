using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace PeterJuhasz.SqlInjection
{
    public partial class InjectorBuilder
    {
        public InjectionOptions Create(Uri uri)
        {
            return new InjectionOptions(uri);
        }

        public InjectionOptions Create(string uri) => Create(new Uri(uri, UriKind.Absolute));        
    }

    public static class InjectorExtensions
    {
        public static InjectionOptions IntoRouteParameter(this InjectionOptions injector, string parameterName = "sql")
        {
            injector.ParameterName = parameterName;
            return injector;
        }

        public static InjectionOptions IntoQueryStringParameter(this InjectionOptions injector, string parameterName = "sql")
        {
            injector.ParameterName = parameterName;
            return injector;
        }

        public static InjectionOptions IntoForm(this InjectionOptions injector, string fieldName, Func<FormUrlEncodedContent> contentFactory = null)
        {
            injector.ParameterName = fieldName;
            injector.ContentFactory = contentFactory;
            return injector;
        }

        public static InjectionOptions AsString(this InjectionOptions injector, string initialValue = null, QuoteStyle quoteStyle = QuoteStyle.SingleQuote)
        {
            injector.InitialValue = initialValue;
            injector.QuoteStyle = quoteStyle;
            return injector;
        }

        public static InjectionOptions AsInteger(this InjectionOptions injector, int? initialValue = null)
        {
            injector.InitialValue = initialValue;
            return injector;
        }

        public static InjectionOptions UseHttpMethod(this InjectionOptions injector, HttpMethod method)
        {
            injector.Method = method;
            return injector;
        }

        public static InjectionOptions UseGet(this InjectionOptions injector) => injector.UseHttpMethod(HttpMethod.Get);
    }
}
