using System.Runtime.Serialization;

namespace Lisp.Exception
{
    public class LispException : System.Exception
    {
        public LispException()
        {
        }

        public LispException(string message) : base(message)
        {
        }

        public LispException(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected LispException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
