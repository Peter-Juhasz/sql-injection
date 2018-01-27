using System;
using System.Threading.Tasks;

namespace PeterJuhasz.SqlInjection
{
    public class TimeBasedBlindSqlInjectionOptions
    {
        public Func<string, Task> InjectAsync { get; set; }

        public TimeSpan InjectedWaitTime { get; set; }

        public TimeSpan SuccessfulTime { get; set; }
    }
}