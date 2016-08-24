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
            MathSymbols.AddSymbols(this);
            SpecialSymbols.AddSymbols(this);
        }

        public Environment(IEnvironment closureEnvironment)
        {
            _closureEnvironment = closureEnvironment;
        }

        public IEnvironment TopEnvironment => _closureEnvironment == null
            ? this
            : _closureEnvironment.TopEnvironment;

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

            return _closureEnvironment != null && _closureEnvironment.TryGetSymbol(name, out sExpression);
        }
    }
}
