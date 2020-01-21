using CodeCracker.Core;

namespace CodeCracker.Console
{
	class Program
    {
		static void Main()
		{
			AllWords.GetInstance();
			var dir = @"C:\Users\Dominic\Desktop\cc";




			new PuzzleSolver(dir).SolveCodeCracker();
		}
    }
}
