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

            var keyword = sExpression as Keyword;
            if (keyword != null)
            {
                textWriter.Write(":");
                textWriter.Write(keyword.Name);
                return;
            }

            var symbol = sExpression as Symbol;
            if (symbol != null)
            {
                textWriter.Write(symbol.Name);
                return;
            }

            var value = sExpression as IValue;
            if (value != null)
            {
                var boolValue = value.Val as bool?;
                if (boolValue != null)
                {
                    textWriter.Write(boolValue.Value ? "true" : "false");
                    return;
                }

                var stringValue = value.Val as string;
                if (stringValue != null)
                {
                    textWriter.Write("\"");
                    textWriter.Write(stringValue);
                    textWriter.Write("\"");
                    return;
                }

                textWriter.Write(value.Val.ToString());
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

            var lambda = sExpression as ILambda;
            if (lambda != null)
            {
                textWriter.Write("<lambda>");
                return;
            }

            textWriter.Write(sExpression.ToString());
        }
    }
}
