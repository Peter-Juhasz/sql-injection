namespace PeterJuhasz.SqlInjection
{
    public class SqlWriterOptions
    {
        public bool WriteOptionalSpaces { get; set; } = true;

        public static SqlWriterOptions Default { get; } = new SqlWriterOptions();
    }
}