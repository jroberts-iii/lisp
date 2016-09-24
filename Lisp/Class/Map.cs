using System;
using System.Collections.Generic;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Map : Collection, IMap
    {
        private readonly Dictionary<ISExpression, ISExpression> _map = new Dictionary<ISExpression, ISExpression>();

        public override bool IsEmpty => _map.Count == 0;

        public bool Exists(ISExpression key)
        {
            ISExpression value;
            return _map.TryGetValue(key, out value);
        }

        public ISExpression Get(ISExpression key)
        {
            return _map[key];
        }

        public override IList ToList()
        {
            throw new NotImplementedException();
        }
    }
}
