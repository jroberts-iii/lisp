using System.IO;
using Lisp.Interface;

namespace Lisp.Class
{
    public static class Writer
    {
        public static void Write(TextWriter textWriter, ISExpression sExpression)
        {
            if (sExpression == null)
            {
                textWriter.Write("nil");
                return;
            }

            var booleanValue = sExpression as IValue<bool>;
            if (booleanValue != null)
            {
                textWriter.Write(booleanValue.Val ? "true" : "false");
                return;
            }

            var doubleValue = sExpression as IValue<double>;
            if (doubleValue != null)
            {
                textWriter.Write(doubleValue.Val);
                return;
            }

            var integerValue = sExpression as IValue<int>;
            if (integerValue != null)
            {
                textWriter.Write(integerValue.Val);
                return;
            }

            var keyword = sExpression as Keyword;
            if (keyword != null)
            {
                textWriter.Write(":");
                textWriter.Write(keyword.Name);
                return;
            }

            var stringValue = sExpression as IValue<string>;
            if (stringValue != null)
            {
                textWriter.Write("\"");
                textWriter.Write(stringValue.Val);
                textWriter.Write("\"");
                return;
            }

            var value = sExpression as IValue;
            if (value != null)
            {
                textWriter.Write(value.ToString());
                return;
            }

            var symbol = sExpression as Symbol;
            if (symbol != null)
            {
                textWriter.Write(symbol.Name);
                return;
            }

            var list = sExpression as IList;
            if (list != null)
            {
                textWriter.Write("(");
                if (!list.IsEmpty)
                {
                    Write(textWriter, list.First);
                    while (!list.Rest.IsEmpty)
                    {
                        list = list.Rest;
                        textWriter.Write(" ");
                        Write(textWriter, list.First);
                    }
                }

                textWriter.Write(")");
                return;
            }

            var closure = sExpression as IClosure;
            if (closure != null)
            {
                textWriter.Write("<lambda>");
                return;
            }

            var macro = sExpression as IMacro;
            if (macro != null)
            {
                textWriter.Write("<macro>");
                return;
            }

            textWriter.Write(sExpression.ToString());
        }
    }
}
