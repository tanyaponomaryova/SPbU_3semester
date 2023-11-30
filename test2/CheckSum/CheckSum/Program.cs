using CheckSum;
using System.Diagnostics;

if (args.Length != 0)
{
    Console.WriteLine("Invalid input: program takes as an input path to directory which check sum should be computed.");
    return;
}

try
{
	var stopwatch = new Stopwatch();
	stopwatch.Start();
	var result1 = CheckSumEvaluation.ComputeSingleThreaded(args[0]);
	stopwatch.Stop();
	Console.WriteLine($"Single-threaded method calculated the value of the check sum for {stopwatch.ElapsedMilliseconds} ms.");

	stopwatch.Restart();
	var result2 = CheckSumEvaluation.ComputeMultiThreaded(args[0]);
	stopwatch.Stop();
	Console.WriteLine($"Multi-threaded method calculated the value of the check sum for {stopwatch.ElapsedMilliseconds} ms.");
}
catch (Exception e)
{
    Console.WriteLine("An error occurred during program execution.");
    Console.WriteLine(e.Message);
}