namespace Lazy;

/// <summary>
/// Class that represents lazy calculation for single-threaded using.
/// </summary>
/// <typeparam name="T">type of returned value.</typeparam>
public class SingleThreadedLazy<T> : ILazy<T>
{
    private Func<T>? supplier;
    private T? result;
    private bool isResultCalculated;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="supplier">calculating function.</param>
    public SingleThreadedLazy(Func<T> supplier) 
        => this.supplier = supplier;

    /// <summary>
    /// Сalculates the value.
    /// </summary>
    /// <returns>calculated value.</returns>
    public T Get()
    {
        if (!isResultCalculated)
        {
            if (supplier == null)
            {
                throw new InvalidOperationException("Supplier is null.");
            }

            result = supplier();
            isResultCalculated = true;
            supplier = null;
        }

        return result!;
    }
}