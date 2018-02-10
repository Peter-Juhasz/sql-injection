using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace PeterJuhasz.SqlInjection
{
    public class InjectionOptions
    {
        public InjectionOptions(Uri uri)
        {
            Uri = uri;
        }

        public HttpMethod Method { get; set; }

        public string ParameterName { get; set; }

        public Type ParameterType { get; set; }

        public QuoteStyle? QuoteStyle { get; set; }

        public Uri Uri { get; set; }

        public Func<HttpContent> ContentFactory { get; set; } = () => null;

        public object InitialValue { get; set; }

    }
}
