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
                {"+", AdditionListEvaluator},
                {"/", DivisionListEvaluator},
                {"*", MultiplicationListEvaluator},
                {"-", SubtractionListEvaluator},
                {"cond", CondListEvaluator},
                {"cons", ConsListEvaluator},
                {"define", DefineListEvaluator},
                {"eval", EvalListEvaluator},
                {"first", FirstListEvaluator},
                {"lambda", LambdaListEvaluator},
                {"macro", MacroListEvaluator},
                {"list", ListListEvaluator},
                {"quasiquote", QuasiquoteListEvaluator},
                {"quote", QuoteListEvaluator},
                {"rest", RestListEvaluator},
                {"unquote", UnquoteListEvaluator},
                {"unquote-splicing", UnquoteSplicingListEvaluator}
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

        private static ISExpression AdditionListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty)
            {
                return null;
            }

            IValue total = null;
            list = list.Rest;
            while (!list.IsEmpty)
            {
                var sExpression = Evaluate(list.First, environment);
                var value = sExpression as IValue;
                if (value == null)
                {
                    continue;
                }

                list = list.Rest;
                total = total == null ? value : total.Addition(value);
            }

            return total;
        }

        private static ISExpression CallListEvaluator(IList list, IEnvironment environment)
        {
            Environment env;
            IList parameters;

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

            var lambda = sExpression as IClosure;
            if (lambda != null)
            {
                env = new Environment(lambda.ClosureEnvironment);

                parameters = list.Rest;
                foreach (var parameterSymbol in lambda.ParameterSymbols)
                {
                    sExpression = Evaluate(parameters.First, environment);
                    env.AddSymbol(parameterSymbol.Name, sExpression);
                    parameters = parameters.Rest;
                }

                return Evaluate(lambda.Body, env);
            }

            var macro = sExpression as IMacro;
            if (macro == null)
            {
                throw new LispException("Expected (<lambda/macro> parameters)");
            }

            while (macro != null)
            {
                env = new Environment();

                parameters = list.Rest;
                foreach (var parameterSymbol in macro.ParameterSymbols)
                {
                    env.AddSymbol(parameterSymbol.Name, parameters.First);
                    parameters = parameters.Rest;
                }

                sExpression = Evaluate(macro.Body, env);
                macro = sExpression as IMacro;
            }

            return Evaluate(sExpression, environment);
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

        private static ISExpression ConsListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (cons form list)");
            }

            var consList = list.Rest.First as IList;
            if (consList == null)
            {
                throw new LispException("Expected (cons form list).  Collection is not a list.");
            }

            return consList.Cons(list.First);
        }

        private static ISExpression DefineListEvaluator(IList list, IEnvironment environment)
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

            var sExpression = Evaluate(list.Rest.Rest.First, environment);
            environment.TopEnvironment.AddSymbol(symbol.Name, sExpression);
            return sExpression;
        }

        private static ISExpression DivisionListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty)
            {
                return null;
            }

            IValue total = null;
            list = list.Rest;
            while (!list.IsEmpty)
            {
                var sExpression = Evaluate(list.First, environment);
                var value = sExpression as IValue;
                if (value == null)
                {
                    continue;
                }

                list = list.Rest;
                total = total == null ? value : total.Division(value);
            }

            return total;
        }

        private static ISExpression EvalListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (eval form)");
            }

            var sExpression = Evaluate(list.Rest.First, environment);
            return Evaluate(sExpression, environment);
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

            var parameterSymbols = new List<ISymbol>();
            while (!parameterList.IsEmpty)
            {
                var symbol = parameterList.First as ISymbol;
                if (symbol == null)
                {
                    throw new LispException("Expected (lambda parameterList body). ParameterList not a list of symbols.");
                }

                parameterSymbols.Add(symbol);
                parameterList = parameterList.Rest;
            }

            return new Closure(list.Rest.Rest.First, environment, parameterSymbols);
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

        private static ISExpression MacroListEvaluator(IList list, IEnvironment environment)
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
            while (!parameterList.IsEmpty)
            {
                var symbol = parameterList.First as ISymbol;
                if (symbol == null)
                {
                    throw new LispException("Expected (macro parameterList body). ParameterList not a list of symbols.");
                }

                parameterSymbols.Add(symbol);
                parameterList = parameterList.Rest;
            }

            return new Macro(list.Rest.Rest.First, parameterSymbols);
        }

        private static ISExpression MultiplicationListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty)
            {
                return null;
            }

            IValue total = null;
            list = list.Rest;
            while (!list.IsEmpty)
            {
                var sExpression = Evaluate(list.First, environment);
                var value = sExpression as IValue;
                if (value == null)
                {
                    continue;
                }

                list = list.Rest;
                total = total == null ? value : total.Multiplication(value);
            }

            return total;
        }

        private static ISExpression QuasiquoteListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (quasiquote form)");
            }

            var formList = list.Rest.First as IList;
            return formList == null ? list.Rest.First : QuasiquoteListEvaluatorRest(formList, environment);
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
                    var first = Evaluate(form.Rest.First, environment);
                    return QuasiquoteListEvaluatorRest(list.Rest, environment).Cons(first);
                }

                if (form.First == Constants.UnquoteSplicing)
                {
                    var first = Evaluate(form.Rest.First, environment);
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

        private static ISExpression QuoteListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
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

        private static ISExpression SubtractionListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty)
            {
                return null;
            }

            IValue total = null;
            list = list.Rest;
            while (!list.IsEmpty)
            {
                var sExpression = Evaluate(list.First, environment);
                var value = sExpression as IValue;
                if (value == null)
                {
                    continue;
                }

                list = list.Rest;
                total = total == null ? value : total.Subtraction(value);
            }

            return total;
        }

        private static ISExpression UnquoteListEvaluator(IList list, IEnvironment environment)
        {
            throw new LispException("Unquote can only appear in a quasiquote.");
        }

        private static ISExpression UnquoteSplicingListEvaluator(IList list, IEnvironment environment)
        {
            throw new LispException("Unquote-splicing can only appear in a quasiquote.");
        }

        private delegate ISExpression ListEvaluator(IList list, IEnvironment environment);
    }
}
