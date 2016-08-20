using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public class TopEnvironment : ITopEnvironment
    {
        private readonly Dictionary<string, ISExpression> _symbolNameToSExpression =
            new Dictionary<string, ISExpression>();

        public TopEnvironment()
        {
            AddLambda(new AdditionLambda());
            AddLambda(new ClosureLambda());
            AddLambda(new CondLambda());
            AddLambda(new DefineLambda());
            AddLambda(new EvaluateLambda());
            AddLambda(new FirstLambda());
            AddLambda(new ListLambda());
            AddLambda(new MacroLambda());
            AddLambda(new PrependLambda());
            AddLambda(new QuasiquoteLambda());
            AddLambda(new QuoteLambda());
            AddLambda(new RestLambda());
            AddLambda(new UnquoteLambda());
            AddLambda(new UnquoteSplicingLambda());

            /*
                {"&", Constants.OpBitwiseAnd},
                {"|", Constants.OpBitwiseOr},
                {"^", Constants.OpBitwiseXor},
                {"=", Constants.OpEqual},
                {">", Constants.OpGreaterThan},
                {">=", Constants.OpGreaterThanOrEqual},
                {"<", Constants.OpLessThan},
                {"<=", Constants.OpLessThanOrEqual},
                {"&&", Constants.OpLogicalAnd},
                {"||", Constants.OpLogicalOr},
                {"^^", Constants.OpLogicalXor},
                {"*", Constants.OpMultiplication},
                {"-", Constants.OpSubtraction}
            */
        }

        public void AddSymbol(string name, ISExpression sExpression)
        {
            _symbolNameToSExpression[name] = sExpression;
        }

        public bool TryGetSymbol(string name, out ISExpression sExpression)
        {
            return _symbolNameToSExpression.TryGetValue(name, out sExpression);
        }

        private void AddLambda(ILambda lambda)
        {
            using (var stringWriter = new StringWriter())
            {
                lambda.Write(stringWriter);
                AddSymbol(stringWriter.ToString(), lambda);
            }
        }
    }

    internal class AdditionLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.IsEmpty)
            {
                throw new LispException("Expected (+ value*)");
            }

            var sum = list.Rest.OfType<IValue>()
                .Aggregate<IValue, object>(null, (current, value) => Operation(current ?? 0, value.Val));
            return new Value(sum);
        }

        public static object Operation(dynamic a, dynamic b)
        {
            return a + b;
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("+");
        }
    }

    internal class ClosureLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (lambda parameterList body)");
            }

            var parameterList = list.Rest.First as IList;
            if (parameterList == null)
            {
                throw new LispException("Expected (lambda parameterList body). ParameterList is not a list.");
            }

            var parameterSymbols = new List<ISymbol>();
            foreach (var sExpression in parameterList)
            {
                var symbol = sExpression as ISymbol;
                if (symbol == null)
                {
                    throw new LispException("Expected (lambda parameterList body). ParameterList not a list of symbols.");
                }

                parameterSymbols.Add(symbol);
            }

            return new Closure(list.Rest.Rest.First, environment, parameterSymbols);
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("closure");
        }
    }

    internal class CondLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.IsEmpty)
            {
                return null;
            }

            foreach (var sExpression in list.Rest)
            {
                var condition = sExpression as IList;
                if (condition == null)
                {
                    throw new LispException("Expected (cond (condition result)*).  Condition must be a list.");
                }

                var conditional = Evaluate(environment, condition.First);
                if (IsTrue(conditional))
                {
                    return Evaluate(environment, condition.Rest.First);
                }
            }

            return null;
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("cond");
        }

        private static bool IsTrue(ISExpression sExpression)
        {
            var value = sExpression as IValue;
            if (value?.Val is bool)
            {
                return (bool) value.Val;
            }

            return sExpression != null;
        }
    }

    internal class DefineLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (define symbol form)");
            }

            var symbol = list.Rest.First as ISymbol;
            if (symbol == null)
            {
                throw new LispException("Expected (define symbol form).  Symbol is not a symbol.");
            }

            var sExpression = Evaluate(environment, list.Rest.Rest.First);
            environment.TopEnvironment.AddSymbol(symbol.Name, sExpression);
            return sExpression;
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("define");
        }
    }

    internal class EvaluateLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (evaluate form)");
            }

            var sExpression = Evaluate(environment, list.Rest.First);
            return Evaluate(environment, sExpression);
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("evaluate");
        }
    }

    internal class FirstLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (first collection)");
            }

            var sExpression = Evaluate(environment, list.Rest.First);
            var collection = sExpression as ICollection;
            if (collection == null)
            {
                throw new LispException("Expected (first collection). Collection is not a collection.");
            }

            return collection.ToList().First;
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("first");
        }
    }

    internal class ListLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            return ListListEvaluatorRest(list.Rest, environment);
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("list");
        }

        private static IList ListListEvaluatorRest(IList list, IEnvironment environment)
        {
            if (list.IsEmpty)
            {
                return Constants.EmptyList;
            }

            var first = Evaluate(environment, list.First);
            return ListListEvaluatorRest(list.Rest, environment).Cons(first);
        }
    }

    internal class MacroLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (macro parameterList body)");
            }

            var parameterList = list.Rest.First as IList;
            if (parameterList == null)
            {
                throw new LispException("Expected (macro parameterList body). ParameterList is not a list.");
            }

            var parameterSymbols = new List<ISymbol>();
            foreach (var sExpression in parameterList)
            {
                var symbol = sExpression as ISymbol;
                if (symbol == null)
                {
                    throw new LispException("Expected (macro parameterList body). ParameterList not a list of symbols.");
                }

                parameterSymbols.Add(symbol);
            }

            return new Macro(list.Rest.Rest.First, parameterSymbols);
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("macro");
        }
    }

    internal class PrependLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (prepend form list)");
            }

            var consList = list.Rest.First as IList;
            if (consList == null)
            {
                throw new LispException("Expected (prepend form list).  List is not a list.");
            }

            return consList.Cons(list.First);
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("prepend");
        }
    }

    internal class QuasiquoteLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (quasiquote form)");
            }

            var formList = list.Rest.First as IList;
            return formList == null ? list.Rest.First : QuasiquoteListEvaluatorRest(formList, environment);
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("quasiquote");
        }

        private static IList QuasiquoteListEvaluatorRest(IList list, IEnvironment environment)
        {
            if (list.IsEmpty)
            {
                return Constants.EmptyList;
            }

            var form = list.First as IList;
            if (form != null)
            {
                if (form.First == Constants.Unquote)
                {
                    var first = Evaluate(environment, form.Rest.First);
                    return QuasiquoteListEvaluatorRest(list.Rest, environment).Cons(first);
                }

                if (form.First == Constants.UnquoteSplicing)
                {
                    var first = Evaluate(environment, form.Rest.First);
                    var splicingList = first as IList;
                    if (splicingList == null)
                    {
                        throw new LispException("Expected (unquote-splicing list)");
                    }

                    form = QuasiquoteListEvaluatorRest(list.Rest, environment);
                    return QuasiquoteUnquoteSplicing(splicingList, form);
                }
            }

            return QuasiquoteListEvaluatorRest(list.Rest, environment).Cons(list.First);
        }

        private static IList QuasiquoteUnquoteSplicing(IList spliceList, IList form)
        {
            if (spliceList.IsEmpty)
            {
                return form;
            }

            return QuasiquoteUnquoteSplicing(spliceList.Rest, form).Cons(spliceList.First);
        }
    }

    internal class QuoteLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (quote form)");
            }

            return list.Rest.First;
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("quote");
        }
    }

    internal class RestLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (rest collection)");
            }

            var sExpression = Evaluate(environment, list.Rest.First);
            var collection = sExpression as ICollection;
            if (collection == null)
            {
                throw new LispException("Expected (rest collection).  Collection is not a collection.");
            }

            return collection.ToList().Rest;
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("rest");
        }
    }

    internal class UnquoteLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            throw new LispException("Unquote can only appear in a quasiquote.");
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("unquote");
        }
    }

    internal class UnquoteSplicingLambda : SExpression, ILambda
    {
        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            throw new LispException("Unquote-splicing can only appear in a quasiquote.");
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("unquote-splicing");
        }
    }
}
