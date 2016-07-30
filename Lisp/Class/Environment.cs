using System.Collections.Generic;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Environment : IEnvironment
    {
        private readonly IEnvironment _closureEnvironment;

        private readonly Dictionary<string, ISExpression> _symbolNameToSExpression =
            new Dictionary<string, ISExpression>();

        public Environment()
        {
            _closureEnvironment = null;
            TopEnvironment = new TopEnvironment();
        }

        public Environment(IEnvironment closureEnvironment)
        {
            _closureEnvironment = closureEnvironment;
            TopEnvironment = closureEnvironment.TopEnvironment;
        }

        public Environment(ITopEnvironment topEnvironment)
        {
            _closureEnvironment = null;
            TopEnvironment = topEnvironment;
        }

        public ITopEnvironment TopEnvironment { get; }

        public void AddSymbol(string name, ISExpression sExpression)
        {
            _symbolNameToSExpression[name] = sExpression;
        }

        public bool TryGetSymbol(string name, out ISExpression sExpression)
        {
            if (_symbolNameToSExpression.TryGetValue(name, out sExpression))
            {
                return true;
            }

            if (_closureEnvironment != null && _closureEnvironment.TryGetSymbol(name, out sExpression))
            {
                return true;
            }

            return TopEnvironment != null && TopEnvironment.TryGetSymbol(name, out sExpression);
        }
    }
}
