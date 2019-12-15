using System;
using System.Collections.Generic;
using System.IO;

namespace CodeCracker.Console
{

	class Program
    {
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

		public static void CreateHtmlResults(Puzzle puzzle)
		{
			using var output = new System.IO.StreamWriter(@"C:\Users\Dominic\Desktop\cc\Results.html");
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

		static void Main()
		{
			SolveCodeCracker();
		}

		static public bool SolveCodeCracker()
        {
			var lettersAvailable = new List<char>();
			var puzzle = new Puzzle(new PuzzleImporter(File.ReadAllText(@"C:\Users\Dominic\Desktop\cc\ccImport.txt")));
			var words = puzzle.GetWordsFromGrid();


			for (int i = 1; i <= 26; i++)
			{
				char letter = NumToAlphabetLetter(i);
				if (!puzzle.IsLetterDecoded(letter))
				{
					lettersAvailable.Add(letter);
				}
			}

			puzzle.DisplayGrid();

			//Prints coded words + count
			for (int i = 0; i < words.Count; i++)
			{
				System.Console.WriteLine(words[i].GetOriginalCode());
			}
			System.Console.WriteLine("Word count is " + words.Count);

			var lettersAvailableBackup = lettersAvailable;
			puzzle.BackupDecodedLetters();
			var wordsBackup = words;
			var guessing = false;
			IEnumerator<string> guessingSolutionsList = null;
			CodedWord guessingSolutionWord = new CodedWord("01");
			CodedWord guessingSolutionWordBackup = new CodedWord("02");


			while (true)
			{
				//Say Current Letters
				System.Console.WriteLine("The current letters are:");
				foreach (var letter in lettersAvailable)
				{
					System.Console.Write($"{letter}  ");
				}
				System.Console.WriteLine();
				
				puzzle.DisplayEncodedLetters();

				//Prints out words with how many blanks they have and update each words' letters codings
				for (int i = 0; i < words.Count; i++)
				{
					var wordChanged = false;
					if (words[i].IsSolved())
					{
						continue;
					}
					for (int j = 1; j <= 26; j++)
					{
						if (!puzzle.IsNumberDecoded(j) || words[i].IsNumberDecoded(j))
						{
							continue;
						}
						words[i].UpdateLetterDecoding(j, puzzle.GetEncodedLetter(j));
						wordChanged = true;
					}
					if (wordChanged)
					{
						System.Console.WriteLine($"{words[i].ToCurrentCode().ToUpper()} has {words[i].UniqueBlanks()} blank/s");
					}
				}

				var wordsWithLowBlanks = new List<CodedWord>();

				for (int i = 0; i < words.Count; i++)
				{
					if (words[i].UniqueBlanks() < 4 && !words[i].IsSolved())
					{
						wordsWithLowBlanks.Add(words[i]);
					}
				}
				if (wordsWithLowBlanks.Count == 0)
				{
					for (int i = 0; i < words.Count; i++)
					{
						if (words[i].UniqueBlanks() == 4 && !words[i].IsSolved())
						{
							wordsWithLowBlanks.Add(words[i]);
						}
					}
				}
				CodedWord lowSolutionsWord = null;
				var lowSolutions = 1000000;
				var anyWordFoundSolutionless = false;
				for (int i = 0; i < wordsWithLowBlanks.Count; i++)
				{
					var wordWithLowBlanks = wordsWithLowBlanks.ToArray()[i];

					System.Console.Write("Searching word " + (i + 1) + " out of " + wordsWithLowBlanks.Count);
					var test = wordWithLowBlanks.FindPossibleSolutions(lettersAvailable).Count;
					System.Console.WriteLine(" - " + wordWithLowBlanks.ToSearchableWord().ToUpper() + " has " + test + " solutions");
					if (test < lowSolutions && test != 0)
					{
						lowSolutions = test;
						lowSolutionsWord = wordWithLowBlanks;
					}
					if (test == 0)
					{
						anyWordFoundSolutionless = true;
						break;
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
						System.Console.WriteLine("Either this puzzle cannot be solved or the import was incorrect!");
						break;
					}
					
					lettersAvailable = lettersAvailableBackup;
					puzzle.RestoreDecodedLetters();
					words = wordsBackup;
					guessingSolutionWord.Copy(guessingSolutionWordBackup);
					guessingSolutionsList.MoveNext();
					var correctSolution = guessingSolutionsList.Current;
					var charDone = new bool[26];
					for (int i = 0; i < correctSolution.Length; i++)
					{
						var codeNumber = guessingSolutionWord.GetEncodedNumber(i);
						var codeLetter = correctSolution.ToLower()[i];
						if (!charDone[AlphabetLetterToNum(codeLetter) - 1] && puzzle.IsNumberDecoded(codeNumber))
						{
							puzzle.DecodeNumber(codeNumber, codeLetter);
							lettersAvailable.Remove(char.ToUpper(codeLetter));

							for (int j = 0; j < words.Count; j++)
							{
								if (!words[j].IsSolved())
								{
									words[j].UpdateLetterDecoding(codeNumber, codeLetter);
									if (words[j].IsSolved())
									{
										System.Console.WriteLine("Word maybe found: " + words[j].GetSolvedWord());
									}
								}
							}

							guessingSolutionWord.UpdateLetterDecoding(codeNumber, codeLetter);

							if (guessingSolutionWord.IsSolved())
							{
								System.Console.WriteLine(guessingSolutionWord.GetSolvedWord() + " is being tested as a solution...");
							}
						}
						charDone[AlphabetLetterToNum(codeLetter) - 1] = true;
					}
					continue;
					
				}

				if (lowSolutionsWord == null) //Puzzle complete, exporting
				{
					System.Console.WriteLine("Congratulations, it is now solved!");
					CreateHtmlResults(puzzle);

					using (var output = new System.IO.StreamWriter(@"C:\Users\Dominic\Desktop\cc\Export.txt"))
					{
						output.WriteLine(puzzle.GetTitle());

						output.WriteLine("----------------------------------------------------------------------------");
						for (int i = 0; i < puzzle.GetGridHeight(); i++)
						{
							for (int j = 0; j < puzzle.GetGridWidth(); j++)
							{
								if (puzzle.IsGridSquareBlank(i, j))
								{
									output.Write("| ");
								}
								else
								{
									output.Write("|" + puzzle.GetEncodedLetter(i, j));
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
							output.Write((i + 1) + ": " + puzzle.GetEncodedLetter(i+1) + "  ");
							if (i == 25)
							{
								output.WriteLine();
							}
						}
					}
					return true;
				}
				else
				{
					System.Console.WriteLine("The word with the lowest solutions is " + lowSolutionsWord.ToSearchableWord().ToUpper() + " and has " + lowSolutions + " solutions");
				}

				if (lowSolutions == 1)
				{
					var lowSolWordEnumerator = lowSolutionsWord.GetPossibleSolutions();
					var correctSolution = lowSolWordEnumerator.Current;
					var charDone = new bool[26];
					for (int i = 0; i < correctSolution.Length; i++)
					{
						var codeNumber = lowSolutionsWord.GetEncodedNumber(i);
						var codeLetter = correctSolution.ToUpper().ToCharArray()[i];
						if (!charDone[AlphabetLetterToNum(codeLetter) - 1] && !puzzle.IsNumberDecoded(codeNumber))
						{
							puzzle.DecodeNumber(codeNumber, codeLetter);
							lettersAvailable.Remove(char.ToUpper(codeLetter));
							for (int j = 0; j < words.Count; j++)
							{
								if (!words[j].IsSolved())
								{
									words[j].UpdateLetterDecoding(codeNumber, codeLetter);
									if (words[j].IsSolved())
									{
										System.Console.WriteLine("Word found: " + words[j].GetSolvedWord());
									}
								}
							}
						}
						charDone[AlphabetLetterToNum(codeLetter) - 1] = true;
					}
					continue;
				}
				else
				{
					if (!guessing)
					{
						guessing = true;
						guessingSolutionsList = lowSolutionsWord.GetPossibleSolutions();
						guessingSolutionWord.Copy(lowSolutionsWord);
						guessingSolutionWordBackup.Copy(guessingSolutionWord);

						lettersAvailableBackup = lettersAvailable;
						puzzle.BackupDecodedLetters();
						wordsBackup = words;

						var correctSolution = guessingSolutionsList.Current;
						var charDone = new bool[26];
						for (int i = 0; i < correctSolution.Length; i++)
						{
							var codeNumber = guessingSolutionWord.GetEncodedNumber(i);
							var codeLetter = correctSolution.ToLower().ToCharArray()[i];
							if (!charDone[AlphabetLetterToNum(codeLetter) - 1] && puzzle.IsNumberDecoded(codeNumber))
							{
								puzzle.DecodeNumber(codeNumber, codeLetter);
								lettersAvailable.Remove(char.ToUpper(codeLetter));

								guessingSolutionWord.UpdateLetterDecoding(codeNumber, codeLetter);
								if (guessingSolutionWord.IsSolved())
								{
									System.Console.WriteLine(guessingSolutionWord.GetSolvedWord() + " is being tested as a solution...");
								}

								for (int j = 0; j < words.Count; j++)
								{
									if (!words[j].IsSolved())
									{
										words[j].UpdateLetterDecoding(codeNumber, codeLetter);
										if (words[j].IsSolved() && !CodedWord.CompareCodes(words[j],guessingSolutionWord))
										{
											System.Console.WriteLine(words[j].GetSolvedWord() + " was maybe found");
										}
									}
								}


							}
							charDone[AlphabetLetterToNum(codeLetter) - 1] = true;
						}
					}
					else
					{
						//This only happens if the letters from all the guessed words (even the correct one) do not lead to a 1 solution word
						System.Console.WriteLine("Bad! (This shouldn't happen and only happens if there is a bug!)");
						break;
					}
				}
			}
			return false;
		}
    }


}
