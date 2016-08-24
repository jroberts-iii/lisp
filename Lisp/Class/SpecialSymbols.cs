using System.Collections.Generic;
using System.IO;
using Lisp.Exception;
using Lisp.Extension;
using Lisp.Interface;

namespace Lisp.Class
{
    public static class SpecialSymbols
    {
        public static void AddSymbols(IEnvironment environment)
        {
            AddLambda(environment, new ClosureLambda());
            AddLambda(environment, new CondLambda());
            AddLambda(environment, new DefineLambda());
            AddLambda(environment, new EmptyLambda());
            AddLambda(environment, new EvaluateLambda());
            AddLambda(environment, new FirstLambda());
            AddLambda(environment, new ListLambda());
            AddLambda(environment, new MacroLambda());
            AddLambda(environment, new PrependLambda());
            AddLambda(environment, new QuasiquoteLambda());
            AddLambda(environment, new QuoteLambda());
            AddLambda(environment, new RestLambda());
            AddLambda(environment, new UnquoteLambda());
            AddLambda(environment, new UnquoteSplicingLambda());
        }

        private static void AddLambda(IEnvironment environment, ILambda lambda)
        {
            using (var stringWriter = new StringWriter())
            {
                lambda.Write(stringWriter);
                environment.AddSymbol(stringWriter.ToString(), lambda);
            }
        }

        private class ClosureLambda : SExpression, ILambda
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
                        throw new LispException(
                            "Expected (lambda parameterList body). ParameterList not a list of symbols.");
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

        private class CondLambda : SExpression, ILambda
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
                    if (conditional.IsTrue())
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
        }

        private class DefineLambda : SExpression, ILambda
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

        private class EmptyLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
                {
                    throw new LispException("Expected (empty? collection)");
                }

                var sExpression = Evaluate(environment, list.Rest.First);
                var collection = sExpression as ICollection;
                if (collection == null)
                {
                    throw new LispException("Expected (empty collection). Collection is not a collection.");
                }

                return new Value(collection.ToList().IsEmpty);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("empty?");
            }
        }

        private class EvaluateLambda : SExpression, ILambda
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

        private class FirstLambda : SExpression, ILambda
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

        private class ListLambda : SExpression, ILambda
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
                return ListListEvaluatorRest(list.Rest, environment).Prepend(first);
            }
        }

        private class MacroLambda : SExpression, ILambda
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
                        throw new LispException(
                            "Expected (macro parameterList body). ParameterList not a list of symbols.");
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

        private class PrependLambda : SExpression, ILambda
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

                return consList.Prepend(list.First);
            }

            public override void Write(TextWriter textWriter)
            {
                textWriter.Write("prepend");
            }
        }

        private class QuasiquoteLambda : SExpression, ILambda
        {
            public ISExpression Evaluate(IEnvironment environment, IList list)
            {
                if (list.Rest.IsEmpty || !list.Rest.Rest.IsEmpty)
                {
                    throw new LispException("Expected (quasiquote form)");
                }

                var formList = list.Rest.First as IList;
                return formList == null
                    ? list.Rest.First
                    : QuasiquoteListEvaluatorRest(formList, environment);
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
                        return QuasiquoteListEvaluatorRest(list.Rest, environment).Prepend(first);
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

                return QuasiquoteListEvaluatorRest(list.Rest, environment).Prepend(list.First);
            }

            private static IList QuasiquoteUnquoteSplicing(IList spliceList, IList form)
            {
                if (spliceList.IsEmpty)
                {
                    return form;
                }

                return QuasiquoteUnquoteSplicing(spliceList.Rest, form).Prepend(spliceList.First);
            }
        }

        private class QuoteLambda : SExpression, ILambda
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

        private class RestLambda : SExpression, ILambda
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

        private class UnquoteLambda : SExpression, ILambda
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

        private class UnquoteSplicingLambda : SExpression, ILambda
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
}
