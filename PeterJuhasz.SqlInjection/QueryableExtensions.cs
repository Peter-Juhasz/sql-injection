using System.Linq;
using System.Threading.Tasks;

namespace PeterJuhasz.SqlInjection
{
    public static class QueryableExtensions
    {
        public static Task<string> FirstAsync(this IQueryable<string> query, int estimatedLength = 48) =>
            (query as IInjectionDbSet).Injection.GetExpressionValueUsingBinarySearchAsync(query, estimatedLength);

        public static Task<bool> FirstAsync(this IQueryable<bool> query) =>
            (query as IInjectionDbSet).Injection.HypothesisTester.TestAsync(query);

        public static Task<int> FirstAsync(this IQueryable<int> query, int estimation = 1000) =>
            (query as IInjectionDbSet).Injection.GetExpressionValueUsingBinarySearchAsync(query, estimation);
        
        public static Task<int> CountAsync(this IQueryable<int> query, int estimation = 1000)
        {
            return Task.FromResult(0);
        }
    }
}
