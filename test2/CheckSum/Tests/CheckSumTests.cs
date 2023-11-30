using CheckSum;
using System.IO;

namespace Tests
{
    public class CheckSumTests
    {
        [TestCase("../..")]
        [TestCase("../../..")]
        public void SingleAndMultiThreadedMethodsReturnsSameResult(string path)
        {
            var singleThreadedResult = CheckSumEvaluation.ComputeSingleThreaded(path);
            var multiThreadedResult = CheckSumEvaluation.ComputeMultiThreaded(path);
            Assert.That(multiThreadedResult, Is.EqualTo(singleThreadedResult));
        }

        [TestCase("../..")]
        [TestCase("../../..")]
        public void SingleThreadedReturnsSameResultWhenCalledAgain(string path)
        {
            var singleThreadedFirstResult = CheckSumEvaluation.ComputeSingleThreaded(path);
            var singleThreadedSecondResult = CheckSumEvaluation.ComputeSingleThreaded(path);

            Assert.That(singleThreadedFirstResult, Is.EqualTo(singleThreadedSecondResult));
        }

        [TestCase("../..")]
        [TestCase("../../..")]
        public void MultiThreadedReturnsSameResultWhenCalledAgain(string path)
        {
            var multiThreadedFirstResult = CheckSumEvaluation.ComputeMultiThreaded(path);
            var multiThreadedSecondResult = CheckSumEvaluation.ComputeMultiThreaded(path);

            Assert.That(multiThreadedFirstResult, Is.EqualTo(multiThreadedSecondResult));
        }

        [TestCase("../..", "../../..")]
        public void SingleThreadedReturnsDifferentValuesForDifferentDirectories(string path1, string path2)
        {
            var singleThreadedFirstResult = CheckSumEvaluation.ComputeSingleThreaded(path1);
            var singleThreadedSecondResult = CheckSumEvaluation.ComputeSingleThreaded(path2);

            Assert.That(singleThreadedFirstResult, Is.Not.EqualTo(singleThreadedSecondResult));
        }

        [TestCase("../..", "../../..")]
        public void MultiThreadedReturnsDifferentValuesForDifferentDirectories(string path1, string path2)
        {
            var multiThreadedFirstResult = CheckSumEvaluation.ComputeMultiThreaded(path1);
            var multiThreadedSecondResult = CheckSumEvaluation.ComputeMultiThreaded(path2);

            Assert.That(multiThreadedSecondResult, Is.Not.EqualTo(multiThreadedFirstResult));
        }
    }
}