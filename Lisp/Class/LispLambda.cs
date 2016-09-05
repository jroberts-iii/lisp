using Lisp.Interface;

namespace Lisp.Class
{
    public class LispLambda : Lambda
    {
        public LispLambda(ISExpression body, IEnvironment closureEnvironment, params string[] parameterNames)
            : base(closureEnvironment, parameterNames)
        {
            Body = body;
        }

        public ISExpression Body { get; }

        public override ISExpression Evaluate(IEnvironment environment)
        {
            return Evaluate(environment, Body);
        }
    }
}
