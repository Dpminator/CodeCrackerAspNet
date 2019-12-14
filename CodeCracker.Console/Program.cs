using System;
using System.Collections.Generic;
using System.IO;

namespace CodeCracker.Console
{

	class Program
    {
		static bool[] LettersAvailable = new bool[26];

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

		public static void CreateHtmlResults(int gHeight, int gWidth, int[,] gArray, char[] numLetterCode, string title)
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
			output.WriteLine(title == null ? $"<h1>{title}</h1>" : "<h1>Results</h1>");
			output.WriteLine("<table class='result'>");
			for (int i = 0; i < gHeight; i++)
			{
				output.WriteLine("<tr>");
				for (int j = 0; j < gWidth; j++)
				{
					if (gArray[i, j] == 0)
					{
						output.WriteLine("<td class = 'blank'></td>");
					}
					else
					{
						output.WriteLine("<td><div class = 'small'>" + gArray[i, j] + "</div><br><div class = 'regular'>" + (numLetterCode[gArray[i, j] - 1] + "").ToUpper() + "</div></td>");
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
				output.WriteLine("<td><div class = 'small'>" + (i + 1) + "</div><br><div class = 'regular'>" + (numLetterCode[i] + "").ToUpper() + "</div></td>");
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
			var import = new PuzzleImporter(File.ReadAllText(@"C:\Users\Dominic\Desktop\cc\ccImport.txt"));
			var puzzle = new Puzzle(import);






			//var inputLines = File.ReadAllText(@"C:\Users\Dominic\Desktop\cc\ccImport.txt").Split("\r\n");
			//var gridHeight = int.Parse(inputLines[0].Split('x')[0]);
			//var gridWidth = int.Parse(inputLines[0].Split('x')[1]);
			//var givenLetters = "";
			//var words = new List<Word>();
			//var wordCount = 0;
			//var grid = new int[gridHeight, gridWidth];
			for (int i = 0; i < 26; i++) LettersAvailable[i] = true;






			//Puts ints into grid
			/*
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
					//puzzleNote = line;
					break;
				}

				for (int j = 0; j < gridWidth; j++)
				{
					var Num = line.Substring(2 * j, 2);
					grid[i-1,j] = int.Parse(Num);
				}
			}
			*/

			//Prints the grid (Yuck)
			/*
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
			*/
			puzzle.DisplayGrid();

			//Checks each square if they are a start of a word
			/*
			for (int i = 0; i < gridHeight - 1; i++)
			{
				for (int j = 0; j < gridWidth; j++)
				{
					var isWord = true;
					if (i != 0)
					{
						if (grid[i-1, j] != 0)
						{
							isWord = false;
						}
					}
					if (grid[i+1, j] == 0)
					{
						isWord = false;
					}
					if (grid[i, j] == 0)
					{
						isWord = false;
					}

					if (isWord)
					{
						var wordLen = 0;
						var word = new int[15];
						while (grid[i + wordLen, j] != 0)
						{
							word[wordLen] = grid[i + wordLen, j];

							wordLen++;
							if (i + wordLen == gridHeight)
							{
								break;
							}
						}

						var CodeWord = "";

						for (int k = 0; k < wordLen; k++)
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
						wordCount++;
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
						var codeWord = "";
						for (int k = 0; k < WordLen; k++)
						{
							if (grid[i, j + k] < 10)
							{
								codeWord += "0" + grid[i, j + k];
							}
							else
							{
								codeWord += grid[i, j + k];
							}
						}
						words.Add(new Word(codeWord));
						wordCount++;
					}
				}
			}
			*/
			var words = puzzle.GetWordsFromGrid();

			//Prints coded words + count
			for (int i = 0; i < words.Count; i++)
			{
				System.Console.WriteLine(words[i].OriginalCode);
			}
			System.Console.WriteLine("Word count is " + words.Count);

			//Counts common letters 
			var commonLetters = new int[26];
			for (int i = 0; i < words.Count; i++)
			{
				var wordHasNumbers = words[i].HasNumber;
				for (int j = 0; j < 26; j++)
				{
					if (wordHasNumbers[j])
					{
						commonLetters[j]++;
					}
				}
			}
			for (int i = 0; i < 26; i++)
			{
				System.Console.WriteLine("The letter " + (i + 1) + " appeared in " + commonLetters[i] + " out of " + words.Count + " words!");
			}
			System.Console.WriteLine("Letters that appeared in 14 or more words:");
			for (int i = 0; i < 26; i++)
			{
				if (commonLetters[i] > 13)
					System.Console.Write(i + 1 + " ");
			}
			System.Console.WriteLine("\nLetters that appeared in 3 or less words:");
			for (int i = 0; i < 26; i++)
			{
				if (commonLetters[i] < 4)
					System.Console.Write(i + 1 + " ");
			}
			System.Console.WriteLine();


			//var numToLetterCode = new char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };

			//Puts given letters in numToLetterCode
			/*
			foreach (var givenLetter in givenLetters.Split(' '))
			{
				if (givenLetter.Length != 3)
				{
					System.Console.WriteLine("Your 'given letter' length isn't 3... BROKE");
					return false;
				}
				var num = int.Parse(givenLetter.Substring(0, 2));
				var letter = givenLetter.ToCharArray()[2];
				numToLetterCode[num - 1] = letter;
				LettersAvailable[AlphabetLetterToNum(letter) - 1] = false;
			}
			*/
			foreach (var (num, letter) in import.GetGivenLetters())
			{
				puzzle.DecodeNumber(num, letter);
				LettersAvailable[AlphabetLetterToNum(letter) - 1] = false;
			}

			

			var lettersAvailableBackup = LettersAvailable;
			//var numToLetterCodeBackup = numToLetterCode;
			puzzle.BackupDecodedLetters();
			var wordsBackup = words;
			var guessing = false;
			var guessSolutionAttemptNum = 0;
			var guessingSolutionsList = new List<string>();
			Word guessingSolutionWord = new Word("01");
			Word guessingSolutionWordBackup = new Word("02");

			while (true)
			{
				//Say Current Letters
				System.Console.WriteLine("The current letters are:");
				for (int i = 0; i < 26; i++)
				{
					if (LettersAvailable[i])
					{
						System.Console.Write(NumToAlphabetLetter(i+1));
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
				/*
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
					System.Console.Write($"{i+1}: {char.ToUpper(numToLetterCode[i])}  ");
					if (i == 25)
					{
						System.Console.WriteLine();
					}
				}
				*/
				puzzle.DisplayEncodedLetters();

				//Prints out words with how many blanks they have and update each words' letters codings
				for (int i = 0; i < wordCount; i++)
				{
					var wordChanged = false;
					if (words[i].Found)
					{
						continue;
					}
					for (int j = 0; j < 26; j++)
					{
						var replacementLetter = "" + numToLetterCode[j];
						if (replacementLetter.Equals(" ") || words[i].LetterFound[j])
						{
							continue;
						}
						words[i].UpdateLetterDecoding(j + 1, numToLetterCode[j]);
						wordChanged = true;
					}
					if (wordChanged)
					{
						System.Console.WriteLine($"{words[i].ToCurrentCode().ToUpper()} has {words[i].UniqueBlanks()} blank/s");
					}
				}

				var wordsWithLowBlanks = new List<Word>();

				for (int i = 0; i < wordCount; i++)
				{
					if (words[i].UniqueBlanks() < 4 && !words[i].Found)
					{
						wordsWithLowBlanks.Add(words[i]);
					}
				}
				if (wordsWithLowBlanks.Count == 0)
				{
					for (int i = 0; i < wordCount; i++)
					{
						if (words[i].UniqueBlanks() == 4 && !words[i].Found)
						{
							wordsWithLowBlanks.Add(words[i]);
						}
					}
				}
				Word lowSolutionsWord = null;
				var lowSolutions = 1000000;
				var anyWordFoundSolutionless = false;
				for (int i = 0; i < wordsWithLowBlanks.Count; i++)
				{
					var wordWithLowBlanks = wordsWithLowBlanks.ToArray()[i];

					System.Console.Write("Searching word " + (i + 1) + " out of " + wordsWithLowBlanks.Count);
					var test = wordWithLowBlanks.FindPossibleSolutions(LettersAvailable);
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
					
					LettersAvailable = lettersAvailableBackup;
					numToLetterCode = numToLetterCodeBackup;
					words = wordsBackup;
					guessingSolutionWord.Copy(guessingSolutionWordBackup);
					guessSolutionAttemptNum++;
					var correctSolution = guessingSolutionsList.ToArray()[guessSolutionAttemptNum] + "";
					var charDone = new bool[26];
					for (int i = 0; i < correctSolution.Length; i++)
					{
						var codeNumber = guessingSolutionWord.Code[i];
						var codeLetter = correctSolution.ToLower().ToCharArray()[i];
						if (!charDone[AlphabetLetterToNum(codeLetter) - 1] && numToLetterCode[codeNumber - 1] == ' ')
						{
							numToLetterCode[codeNumber - 1] = codeLetter;
							LettersAvailable[AlphabetLetterToNum(codeLetter) - 1] = false;

							for (int j = 0; j < wordCount; j++)
							{
								if (!words[j].Found)
								{
									words[j].UpdateLetterDecoding(codeNumber, codeLetter);
									if (words[j].Found)
									{
										System.Console.WriteLine("Word maybe found: " + words[j].FoundWord);
									}
								}
							}

							guessingSolutionWord.UpdateLetterDecoding(codeNumber, codeLetter);

							if (guessingSolutionWord.Found)
							{
								System.Console.WriteLine(guessingSolutionWord.FoundWord + " is being tested as a solution...");
							}
						}
						charDone[AlphabetLetterToNum(codeLetter) - 1] = true;
					}
					continue;
					
				}

				if (lowSolutionsWord == null) //Puzzle complete, exporting
				{
					System.Console.WriteLine("Congratulations, it is now solved!");
					CreateHtmlResults(gridHeight, gridWidth, grid, numToLetterCode, puzzle.GetTitle());

					using (var output = new System.IO.StreamWriter(@"C:\Users\Dominic\Desktop\cc\Export.txt"))
					{
						output.WriteLine(puzzle.GetTitle());

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
									char Letter = (numToLetterCode[grid[i, j] - 1] + "").ToUpper().ToCharArray()[0];
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
							output.Write((i + 1) + ": " + Char.ToUpper(numToLetterCode[i]) + "  ");
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
					var correctSolution = lowSolutionsWord.PossibleSolutions.ToArray()[lowSolutionsWord.PossibleSolutions.Count - 1] + "";
					var charDone = new bool[26];
					for (int i = 0; i < correctSolution.Length; i++)
					{
						var codeNumber = lowSolutionsWord.Code[i];
						var codeLetter = correctSolution.ToUpper().ToCharArray()[i];
						if (!charDone[AlphabetLetterToNum(codeLetter) - 1] && numToLetterCode[codeNumber - 1] == ' ')
						{
							numToLetterCode[codeNumber - 1] = codeLetter;
							LettersAvailable[AlphabetLetterToNum(codeLetter) - 1] = false;
							for (int j = 0; j < wordCount; j++)
							{
								if (!words[j].Found)
								{
									words[j].UpdateLetterDecoding(codeNumber, codeLetter);
									if (words[j].Found)
									{
										System.Console.WriteLine("Word found: " + words[j].FoundWord);
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
						guessingSolutionsList = lowSolutionsWord.PossibleSolutions;
						guessingSolutionWord.Copy(lowSolutionsWord);
						guessingSolutionWordBackup.Copy(guessingSolutionWord);

						lettersAvailableBackup = LettersAvailable;
						numToLetterCodeBackup = numToLetterCode;
						wordsBackup = words;

						var correctSolution = guessingSolutionsList.ToArray()[0];
						var charDone = new bool[26];
						for (int i = 0; i < correctSolution.Length; i++)
						{
							var codeNumber = guessingSolutionWord.Code[i];
							var codeLetter = correctSolution.ToLower().ToCharArray()[i];
							if (!charDone[AlphabetLetterToNum(codeLetter) - 1] && numToLetterCode[codeNumber - 1] == ' ')
							{
								numToLetterCode[codeNumber - 1] = codeLetter;
								LettersAvailable[AlphabetLetterToNum(codeLetter) - 1] = false;

								guessingSolutionWord.UpdateLetterDecoding(codeNumber, codeLetter);
								if (guessingSolutionWord.Found)
								{
									System.Console.WriteLine(guessingSolutionWord.FoundWord + " is being tested as a solution...");
								}

								for (int j = 0; j < wordCount; j++)
								{
									if (!words[j].Found)
									{
										words[j].UpdateLetterDecoding(codeNumber, codeLetter);
										if (words[j].Found && words[j].Code != guessingSolutionWord.Code)
										{
											System.Console.WriteLine(words[j].FoundWord + " was maybe found");
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
