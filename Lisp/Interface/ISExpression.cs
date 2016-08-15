using System;
using System.IO;

namespace Lisp.Interface
{
    public interface ISExpression
    {
        ISExpression Evaluate(IEnvironment environment);
        Type GetNativeType();
        object GetNativeValue();
        void Write(TextWriter textWriter);
    }
}
