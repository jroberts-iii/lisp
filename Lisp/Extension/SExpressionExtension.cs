using System;
using System.IO;
using Lisp.Class;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Extension
{
    public static class Math
    {
        public static ISExpression EvaluateManyParameters(this ISExpression @this,
            IEnvironment environment,
            IList list,
            Func<dynamic, dynamic, object> opFunc)
        {
            if (list.Rest.IsEmpty)
            {
                using (var writeString = new StringWriter())
                {
                    @this.Write(writeString);
                    throw new LispException($"Expected ({writeString} form*)");
                }
            }

            object current = null;
            foreach (var sExpression in list.Rest)
            {
                var value = SExpression.Evaluate(environment, sExpression) as IValue;
                if (value != null)
                {
                    current = current != null
                        ? opFunc(current, value.Val)
                        : value.Val;
                }
            }

            return new Value(current);
        }

        public static ISExpression EvaluateManyParametersBoolean(this ISExpression @this,
            IEnvironment environment,
            IList list,
            Func<dynamic, dynamic, object> opFunc)
        {
            if (list.Rest.IsEmpty)
            {
                using (var writeString = new StringWriter())
                {
                    @this.Write(writeString);
                    throw new LispException($"Expected ({writeString} form*)");
                }
            }

            object current = null;
            foreach (var sExpression in list.Rest)
            {
                var value = SExpression.Evaluate(environment, sExpression).IsTrue();
                current = current != null
                    ? opFunc(current, value)
                    : value;
            }

            return new Value(current);
        }

        public static ISExpression EvaluateTwoParameters(this ISExpression @this,
            IEnvironment environment,
            IList list,
            Func<dynamic, dynamic, object> opFunc)
        {
            if (list.Rest.IsEmpty || !list.Rest.Rest.Rest.IsEmpty)
            {
                using (var writeString = new StringWriter())
                {
                    @this.Write(writeString);
                    throw new LispException($"Expected ({writeString} form form)");
                }
            }

            var value1 = SExpression.Evaluate(environment, list.Rest.First) as IValue;
            var value2 = SExpression.Evaluate(environment, list.Rest.Rest.First) as IValue;
            var result = opFunc(value1?.Val, value2?.Val);

            return new Value(result);
        }

        public static bool IsTrue(this ISExpression @this)
        {
            var value = @this as IValue;
            if (value?.Val is bool)
            {
                return (bool) value.Val;
            }

            return @this != null;
        }
    }
}
