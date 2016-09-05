using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lisp.Exception;
using Lisp.Interface;
using IList = Lisp.Interface.IList;

namespace Lisp.Class
{
    public class List : SExpression, IList
    {
        public static IList Empty = new List();

        private List()
        {
            IsEmpty = true;
            First = null;
            Rest = this;
        }

        private List(ISExpression first, IList rest)
        {
            IsEmpty = false;
            First = first;
            Rest = rest;
        }

        public ISExpression First { get; }
        public bool IsEmpty { get; }
        public IList Rest { get; }

        public override ISExpression Evaluate(IEnvironment environment)
        {
            if (IsEmpty)
            {
                return Constants.EmptyList;
            }

            if (First == null)
            {
                throw new LispException("List.Evaluate expected First to not be null.");
            }

            var lambda = Evaluate(environment, First) as ILambda;
            if (lambda == null)
            {
                throw new LispException("List.Evaluate expected First to evaluate to a lambda.");
            }

            return lambda.Evaluate(environment, this);
        }

        public IEnumerator<ISExpression> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IList Prepend(ISExpression sExpression)
        {
            return new List(sExpression, this);
        }

        public IList ToList()
        {
            return this;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("(");
            if (!IsEmpty)
            {
                stringBuilder.Append(First);
                foreach (var sExpression in Rest)
                {
                    stringBuilder.Append(" ");
                    stringBuilder.Append(ToString(sExpression));
                }
            }

            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Enumerator : IEnumerator<ISExpression>
        {
            private IList _list;

            public Enumerator(IList list)
            {
                _list = new List(null, list);
            }

            public ISExpression Current => _list.First;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                _list = _list.Rest;
                return !_list.IsEmpty;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
