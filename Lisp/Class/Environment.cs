using System.Collections.Generic;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Environment : IEnvironment
    {
        private readonly IEnvironment _closureEnvironment;

        private readonly Dictionary<string, ISExpression> _symbolNameToSExpression =
            new Dictionary<string, ISExpression>();

        public Environment()
        {
            _closureEnvironment = null;
            EnclosingEnvironment = null;
            Parameters = new ISExpression[0];

            // Conditionals
            AddLambda(new UnaryLambda("atom?", x => x is IAtom));
            AddLambda(new UnaryLambda("collection?", x => x is ICollection));
            AddLambda(new UnaryLambda("empty?",
                x =>
                {
                    var collection = x as ICollection;
                    if (collection == null)
                    {
                        throw new LispException("Expected (empty? collection). Collection is not a collection.");
                    }

                    return collection.IsEmpty
                        ? Constants.True
                        : Constants.False;
                }));
            AddLambda(new UnaryLambda("keyword?", x => x is IKeyword));
            AddLambda(new UnaryLambda("lambda?", x => x is ILambda));
            AddLambda(new UnaryLambda("list?", x => x is IList));
            AddLambda(new UnaryLambda("macro?", x => x is IMacro));
            AddLambda(new UnaryLambda("map?", x => x is IMap));
            AddLambda(new UnaryLambda("null?", x => x == null));
            AddLambda(new UnaryLambda("pair?", x => x is IPair));
            AddLambda(new UnaryLambda("symbol?", x => x is ISymbol));
            AddLambda(new UnaryLambda("value?", x => x is IValue));
            AddLambda(new UnaryLambda("vector?", x => x is IVector));

            AddLambda(new BinaryLambda("==", (x, y) => x == y));
            AddLambda(new BinaryLambda(">", (x, y) => x > y));
            AddLambda(new BinaryLambda(">=", (x, y) => x >= y));
            AddLambda(new BinaryLambda("<", (x, y) => x < y));
            AddLambda(new BinaryLambda("<=", (x, y) => x <= y));
            AddLambda(new BinaryLambda("!=", (x, y) => x != y));

            // Math
            AddLambda(new NaryLambda("+", (x, y) => x + y));
            AddLambda(new NaryLambda("&", (x, y) => x & y));
            AddLambda(new NaryLambda("|", (x, y) => x | y));
            AddLambda(new NaryLambda("^", (x, y) => x ^ y));
            AddLambda(new NaryLambda("/", (x, y) => x/y));
            AddLambda(new NaryLambda("&&", (x, y) => x && y));
            AddLambda(new NaryLambda("||", (x, y) => x || y));
            AddLambda(new NaryLambda("*", (x, y) => x*y));
            AddLambda(new NaryLambda("-", (x, y) => x - y));

            // Special Forms
            AddLambda(new LambdaLambda());
            AddLambda(new ListLambda());
            AddLambda(new PrependLambda());
            AddLambda(new QuasiquoteLambda());
            AddLambda(new QuoteLambda());
            AddLambda(new UnquoteLambda());
            AddLambda(new UnquoteSplicingLambda());

            // Standard Forms
            AddLambda(new DefineLambda());
            AddLambda(new FirstLambda());
            AddLambda(new MacroLambda());
            AddLambda(new RestLambda());
        }

        private Environment(IEnvironment closureEnvironment,
            IEnvironment enclosingEnvironment,
            ISExpression[] parameters)
        {
            _closureEnvironment = closureEnvironment;
            EnclosingEnvironment = enclosingEnvironment;
            Parameters = parameters;
        }

        public IEnvironment EnclosingEnvironment { get; }

        public IEnvironment GlobalEnvironment => EnclosingEnvironment == null
            ? this
            : EnclosingEnvironment.GlobalEnvironment;

        public ISExpression[] Parameters { get; }

        public void AddSymbol(string name, ISExpression sExpression)
        {
            _symbolNameToSExpression[name] = sExpression;
        }

        public IEnvironment Push(IEnvironment closureEnvironment, ISExpression[] parameters)
        {
            return new Environment(closureEnvironment, this, parameters);
        }

        public bool TryGetSymbol(string name, out ISExpression sExpression)
        {
            if (_symbolNameToSExpression.TryGetValue(name, out sExpression))
            {
                return true;
            }

            if (_closureEnvironment != null)
            {
                if (_closureEnvironment.TryGetSymbol(name, out sExpression))
                {
                    return true;
                }
            }

            return (EnclosingEnvironment != null) && EnclosingEnvironment.TryGetSymbol(name, out sExpression);
        }

        private void AddLambda(ILambda lambda)
        {
            AddSymbol(lambda.ToString(), lambda);
        }

        private class DefineLambda : Lambda
        {
            public DefineLambda() : base(null, "symbol", "sExpression")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment)
            {
                var symbol = environment.Parameters[0] as ISymbol;
                if (symbol == null)
                {
                    throw new LispException($"{ExceptionMessage()} Symbol is not a symbol.");
                }

                var sExpression = environment.Parameters[1];
                environment.GlobalEnvironment.AddSymbol(symbol.Name, sExpression);
                return sExpression;
            }

            public override string ToString()
            {
                return "define";
            }
        }

        private class FirstLambda : Lambda
        {
            public FirstLambda() : base(null, "list")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment)
            {
                var firstList = environment.Parameters[0] as IList;
                if (firstList == null)
                {
                    throw new LispException($"{ExceptionMessage()} List is not a list.");
                }

                return firstList.First;
            }

            public override string ToString()
            {
                return "first";
            }
        }

        private class LambdaLambda : Lambda
        {
            public LambdaLambda() : base(null, "parameterList", "body")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment, IList list)
            {
                if (list.Rest.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
                {
                    throw new LispException(ExceptionMessage());
                }

                var parameterList = list.Rest.First as IList;
                if (parameterList == null)
                {
                    throw new LispException($"{ExceptionMessage()} ParameterList is not a list.");
                }

                var parameterNames = new List<string>();
                foreach (var sExpression in parameterList)
                {
                    var symbol = sExpression as ISymbol;
                    if (symbol == null)
                    {
                        throw new LispException($"{ExceptionMessage()} ParameterList not a list of symbols.");
                    }

                    parameterNames.Add(symbol.Name);
                }

                return new LispLambda(list.Rest.Rest.First, environment, parameterNames.ToArray());
            }

            public override string ToString()
            {
                return "lambda";
            }
        }

        private class ListLambda : Lambda
        {
            public ListLambda() : base(null, "sExpression*")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment, IList list)
            {
                list = list.Rest;
                if (list.IsEmpty)
                {
                    return Constants.EmptyList;
                }

                var first = Evaluate(environment, list.First);
                return ListListEvaluatorRest(list.Rest, environment).Prepend(first);
            }

            public override string ToString()
            {
                return "list";
            }

            private static IList ListListEvaluatorRest(IList list, IEnvironment environment)
            {
                if (list.IsEmpty)
                {
                    return Constants.EmptyList;
                }

                var first = Evaluate(environment, list.First);
                return ListListEvaluatorRest(list.Rest, environment).Prepend(first);
            }
        }

        private class MacroLambda : Lambda
        {
            public MacroLambda() : base(null, "parameterList", "body")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment)
            {
                var parameterList = environment.Parameters[0] as IList;
                if (parameterList == null)
                {
                    throw new LispException($"{ExceptionMessage()} ParameterList is not a list.");
                }

                var parameterNames = new List<string>();
                foreach (var sExpression in parameterList)
                {
                    var symbol = sExpression as ISymbol;
                    if (symbol == null)
                    {
                        throw new LispException($"{ExceptionMessage()} ParameterList is not a list of symbols.");
                    }

                    parameterNames.Add(symbol.Name);
                }

                var body = environment.Parameters[1];
                return new Macro(body, parameterNames.ToArray());
            }

            public override string ToString()
            {
                return "macro";
            }
        }

        private class PrependLambda : Lambda
        {
            public PrependLambda() : base(null, "sExpression", "list")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment, IList list)
            {
                if (list.Rest.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
                {
                    throw new LispException(ExceptionMessage());
                }

                var consList = list.Rest.First as IList;
                if (consList == null)
                {
                    throw new LispException($"{ExceptionMessage()} List is not a list.");
                }

                return consList.Prepend(list.First);
            }

            public override string ToString()
            {
                return "prepend";
            }
        }

        private class QuasiquoteLambda : Lambda
        {
            public QuasiquoteLambda() : base(null, "sExpression")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment, IList list)
            {
                if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
                {
                    throw new LispException(ExceptionMessage());
                }

                var formList = list.Rest.First as IList;
                return formList == null
                    ? list.Rest.First
                    : QuasiquoteListEvaluatorRest(environment, formList);
            }

            public override string ToString()
            {
                return "quasiquote";
            }

            private static IList QuasiquoteListEvaluatorRest(IEnvironment environment, IList list)
            {
                if (list.IsEmpty)
                {
                    return Constants.EmptyList;
                }

                var sExpression = list.First as IList;
                if (sExpression != null)
                {
                    if (sExpression.First == Constants.Unquote)
                    {
                        return
                            QuasiquoteListEvaluatorRest(environment, list.Rest)
                                .Prepend(Evaluate(environment, sExpression.Rest.First));
                    }

                    if (sExpression.First == Constants.UnquoteSplicing)
                    {
                        var splicingList = Evaluate(environment, sExpression.Rest.First) as IList;
                        if (splicingList == null)
                        {
                            throw new LispException("Expected (unquote-splicing list).");
                        }

                        sExpression = QuasiquoteListEvaluatorRest(environment, list.Rest);
                        return QuasiquoteUnquoteSplicing(splicingList, sExpression);
                    }
                }

                return QuasiquoteListEvaluatorRest(environment, list.Rest).Prepend(list.First);
            }

            private static IList QuasiquoteUnquoteSplicing(IList spliceList, IList sExpression)
            {
                if (spliceList.IsEmpty)
                {
                    return sExpression;
                }

                return QuasiquoteUnquoteSplicing(spliceList.Rest, sExpression).Prepend(spliceList.First);
            }
        }

        private class QuoteLambda : Lambda
        {
            public QuoteLambda() : base(null, "sExpression")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment, IList list)
            {
                if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
                {
                    throw new LispException(ExceptionMessage());
                }

                return list.Rest.First;
            }

            public override string ToString()
            {
                return "quote";
            }
        }

        private class RestLambda : Lambda
        {
            public RestLambda() : base(null, "list")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment)
            {
                var restList = environment.Parameters[0] as IList;
                if (restList == null)
                {
                    throw new LispException($"{ExceptionMessage()} List is not a list.");
                }

                return restList.Rest;
            }

            public override string ToString()
            {
                return "rest";
            }
        }

        private class UnquoteLambda : Lambda
        {
            public UnquoteLambda() : base(null, "sExpression")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment, IList list)
            {
                throw new LispException("Unquote can only appear in a quasiquote.");
            }

            public override string ToString()
            {
                return "unquote";
            }
        }

        private class UnquoteSplicingLambda : Lambda
        {
            public UnquoteSplicingLambda() : base(null, "sExpression")
            {
            }

            public override ISExpression Evaluate(IEnvironment environment, IList list)
            {
                throw new LispException("Unquote-splicing can only appear in a quasiquote.");
            }

            public override string ToString()
            {
                return "unquote-splicing";
            }
        }
    }
}
