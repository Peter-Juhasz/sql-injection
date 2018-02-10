using System;

namespace PeterJuhasz.SqlInjection
{
    public class DynamicEntity
    {
        public string this[string column] => Value<string>(column);

        public T Value<T>(string name) => throw new InvalidOperationException();
    }
}
