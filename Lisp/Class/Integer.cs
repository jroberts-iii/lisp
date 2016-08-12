using Lisp.Interface;

namespace Lisp.Class
{
    public class Integer : IInteger
    {
        public Integer(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
