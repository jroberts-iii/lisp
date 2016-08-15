using System;
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
            return Evaluate(environment, sExpression);
        }

        public virtual Type GetNativeType()
        {
            return GetType();
        }

        public virtual object GetNativeValue()
        {
            return this;
        }

        public virtual void Write(TextWriter textWriter)
        {
            textWriter.Write(ToString());
        }

        public static void Write(TextWriter textWriter, ISExpression sExpression)
        {
            if (sExpression == null)
            {
                textWriter.Write("nil");
            }
            else
            {
                sExpression.Write(textWriter);
            }
        }
    }
}
