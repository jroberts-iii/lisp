using System.IO;
using Lisp.Class;
using Lisp.Interface;
using NUnit.Framework;

namespace Test.Lisp.Class
{
    public class TestCompiler
    {
        [Test]
        public void Evaluate_Anonymous_Successful()
        {
            var environment = new Environment();
            Check("((lambda (x y) (list x y)) 1 2)", "(1 2)", environment);
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
        }

        // ReSharper disable once UnusedParameter.Local
        private static void Check(string input, string output, IEnvironment environment)
        {
            var sExpression = Read(input);
            var compiled = Compiler.Compile(sExpression, environment);
            var result = Write(compiled);
            if (result != output)
            {
                Assert.Fail($"{input} => {output} ! {result}");
            }
            else
            {
                Assert.Pass();
            }
        }

        private static ISExpression Read(string expression)
        {
            return Reader.Read(new StringTextReader(expression));
        }

        private static string Write(ISExpression sExpression)
        {
            using (var stringWriter = new StringWriter())
            {
                Writer.Write(stringWriter, sExpression);
                return stringWriter.ToString();
            }
        }
    }
}
