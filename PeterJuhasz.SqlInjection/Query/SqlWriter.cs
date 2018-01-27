using System.Globalization;
using System.IO;
using System.Text;

namespace PeterJuhasz.SqlInjection
{
    public class SqlWriter : TextWriter
    {
        public SqlWriter(SqlWriterOptions options)
        {
            Options = options;
        }

        public SqlWriterOptions Options { get; }


        private StringBuilder builder = new StringBuilder();


        public override Encoding Encoding => Encoding.UTF8;

        private static CultureInfo culture = new CultureInfo("en-us");


        public override void Write(string text) => builder.Append(text);

        public virtual void WriteKeyword(string text) => Write(text);

        public virtual void WriteWhiteSpace() => Write(" ");

        public virtual void WriteIdentifier(string name) => Write(name);

        public virtual void WriteIdentifier(QualifiedName name)
        {
            bool first = true;
            foreach (var sub in name.Parts)
            {
                if (!first)
                    Write(".");

                WriteIdentifier(sub);

                first = false;
            }
        }

        public virtual void WriteOptionalWhiteSpace()
        {
            if (Options.WriteOptionalSpaces)
                WriteWhiteSpace();
        }

        public virtual void WriteString(string str) => Write($"'{EscapeSingleQuote(str)}'");

        public virtual void WriteNumber(int str) => Write(str);

        public virtual void WriteNumber(double str) => Write(str.ToString(culture));

        public virtual void WriteBoolean(bool b) => WriteKeyword(b.ToString());


        public void Clear() => builder.Clear();

        public override string ToString() => builder.ToString();


        public static string EscapeSingleQuote(string literal) => literal.Replace("'", "''");


        public static SqlWriter Default { get; } = new SqlWriter(SqlWriterOptions.Default);
    }
}
