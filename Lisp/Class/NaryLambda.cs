using System;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public class NaryLambda : SExpression, ILambda
    {
        private readonly string _name;
        private readonly Func<dynamic, dynamic, object> _opFunc;

        public NaryLambda(string name, Func<dynamic, dynamic, object> opFunc)
        {
            _name = name;
            _opFunc = opFunc;
        }

        public IEnvironment ClosureEnvironment => null;
        public string[] ParameterNames => new[] {"*"};

        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            if (list.Rest.IsEmpty)
            {
                throw new LispException($"Expected ({ToString()} sExpression*)");
            }

            object current = null;
            foreach (var sExpression in list.Rest)
            {
                var value = sExpression as IValue;
                if (value != null)
                {
                    current = current != null
                        ? _opFunc(current, value.Val)
                        : value.Val;
                }
            }

            if (current is bool)
            {
                return (bool) current
                    ? Constants.True
                    : Constants.False;
            }

            return new Value(current);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
