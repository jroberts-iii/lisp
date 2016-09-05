using Lisp.Interface;

namespace Lisp.Class
{
    public class Keyword : SExpression, IKeyword
    {
        public Keyword(string name) : this(name, string.Empty)
        {
        }

        public Keyword(string name, string ns)
        {
            Name = name;
            Namespace = ns;
        }

        public string FullName => string.IsNullOrEmpty(Namespace)
            ? Name
            : $"{Namespace}.{Name}";

        public string Name { get; }
        public string Namespace { get; }

        public override string ToString()
        {
            return ":" + FullName;
        }
    }
}
