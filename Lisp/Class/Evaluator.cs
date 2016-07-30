using System.Collections.Generic;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public static class Evaluator
    {
        private static readonly Dictionary<string, ListEvaluator> NameToListEvaluator =
            new Dictionary<string, ListEvaluator>
            {
                {"cond", CondListEvaluator},
                {"define", DefineListEvaluator},
                {"eval", EvalListEvaluator},
                {"first", FirstListEvaluator},
                {"lambda", LambdaListEvaluator},
                {"list", ListListEvaluator},
                {"quote", QuoteListEvaluator},
                {"rest", RestListEvaluator}
            };

        public static ISExpression Evaluate(ISExpression sExpression, IEnvironment environment)
        {
            var list = sExpression as IList;
            if (list != null)
            {
                return CallListEvaluator(list, environment);
            }

            var symbol = sExpression as ISymbol;
            if (symbol == null)
            {
                return sExpression;
            }

            if (environment.TryGetSymbol(symbol.Name, out sExpression))
            {
                return sExpression;
            }

            throw new LispException($"Undefined symbol {symbol.Name}.");
        }

        private static ISExpression CallListEvaluator(IList list, IEnvironment environment)
        {
            if (list.IsEmpty)
            {
                return Constants.EmptyList;
            }

            var specialSymbol = list.First as ISymbol;
            if (specialSymbol != null)
            {
                ListEvaluator listEvaluator;
                if (NameToListEvaluator.TryGetValue(specialSymbol.Name, out listEvaluator))
                {
                    return listEvaluator(list, environment);
                }
            }

            var sExpression = Evaluate(list.First, environment);
            var lambda = sExpression as Lambda;
            if (lambda == null)
            {
                throw new LispException("Expected (<lambda> parameters)");
            }

            var env = new Environment(lambda.ClosurEnvironment);

            var parameters = list.Rest;
            foreach (var parameterSymbol in lambda.ParameterSymbols)
            {
                sExpression = Evaluate(parameters.First, environment);
                env.AddSymbol(parameterSymbol.Name, sExpression);
                parameters = parameters.Rest;
            }

            var body = lambda.Body;

            sExpression = null;
            while (!body.IsEmpty)
            {
                sExpression = Evaluate(body.First, env);
                body = body.Rest;
            }

            return sExpression;
        }

        private static ISExpression CondListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty)
            {
                return null;
            }

            var conditions = list.Rest;
            while (!conditions.IsEmpty)
            {
                var condition = conditions.First as IList;
                if (condition == null)
                {
                    throw new LispException(
                        "Expected (cond (condition1 result1) (condition2 result 2)... (T resultN)).  Conditions must be a list.");
                }

                var conditional = Evaluate(condition.First, environment);
                if (conditional != null)
                {
                    return Evaluate(condition.Rest.First, environment);
                }

                conditions = conditions.Rest;
            }

            return null;
        }

        private static ISExpression DefineListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (define symbol form)");
            }

            var symbol = list.Rest.First as Symbol;
            if (symbol == null)
            {
                throw new LispException("Expected (define symbol form).  Symbol is not a symbol.");
            }

            var sExpression = Evaluate(list.Rest.Rest.First, environment);
            environment.TopEnvironment.AddSymbol(symbol.Name, sExpression);
            return sExpression;
        }

        private static ISExpression EvalListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (eval form)");
            }

            return Evaluate(list.Rest.First, environment);
        }

        private static ISExpression FirstListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (first list)");
            }

            var sExpression = Evaluate(list.Rest.First, environment);
            var lst = sExpression as IList;
            if (lst == null)
            {
                throw new LispException("Expected (first list).  IList is not a list.");
            }

            return lst.First;
        }

        private static ISExpression LambdaListEvaluator(IList list, IEnvironment environment)
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

            var parameterSymbols = new List<Symbol>();
            while (!parameterList.IsEmpty)
            {
                var symbol = parameterList.First as Symbol;
                if (symbol == null)
                {
                    throw new LispException("Expected (lambda parameterList body). ParameterList not a list of symbols.");
                }

                parameterSymbols.Add(symbol);
                parameterList = parameterList.Rest;
            }

            return new Lambda(list.Rest.Rest, environment, parameterSymbols);
        }

        private static IList ListListEvaluator(IList list, IEnvironment environment)
        {
            return ListListEvaluatorRest(list.Rest, environment);
        }

        private static IList ListListEvaluatorRest(IList list, IEnvironment environment)
        {
            if (list.IsEmpty)
            {
                return Constants.EmptyList;
            }

            var first = Evaluate(list.First, environment);
            return ListListEvaluatorRest(list.Rest, environment).Cons(first);
        }

        private static ISExpression QuoteListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty)
            {
                throw new LispException("Expected (quote form)");
            }

            return list.Rest.First;
        }

        private static ISExpression RestListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (rest list)");
            }

            var sExpression = Evaluate(list.Rest.First, environment);
            var lst = sExpression as IList;
            if (lst == null)
            {
                throw new LispException("Expected (rest list).  IList is not a list.");
            }

            return lst.Rest;
        }

        private delegate ISExpression ListEvaluator(IList list, IEnvironment environment);
    }
}
