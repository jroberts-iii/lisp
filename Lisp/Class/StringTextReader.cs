using Lisp.Interface;

namespace Lisp.Class
{
    public class StringTextReader : ITextReader
    {
        private readonly string _value;
        private int _currentOffset;

        public StringTextReader(string value)
        {
            _value = value;
        }

        public char Peek()
        {
            return _currentOffset < _value.Length ? _value[_currentOffset] : '\0';
        }

        public char Read()
        {
            var currentChar = Peek();
            if (currentChar != '\0')
            {
                _currentOffset += 1;
            }

            return currentChar;
        }
    }
}
