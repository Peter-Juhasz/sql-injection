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

        public InjectionLocation Location { get; set; }

        public Type ParameterType { get; set; }

        public QuoteStyle? QuoteStyle { get; set; }

        public Uri Uri { get; set; }

        public Func<HttpContent> ContentFactory { get; set; } = () => null;

        public Func<IEnumerable<KeyValuePair<string, string>>> FormFields { get; set; }

        public object InitialValue { get; set; }

        public CommentStyle? CommentStyle { get; set; }

        public bool EndStatementBeforeComment { get; set; } = false;

        public int ParenthesisNestingDepth { get; set; } = 0;
    }
}
