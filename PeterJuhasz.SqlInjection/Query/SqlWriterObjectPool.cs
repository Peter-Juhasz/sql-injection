using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using System;

namespace PeterJuhasz.SqlInjection
{
    internal class SqlWriterPooledObjectPolicy : IPooledObjectPolicy<SqlWriter>
    {
        public SqlWriterPooledObjectPolicy(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public SqlWriter Create() => ServiceProvider.GetRequiredService<SqlWriter>();

        public bool Return(SqlWriter obj)
        {
            obj.Clear();
            return true;
        }
    }
}
