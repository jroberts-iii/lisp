using System;
using Lisp.Interface;

namespace Lisp.Class
{
    public class String : IString
    {
        public String(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public IValue Addition(IValue value)
        {
            return new String(Value + value.ToObject());
        }

        public IValue Division(IValue value)
        {
            throw new NotImplementedException();
        }

        public IValue Multiplication(IValue value)
        {
            throw new NotImplementedException();
        }

        public IValue Subtraction(IValue value)
        {
            throw new NotImplementedException();
        }

        public object ToObject()
        {
            return Value;
        }
    }
}
