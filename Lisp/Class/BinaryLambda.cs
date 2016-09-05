using System;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public class BinaryLambda : SExpression, ILambda
    {
        private readonly string _name;
        private readonly Func<dynamic, dynamic, object> _opFunc;

        public BinaryLambda(string name, Func<dynamic, dynamic, object> opFunc)
        {
            _name = name;
            _opFunc = opFunc;
        }

        public IEnvironment ClosureEnvironment => null;
        public string[] ParameterNames => new[] {"x", "y"};

        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
            {
                throw new LispException($"Expected ({ToString()} sExpression sExpression)");
            }

            var xValue = list.Rest.First as IValue;
            var yValue = list.Rest.Rest.First as IValue;

            var result = _opFunc(xValue?.Val, yValue?.Val);
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
