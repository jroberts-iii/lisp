using Lisp.Interface;

namespace Lisp.Class
{
    public static class Constants
    {
        public static readonly IList EmptyList = List.Empty;

        public static readonly IValue<bool> True = Value.Create(true);
        public static readonly IValue<bool> False = Value.Create(false);

        public static readonly ISymbol Quasiquote = new Symbol("quasiquote");
        public static readonly ISymbol Quote = new Symbol("quote");
        public static readonly ISymbol Unquote = new Symbol("unquote");
        public static readonly ISymbol UnquoteSplicing = new Symbol("unquote-splicing");

        public static readonly ISymbol OpAddition = new Symbol("op-addition");
        public static readonly ISymbol OpBitwiseAnd = new Symbol("op-bitwise-and");
        public static readonly ISymbol OpBitwiseOr = new Symbol("op-bitwise-or");
        public static readonly ISymbol OpBitwiseXor = new Symbol("op-bitwise-xor");
        public static readonly ISymbol OpEqual = new Symbol("op-equal");
        public static readonly ISymbol OpGreaterThan = new Symbol("op-greater-than");
        public static readonly ISymbol OpGreaterThanOrEqual = new Symbol("op-greater-than-or-equal");
        public static readonly ISymbol OpLessThan = new Symbol("op-less-than");
        public static readonly ISymbol OpLessThanOrEqual = new Symbol("op-less-than-or-equal");
        public static readonly ISymbol OpLogicalAnd = new Symbol("op-logical-and");
        public static readonly ISymbol OpLogicalOr = new Symbol("op-logical-or");
        public static readonly ISymbol OpLogicalXor = new Symbol("op-logical-xor");
        public static readonly ISymbol OpMultiplication = new Symbol("op-multiplication");
        public static readonly ISymbol OpSubtraction = new Symbol("op-substraction");
    }
}
