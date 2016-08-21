using System.IO;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Value : SExpression, IValue
    {
        public Value(object val)
        {
            Val = val;
        }

        public object Val { get; }

        public override void Write(TextWriter textWriter)
        {
            if (Val is bool)
            {
                textWriter.Write((bool) Val
                    ? "true"
                    : "false");
                return;
            }

            if (Val is string)
            {
                textWriter.Write("\"");
                textWriter.Write(Val);
                textWriter.Write("\"");
                return;
            }

            textWriter.Write(Val.ToString());
        }
    }
}
