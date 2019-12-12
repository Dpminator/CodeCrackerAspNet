using System;
using System.Collections.Generic;
using System.IO;

namespace CodeCracker.Console
{
	public class Word
	{
		public string OriginalCode;                                  //The original code inputed (e.g: "0224180415")
		public int Letters;                                          //The number of letters the word has
		public int[] Code;                                           //An array (size = letters) that has each int of the code
		public char[] DecodedLetters;                                //An Array (size = letters) that has the decoded letters of the code (or a ? where it isn't solved)
		public bool Found;                                           //A boolean to see if the word is complete or not
		public bool Dummy = false;                                   //A boolean to denote whether this object is a dummy
		public bool[] HasNumber;                                     //A boolean array (size = 26) that says whether the word has the NUMBER (not the letter of the alphabet)
		public bool[] LetterFound = new bool[26];                    //A boolean array (size = 26) to say whether that letter is found
		public List<string> PossibleSolutions = new List<string>();  //An ArrayList of all the possible solutions
		public string FoundWord;                                     //The string of the word after it has been found

		public Word(string codedWord, bool dummy = false)
		{
			OriginalCode = codedWord;
			Dummy = dummy;
			if (!Dummy)
			{
				Letters = LettersInWord();
				Code = SeparateNumbers();
				DecodedLetters = new char[Letters];
				for (int i = 0; i < Letters; i++)
					DecodedLetters[i] = '?';
				Found = false;
				HasNumber = CheckNumbers();
			}
		}

		public void Copy(Word OtherWord)
		{
			OriginalCode = OtherWord.OriginalCode;
			Letters = OtherWord.Letters;
			Code = OtherWord.Code;
			DecodedLetters = OtherWord.DecodedLetters;
			Found = OtherWord.Found;
			HasNumber = OtherWord.HasNumber;
			LetterFound = OtherWord.LetterFound;
			PossibleSolutions = OtherWord.PossibleSolutions;
			FoundWord = OtherWord.FoundWord;
		}

		public int UniqueBlanks()
		{
			var blanks = 0;
			for (int i = 0; i < 26; i++)
			{
				if (HasNumber[i] && !LetterFound[i])
				{
					blanks++;
				}
			}
			return blanks;
		}

		public static bool CheckWord(string testingWord)
		{
			var wordLength = testingWord.Length;
        
			if (wordLength > 10)
			{
        		wordLength = 10;
			}

			var input = File.ReadAllText($@"C:\Users\Dominic\Desktop\cc\Words{wordLength}.txt");
			var allWords = input.Split("\r\n", StringSplitOptions.None);

			foreach (var word in allWords)
			{
				if (word.Trim().ToLower().Equals(testingWord.ToLower()))
				{
					return true;
				}
			}

			return false;
		}
	
		public void UpdateLetterDecoding(int letterCode, char realLetter)
		{
			for (int i = 0; i < Letters; i++)
			{
				if (Code[i] == letterCode)
				{
					DecodedLetters[i] = realLetter;
					LetterFound[letterCode - 1] = true;
				}
			}
			UpdateIfFound();
		}


		private void UpdateIfFound()
		{
			var hasUnkownLetter = false;
			var word = "";
			for (int i = 0; i < Letters; i++)
			{
				if (DecodedLetters[i] == '?')
				{
					hasUnkownLetter = true;
				}
				else
				{
					word += DecodedLetters[i];
				}
			}
			if (!hasUnkownLetter)
			{
				FoundWord = word;
				Found = true;
			}
		}

		public string ToCurrentCode()
		{
			var currentCode = "";
			for (int i = 0; i < Letters; i++)
			{
				if (DecodedLetters[i] != '?')
				{
					currentCode += DecodedLetters[i];
				}
				else
				{
					if (Code[i] < 10)
					{
						currentCode += "0" + Code[i];
					}
					else
					{
						currentCode += Code[i];
					}
				}
			}
			return currentCode;
		}

