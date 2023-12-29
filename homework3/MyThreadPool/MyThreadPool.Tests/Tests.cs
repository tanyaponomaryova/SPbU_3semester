namespace MyThreadPoolTests;

public class Tests
{
    private MyThreadPool.MyThreadPool threadPool;
    private int numberOfThreads = Environment.ProcessorCount;

    [SetUp]
    public void Setup() 
        => threadPool = new(numberOfThreads);

    [TearDown]
    public void Teardown() 
        => threadPool.Shutdown();

    [Test]
    public void SubmitMethodTest()
    {
        var task1 = threadPool.Submit(() => 1 + 2);
        var task2 = threadPool.Submit(() => "result");
        Assert.That(task1.Result, Is.EqualTo(3));
        Assert.That(task2.Result, Is.EqualTo("result"));
    }

    [Test]
    public void ContinueWithMethodTest()
    {
        var task = threadPool.Submit(() => 5).ContinueWith(x => x + 2)
                                             .ContinueWith(x => x * 3)
                                             .ContinueWith(x => x.ToString() + "18");
        var continuation1 = task.ContinueWith(x => x + "78");
        var continuation2 = task.ContinueWith(x => x + "44");
        Assert.That(continuation1.Result, Is.EqualTo("211878"));
        Assert.That(continuation2.Result, Is.EqualTo("211844"));
    }

    [Test]
    public void NumberOfThreadsMatchesNumberFromConstructor()
    {
        CountdownEvent countdownEvent = new(numberOfThreads);
        ManualResetEvent mre = new(false);

        for (var i = 0; i < numberOfThreads; i++)
        {
            threadPool.Submit(() =>
            {
                countdownEvent.Signal();
                mre.WaitOne();
                return 0;
            });
        }

        var controlThread = new Thread(() =>
        {
            countdownEvent.Wait();
            mre.Set();
        });

        controlThread.Start();

        if (!controlThread.Join(10))
        {
            Assert.Fail();
        }
    }

    [Test]
    public void SubmitAfterShutdownShouldThrowInvalidOperationEx()
    {
        threadPool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 1));
    }

    // тест долгая задача плюс континью и шатдаун

    [Test]
    public void ContinueWithAfterShutdownShouldThrowInvalidOperationEx()
    {
        var task = threadPool.Submit(() => 1);
        threadPool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => task.ContinueWith(x => x + 1));
        Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 1).ContinueWith(x => x + 1));
    }

    [Test]
    public void ShutdownAfterContinueWithOfLongTask()
    {
        var longTask = threadPool.Submit(() =>
        {
            Thread.Sleep(100);
            return 0;
        });
        var continuation = longTask.ContinueWith(x => x);

        threadPool.Shutdown();

        Assert.That(continuation.Result, Is.EqualTo(0));
    }
}