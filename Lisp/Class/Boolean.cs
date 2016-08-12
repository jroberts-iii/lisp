using Lisp.Interface;

namespace Lisp.Class
{
    public class Boolean : IBoolean
    {
        public Boolean(bool value)
        {
            Value = value;
        }

        public bool Value { get; }
    }
}
