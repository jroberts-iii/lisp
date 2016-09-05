using System.IO;

namespace Lisp.Interface
{
    public interface ISExpression
    {
        ISExpression Evaluate(IEnvironment environment);
        bool IsTrue();
        void Write(TextWriter textWriter);
    }
}
