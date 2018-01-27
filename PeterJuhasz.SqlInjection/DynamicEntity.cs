using System;

namespace PeterJuhasz.SqlInjection
{
    public class DynamicEntity
    {
        public string this[string column] => String(column);

        public string String(string name) => throw new InvalidOperationException();

        public int Integer(string name) => throw new InvalidOperationException();
    }
}
