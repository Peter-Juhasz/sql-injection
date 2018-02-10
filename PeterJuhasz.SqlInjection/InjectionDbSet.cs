using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;

namespace PeterJuhasz.SqlInjection
{
    internal class InjectionDbSet<TData> : IInjectionDbSet<TData>
    {
        /// <summary> 
        /// This constructor is called by the client to create the data source. 
        /// </summary> 
        public InjectionDbSet(BlindSqlInjection injection, QualifiedName table)
        {
            Provider = new InjectionQueryProvider(injection, table);
            Expression = Expression.Constant(this);
            Injection = injection;
            Table = table;
            Operators = new Collection<ResultOperatorBase>();
        }

        /// <summary> 
        /// This constructor is called by Provider.CreateQuery(). 
        /// </summary> 
        /// <param name="expression"></param>
        public InjectionDbSet(InjectionQueryProvider provider, Expression expression, BlindSqlInjection injection, QualifiedName table, ICollection<ResultOperatorBase> operators)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof(IQueryable<TData>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            Provider = provider;
            Expression = expression;
            Injection = injection;
            Table = table;
            Operators = operators;
        }


        public IQueryProvider Provider { get; private set; }
        public Expression Expression { get; private set; }

        public Type ElementType => typeof(TData);

        public BlindSqlInjection Injection { get; }
        public QualifiedName Table { get; }

        public ICollection<ResultOperatorBase> Operators { get; }

        
        public IEnumerator<TData> GetEnumerator() => (Provider.Execute<IEnumerable<TData>>(Expression)).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => (Provider.Execute<System.Collections.IEnumerable>(Expression)).GetEnumerator();
    }
}
