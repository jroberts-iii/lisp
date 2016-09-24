using Lisp.Interface;

namespace Lisp.Class
{
    public class Value : Atom, IValue
    {
        public Value(object val)
        {
            Val = val;
        }

        public object Val { get; }

        public override string ToString()
        {
            if (Val is bool)
            {
                return (bool) Val
                    ? "true"
                    : "false";
            }

            if (Val is string)
            {
                return "\"" + Val + "\"";
            }

            return Val.ToString();
        }
    }
}
