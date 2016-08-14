using System;
using Lisp.Interface;

namespace Lisp.Class
{
    public abstract class Value : SExpression, IValue
    {
        public static IValue<T> Create<T>(T value)
        {
            return new Value<T>(value);
        }
    }

    public class Value<T> : Value, IValue<T>
    {
        public Value(T val)
        {
            Val = val;
        }

        public T Val { get; }

        public override Type GetNativeType()
        {
            return typeof(T);
        }

        public override object GetNativeValue()
        {
            return Val;
        }

        public T OpAddition(int value)
        {
            return default(T);
        }
    }
}
