using Lisp.Interface;

namespace Lisp.Class
{
    public abstract class Identifier : Atom, IIdentifier
    {
        protected Identifier(string name) : this(name, string.Empty)
        {
        }

        protected Identifier(string name, string @namespace)
        {
            Name = name;
            Namespace = @namespace;
        }

        public string FullName => string.IsNullOrEmpty(Namespace)
            ? Name
            : $"{Namespace}.{Name}";

        public string Name { get; }
        public string Namespace { get; }
    }
}
