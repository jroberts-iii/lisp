using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public static class Reader
    {
        private static readonly Dictionary<char, SExpressionReader> CharToSExpressionReader =
            new Dictionary<char, SExpressionReader>
            {
                {';', CommentReader},
                {'\0', EndReader},
                {'(', ListReader},
                {')', ListTerminatorReader},
                {'\'', QuoteReader},
                {'"', StringReader},
                {'`', SyntaxQuoteReader},
                {'~', UnquoteReader}
            };

        public static ISExpression Read(ITextReader textReader)
        {
            return GetSExpressionReader(textReader)(textReader);
        }

        public static string ReadLine(this ITextReader @this)
        {
            var stringBuilder = new StringBuilder();

            var currentChar = @this.Read();
            while ((currentChar != '\0') && (currentChar != '\n') && (currentChar != '\r'))
            {
                stringBuilder.Append(currentChar);
                currentChar = @this.Read();
            }

            if (currentChar == '\r')
            {
                if (@this.Peek() == '\n')
                {
                    @this.Read();
                }
            }

            return stringBuilder.ToString();
        }

        private static ISExpression AtomReader(ITextReader textReader)
        {
            var stringBuilder = new StringBuilder();
            while (true)
            {
                var nextChar = textReader.Peek();

                SExpressionReader sExpressionReader;
                if (nextChar.IsWhiteSpace() || CharToSExpressionReader.TryGetValue(nextChar, out sExpressionReader))
                {
                    break;
                }

                stringBuilder.Append(textReader.Read());
            }

            var value = stringBuilder.ToString();
            switch (value)
            {
                case "false":
                    return Constants.False;

                case "nil":
                    return null;

                case "true":
                    return Constants.True;

                default:
                    double doubleValue;
                    if (double.TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out doubleValue))
                    {
                        return new Value((int) doubleValue);
                    }

                    if (double.TryParse(value, NumberStyles.Float, NumberFormatInfo.CurrentInfo, out doubleValue))
                    {
                        return new Value(doubleValue);
                    }

                    var name = stringBuilder.ToString();
                    if (name[0] == ':')
                    {
                        return new Keyword(name.Substring(1));
                    }

                    return new Symbol(name);
            }
        }

        private static ISExpression CommentReader(ITextReader textReader)
        {
            var nextChar = textReader.Read();
            Trace.Assert(nextChar == ';');

            textReader.ReadLine();
            return Read(textReader);
        }

        private static ISExpression EndReader(ITextReader textReader)
        {
            return null;
        }

        private static SExpressionReader GetSExpressionReader(ITextReader textReader)
        {
            SExpressionReader sExpressionReader;
            textReader.ReadWhiteSpace();
            return CharToSExpressionReader.TryGetValue(textReader.Peek(), out sExpressionReader)
                ? sExpressionReader
                : AtomReader;
        }

        private static bool IsWhiteSpace(this char @this)
        {
            return (@this == ',') || char.IsWhiteSpace(@this);
        }

        private static IList ListReader(ITextReader textReader)
        {
            var nextChar = textReader.Read();
            Trace.Assert(nextChar == '(');

            return ListReaderRest(textReader);
        }

        private static IList ListReaderRest(ITextReader textReader)
        {
            textReader.ReadWhiteSpace();
            switch (textReader.Peek())
            {
                case '\0':
                    throw new LispException("Missing terminating ')'.");

                case ')':
                    textReader.Read();
                    return Constants.EmptyList;

                default:
                    var first = Read(textReader);
                    return ListReaderRest(textReader).Cons(first);
            }
        }

        private static ISExpression ListTerminatorReader(ITextReader textReader)
        {
            throw new LispException("Missing terminating ')'.");
        }

        private static ISExpression QuoteReader(ITextReader textReader)
        {
            var nextChar = textReader.Read();
            Trace.Assert(nextChar == '\'');

            return Constants.EmptyList.Cons(Read(textReader)).Cons(Constants.Quote);
        }

        private static void ReadWhiteSpace(this ITextReader @this)
        {
            var currentChar = @this.Peek();
            while (currentChar.IsWhiteSpace())
            {
                @this.Read();
                currentChar = @this.Peek();
            }
        }

        private static ISExpression StringReader(ITextReader textReader)
        {
            var stringBuilder = new StringBuilder();

            var nextChar = textReader.Read();
            Trace.Assert(nextChar == '\"');

            nextChar = textReader.Read();
            while (nextChar != '"')
            {
                switch (nextChar)
                {
                    case '\0':
                        throw new LispException("Missing terminating '\"'.");

                    case '\\':
                        nextChar = textReader.Read();
                        switch (nextChar)
                        {
                            case '\0':
                                throw new LispException("Missing excaped character.");

                            case 't':
                                stringBuilder.Append('\t');
                                break;

                            case 'r':
                                stringBuilder.Append('\r');
                                break;

                            case 'n':
                                stringBuilder.Append('\n');
                                break;

                            case '\\':
                            case '"':
                                stringBuilder.Append(nextChar);
                                break;

                            default:
                                throw new System.Exception("Unknown escape character.");
                        }
                        break;

                    default:
                        stringBuilder.Append(nextChar);
                        break;
                }

                nextChar = textReader.Read();
            }

            return new Value(stringBuilder.ToString());
        }

        private static ISExpression SyntaxQuoteReader(ITextReader textReader)
        {
            var nextChar = textReader.Read();
            Trace.Assert(nextChar == '`');

            return Constants.EmptyList.Cons(Read(textReader)).Cons(new Value("`"));
        }

        private static ISExpression UnquoteReader(ITextReader textReader)
        {
            var nextChar = textReader.Read();
            Trace.Assert(nextChar == '~');

            return Constants.EmptyList.Cons(Read(textReader)).Cons(new Value("~"));
        }

        private delegate ISExpression SExpressionReader(ITextReader textReader);
    }
}
