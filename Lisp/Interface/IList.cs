using System.Collections.Generic;

namespace Lisp.Interface
{
    public interface IList : ICollection, IEnumerable<ISExpression>
    {
        ISExpression First { get; }
        IList Rest { get; }

        IList Prepend(ISExpression sExpression);
    }
}
