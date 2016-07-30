using System.Collections.Generic;
using Lisp.Interface;

namespace Lisp.Class
{
    public static class Compiler
    {
        private static readonly Dictionary<string, ListEvaluator> NameToListEvaluator =
            new Dictionary<string, ListEvaluator>
            {
                {"defmacro", DefmacroListEvaluator}
            };

        public static ISExpression Compile(ISExpression sExpression, IEnvironment environment)
        {
            var list = sExpression as IList;
            return list == null ? sExpression : CallListEvaluator(list, environment);
        }

        private static ISExpression CallListEvaluator(IList list, IEnvironment environment)
        {
            if (list.IsEmpty)
            {
                return list;
            }

            var specialSymbol = list.First as ISymbol;
            if (specialSymbol == null)
            {
                return list;
            }

            ListEvaluator listEvaluator;
            return NameToListEvaluator.TryGetValue(specialSymbol.Name, out listEvaluator)
                ? listEvaluator(list, environment)
                : list;
        }

        private static ISExpression DefmacroListEvaluator(IList list, IEnvironment environment)
        {
            return list;
        }

        private delegate ISExpression ListEvaluator(IList list, IEnvironment environment);
    }
}
