namespace Lisp.Interface
{
    public interface INamed : IAtom
    {
        string FullName { get; }
        string Name { get; }
        string Namespace { get; }
    }
}
