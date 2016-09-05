using System;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public class UnaryLambda : SExpression, ILambda
    {
        private readonly string _name;
        private readonly Func<object, object> _opFunc;

        public UnaryLambda(string name, Func<object, object> opFunc)
        {
            _name = name;
            _opFunc = opFunc;
        }

        public IEnvironment ClosureEnvironment => null;
        public string[] ParameterNames => new[] {"x"};

        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
            {
                throw new LispException($"Expected ({ToString()} sExpression)");
            }

            var result = _opFunc(list.Rest.First);
            if (result is bool)
            {
                return (bool) result
                    ? Constants.True
                    : Constants.False;
            }

            return new Value(result);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
