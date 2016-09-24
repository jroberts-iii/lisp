using Lisp.Interface;

namespace Lisp.Class
{
    public abstract class Collection : SExpression, ICollection
    {
        public abstract bool IsEmpty { get; }
        public abstract IList ToList();
    }
}
