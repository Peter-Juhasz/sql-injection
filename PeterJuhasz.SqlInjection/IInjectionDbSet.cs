using Remotion.Linq.Clauses;
using System.Collections.Generic;
using System.Linq;

namespace PeterJuhasz.SqlInjection
{
    public interface IInjectionDbSet : IQueryable, IOrderedQueryable
    {
        QualifiedName Table { get; }

        BlindSqlInjection Injection { get; }

        ICollection<ResultOperatorBase> Operators { get; }
    }

    public interface IInjectionDbSet<out T> : IQueryable<T>, IOrderedQueryable<T>, IInjectionDbSet
    {
    }


    internal static class InjectionDbSetExtensions
    {
        public static IQueryable<T> WithResultOperator<T>(this IQueryable<T> query, ResultOperatorBase @operator)
        {
            (query as IInjectionDbSet).Operators.Add(@operator);
            return query;
        }
    }
}
