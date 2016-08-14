using System;

namespace Lisp.Interface
{
    public interface ISExpression
    {
        Type GetNativeType();
        object GetNativeValue();
        ISExpression InvokeNativeMethod(IList list, IEnvironment environment);
    }
}
