using System;
using System.Collections.Generic;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Vector : Collection, IVector
    {
        private readonly List<ISExpression> _list = new List<ISExpression>();

        public override bool IsEmpty => _list.Count == 0;

        public ISExpression Get(int index)
        {
            return _list[index];
        }

        public int Length()
        {
            return _list.Count;
        }

        public override IList ToList()
        {
            throw new NotImplementedException();
        }
    }
}
