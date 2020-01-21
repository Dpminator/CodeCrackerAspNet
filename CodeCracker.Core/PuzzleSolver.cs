using System;
using System.Collections.Generic;
using System.IO;

namespace CodeCracker.Core
{
    public class PuzzleSolver
    {
		private static string _directory = @"C:\Users\Dominic\Desktop\cc";

		private List<char> LettersAvailable;
		private readonly Puzzle PuzzleToSolve;
		private List<CodedWord> PuzzleWords;


		public PuzzleSolver(string lines)
		{
			LettersAvailable = new List<char>();
			PuzzleToSolve = new Puzzle(new PuzzleImporter(lines));
			PuzzleWords = PuzzleToSolve.GetWordsFromGrid();
		}

		public static char NumToAlphabetLetter(int number)
		{
			var letter = (char)(number + 64);
			if (!char.IsLetter(letter))
				throw new InvalidOperationException();
			return char.ToUpper(letter);
		}

		public static int AlphabetLetterToNum(char letter)
		{
			if (!char.IsLetter(letter))
				throw new InvalidOperationException();
			return char.ToUpper(letter) - 64;
		}

		public void CreateHtmlResults(Puzzle puzzle)
		{
			using var output = new System.IO.StreamWriter($@"{_directory}Results.html");
			output.WriteLine("<!DOCTYPE html>");
			output.WriteLine("<html>");
			output.WriteLine("<head>");
			output.WriteLine("<title>Results for Code Cracker</title>");
			output.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>");
			output.WriteLine("<link href='https://fonts.googleapis.com/css?family=Patrick+Hand' rel='stylesheet'>");
			output.WriteLine("<style type='text/css'>");
			output.WriteLine("body { background: #e3cd73; color: #000000; margin-right: 20px; margin-left: 20px; font-size: 14px; font-family: Arial, sans-serif, sans; }");
			output.WriteLine("h1 { color: #000000; font-size: 36px; text-align: center; }");
			output.WriteLine("h2 { text-align: center; font-size: 20px; }");
			output.WriteLine("table.result { border: 1px solid #000000; margin-left: auto; margin-right: auto; padding: 0px; border-spacing: 0px; font-family: sans; font-size: 170%;}");
			output.WriteLine("table.result td { border: 1px solid #000000; padding: 0px; background: #ffffff; text-align: center; width: 32px; height: 30px; font-family: 'Patrick Hand'; line-height: 0%}");
			output.WriteLine("table tr td.blank {background: #000000;}");
			output.WriteLine("div.small{ font-size: 55%; text-align: left; line-height: 100%; position: relative; bottom:9px; left:1px;}");
			output.WriteLine("div.regular{ position: relative; bottom:4px;}");
			output.WriteLine("</style>");
			output.WriteLine("</head>");
			output.WriteLine("<body>");
			output.WriteLine($"<h1>{puzzle.GetTitle()}</h1>");
			output.WriteLine("<table class='result'>");
			for (int i = 0; i < puzzle.GetGridHeight(); i++)
			{
				output.WriteLine("<tr>");
				for (int j = 0; j < puzzle.GetGridWidth(); j++)
				{
					if (puzzle.IsGridSquareBlank(i, j))
					{
						output.WriteLine("<td class = 'blank'></td>");
					}
					else
					{
						output.WriteLine("<td><div class = 'small'>" + puzzle.GetGridNumberAsString(i, j) + "</div><br><div class = 'regular'>" + puzzle.GetEncodedLetter(i, j) + "</div></td>");
					}
				}
				output.WriteLine("</tr>");
			}
			output.WriteLine("</table>");
			output.WriteLine("<h2>Letters:</h2>");
			output.WriteLine("<table class='result'><tr>");
			for (int i = 1; i <= 26; i++)
			{
				if (i == 14) output.WriteLine("</tr><tr>");
				output.WriteLine("<td><div class = 'small'>" + i + "</div><br><div class = 'regular'>" + puzzle.GetEncodedLetter(i) + "</div></td>");
			}
			output.WriteLine("</tr></table>");
			output.WriteLine("</table>");
			output.WriteLine("</body>");
			output.WriteLine("</html>");
		}

