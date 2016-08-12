namespace Lisp.Interface
{
    public interface IName : IAtom
    {
        string FullName { get; }
        string Name { get; }
        string Namespace { get; }
    }
}
