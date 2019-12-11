using System;
using System.Collections.Generic;

namespace CodeCracker.Console
{

    class Program
    {
		static readonly char[] Alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
		static bool[] LettersAvailable = new bool[26];

		public static int AlphabetLetterToNum(char Letter)
		{
			Letter = char.ToUpper(Letter);
			for (int i = 0; i < 26; i++)
			{
				if (Alphabet[i] == Letter)
				{
					return i + 1;
				}
			}
			return -1;
		}

		public static void CreateHtmlResults(int GridHeight, int GridWidth, int[,] Grid, char[] NumToLetterCode, string PuzzleNote)
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
			if (PuzzleNote == null)
			{
				output.WriteLine("<h1>Results</h1>");
			}
			else
			{
				output.WriteLine("<h1>" + PuzzleNote + "</h1>");
			}
			output.WriteLine("<table class='result'>");
			for (int i = 0; i < GridHeight; i++)
			{
				output.WriteLine("<tr>");
				for (int j = 0; j < GridWidth; j++)
				{
					if (Grid[i, j] == 0)
					{
						output.WriteLine("<td class = 'blank'></td>");
					}
					else
					{
						output.WriteLine("<td><div class = 'small'>" + Grid[i, j] + "</div><br><div class = 'regular'>" + (NumToLetterCode[Grid[i, j] - 1] + "").ToUpper() + "</div></td>");
					}
				}
				output.WriteLine("</tr>");
			}
			output.WriteLine("</table>");
			output.WriteLine("<h2>Letters:</h2>");
			output.WriteLine("<table class='result'><tr>");
			for (int i = 0; i < 26; i++)
			{
				if (i == 13)
				{
					output.WriteLine("</tr><tr>");
				}
				output.WriteLine("<td><div class = 'small'>" + (i + 1) + "</div><br><div class = 'regular'>" + (NumToLetterCode[i] + "").ToUpper() + "</div></td>");
			}
			output.WriteLine("</tr></table>");
			output.WriteLine("</table>");
			output.WriteLine("</body>");
			output.WriteLine("</html>");
		}

		static void Main()
		{
			ImportGrid();
		}

