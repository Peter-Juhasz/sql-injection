using System.Linq;

namespace PeterJuhasz.SqlInjection
{
    public interface IInjectionDbSet : IQueryable, IOrderedQueryable
    {
        QualifiedName Table { get; }

        BlindSqlInjection Injection { get; }
    }

    public interface IInjectionDbSet<out T> : IQueryable<T>, IOrderedQueryable<T>, IInjectionDbSet
    {
    }
}
