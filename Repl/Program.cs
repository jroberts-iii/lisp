using System;
using System.IO;
using Lisp.Class;
using Environment = Lisp.Class.Environment;

namespace Repl
{
    internal class Program
    {
        private static void Main()
        {
            var environment = new Environment();
            while (true)
            {
                Console.Write(">");
                var text = Console.ReadLine();
                try
                {
                    var sExpression = Reader.Read(new StringTextReader(text));
                    sExpression = Evaluator.Evaluate(sExpression, environment);
                    using (var stringWriter = new StringWriter())
                    {
                        Writer.Write(stringWriter, sExpression);
                        Console.WriteLine(stringWriter.ToString());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