		static public bool ImportGrid()
        {
			var inputLines = System.IO.File.ReadAllText(@"C:\Users\Dominic\Desktop\cc\ccImport.txt").Split("\r\n", StringSplitOptions.None);
			var gridHeight = int.Parse(inputLines[0].Split('x', StringSplitOptions.None)[0]);
			var gridWidth = int.Parse(inputLines[0].Split('x', StringSplitOptions.None)[1]);
			var givenLetters = "";
			var words = new List<Word>();
			var puzzleNote = "";
			var grid = new int[gridHeight, gridWidth];

			for (int i = 0; i < 26; i++) LettersAvailable[i] = true;

			//Puts ints into grid
			for (int i = 1; i < gridHeight + 3; i++)
			{
				var line = inputLines[i].Trim();

				if (i == gridHeight + 1)
				{
					givenLetters = line;
					break;
				}

				if (i == gridHeight + 2)
				{
					puzzleNote = line;
					break;
				}

				for (int j = 0; j < gridWidth; j++)
				{
					var Num = line.Substring(2 * j, 2);
					grid[i-1,j] = int.Parse(Num);
				}
			}

			//Prints the grid (Yuck)
			for (int i = 0; i < gridHeight; i++)
			{
				for (int j = 0; j < gridWidth; j++)
				{
					if (grid[i,j] == 0)
					{
						System.Console.Write("|  ");
					}
					else
					{
						if (grid[i,j] < 10)
						{
							System.Console.Write("|0" + grid[i,j]);
						}
						else
						{
							System.Console.Write("|" + grid[i,j]);
						}
					}

				}
				System.Console.WriteLine("|");
			}

			int WordCount = 0;

			//Checks each square if they are a sstart of a word
			for (int i = 0; i < gridHeight - 1; i++)
			{
				for (int j = 0; j < gridWidth; j++)
				{
					bool IsWord = true;
					if (i != 0)
					{
						if (grid[i-1, j] != 0)
						{
							IsWord = false;
						}
					}
					if (grid[i+1, j] == 0)
					{
						IsWord = false;
					}
					if (grid[i, j] == 0)
					{
						IsWord = false;
					}

					if (IsWord)
					{
						int WordLen = 0;
						int[] Word = new int[15];
						while (grid[i + WordLen, j] != 0)
						{
							Word[WordLen] = grid[i + WordLen, j];

							WordLen++;
							if (i + WordLen == gridHeight)
							{
								break;
							}
						}

						var CodeWord = "";

						for (int k = 0; k < WordLen; k++)
						{
							if (grid[i + k, j] < 10)
							{
								CodeWord += "0" + grid[i + k, j];
							}
							else
							{
								CodeWord += grid[i + k, j];
							}
						}
						words.Add(new Word(CodeWord));
						WordCount++;
					}
				}
			}

			for (int i = 0; i < gridHeight; i++)
			{
				for (int j = 0; j < gridWidth - 1; j++)
				{
					bool IsWord = true;
					if (j != 0)
					{
						if (grid[i, j - 1] != 0)
						{
							IsWord = false;
						}
					}
					if (grid[i, j + 1] == 0)
					{
						IsWord = false;
					}
					if (grid[i, j] == 0)
					{
						IsWord = false;
					}

					if (IsWord)
					{
						int WordLen = 0;
						int[] Word = new int[15];
						while (grid[i, j + WordLen] != 0)
						{
							Word[WordLen] = grid[i, j + WordLen];

							WordLen++;
							if (j + WordLen == gridWidth)
							{
								break;
							}
						}
						String CodeWord = "";
						for (int k = 0; k < WordLen; k++)
						{
							if (grid[i, j + k] < 10)
							{
								CodeWord = CodeWord + "0" + grid[i, j + k];
							}
							else
							{
								CodeWord += grid[i, j + k];
							}
						}
						words.Add(new Word(CodeWord));
						WordCount++;
					}
				}
			}

			for (int i = 0; i < WordCount; i++)
			{
				System.Console.WriteLine(words[i].OriginalCode);
			}
			System.Console.WriteLine("Word count is " + WordCount);
			var CommonLetters = new int[26];
			for (int i = 0; i < WordCount; i++)
			{
				bool[] WordHasNumbers = words[i].HasNumber;
				for (int j = 0; j < 26; j++)
				{
					if (WordHasNumbers[j])
					{
						CommonLetters[j]++;
					}
				}
			}

			for (int i = 0; i < 26; i++)
			{
				System.Console.WriteLine("The letter " + (i + 1) + " appeared in " + CommonLetters[i] + " out of " + WordCount + " words!");
			}
			System.Console.WriteLine("Letters that appeared in 14 or more words:");
			for (int i = 0; i < 26; i++)
			{
				if (CommonLetters[i] > 13)
					System.Console.Write(i + 1 + " ");
			}
			System.Console.WriteLine("\nLetters that appeared in 3 or less words:");
			for (int i = 0; i < 26; i++)
			{
				if (CommonLetters[i] < 4)
					System.Console.Write(i + 1 + " ");
			}
			System.Console.WriteLine();


			char[] NumToLetterCode = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };

			foreach (var givenLetter in givenLetters.Split(' ', StringSplitOptions.None))
			{
				if (givenLetter.Length != 3)
				{
					System.Console.WriteLine("Your 'given letter' length isn't 3... BROKE");
					return false;
				}
				int Num = int.Parse(givenLetter.Substring(0, 2));
				char Letter = givenLetter.ToCharArray()[2];
				NumToLetterCode[Num - 1] = Letter;
				LettersAvailable[AlphabetLetterToNum(Letter) - 1] = false;
			}

			var LettersAvailableBackup = LettersAvailable;
			var NumToLetterCodeBackup = NumToLetterCode;
			var wordsBackup = words;
			bool Guessing = false;
			int GuessSolutionAttemptNum = 0;
			var GuessingSolutionsList = new List<String>();
			Word GuessingSolutionWord = new Word("01");
			Word GuessingSolutionWordBackup = new Word("02");

