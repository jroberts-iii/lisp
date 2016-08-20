using System.IO;
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

        public override void Write(TextWriter textWriter)
        {
            var booleanValue = this as IValue<bool>;
            if (booleanValue != null)
            {
                textWriter.Write(booleanValue.Val ? "true" : "false");
                return;
            }

            var doubleValue = this as IValue<double>;
            if (doubleValue != null)
            {
                textWriter.Write(doubleValue.Val);
                return;
            }

            var integerValue = this as IValue<int>;
            if (integerValue != null)
            {
                textWriter.Write(integerValue.Val);
                return;
            }

            var stringValue = this as IValue<string>;
            if (stringValue != null)
            {
                textWriter.Write("\"");
                textWriter.Write(stringValue.Val);
                textWriter.Write("\"");
                return;
            }

            textWriter.Write(Val.ToString());
        }
    }
}
