using System;
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

        public IValue Addition(IValue value)
        {
            return new Double(Value + Convert.ToDouble(value.ToObject()));
        }

        public IValue Division(IValue value)
        {
            return new Double(Value/Convert.ToDouble(value.ToObject()));
        }

        public IValue Multiplication(IValue value)
        {
            return new Double(Value*Convert.ToDouble(value.ToObject()));
        }

        public IValue Subtraction(IValue value)
        {
            return new Double(Value - Convert.ToDouble(value.ToObject()));
        }

        public object ToObject()
        {
            return Value;
        }
    }
}
