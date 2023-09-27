namespace Lazy
{
    /// <summary>
    /// Interface that represents lazy calculation.
    /// </summary>
    /// <typeparam name="T">type of returned value.</typeparam>
    public interface ILazy<T>
    {
        T Get();
    }
}