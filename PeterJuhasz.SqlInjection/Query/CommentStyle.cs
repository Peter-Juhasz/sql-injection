using System;

namespace PeterJuhasz.SqlInjection
{
    public enum CommentStyle
    {
        MultiLine,
        Hashmark,
        DashDash,
    }

    public static partial class Extensions
    {
        public static string AsString(this CommentStyle quoteStyle)
        {
            switch (quoteStyle)
            {
                case CommentStyle.MultiLine: return "/*";
                case CommentStyle.Hashmark: return "#";
                case CommentStyle.DashDash: return "--";
                default: throw new NotSupportedException();
            }
        }
    }
}
