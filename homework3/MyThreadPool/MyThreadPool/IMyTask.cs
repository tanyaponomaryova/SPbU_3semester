namespace MyThreadPool;

public interface IMyTask<TResult>
{
    /// <summary>
    /// Returns true if the task is completed.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Returns the result of the task.
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Accepts an object of type Func<TResult, TNewResult>, which can be applied to the result of a given task.
    /// </summary>
    /// <returns> a new task accepted for execution. </returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> function);
}