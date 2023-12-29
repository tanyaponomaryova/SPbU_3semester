namespace Lazy;

/// <summary>
/// Class that represents lazy calculation for multi-threaded using.
/// </summary>
/// <typeparam name="T">type of returned value.</typeparam>
public class MultiThreadedLazy<T> : ILazy<T>
{
    private Func<T>? supplier;
    private T? result;
    private volatile bool isResultCalculated = false;
    private object locker = new();

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="supplier">calculating function.</param>
    public MultiThreadedLazy(Func<T> supplier)
    {
        this.supplier = supplier;
    }

    /// <summary>
    /// Сalculates the value.
    /// </summary>
    /// <returns>calculated value.</returns>
    public T Get()
    {
        if (isResultCalculated)
        {
            return result!;
        }
        lock (locker)
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
}