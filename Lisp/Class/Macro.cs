using Lisp.Interface;

namespace Lisp.Class
{
    public class Macro : Atom, IMacro
    {
        public Macro(ISExpression body, params string[] parameterNames)
        {
            Body = body;
            ParameterNames = parameterNames;
        }

        public ISExpression Body { get; }
        public IEnvironment ClosureEnvironment => null;
        public string[] ParameterNames { get; }

        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            var env = new Environment();

            list = list.Rest;
            foreach (var parameterName in ParameterNames)
            {
                env.AddSymbol(parameterName, list.First);
                list = list.Rest;
            }

            var sExpression = Evaluate(env, Body);
            return Evaluate(environment, sExpression);
        }

        public override string ToString()
        {
            return "<macro>";
        }
    }
}
