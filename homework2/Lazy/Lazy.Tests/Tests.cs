namespace Lazy.Tests
{
    [TestFixture]
    public class Tests
    {
        const int numberOfThreads = 100;

        [Test]
        public void SingleThreadedLazyReturnsExpectedResult()
        {
            var lazyStringConcat = new SingleThreadedLazy<string>(() => string.Concat("string1", "string2"));
            Assert.That(lazyStringConcat.Get(), Is.EqualTo("string1string2"));
            Assert.That(lazyStringConcat.Get(), Is.EqualTo("string1string2"));

            var lazyObjectCreation = new SingleThreadedLazy<object>(() => new object());
            var firstCallResult = lazyObjectCreation.Get();
            var secondCallResult = lazyObjectCreation.Get();
            Assert.That(secondCallResult, Is.SameAs(firstCallResult));
        }

        [Test]
        public void MultiThreadedLazyReturnsExpectedResultInSeveralThreadsStringConcat()
        {
            var lazyStringConcat = new SingleThreadedLazy<string>(() => string.Concat("string1", "string2"));
            var threads = new Thread[numberOfThreads];
            var threadsExecutionResults = new string[threads.Length];
            for (int i = 0; i < threads.Length; i++)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    threadsExecutionResults[localI] = lazyStringConcat.Get();
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            for (int i = 0; i < threadsExecutionResults.Length - 1; i++)
            {
                Assert.That(threadsExecutionResults[i + 1], Is.EqualTo(threadsExecutionResults[i]));
            }
        }

        [Test]
        public void MultiThreadedLazyReturnsExpectedResultInSeveralThreadsObjectCreation()
        {
            var lazyObjectCreation = new MultiThreadedLazy<object>(() => new object());
            var threads = new Thread[numberOfThreads];
            var threadsExecutionResults = new object[threads.Length];
            for (int i = 0; i < threads.Length; i++)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    threadsExecutionResults[localI] = lazyObjectCreation.Get();
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            for (int i = 0; i < threadsExecutionResults.Length - 1; i++)
            {
                Assert.That(threadsExecutionResults[i + 1], Is.EqualTo(threadsExecutionResults[i]));
            }
        }

        [Test]
        public void MultiThreadedLazyTestIncrementation()
        {
            int value = 0;
            var lazyIncrementation = new MultiThreadedLazy<int>(() => ++value);
            var threads = new Thread[numberOfThreads];
            var threadsExecutionResults = new int[threads.Length];
            for (int i = 0; i < threads.Length; i++)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    threadsExecutionResults[localI] = lazyIncrementation.Get();
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            for (int i = 0; i < threadsExecutionResults.Length - 1; i++)
            {
                Assert.That(threadsExecutionResults[i + 1], Is.EqualTo(threadsExecutionResults[i]));
            }
        }
    }
}