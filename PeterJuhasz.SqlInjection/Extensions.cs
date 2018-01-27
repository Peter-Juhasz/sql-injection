using System;
using System.Linq;
using System.Text;

namespace PeterJuhasz.SqlInjection
{
    internal static class Extensions
    {
        public static string ToHex(this byte[] data) => String.Concat(data.Select(b => $"{b:X2}"));

        public static string ToHex(this string data) => Encoding.UTF8.GetBytes(data).ToHex();

        public static string ToHex(this char @char) => @char.ToString().ToHex();
    }
}
