if (args.Length == 0 || args.Length > 1)
{
    Console.WriteLine("Invalid arguments: the program takes as input the path along which the tests need to be executed.");
    return;
}

Console.WriteLine("Tests are running...");
var myNUnit = new MyNUnit.MyNUnit();
myNUnit.RunTests(args[0]);
myNUnit.PrintResults();