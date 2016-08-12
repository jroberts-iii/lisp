using Lisp.Interface;

namespace Lisp.Class
{
    public class Double : IDouble
    {
        public Double(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }
}
