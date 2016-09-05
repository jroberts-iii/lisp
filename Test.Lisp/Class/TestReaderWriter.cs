using System.IO;
using Lisp.Class;
using Lisp.Interface;
using NUnit.Framework;

namespace Test.Lisp.Class
{
    public class TestReaderWriter
    {
        [Test]
        public void Read_A_Successful()
        {
            Check("a");
        }

        [Test]
        public void Read_DinkA_Successful()
        {
            Check("'a", "(quote a)");
        }

        [Test]
        public void Read_DinkListA_Successful()
        {
            Check("'(a)", "(quote (a))");
        }

        [Test]
        public void Read_DinkListAB_Successful()
        {
            Check("'(a b)", "(quote (a b))");
        }

        [Test]
        public void Read_DinkNull_Successful()
        {
            Check("'()", "(quote ())");
        }

        [Test]
        public void Read_Empty_Successful()
        {
            Check("(empty? ())", "(empty? ())");
        }

        [Test]
        public void Read_FirstA_Successful()
        {
            Check("(first 'a)", "(first (quote a))");
        }

        [Test]
        public void Read_FirstListA_Successful()
        {
            Check("(first '(a))", "(first (quote (a)))");
        }

        [Test]
        public void Read_FirstListAB_Successful()
        {
            Check("(first '(a b))", "(first (quote (a b)))");
        }

        [Test]
        public void Read_FirstNull_Successful()
        {
            Check("(first ())");
        }

        [Test]
        public void Read_List_Successful()
        {
            Check("(list)");
        }

        [Test]
        public void Read_List1_Successful()
        {
            Check("(list 1)");
        }

        [Test]
        public void Read_List12_Successful()
        {
            Check("(list 1 2)");
        }

        [Test]
        public void Read_List123_Successful()
        {
            Check("(list 1 2 3)");
        }

        [Test]
        public void Read_Null_Successful()
        {
            Check("()");
        }

        [Test]
        public void Read_QuoteA_Successful()
        {
            Check("(quote a)");
        }

        [Test]
        public void Read_QuoteListA_Successful()
        {
            Check("(quote (a))");
        }

        [Test]
        public void Read_QuoteListAB_Successful()
        {
            Check("(quote (a b))");
        }

        [Test]
        public void Read_QuoteNull_Successful()
        {
            Check("(quote ())");
        }

        [Test]
        public void Read_RestA_Successful()
        {
            Check("(rest 'a)", "(rest (quote a))");
        }

        [Test]
        public void Read_RestListA_Successful()
        {
            Check("(rest '(a))", "(rest (quote (a)))");
        }

        [Test]
        public void Read_RestListAB_Successful()
        {
            Check("(rest '(a b))", "(rest (quote (a b)))");
        }

        [Test]
        public void Read_RestNull_Successful()
        {
            Check("(rest ())");
        }

        [Test]
        public void Read_StringA_Successful()
        {
            Check("\"a\"");
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

        private static void Check(string input)
        {
            Check(input, input);
        }

        // ReSharper disable once UnusedParameter.Local
        private static void Check(string input, string output)
        {
            var sExpression = Read(input);
            var result = Write(sExpression);
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
