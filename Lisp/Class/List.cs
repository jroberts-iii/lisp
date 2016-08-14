using Lisp.Interface;

namespace Lisp.Class
{
    public class List : SExpression, IList
    {
        public static IList Empty = new List();

        private List()
        {
            IsEmpty = true;
            First = null;
            Rest = this;
        }

        private List(ISExpression first, IList rest)
        {
            IsEmpty = false;
            First = first;
            Rest = rest;
        }

        public ISExpression First { get; }
        public bool IsEmpty { get; }
        public IList Rest { get; }

        public IList Cons(ISExpression sExpression)
        {
            return new List(sExpression, this);
        }

        public IList ToList()
        {
            return this;
        }
    }
}
