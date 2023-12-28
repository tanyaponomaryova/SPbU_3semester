using System.Collections.Concurrent;

public class MyThreadPool
{
    // очередь из функций () => () то есть ничего не принимают и ничего не возвращают
    private ConcurrentQueue<Action> tasksQueue = new();
    private Thread[] threads;
    private AutoResetEvent queueIsNotEmptyEvent = new(false); // изначально в несигнальном состоянии (очередь ещё пустая)
                                                              // пропускает по-одному
    public MyThreadPool(int numberOfThreads)
    {
        if (numberOfThreads <= 0)
        {
            throw new ArgumentOutOfRangeException("The number of threads must be at least 1.");
        }

        threads = new Thread[numberOfThreads];
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() => RunAction());
            threads[i].Name = $"MyThreadPool Thread {i + 1}";
            threads[i].IsBackground = true;
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
                queueIsNotEmptyEvent.WaitOne(); // ожидаем когда очередь будет не пустой
                                                // сейчас в очереди хотя бы один task
                if (tasksQueue.Count >= 2)
                { // если там ещё остались taskи то надо выпустить остальные потоки  
                    queueIsNotEmptyEvent.Set();
                }
            }
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        var task = new MyTask<TResult>(function, this);
        tasksQueue.Enqueue(() => task.Run());
        queueIsNotEmptyEvent.Set();
        return task;
    }

    public void SubmitContinuation(Action continuationAction)
    {
        tasksQueue.Enqueue(continuationAction);
        queueIsNotEmptyEvent.Set(); // в очередь добавилось task => можно выпустить один (как минимум) поток из ожидания
    }
}

public class MyTask<TResult> : IMyTask<TResult>
{
    public MyTask(Func<TResult> function, MyThreadPool threadPool)
    {
        this.function = function;
        this.threadPool = threadPool;
    }

    public Func<TResult> function;
    public MyThreadPool threadPool;
    public ManualResetEvent resultIsReadyEvent = new(false); // изначально в несигнальном состоянии (занято)
    private object locker = new();
    public Exception? caughtException;
    public List<Action> continuationActions = new();
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
                resultIsReadyEvent.Set(); // сигнализируем о том что результат готов (освобождаем)
            }

            RunContinuationTasks();
        }
    }

    private void RunContinuationTasks()
    {
        Action? continuation = null;
        if (continuationActions.Count > 0)
        {
            continuation = continuationActions[0];
            continuationActions.RemoveAt(0);
            threadPool.SubmitContinuation(continuation);
        }
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> function)
    {
        var continuationTask = new MyTask<TNewResult>(() => function(Result), threadPool);

        lock (locker)
        {
            if (!IsCompleted)
            { // результата пока нет => откладываем вычисление до момента пока не появится результат
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