			while (true)
			{
				//Say Current Letters
				System.Console.WriteLine("The current letters are:");
				for (int i = 0; i < 26; i++)
				{
					if (LettersAvailable[i])
					{
						System.Console.Write(Alphabet[i]);
						if (i != 25)
						{
							System.Console.Write("  ");
						}
					}
					if (i == 25)
					{
						System.Console.WriteLine();
					}
				}
				//Say Current letter/number code
				System.Console.WriteLine("The current letter/number code is:");
				for (int i = 0; i < 26; i++)
				{
					if (i == 13)
					{
						System.Console.WriteLine();
					}
					if (i < 9)
					{
						System.Console.Write("0");
					}
					System.Console.Write((i + 1) + ": " + Char.ToUpper(NumToLetterCode[i]) + "  ");
					if (i == 25)
					{
						System.Console.WriteLine();
					}
				}

				//Prints out words with how many blanks they have and update each words' letters codings
				for (int i = 0; i < WordCount; i++)
				{
					bool WordChanged = false;
					if (words[i].Found)
					{
						continue;
					}
					for (int j = 0; j < 26; j++)
					{
						String ReplacementLetter = "" + NumToLetterCode[j];
						if (ReplacementLetter.Equals(" ") || words[i].LetterFound[j])
						{
							continue;
						}
						words[i].UpdateLetterDecoding(j + 1, NumToLetterCode[j]);
						WordChanged = true;
					}
					if (WordChanged)
					{
						System.Console.WriteLine(words[i].ToCurrentCode().ToUpper() + " has " + words[i].UniqueBlanks() + " blank/s");
					}
				}

				var wordsWithLowBlanks = new List<Word>();

				for (int i = 0; i < WordCount; i++)
				{
					if (words[i].UniqueBlanks() < 4 && !words[i].Found)
					{
						wordsWithLowBlanks.Add(words[i]);
					}
				}
				if (wordsWithLowBlanks.Count == 0)
				{
					for (int i = 0; i < WordCount; i++)
					{
						if (words[i].UniqueBlanks() == 4 && !words[i].Found)
						{
							wordsWithLowBlanks.Add(words[i]);
						}
					}
				}
				Word LowSolutionsWord = null;
				int LowSolutions = 1000000;
				bool AnyWordFoundSolutionless = false;
				for (int i = 0; i < wordsWithLowBlanks.Count; i++)
				{
					var wordWithLowBlanks = wordsWithLowBlanks.ToArray()[i];

					System.Console.Write("Searching word " + (i + 1) + " out of " + wordsWithLowBlanks.Count);
					int Test = wordWithLowBlanks.FindPossibleSolutions(wordWithLowBlanks.ToSearchableWord(), LettersAvailable);
					System.Console.WriteLine(" - " + wordWithLowBlanks.ToSearchableWord().ToUpper() + " has " + Test + " solutions");
					if (Test < LowSolutions && Test != 0)
					{
						LowSolutions = Test;
						LowSolutionsWord = wordWithLowBlanks;
					}
					if (Test == 0)
					{
						AnyWordFoundSolutionless = true;
						break;
					}
					if (LowSolutions == 1)
					{
						break;
					}
				}

				if (AnyWordFoundSolutionless)
				{
					if (Guessing)//If guess was wrong!
					{
						LettersAvailable = LettersAvailableBackup;
						NumToLetterCode = NumToLetterCodeBackup;
						words = wordsBackup;
						GuessingSolutionWord.Copy(GuessingSolutionWordBackup);
						GuessSolutionAttemptNum++;
						String CorrectSolution = GuessingSolutionsList.ToArray()[GuessSolutionAttemptNum] + "";
						bool[] CharDone = new bool[26];
						for (int i = 0; i < CorrectSolution.Length; i++)
						{
							int CodeNumber = GuessingSolutionWord.Code[i];
							char CodeLetter = CorrectSolution.ToLower().ToCharArray()[i];
							if (!CharDone[AlphabetLetterToNum(CodeLetter) - 1] && NumToLetterCode[CodeNumber - 1] == ' ')
							{
								NumToLetterCode[CodeNumber - 1] = CodeLetter;
								LettersAvailable[AlphabetLetterToNum(CodeLetter) - 1] = false;

								for (int j = 0; j < WordCount; j++)
								{
									if (!words[j].Found)
									{
										words[j].UpdateLetterDecoding(CodeNumber, CodeLetter);
										if (words[j].Found)
										{
											System.Console.WriteLine("Word maybe found: " + words[j].FoundWord);
										}
									}
								}

								GuessingSolutionWord.UpdateLetterDecoding(CodeNumber, CodeLetter);

								if (GuessingSolutionWord.Found)
								{
									System.Console.WriteLine(GuessingSolutionWord.FoundWord + " is being tested as a solution...");
								}
							}
							CharDone[AlphabetLetterToNum(CodeLetter) - 1] = true;
						}
						continue;
					}
					else
					{
						System.Console.WriteLine("Either this puzzle cannot be solved or the import was incorrect!");
						break;
					}
				}

				if (LowSolutionsWord == null) //Puzzle complete, exporting
				{
					System.Console.WriteLine("Congratulations, it is now solved!");
					CreateHtmlResults(gridHeight, gridWidth, grid, NumToLetterCode, puzzleNote);

					using (var output = new System.IO.StreamWriter(@"C:\Users\Dominic\Desktop\cc\Export.txt"))
					{
						if (puzzleNote != null)
						{
							output.WriteLine(puzzleNote);
						}

						output.WriteLine("----------------------------------------------------------------------------");
						for (int i = 0; i < gridHeight; i++)
						{
							for (int j = 0; j < gridWidth; j++)
							{
								if (grid[i, j] == 0)
								{
									output.Write("| ");
								}
								else
								{
									char Letter = (NumToLetterCode[grid[i, j] - 1] + "").ToUpper().ToCharArray()[0];
									output.Write("|" + Letter);
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
							output.Write((i + 1) + ": " + Char.ToUpper(NumToLetterCode[i]) + "  ");
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
					System.Console.WriteLine("The word with the lowest solutions is " + LowSolutionsWord.ToSearchableWord().ToUpper() + " and has " + LowSolutions + " solutions");
				}

				if (LowSolutions == 1)
				{
					String CorrectSolution = LowSolutionsWord.PossibleSolutions.ToArray()[LowSolutionsWord.PossibleSolutions.Count - 1] + "";
					bool[] CharDone = new bool[26];
					for (int i = 0; i < CorrectSolution.Length; i++)
					{
						int CodeNumber = LowSolutionsWord.Code[i];
						char CodeLetter = CorrectSolution.ToUpper().ToCharArray()[i];
						if (!CharDone[AlphabetLetterToNum(CodeLetter) - 1] && NumToLetterCode[CodeNumber - 1] == ' ')
						{
							NumToLetterCode[CodeNumber - 1] = CodeLetter;
							LettersAvailable[AlphabetLetterToNum(CodeLetter) - 1] = false;
							for (int j = 0; j < WordCount; j++)
							{
								if (!words[j].Found)
								{
									words[j].UpdateLetterDecoding(CodeNumber, CodeLetter);
									if (words[j].Found)
									{
										System.Console.WriteLine("Word found: " + words[j].FoundWord);
									}
								}
							}
						}
						CharDone[AlphabetLetterToNum(CodeLetter) - 1] = true;
					}
					continue;
				}
				else
				{
					if (!Guessing)
					{
						Guessing = true;
						GuessingSolutionsList = LowSolutionsWord.PossibleSolutions;
						GuessingSolutionWord.Copy(LowSolutionsWord);
						GuessingSolutionWordBackup.Copy(GuessingSolutionWord);

						LettersAvailableBackup = LettersAvailable;
						NumToLetterCodeBackup = NumToLetterCode;
						wordsBackup = words;

						String CorrectSolution = GuessingSolutionsList.ToArray()[0];
						bool[] CharDone = new bool[26];
						for (int i = 0; i < CorrectSolution.Length; i++)
						{
							int CodeNumber = GuessingSolutionWord.Code[i];
							char CodeLetter = CorrectSolution.ToLower().ToCharArray()[i];
							if (!CharDone[AlphabetLetterToNum(CodeLetter) - 1] && NumToLetterCode[CodeNumber - 1] == ' ')
							{
								NumToLetterCode[CodeNumber - 1] = CodeLetter;
								LettersAvailable[AlphabetLetterToNum(CodeLetter) - 1] = false;

								GuessingSolutionWord.UpdateLetterDecoding(CodeNumber, CodeLetter);
								if (GuessingSolutionWord.Found)
								{
									System.Console.WriteLine(GuessingSolutionWord.FoundWord + " is being tested as a solution...");
								}

								for (int j = 0; j < WordCount; j++)
								{
									if (!words[j].Found)
									{
										words[j].UpdateLetterDecoding(CodeNumber, CodeLetter);
										if (words[j].Found && words[j].Code != GuessingSolutionWord.Code)
										{
											System.Console.WriteLine(words[j].FoundWord + " was maybe found");
										}
									}
								}


							}
							CharDone[AlphabetLetterToNum(CodeLetter) - 1] = true;
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
