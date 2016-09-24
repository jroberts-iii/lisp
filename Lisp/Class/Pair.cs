using Lisp.Interface;

namespace Lisp.Class
{
    public class Pair : Atom, IPair
    {
        public Pair(object key, object value)
        {
            Key = key;
            Value = value;
        }

        public object Key { get; }
        public object Value { get; }
    }
}
