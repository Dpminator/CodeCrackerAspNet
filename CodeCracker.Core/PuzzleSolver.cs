using System;
using System.Collections.Generic;

namespace CodeCracker.Core
{
    public class PuzzleSolver
    {
		private List<char> LettersAvailable;
		private readonly Puzzle PuzzleToSolve;
		private List<CodedWord> PuzzleWords;


		public PuzzleSolver(string lines)
		{
			LettersAvailable = new List<char>();
			PuzzleToSolve = new Puzzle(new PuzzleImporter(lines));
			PuzzleWords = PuzzleToSolve.GetWordsFromGrid();
		}

		public PuzzleSolver(int height, int width, string code, string letters, string name)
		{
			LettersAvailable = new List<char>();
			PuzzleToSolve = new Puzzle(new PuzzleImporter(height, width, code, letters, name));
			PuzzleWords = PuzzleToSolve.GetWordsFromGrid();
		}

		public PuzzleSolver((int height, int width, string code, string letters, string name) _)
		{
			LettersAvailable = new List<char>();
			PuzzleToSolve = new Puzzle(new PuzzleImporter(_.height, _.width, _.code, _.letters, _.name));
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

		private CodedWord ProcessSolvedWord(string solvedWord, bool guessing, CodedWord checkingWord)
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
								Console.WriteLine($"Word{(guessing ? " maybe " : " ")}found: {word.GetSolvedWord()}");
							}
						}
					}

					if (guessing)
					{
						checkingWord.UpdateLetterDecoding(codeNumber, codeLetter);
						if (checkingWord.IsSolved())
						{
							Console.WriteLine(checkingWord.GetSolvedWord() + " is being tested as a solution...");
						}
					}
				}
				charDone[AlphabetLetterToNum(codeLetter) - 1] = true;
			}
			return checkingWord;
		}

		public Puzzle SolveCodeCracker()
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
			foreach (var word in PuzzleWords) Console.WriteLine(word.GetOriginalCode());
			Console.WriteLine($"Word count is {PuzzleWords.Count}\n");

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
					Console.WriteLine("Congratulations, it is now solved!");
					return PuzzleToSolve;
				}

				//Say Current Letters
				Console.WriteLine("The current letters are:");
				foreach (var letter in LettersAvailable)
				{
					Console.Write($"{letter}  ");
				}
				Console.WriteLine();

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
						Console.WriteLine($"{word.ToCurrentCode().ToUpper()} has {word.UniqueBlanks()} blank/s");
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
					Console.Write($"Searching word {wordsWithLowBlanks.IndexOf(wordWithLowBlanks) + 1} out of {wordsWithLowBlanks.Count}");
					var test = wordWithLowBlanks.FindPossibleSolutions(LettersAvailable).Count;
					Console.WriteLine($" - {wordWithLowBlanks.ToSearchableWord()} has {test} solutions");

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

				Console.WriteLine("The word with the lowest solutions is " + lowSolutionsWord.ToSearchableWord().ToUpper() + " and has " + lowSolutions + " solutions");

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
		}

	}
}
