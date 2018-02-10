using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            Operators = new Collection<ResultOperatorBase>();
        }

        internal BlindSqlInjection Injection { get; }

        internal QualifiedName Table { get; }

        public ICollection<ResultOperatorBase> Operators { get; }


        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type;
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(InjectionDbSet<>).MakeGenericType(elementType), new object[] { this, expression, Injection, Table, Operators });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }
        
        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new InjectionDbSet<TResult>(this, expression, Injection, Table, Operators);
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
