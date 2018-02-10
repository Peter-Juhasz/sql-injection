using System.Threading;
using System.Threading.Tasks;

namespace PeterJuhasz.SqlInjection
{
    public interface ITimeBasedInjector
    {
        Task InjectAsync(string sql, CancellationToken cancellationToken);
    }
}
