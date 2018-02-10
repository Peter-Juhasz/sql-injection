using System;
using System.Threading.Tasks;

namespace PeterJuhasz.SqlInjection
{
    public class TimeBasedBlindSqlInjectionOptions
    {
        public TimeSpan InjectedWaitTime { get; set; }

        public TimeSpan SuccessfulTime { get; set; }
    }
}