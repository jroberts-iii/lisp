using Lisp.Interface;

namespace Lisp.Class
{
    public class Keyword : Identifier, IKeyword
    {
        public Keyword(string name) : base(name, string.Empty)
        {
        }

        public Keyword(string name, string @namespace) : base(name, @namespace)
        {
        }

        public override string ToString()
        {
            return ":" + FullName;
        }
    }
}
