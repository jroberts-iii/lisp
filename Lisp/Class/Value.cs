using Lisp.Interface;

namespace Lisp.Class
{
    public class Value : IValue
    {
        public Value(object val)
        {
            Val = val;
        }

        public object Val { get; }
    }
}
