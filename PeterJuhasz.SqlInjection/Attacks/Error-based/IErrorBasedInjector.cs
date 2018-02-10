using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PeterJuhasz.SqlInjection
{
    public interface IErrorBasedInjector
    {
        Task<HttpResponseMessage> InjectAsync(string sql, CancellationToken cancellationToken);
    }
}