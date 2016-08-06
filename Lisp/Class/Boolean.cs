using System;
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

        public IValue Addition(IValue value)
        {
            return new Integer(Value ? 1 : 0 + Convert.ToInt32(value.ToObject()));
        }

        public IValue Division(IValue value)
        {
            return new Integer(Value ? 1 : 0/Convert.ToInt32(value.ToObject()));
        }

        public IValue Multiplication(IValue value)
        {
            return new Integer(Value ? 1 : 0*Convert.ToInt32(value.ToObject()));
        }

        public IValue Subtraction(IValue value)
        {
            return new Integer(Value ? 1 : 0 - Convert.ToInt32(value.ToObject()));
        }

        public object ToObject()
        {
            return Value;
        }
    }
}
