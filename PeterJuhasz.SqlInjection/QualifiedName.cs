using System;

namespace PeterJuhasz.SqlInjection
{
    public struct QualifiedName
    {
        public QualifiedName(params string[] parts)
        {
            Parts = parts;
        }

        public string[] Parts { get; }

        public static implicit operator QualifiedName(string name) => new QualifiedName(name);

        public override string ToString() => String.Join(".", Parts);
    }

}