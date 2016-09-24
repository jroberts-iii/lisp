namespace Lisp.Interface
{
    public interface IIdentifier : IAtom
    {
        string FullName { get; }
        string Name { get; }
        string Namespace { get; }
    }
}
