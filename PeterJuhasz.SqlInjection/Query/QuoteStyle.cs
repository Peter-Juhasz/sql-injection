using System;

namespace PeterJuhasz.SqlInjection
{
    public enum QuoteStyle
    {
        SingleQuote,
        DoubleQuote,
        Backtick,
    }

    public static partial class Extensions
    {
        public static char ToChar(this QuoteStyle quoteStyle)
        {
            switch (quoteStyle)
            {
                case QuoteStyle.SingleQuote: return '\'';
                case QuoteStyle.DoubleQuote: return '"';
                case QuoteStyle.Backtick: return '`';
                default: throw new NotSupportedException();
            }
        }
    }
}
