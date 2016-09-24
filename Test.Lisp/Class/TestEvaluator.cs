using System.IO;
using Lisp.Class;
using Lisp.Exception;
using Lisp.Interface;
using NUnit.Framework;

namespace Test.Lisp.Class
{
    public class TestEvaluator
    {
        [Test]
        public void Evaluate_A_LispException()
        {
            var environment = new Environment();
            Assert.That(() => Check("a", null, environment), Throws.TypeOf<LispException>());
        }

        [Test]
        public void Evaluate_Anonymous_Successful()
        {
            var environment = new Environment();
            Check("((lambda (x y) (list x y)) 1 2)", "(1 2)", environment);
        }

        [Test]
        public void Evaluate_DinkA_Successful()
        {
            var environment = new Environment();
            Check("'a", "a", environment);
        }

        [Test]
        public void Evaluate_DinkListA_Successful()
        {
            var environment = new Environment();
            Check("'(a)", "(a)", environment);
        }

        [Test]
        public void Evaluate_DinkListAB_Successful()
        {
            var environment = new Environment();
            Check("'(a b)", "(a b)", environment);
        }

        [Test]
        public void Evaluate_DinkNull_Successful()
        {
            var environment = new Environment();
            Check("'()", "()", environment);
        }

        [Test]
        public void Evaluate_EmptyFalse_Successful()
        {
            var environment = new Environment();
            Check("(empty? (list 1))", "false", environment);
        }

        [Test]
        public void Evaluate_EmptyTrue_Successful()
        {
            var environment = new Environment();
            Check("(empty? ())", "true", environment);
        }

        [Test]
        public void Evaluate_FirstA_Successful()
        {
            var environment = new Environment();
            Assert.That(() => Check("(first 'a)", null, environment), Throws.TypeOf<LispException>());
        }

        [Test]
        public void Evaluate_FirstListA_Successful()
        {
            var environment = new Environment();
            Check("(first '(a))", "a", environment);
        }

        [Test]
        public void Evaluate_FirstListAB_Successful()
        {
            var environment = new Environment();
            Check("(first '(a b))", "a", environment);
        }

        [Test]
        public void Evaluate_FirstNoParameters_Successful()
        {
            var environment = new Environment();
            Assert.That(() => Check("(first)", null, environment), Throws.TypeOf<LispException>());
        }

        [Test]
        public void Evaluate_FirstNull_Successful()
        {
            var environment = new Environment();
            Check("(first ())", "null", environment);
        }

        [Test]
        public void Evaluate_Lambda_Successful()
        {
            var environment = new Environment();
            Check("(lambda (x) x)", "<lambda>", environment);
        }

        [Test]
        public void Evaluate_List_Successful()
        {
            var environment = new Environment();
            Check("(list)", "()", environment);
        }

        [Test]
        public void Evaluate_List1_Successful()
        {
            var environment = new Environment();
            Check("(list 1)", "(1)", environment);
        }

        [Test]
        public void Evaluate_List12_Successful()
        {
            var environment = new Environment();
            Check("(list 1 2)", "(1 2)", environment);
        }

        [Test]
        public void Evaluate_List123_Successful()
        {
            var environment = new Environment();
            Check("(list 1 2 3)", "(1 2 3)", environment);
        }

        [Test]
        public void Evaluate_Null_Successful()
        {
            var environment = new Environment();
            Check("()", "()", environment);
        }

        [Test]
        public void Evaluate_Quasiquote1_Successful()
        {
            var environment = new Environment();
            Check("`(a (+ 1 2) c)", "(a (+ 1 2) c)", environment);
        }

        [Test]
        public void Evaluate_Quasiquote2_Successful()
        {
            var environment = new Environment();
            Check("`(a ~(+ 1 2) c)", "(a 3 c)", environment);
        }

        [Test]
        public void Evaluate_Quasiquote3_Successful()
        {
            var environment = new Environment();
            Check("`(a (list 1 2) c)", "(a (list 1 2) c)", environment);
        }

        [Test]
        public void Evaluate_Quasiquote4_Successful()
        {
            var environment = new Environment();
            Check("`(a ~(list 1 2) c)", "(a (1 2) c)", environment);
        }

        [Test]
        public void Evaluate_Quasiquote5_Successful()
        {
            var environment = new Environment();
            Check("`(a ~@(list 1 2) c)", "(a 1 2 c)", environment);
        }

        [Test]
        public void Evaluate_QuoteA_Successful()
        {
            var environment = new Environment();
            Check("(quote a)", "a", environment);
        }

        [Test]
        public void Evaluate_QuoteListA_Successful()
        {
            var environment = new Environment();
            Check("(quote (a))", "(a)", environment);
        }

        [Test]
        public void Evaluate_QuoteListAB_Successful()
        {
            var environment = new Environment();
            Check("(quote (a b))", "(a b)", environment);
        }

        [Test]
        public void Evaluate_QuoteNull_Successful()
        {
            var environment = new Environment();
            Check("(quote ())", "()", environment);
        }

        [Test]
        public void Evaluate_RestA_Successful()
        {
            var environment = new Environment();
            Assert.That(() => Check("(rest 'a)", null, environment), Throws.TypeOf<LispException>());
        }

        [Test]
        public void Evaluate_RestListA_Successful()
        {
            var environment = new Environment();
            Check("(rest '(a))", "()", environment);
        }

        [Test]
        public void Evaluate_RestListAB_Successful()
        {
            var environment = new Environment();
            Check("(rest '(a b))", "(b)", environment);
        }

        [Test]
        public void Evaluate_RestNoParameters_Successful()
        {
            var environment = new Environment();
            Assert.That(() => Check("(rest)", null, environment), Throws.TypeOf<LispException>());
        }

        [Test]
        public void Evaluate_RestNull_Successful()
        {
            var environment = new Environment();
            Check("(rest ())", "()", environment);
        }

        [Test]
        public void Read_StringA_Successful()
        {
            var environment = new Environment();
            Check("\"a\"", "\"a\"", environment);
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
            var evaluated = SExpression.Evaluate(environment, sExpression);
            var result = Write(evaluated);
            if (result != output)
            {
                Assert.Fail($"{input} => {output} ! {result}");
            }
            else
            {
                Assert.Pass();
            }
        }

        private static ISExpression Read(string text)
        {
            return Reader.Read(new StringTextReader(text));
        }

        private static string Write(ISExpression sExpression)
        {
            using (var stringWriter = new StringWriter())
            {
                SExpression.Write(stringWriter, sExpression);
                return stringWriter.ToString();
            }
        }
    }
}
