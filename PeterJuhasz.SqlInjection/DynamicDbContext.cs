namespace PeterJuhasz.SqlInjection
{
    public class DynamicDbContext
    {
        public DynamicDbContext(BlindSqlInjection injection)
        {
            Injection = injection;
        }

        internal BlindSqlInjection Injection { get; }


        public IInjectionDbSet<DynamicEntity> GetTable(string name) => new InjectionDbSet<DynamicEntity>(Injection, name);
    }
}
