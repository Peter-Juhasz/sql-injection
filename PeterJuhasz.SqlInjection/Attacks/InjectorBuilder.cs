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
            injector.Location = InjectionLocation.Route;
            return injector;
        }

        public static InjectionOptions IntoRouteParameter(this InjectionOptions injector, string initialValue, string parameterName = "sql", QuoteStyle quoteStyle = QuoteStyle.SingleQuote)
        {
            injector.ParameterName = parameterName;
            injector.Location = InjectionLocation.Route;
            injector.InitialValue = initialValue;
            injector.ParameterType = typeof(string);
            injector.QuoteStyle = quoteStyle;
            return injector;
        }

        public static InjectionOptions IntoRouteParameter(this InjectionOptions injector, int initialValue, string parameterName = "sql")
        {
            injector.ParameterName = parameterName;
            injector.Location = InjectionLocation.Route;
            injector.InitialValue = initialValue;
            injector.ParameterType = typeof(int);
            return injector;
        }


        public static InjectionOptions IntoQueryStringParameter(this InjectionOptions injector, string parameterName)
        {
            injector.ParameterName = parameterName;
            injector.Location = InjectionLocation.QueryString;
            return injector;
        }

        public static InjectionOptions IntoQueryStringParameter(this InjectionOptions injector, string parameterName, string initialValue, QuoteStyle quoteStyle = QuoteStyle.SingleQuote)
        {
            injector.ParameterName = parameterName;
            injector.Location = InjectionLocation.QueryString;
            injector.InitialValue = initialValue;
            injector.ParameterType = typeof(string);
            injector.QuoteStyle = quoteStyle;
            return injector;
        }

        public static InjectionOptions IntoQueryStringParameter(this InjectionOptions injector, string parameterName, int initialValue)
        {
            injector.ParameterName = parameterName;
            injector.Location = InjectionLocation.QueryString;
            injector.InitialValue = initialValue;
            injector.ParameterType = typeof(int);
            return injector;
        }


        public static InjectionOptions IntoForm(this InjectionOptions injector, string fieldName)
        {
            injector.ParameterName = fieldName;
            injector.Location = InjectionLocation.Form;
            return injector;
        }

        public static InjectionOptions IntoForm(this InjectionOptions injector, string fieldName, string initialValue, Func < IEnumerable<KeyValuePair<string, string>>> formFields = null, QuoteStyle quoteStyle = QuoteStyle.SingleQuote)
        {
            injector.ParameterName = fieldName;
            injector.Location = InjectionLocation.Form;
            injector.FormFields = formFields;
            injector.InitialValue = initialValue;
            injector.ParameterType = typeof(string);
            injector.QuoteStyle = quoteStyle;
            return injector;
        }

        public static InjectionOptions IntoForm(this InjectionOptions injector, string fieldName, int initialValue, Func<IEnumerable<KeyValuePair<string, string>>> formFields = null)
        {
            injector.ParameterName = fieldName;
            injector.Location = InjectionLocation.Form;
            injector.FormFields = formFields;
            injector.InitialValue = initialValue;
            injector.ParameterType = typeof(int);
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

        public static InjectionOptions UsePost(this InjectionOptions injector, Func<HttpContent> contentFactory = null)
        {
            injector.Method = HttpMethod.Post;
            injector.ContentFactory = contentFactory;
            return injector;
        }
    }
}
