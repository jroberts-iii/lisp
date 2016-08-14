using System;
using Lisp.Interface;

namespace Lisp.Class
{
    public class SExpression : ISExpression
    {
        public virtual Type GetNativeType()
        {
            return GetType();
        }

        public virtual object GetNativeValue()
        {
            return this;
        }

        public virtual ISExpression InvokeNativeMethod(IList list, IEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }
}
