using System;

namespace PeterJuhasz.SqlInjection
{
    public static class MySqlFunctions
    {
        public static string User() => throw new InvalidOperationException();

        public static string Database() => throw new InvalidOperationException();

        public static object Sleep(double seconds) => throw new InvalidOperationException();

        public static object Sleep(int seconds) => throw new InvalidOperationException();

        public static int Length(string str) => throw new InvalidOperationException();

        public static bool Like(this string str, string pattern) => throw new InvalidOperationException();

        public static bool Between(this int expression, int min, int max) => throw new InvalidOperationException();

        public static T If<T>(bool expression, T min, T max) => throw new InvalidOperationException();

        public static int Ascii(string str) => throw new InvalidOperationException();

        public static char Char(int c) => throw new InvalidOperationException();

        public static string Substring(string str, int index, int length) => throw new InvalidOperationException();

        public static object Benchmark(int count, object function) => throw new InvalidOperationException();
    }
}
