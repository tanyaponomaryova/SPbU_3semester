namespace MyThreadPool;

using System.Collections.Concurrent;

public class MyThreadPool
{
    private class MyTask<TResult> : IMyTask<TResult>
    {
        public MyTask(Func<TResult> function, MyThreadPool threadPool)
        {
            this.function = function;
            this.threadPool = threadPool;
        }

        private Func<TResult> function;
        private MyThreadPool threadPool;
        private ManualResetEvent resultIsReadyEvent = new(false);
        private object locker = new();
        private Exception? caughtException;
        private List<Action> continuationActions = new();
        public bool IsCompleted { get; private set; }

        private TResult? result;
        public TResult? Result
        {
            get
            {
                resultIsReadyEvent.WaitOne();
                if (caughtException != null)
                {
                    throw new AggregateException(caughtException);
                }

                return result;
            }
        }

        public void Run()
        {
            try
            {
                result = function();
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }
            finally
            {
                lock (locker)
                {
                    IsCompleted = true;
                    resultIsReadyEvent.Set();
                }

                RunContinuationTasks();
            }
        }

        private void RunContinuationTasks()
        {
            foreach (var continuation in continuationActions)
            {
                threadPool.SubmitContinuation(continuation);
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> function)
        {
            if (threadPool.cts.IsCancellationRequested)
            {
                throw new InvalidOperationException("It is impossible to add new tasks after shutdown.");
            }

            var continuationTask = new MyTask<TNewResult>(() => function(Result), threadPool);

            lock (locker)
            {
                if (!IsCompleted)
                {
                    continuationActions.Add(() => continuationTask.Run());
                }
                else
                {
                    threadPool.SubmitContinuation(() => continuationTask.Run());
                }
            }

            return continuationTask;
        }
    }

    private ConcurrentQueue<Action> tasksQueue = new();
    private Thread[] threads;
    private CancellationTokenSource cts = new();
    private object ctsLocker = new();
    private ManualResetEvent shutdownRequestedEvent = new(false);
    private AutoResetEvent queueIsNotEmptyEvent = new(false);
    private WaitHandle[] events;

    public MyThreadPool(int numberOfThreads)
    {
        if (numberOfThreads <= 0)
        {
            throw new ArgumentOutOfRangeException("The number of threads must be at least 1.");
        }

        events = new WaitHandle[] { shutdownRequestedEvent, queueIsNotEmptyEvent };

        threads = new Thread[numberOfThreads];
        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() => RunAction())
            {
                IsBackground = true
            };
            threads[i].Start();
        }
    }

    private void RunAction()
    {
        while (true)
        {
            if (tasksQueue.TryDequeue(out var task))
            {
                task();
            }
            else
            {
                if (cts.IsCancellationRequested)
                {
                    break;
                }

                WaitHandle.WaitAny(events);

                if (tasksQueue.Count >= 2)
                {
                    queueIsNotEmptyEvent.Set();
                }
            }
        }
    }

    /// <summary>
    /// Submits the task for execution.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        if (cts.IsCancellationRequested)
        {
            throw new InvalidOperationException("It is impossible to add new tasks after shutdown.");
        }

        var task = new MyTask<TResult>(function, this);
        tasksQueue.Enqueue(() => task.Run());
        queueIsNotEmptyEvent.Set();
        return task;
    }

    private void SubmitContinuation(Action continuationAction)
    {
        tasksQueue.Enqueue(continuationAction);
        queueIsNotEmptyEvent.Set();
    }

    /// <summary>
    /// Terminates the threads. Already running tasks are not interrupted, 
    /// but new tasks are not accepted for execution by threads from the pool.
    /// </summary>
    public void Shutdown()
    {
        cts.Cancel();
        shutdownRequestedEvent.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}