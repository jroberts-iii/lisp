using System.IO;
using Lisp.Interface;

namespace Lisp.Class
{
    public class SExpression : ISExpression
    {
        public virtual ISExpression Evaluate(IEnvironment environment)
        {
            return this;
        }

        public static ISExpression Evaluate(IEnvironment environment, ISExpression sExpression)
        {
            return sExpression?.Evaluate(environment);
        }

        public virtual bool IsTrue()
        {
            return true;
        }

        public static string ToString(ISExpression sExpression)
        {
            return sExpression?.ToString() ?? "null";
        }

        public virtual void Write(TextWriter textWriter)
        {
            textWriter.Write(ToString());
        }

        public static void Write(TextWriter textWriter, ISExpression sExpression)
        {
            textWriter.Write(ToString(sExpression));
        }
    }
}
