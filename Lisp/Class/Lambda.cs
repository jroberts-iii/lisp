using System.Collections.Generic;
using System.Text;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public abstract class Lambda : Atom, ILambda
    {
        protected Lambda(IEnvironment closureEnvironment, params string[] parameterNames)
        {
            ClosureEnvironment = closureEnvironment;
            ParameterNames = parameterNames;
        }

        public IEnvironment ClosureEnvironment { get; }
        public string[] ParameterNames { get; }

        public virtual ISExpression Evaluate(IEnvironment environment, IList list)
        {
            var parameters = new List<ISExpression>();
            list = list.Rest;
            while (!list.IsEmpty)
            {
                parameters.Add(Evaluate(environment, list.First));
                list = list.Rest;
            }

            var parametersArray = parameters.ToArray();
            if (parametersArray.Length != ParameterNames.Length)
            {
                throw new LispException($"{ExceptionMessage()} Incorrect number of parameters.");
            }

            var env = environment.Push(ClosureEnvironment, parametersArray);
            for (var index = 0; index < ParameterNames.Length; index += 1)
            {
                env.AddSymbol(ParameterNames[index], parametersArray[index]);
            }

            return Evaluate(env);
        }

        public string ExceptionMessage()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Expected (");
            stringBuilder.Append(ToString());
            foreach (var parameterName in ParameterNames)
            {
                stringBuilder.Append(" ");
                stringBuilder.Append(parameterName);
            }

            stringBuilder.Append(").");
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return "<lambda>";
        }
    }
}
