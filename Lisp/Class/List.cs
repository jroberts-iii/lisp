using System.Collections.Generic;
using System.IO;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public class List : SExpression, IList
    {
        private static readonly Dictionary<string, ListEvaluator> NameToListEvaluator =
            new Dictionary<string, ListEvaluator>
            {
                {"cond", CondListEvaluator},
                {"define", DefineListEvaluator},
                {"evaluate", EvaluateListEvaluator},
                {"first", FirstListEvaluator},
                {"lambda", LambdaListEvaluator},
                {"list", ListListEvaluator},
                {"macro", MacroListEvaluator},
                {"prepend", PrependListEvaluator},
                {"quasiquote", QuasiquoteListEvaluator},
                {"quote", QuoteListEvaluator},
                {"rest", RestListEvaluator},
                {"unquote", UnquoteListEvaluator},
                {"unquote-splicing", UnquoteSplicingListEvaluator}
            };

        public static IList Empty = new List();

        private List()
        {
            IsEmpty = true;
            First = null;
            Rest = this;
        }

        private List(ISExpression first, IList rest)
        {
            IsEmpty = false;
            First = first;
            Rest = rest;
        }

        public ISExpression First { get; }
        public bool IsEmpty { get; }
        public IList Rest { get; }

        public IList Cons(ISExpression sExpression)
        {
            return new List(sExpression, this);
        }

        public override ISExpression Evaluate(IEnvironment environment)
        {
            if (IsEmpty)
            {
                return Constants.EmptyList;
            }

            if (First == null)
            {
                throw new LispException("List.Evaluate expected First to be not null.");
            }

            var specialSymbol = First as ISymbol;
            if (specialSymbol != null)
            {
                ListEvaluator listEvaluator;
                if (NameToListEvaluator.TryGetValue(specialSymbol.FullName, out listEvaluator))
                {
                    return listEvaluator(this, environment);
                }

                ISExpression sExpression;
                if (environment.TryGetSymbol(specialSymbol.FullName, out sExpression))
                {
                    return sExpression;
                }
            }

            var lambda = Evaluate(environment, First) as ILambda;
            if (lambda == null)
            {
                throw new LispException("List.Evaluate expected First to evaluate to a lambda.");
            }

            return lambda.Evaluate(environment, Rest);
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("(");
            IList list = this;
            if (!list.IsEmpty)
            {
                SExpression.Write(textWriter, list.First);
                while (!list.Rest.IsEmpty)
                {
                    list = list.Rest;
                    textWriter.Write(" ");
                    SExpression.Write(textWriter, list.First);
                }
            }

            textWriter.Write(")");
        }

        public IList ToList()
        {
            return this;
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
                    throw new LispException("Expected (cond (condition result)*).  Condition must be a list.");
                }

                var conditional = Evaluate(environment, condition.First);
                if (IsTrue(conditional))
                {
                    return Evaluate(environment, condition.Rest.First);
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

            var symbol = list.Rest.First as ISymbol;
            if (symbol == null)
            {
                throw new LispException("Expected (define symbol form).  Symbol is not a symbol.");
            }

            var sExpression = Evaluate(environment, list.Rest.Rest.First);
            environment.TopEnvironment.AddSymbol(symbol.Name, sExpression);
            return sExpression;
        }

        private static ISExpression EvaluateListEvaluator(IList list, IEnvironment environment)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
            {
                throw new LispException("Expected (evaluate form)");
            }

            var sExpression = Evaluate(environment, list.Rest.First);
            return Evaluate(environment, sExpression);
        }

        private static ISExpression FirstListEvaluator(IList list, IEnvironment environment)
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

        private static bool IsTrue(ISExpression sExpression)
        {
            if (sExpression == null)
            {
                return false;
            }

            var boolean = sExpression as IValue<bool>;
            return boolean == null || boolean.Val;
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

            var first = Evaluate(environment, list.First);
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

        private static ISExpression PrependListEvaluator(IList list, IEnvironment environment)
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
