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

            var booleanValue = sExpression as IBoolean;
            if (booleanValue != null)
            {
                textWriter.Write(booleanValue.Value ? "true" : "false");
                return;
            }

            var doubleValue = sExpression as IDouble;
            if (doubleValue != null)
            {
                textWriter.Write(doubleValue.Value);
                return;
            }

            var integerValue = sExpression as IInteger;
            if (integerValue != null)
            {
                textWriter.Write(integerValue.Value);
                return;
            }

            var keyword = sExpression as Keyword;
            if (keyword != null)
            {
                textWriter.Write(":");
                textWriter.Write(keyword.Name);
                return;
            }

            var stringValue = sExpression as IString;
            if (stringValue != null)
            {
                textWriter.Write("\"");
                textWriter.Write(stringValue.Value);
                textWriter.Write("\"");
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
