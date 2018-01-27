using System;
using System.Linq;
using System.Linq.Expressions;

namespace PeterJuhasz.SqlInjection
{
    internal class InjectionQueryProvider : IQueryProvider
    {
        public InjectionQueryProvider(BlindSqlInjection injection, QualifiedName table)
        {
            Injection = injection;
            Table = table;
        }

        internal BlindSqlInjection Injection { get; }

        internal QualifiedName Table { get; }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type;
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(InjectionDbSet<>).MakeGenericType(elementType), new object[] { this, expression, Injection, Table });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }
        
        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new InjectionDbSet<TResult>(this, expression, Injection, Table);
        }

        public object Execute(Expression expression)
        {
            throw new NotSupportedException();
        }
        
        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotSupportedException();
        }
    }
}
