using System.Globalization;

namespace PeterJuhasz.SqlInjection
{
    public class MySqlWriter : SqlWriter
    {
        public MySqlWriter(SqlWriterOptions options)
            : base(options)
        { }
        
        
        private static CultureInfo culture = new CultureInfo("en-us");

        
        public override void WriteIdentifier(string name) => Write(name);

        public override void WriteIdentifier(QualifiedName name)
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
        

        public override void WriteString(string str) => Write($"'{EscapeSingleQuote(str)}'");
        
        

        public new static string EscapeSingleQuote(string literal) => literal.Replace("'", "''").Replace("\\", "\\\\");


        public new static MySqlWriter Default { get; } = new MySqlWriter(SqlWriterOptions.Default);
    }
}
