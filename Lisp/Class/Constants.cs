using Lisp.Interface;

namespace Lisp.Class
{
    public static class Constants
    {
        public static readonly ISymbol Quasiquote = new Symbol("quasiquote");
        public static readonly ISymbol Quote = new Symbol("quote");
        public static readonly ISymbol Unquote = new Symbol("unquote");
        public static readonly ISymbol UnquoteSplicing = new Symbol("unquote-splicing");

        public static readonly IValue True = new Boolean(true);
        public static readonly IValue False = new Boolean(false);

        public static readonly IList EmptyList = List.Empty;
    }
}
