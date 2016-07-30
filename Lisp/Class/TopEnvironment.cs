using System.Collections.Generic;
using Lisp.Interface;

namespace Lisp.Class
{
    public class TopEnvironment : ITopEnvironment
    {
        private readonly Dictionary<string, ISExpression> _symbolNameToSExpression =
            new Dictionary<string, ISExpression>();

        public void AddSymbol(string name, ISExpression sExpression)
        {
            _symbolNameToSExpression[name] = sExpression;
        }

        public bool TryGetSymbol(string name, out ISExpression sExpression)
        {
            return _symbolNameToSExpression.TryGetValue(name, out sExpression);
        }
    }
}