		public string ToSearchableWord()
		{
			var lettersDone = new bool[Letters];
			var fixedLetters = new char[Letters];
			var fixedWord = "";
			var currentBlank = 1;

			for (int i = 0; i < Letters; i++)
			{
				if (char.IsLetter(DecodedLetters[i]))
				{
					fixedLetters[i] = DecodedLetters[i];
				}
				else if (!lettersDone[i])
				{
					fixedLetters[i] = ("" + currentBlank).ToCharArray()[0];
					lettersDone[i] = true;
					for (int j = 0; j < Letters; j++)
					{
						if (Code[j] == Code[i] && j != i)
						{
							fixedLetters[j] = ("" + currentBlank).ToCharArray()[0];
							lettersDone[j] = true;
						}
					}
					currentBlank++;
				}
			}
			for (int i = 0; i < Letters; i++)
			{
				fixedWord += fixedLetters[i];
			}

			return fixedWord.ToLower();
		}

		public static int FindSolutions(string codedWord, bool[] lettersAvailable)
		{
			return new Word(codedWord, true).FindPossibleSolutions(lettersAvailable);
		}

		public int FindPossibleSolutions(bool[] lettersAvailable)
		{
			var codedWord = ToSearchableWord();
			var wordLength = codedWord.Length;
			if (wordLength > 10) wordLength = 10;
			var allWords = File.ReadAllText($@"C:\Users\Dominic\Desktop\cc\Words{wordLength}.txt").Split("\r\n");
			var solutions = 0;
			PossibleSolutions.RemoveRange(0, PossibleSolutions.Count);

			foreach (var DictionaryWord in allWords) //Does the CodedWord match up with the dictionary word?
			{
				if (DictionaryWord.Length == 0) break;
				var blankChar = new char[]{ ' ', ' ', ' ', ' ' };
				var match = true;
				for (int i = 0; i < wordLength; i++)
				{
					if (char.IsLetter(codedWord.ToCharArray()[i]))
					{
						if (codedWord.ToCharArray()[i] != DictionaryWord.ToCharArray()[i])
						{
							match = false;
							break;
						}
					}else
					{
						var blankNumber = int.Parse("" + codedWord.ToCharArray()[i]) - 1;
						if (blankChar[blankNumber] == ' ')
						{
							if (!lettersAvailable[Program.AlphabetLetterToNum(DictionaryWord.ToCharArray()[i]) - 1])
							{
								match = false;
								break;
							}
							blankChar[blankNumber] = DictionaryWord.ToCharArray()[i];
						}else
						{
							if (blankChar[blankNumber] != DictionaryWord.ToCharArray()[i] || !lettersAvailable[Program.AlphabetLetterToNum(DictionaryWord.ToCharArray()[i]) - 1])
							{
								match = false;
								break;
							}
						}
					}
				}
				if (match)
				{
					solutions++;
					if (!Dummy)
					{
						PossibleSolutions.Add(DictionaryWord);
					}else
					{
						if (!DictionaryWord.Equals(codedWord))
						{
							System.Console.WriteLine(DictionaryWord);
						}
					}
				}
			}

			return solutions;
		}
		
		private int LettersInWord()
		{
			if (OriginalCode.Length % 2 == 0)
			{
				var lettersInWord = OriginalCode.Length / 2;
				return lettersInWord;
			}
			else 
				throw new InvalidOperationException();
			
		}

		private int[] SeparateNumbers()
		{
			var code = new int[Letters];
			var originalCodeArray = OriginalCode.ToCharArray();
			for (int i = 0, j = 0; i < Letters; i++, j += 2)
			{
				code[i] = int.Parse($"{originalCodeArray[j]}{originalCodeArray[j + 1]}");
			}
			return code;
		}

		private bool[] CheckNumbers()
		{
			var hasNumbers = new bool[26];
			for (int i = 0; i < Code.Length; i++)
			{
				hasNumbers[Code[i] - 1] = true;
			}
			return hasNumbers;
		}
	}
}
