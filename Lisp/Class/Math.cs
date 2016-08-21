using System.IO;
using Lisp.Extension;
using Lisp.Interface;

namespace Lisp.Class
{
    public static class Math
    {
        public static void AddSymbols(IEnvironment environment)
        {
            AddLambda(environment, new AdditionLambda());
            AddLambda(environment, new BitwiseAndLambda());
            AddLambda(environment, new BitwiseOrLambda());
            AddLambda(environment, new BitwiseXorLambda());
            AddLambda(environment, new DivisionLambda());
            AddLambda(environment, new EqualLambda());
            AddLambda(environment, new GreaterThanLambda());
            AddLambda(environment, new GreaterThanOrEqualLambda());
            AddLambda(environment, new LessThanLambda());
            AddLambda(environment, new LessThanOrEqualLambda());
            AddLambda(environment, new LogicalAndLambda());
            AddLambda(environment, new LogicalOrLambda());
            AddLambda(environment, new MultiplicationLambda());
            AddLambda(environment, new SubtractionLambda());
        }

        private static void AddLambda(IEnvironment environment, ILambda lambda)
        {
            using (var stringWriter = new StringWriter())
            {
                lambda.Write(stringWriter);
                environment.AddSymbol(stringWriter.ToString(), lambda);
            }
        }

        private class AdditionLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateManyParameters(environment, list, (a, b) => a + b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("+");
            }
        }

        private class BitwiseAndLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateManyParameters(environment, list, (a, b) => a & b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("&");
            }
        }

        private class BitwiseOrLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateManyParameters(environment, list, (a, b) => a | b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("|");
            }
        }

        private class BitwiseXorLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateManyParameters(environment, list, (a, b) => a ^ b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("^");
            }
        }

        private class DivisionLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateManyParameters(environment, list, (a, b) => a/b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("/");
            }
        }

        private class EqualLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateTwoParameters(environment, list, (a, b) => a == b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("=");
            }
        }

        private class GreaterThanLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateTwoParameters(environment, list, (a, b) => a > b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write(">");
            }
        }

        private class GreaterThanOrEqualLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateTwoParameters(environment, list, (a, b) => a >= b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write(">=");
            }
        }

        private class LessThanLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateTwoParameters(environment, list, (a, b) => a < b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("<");
            }
        }

        private class LessThanOrEqualLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateTwoParameters(environment, list, (a, b) => a <= b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("<=");
            }
        }

        private class LogicalAndLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateManyParametersBoolean(environment, list, (a, b) => a && b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("&&");
            }
        }

        private class LogicalOrLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateManyParametersBoolean(environment, list, (a, b) => a || b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("||");
            }
        }

        private class MultiplicationLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateManyParameters(environment, list, (a, b) => a*b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("*");
            }
        }

        private class SubtractionLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                return this.EvaluateManyParameters(environment, list, (a, b) => a - b);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("-");
            }
        }
    }
}
