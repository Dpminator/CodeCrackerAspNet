using CodeCracker.Core;
using System.IO;

namespace CodeCracker.Console
{
	class Program
    {
		private static string _directory = @"C:\Users\Dominic\Desktop\cc";

		static void Main()
		{
			AllWords.GetInstance();

			var lines = File.ReadAllText($@"{_directory}\ccImport.txt");
			var useNewCode = true;

			var solvedPuzzle = (useNewCode ? new PuzzleSolver(PuzzleImporter.ConvertOldToNew(lines)) : new PuzzleSolver(lines)).SolveCodeCracker();
			solvedPuzzle.CreateHtmlResults(_directory);
			solvedPuzzle.CreateTextFileResults(_directory);
		}
	}
}
