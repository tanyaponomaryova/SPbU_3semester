namespace SimpleFTP.Tests
{
    public class Tests
    {
        private int port = 8888;
        private Server server;
        private Client client;

        [SetUp]
        public void Setup()
        {
            server = new(port);
            client = new("127.0.0.1", port);
            _ = server.Start();
        }

        [TearDown]
        public void TearDown()
        {
            server.Stop();
        }

        [Test]
        public async Task NonValidRequest()
        {
            var response = await client.ExecuteRequest($"blablabla");
            Assert.That(response, Is.EqualTo("Incorrect request format."));
        }


        [Test]
        public async Task ListRequestTest()
        {
            var response = await client.ExecuteRequest($"1 ../../../FilesForTesting");
            Assert.That(response, Is.EqualTo("3 ../../../FilesForTesting\\TextFile1.txt false ../../../FilesForTesting\\TextFile2.txt false ../../../FilesForTesting\\SomeDirectory true\r\n.."));

            response = await client.ExecuteRequest($"1 ../../../FilesForTesting/SomeDirectory");
            Assert.That(response, Is.EqualTo("0\r\n"));

            response = await client.ExecuteRequest($"1 ../../../FilesForTesting/NonExistentDirectory");
            Assert.That(response, Is.EqualTo("The specified directory was not found."));
        }

        [Test]
        public async Task GetRequestTest()
        {
            var response = await client.ExecuteRequest($"2 ../../../FilesForTesting/TextFile1.txt");
            Assert.That(response, Is.EqualTo("33 balblabla\r\nblabla\r\nblablablabla\r\n\r\n"));

            response = await client.ExecuteRequest($"2 ../../../FilesForTesting/TextFile2.txt");
            Assert.That(response, Is.EqualTo("12 Hello World!\r\n"));

            response = await client.ExecuteRequest($"2 ../../../FilesForTesting/NonExistentFile.txt");
            Assert.That(response, Is.EqualTo("The specified file was not found."));
        }
    }
}