using System;
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

        public IValue Addition(IValue value)
        {
            return new Integer(Value + Convert.ToInt32(value.ToObject()));
        }

        public IValue Division(IValue value)
        {
            return new Integer(Value/Convert.ToInt32(value.ToObject()));
        }

        public IValue Multiplication(IValue value)
        {
            return new Integer(Value*Convert.ToInt32(value.ToObject()));
        }

        public IValue Subtraction(IValue value)
        {
            return new Integer(Value - Convert.ToInt32(value.ToObject()));
        }

        public object ToObject()
        {
            return Value;
        }
    }
}
