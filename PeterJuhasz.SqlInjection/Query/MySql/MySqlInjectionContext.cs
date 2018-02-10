using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PeterJuhasz.SqlInjection
{
    public class MySqlInjectionContext
    {
        public MySqlInjectionContext(BlindSqlInjection injection)
        {
            Injection = injection;
        }

        internal BlindSqlInjection Injection { get; }


        public IQueryable<object> Dual => new InjectionDbSet<object>(Injection, "dual");

        public InformationSchemaDbContext InformationSchema => new InformationSchemaDbContext(Injection);

        public MySqlDbContext MySql => new MySqlDbContext(Injection);

        public DynamicDbContext GetDatabase(string name) => new DynamicDbContext(Injection);

        public DynamicDbContext Current => new DynamicDbContext(Injection);


        public class InformationSchemaDbContext
        {
            public InformationSchemaDbContext(BlindSqlInjection injection)
            {
                Injection = injection;
                Tables = new InjectionDbSet<Table>(Injection, new QualifiedName("information_schema", "tables"));
                Columns = new InjectionDbSet<Column>(Injection, new QualifiedName("information_schema", "columns"));
            }

            internal BlindSqlInjection Injection { get; }


            public IQueryable<Table> Tables { get; } 

            public IQueryable<Column> Columns { get; }


            public class Table
            {
                [Column("table_name")]
                public string Name { get; set; }

                [Column("schema_name")]
                public string Schema { get; set; }
            }

            public class Column
            {
                [Column("column_name")]
                public string Name { get; set; }
            }
        }

        public class MySqlDbContext
        {
            public MySqlDbContext(BlindSqlInjection injection)
            {
                Injection = injection;
                Users = new InjectionDbSet<User>(Injection, new QualifiedName("mysql", "user"));
            }

            internal BlindSqlInjection Injection { get; }


            public IQueryable<User> Users { get; }


            public class User
            {
                [Column("User")]
                public string Name { get; set; }

                [Column("Host")]
                public string Host { get; set; }

                [Column("authentication_string")]
                public string AuthenticationString { get; set; }

                [Column("account_locked")]
                public bool AccountLocked { get; set; }

                [Column("password_expired")]
                public bool PasswordExpired { get; set; }
            }
        }
    }
}
