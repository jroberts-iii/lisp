namespace Lisp.Interface
{
    public interface INamed
    {
        string FullName { get; }
        string Name { get; }
        string Namespace { get; }
    }
}
