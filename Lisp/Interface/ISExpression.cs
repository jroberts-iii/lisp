using System.IO;

namespace Lisp.Interface
{
    public interface ISExpression
    {
        ISExpression Evaluate(IEnvironment environment);
        void Write(TextWriter textWriter);
    }
}
