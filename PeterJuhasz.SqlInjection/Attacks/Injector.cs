using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeterJuhasz.SqlInjection
{
    public class Injector : ITimeBasedInjector, IErrorBasedInjector
    {
        public Injector(InjectionOptions options, HttpClient http)
        {
            Options = options;
            Http = http;
        }

        public InjectionOptions Options { get; }
        public HttpClient Http { get; }


        Task ITimeBasedInjector.InjectAsync(string sql, CancellationToken cancellationToken) =>
            InjectAsync(sql, cancellationToken);

        public Task<HttpResponseMessage> InjectAsync(string sql, CancellationToken cancellationToken)
        {
            // construct uri
            string uri = GetUri(sql);

            using (var request = new HttpRequestMessage(Options.Method, uri))
            using (var content = GetContent(sql))
            {
                request.Content = content;

                return Http.SendAsync(request);
            }
        }

        private HttpContent GetContent(string sql)
        {
            if (Options.ContentFactory != null)
                return Options.ContentFactory();

            if (Options.Location == InjectionLocation.Form)
            {
                var fields = Options.FormFields().ToDictionary(k => k.Key, k => k.Value);
                fields.Add(Options.ParameterName, GetSql(sql));
                var form = new FormUrlEncodedContent(fields);
            }

            return null;
        }

        private string GetUri(string sql)
        {
            switch (Options.Location)
            {
                case InjectionLocation.Route:
                    return Options.Uri.ToString().Replace($"{{{Options.ParameterName}}}", Uri.EscapeDataString(GetSql(sql)));

                case InjectionLocation.QueryString:
                    UriBuilder builder = new UriBuilder(Options.Uri);
                    var query = QueryString.FromUriComponent(Options.Uri);
                    query = query.Add(Options.ParameterName, GetSql(sql));
                    builder.Query = query.Value;
                    return builder.Uri.ToString();
            }

            return Options.Uri.ToString();
        }

        protected string GetSql(string sql)
        {
            StringBuilder sb = new StringBuilder();

            if (this.Options.InitialValue != null)
                sb.Append(this.Options.InitialValue);

            if (this.Options.QuoteStyle != null)
                sb.Append(this.Options.QuoteStyle.Value.ToChar());

            sb.Append('+');
            sb.Append('(');
            sb.Append(sql);
            sb.Append(')');
            sb.Append(" LIMIT 1");

            return sb.ToString();
        }
    }
}
