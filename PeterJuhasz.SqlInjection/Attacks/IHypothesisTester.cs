using System.Linq;
using System.Threading.Tasks;

namespace PeterJuhasz.SqlInjection
{
    public interface IHypothesisTester
    {
        Task<bool> TestAsync(IQueryable<bool> hypothesis);
    }
}