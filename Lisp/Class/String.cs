using Lisp.Interface;

namespace Lisp.Class
{
    public class String : IString
    {
        public String(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