		private static List<CodedWord> FindWordsWithLowBlanks(List<CodedWord> words, int blankCount, List<CodedWord> wordsWithLowBlanks)
		{
			foreach (var word in words)
			{
				if (word.UniqueBlanks() == blankCount && !word.IsSolved())
				{
					wordsWithLowBlanks.Add(word);
				}
			}
			return wordsWithLowBlanks;
		}

		public bool SolveCodeCracker()
		{
			for (int i = 1; i <= 26; i++)
			{
				char letter = NumToAlphabetLetter(i);
				if (!PuzzleToSolve.IsLetterDecoded(letter))
				{
					LettersAvailable.Add(letter);
				}
			}

			PuzzleToSolve.DisplayGrid();

			//Prints coded words + count
			foreach (var word in PuzzleWords) System.Console.WriteLine(word.GetOriginalCode());
			System.Console.WriteLine($"Word count is {PuzzleWords.Count}\n");

			var lettersAvailableBackup = LettersAvailable;
			PuzzleToSolve.BackupDecodedLetters();
			var wordsBackup = PuzzleWords;
			var guessing = false;
			IEnumerator<string> guessingSolutionsList = null;
			CodedWord guessingSolutionWord = null;
			CodedWord guessingSolutionWordBackup = null;


			while (true)
			{
				if (LettersAvailable.Count == 0)
				{
					System.Console.WriteLine("Congratulations, it is now solved!");
					CreateHtmlResults(PuzzleToSolve);

					using (var output = new System.IO.StreamWriter($@"{_directory}Export.txt"))
					{
						output.WriteLine(PuzzleToSolve.GetTitle());

						output.WriteLine("----------------------------------------------------------------------------");
						for (int i = 0; i < PuzzleToSolve.GetGridHeight(); i++)
						{
							for (int j = 0; j < PuzzleToSolve.GetGridWidth(); j++)
							{
								if (PuzzleToSolve.IsGridSquareBlank(i, j))
								{
									output.Write("| ");
								}
								else
								{
									output.Write("|" + PuzzleToSolve.GetEncodedLetter(i, j));
								}
							}
							output.WriteLine("|");
						}
						output.WriteLine("----------------------------------------------------------------------------\n");

						for (int i = 0; i < 26; i++)
						{
							if (i == 13)
							{
								output.WriteLine();
							}
							if (i < 9)
							{
								output.Write("0");
							}
							output.Write((i + 1) + ": " + PuzzleToSolve.GetEncodedLetter(i + 1) + "  ");
							if (i == 25)
							{
								output.WriteLine();
							}
						}
					}
					return true;
				}

				//Say Current Letters
				System.Console.WriteLine("The current letters are:");
				foreach (var letter in LettersAvailable)
				{
					System.Console.Write($"{letter}  ");
				}
				System.Console.WriteLine();

				PuzzleToSolve.DisplayEncodedLetters();

				//Prints out words with how many blanks they have and update each words' letters codings
				foreach (var word in PuzzleWords)
				{
					if (word.IsSolved())
						continue;

					var wordChanged = false;

					for (int j = 1; j <= 26; j++)
					{
						if (!PuzzleToSolve.IsNumberDecoded(j) || !word.CodeContainsNumber(j) || word.IsNumberDecoded(j))
						{
							continue;
						}
						word.UpdateLetterDecoding(j, PuzzleToSolve.GetEncodedLetter(j));
						wordChanged = true;
					}
					if (wordChanged)
					{
						System.Console.WriteLine($"{word.ToCurrentCode().ToUpper()} has {word.UniqueBlanks()} blank/s");
					}
				}

				var wordsWithLowBlanks = new List<CodedWord>();
				int BlankCount = 1;
				while (BlankCount < 4 || wordsWithLowBlanks.Count == 0)
				{
					FindWordsWithLowBlanks(PuzzleWords, BlankCount++, wordsWithLowBlanks);
				}

				CodedWord lowSolutionsWord = null;
				var lowSolutions = 1000000;
				var anyWordFoundSolutionless = false;
				foreach (var wordWithLowBlanks in wordsWithLowBlanks)
				{
					System.Console.Write($"Searching word {wordsWithLowBlanks.IndexOf(wordWithLowBlanks) + 1} out of {wordsWithLowBlanks.Count}");
					var test = wordWithLowBlanks.FindPossibleSolutions(LettersAvailable).Count;
					System.Console.WriteLine($" - {wordWithLowBlanks.ToSearchableWord()} has {test} solutions");

					if (test == 0)
					{
						anyWordFoundSolutionless = true;
						break;
					}
					if (test < lowSolutions)
					{
						lowSolutions = test;
						lowSolutionsWord = wordWithLowBlanks;
					}

					if (lowSolutions == 1)
					{
						break;
					}
				}

				if (anyWordFoundSolutionless)
				{
					if (!guessing)
					{
						throw new InvalidOperationException("Either this puzzle cannot be solved or the import was incorrect!");
					}

					LettersAvailable = lettersAvailableBackup;
					PuzzleToSolve.RestoreDecodedLetters();
					PuzzleWords = wordsBackup;
					guessingSolutionWord = guessingSolutionWordBackup;
					//Breaks if list has only 1 word total
					guessingSolutionsList.MoveNext();

					guessingSolutionWord = ProcessSolvedWord(guessingSolutionsList.Current, true, guessingSolutionWord);
					continue;
				}

				System.Console.WriteLine("The word with the lowest solutions is " + lowSolutionsWord.ToSearchableWord().ToUpper() + " and has " + lowSolutions + " solutions");

				if (lowSolutions == 1)
				{
					var lowSolWordEnumerator = lowSolutionsWord.GetPossibleSolutions();

					ProcessSolvedWord(lowSolWordEnumerator.Current, false, lowSolutionsWord);
					continue;
				}
				else
				{
					if (!guessing)
					{
						guessing = true;
						guessingSolutionsList = lowSolutionsWord.GetPossibleSolutions();
						guessingSolutionWord = lowSolutionsWord;
						guessingSolutionWordBackup = guessingSolutionWord;

						lettersAvailableBackup = LettersAvailable;
						PuzzleToSolve.BackupDecodedLetters();
						wordsBackup = PuzzleWords;

						guessingSolutionWord = ProcessSolvedWord(guessingSolutionsList.Current, true, guessingSolutionWord);
					}
					else
					{
						//This only happens if the letters from all the guessed words (even the correct one) do not lead to a 1 solution word
						throw new InvalidOperationException("Bad! (This shouldn't happen and only happens if there is a bug!)");
					}
				}
			}

			CodedWord ProcessSolvedWord(string solvedWord, bool guessing, CodedWord checkingWord)
			{
				var charDone = new bool[26];
				foreach ((var codeLetter, var index) in solvedWord.ToUpper().ToListWithIndex())
				{
					var codeNumber = checkingWord.GetEncodedNumber(index);

					if (!charDone[AlphabetLetterToNum(codeLetter) - 1] && !PuzzleToSolve.IsNumberDecoded(codeNumber))
					{
						PuzzleToSolve.DecodeNumber(codeNumber, codeLetter);
						LettersAvailable.Remove(char.ToUpper(codeLetter));

						foreach (var word in PuzzleWords)
						{
							if (!word.IsSolved())
							{
								word.UpdateLetterDecoding(codeNumber, codeLetter);
								if (word.IsSolved())
								{
									System.Console.WriteLine($"Word{(guessing ? " maybe " : " ")}found: {word.GetSolvedWord()}");
								}
							}
						}

						if (guessing)
						{
							checkingWord.UpdateLetterDecoding(codeNumber, codeLetter);
							if (checkingWord.IsSolved())
							{
								System.Console.WriteLine(checkingWord.GetSolvedWord() + " is being tested as a solution...");
							}
						}
					}
					charDone[AlphabetLetterToNum(codeLetter) - 1] = true;
				}
				return checkingWord;
			}
		}
	}
}
