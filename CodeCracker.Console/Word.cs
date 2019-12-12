using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeCracker.Console
{
	public class Word
	{
		public string OriginalCode;                                  //The original code inputed (e.g: "0224180415")
		public int Letters;                                          //The number of letters the word has
		public int[] Code;                                           //An array (size = letters) that has each int of the code
		public char[] DecodedLetters;                                //An Array (size = letters) that has the decoded letters of the code (or a ? where it isn't solved)
		public bool Found;                                        //A boolean to see if the word is complete or not
		public bool Dummy = false;                                //A boolean to denote whether this object is a dummy
		public bool[] HasNumber;                                  //A boolean array (size = 26) that says whether the word has the NUMBER (not the letter of the alphabet)
		public bool[] LetterFound = new bool[26];              //A boolean array (size = 26) to say whether that letter is found (
		public List<string> PossibleSolutions = new List<string>();  //An ArrayList of all the possible solutions
		public string FoundWord;                                               //The string of the word after it has been found

		public Word(string CodedWord)
		{
			this.OriginalCode = CodedWord;
			this.Letters = LettersInWord(OriginalCode);
			this.Code = SeparateNumbers(OriginalCode, Letters);
			DecodedLetters = new char[Letters];
			for (int i = 0; i < Letters; i++)
				DecodedLetters[i] = '?';
			this.Found = false;
			HasNumber = CheckNumbers(Code);
		}

		Word()
		{
			this.Dummy = true;
		}

		public void Copy(Word OtherWord)
		{
			this.OriginalCode = OtherWord.OriginalCode + "";
			this.Letters = OtherWord.Letters;
			this.Code = OtherWord.Code;
			this.DecodedLetters = OtherWord.DecodedLetters;
			this.Found = OtherWord.Found;
			this.HasNumber = OtherWord.HasNumber;
			this.LetterFound = OtherWord.LetterFound;
			this.PossibleSolutions = (List<string>)OtherWord.PossibleSolutions;
			this.FoundWord = OtherWord.FoundWord + "";
		}

		public int UniqueBlanks()
		{
			int Blanks = 0;
			for (int i = 0; i < 26; i++)
			{
				if (HasNumber[i] && !LetterFound[i])
				{
					Blanks++;
				}
			}
			return Blanks;
		}

		public static bool CheckWord(string TestingWord)
		{
			int WordLength = TestingWord.Length;
        
			if (WordLength > 10)
			{
        		WordLength = 10;
			}

			var input = System.IO.File.ReadAllText($@"C:\Users\Dominic\Desktop\cc\Words{WordLength}.txt");
			var allWords = input.Split("\r\n", StringSplitOptions.None);

			foreach (var word in allWords)
			{
				if (word.Trim().ToLower().Equals(TestingWord.ToLower()))
				{
					return true;
				}
			}

			return false;
		}
	
		public void UpdateLetterDecoding(int LetterCode, char RealLetter)
		{
			for (int i = 0; i < Letters; i++)
			{
				if (Code[i] == LetterCode)
				{
					DecodedLetters[i] = RealLetter;
					LetterFound[LetterCode - 1] = true;
				}
			}
			UpdateIfFound();
		}


		private void UpdateIfFound()
		{
			bool HasUnkownLetter = false;
			string Word = "";
			for (int i = 0; i < Letters; i++)
			{
				if (DecodedLetters[i] == '?')
				{
					HasUnkownLetter = true;
				}
				else
				{
					Word += DecodedLetters[i];
				}
			}
			if (!HasUnkownLetter)
			{
				FoundWord = Word + "";
				Found = true;
			}
		}

		public string ToCurrentCode()
		{
			var CurrentCode = "";
			for (int i = 0; i < Letters; i++)
			{
				if (DecodedLetters[i] != '?')
				{
					CurrentCode += DecodedLetters[i];
				}
				else
				{
					if (Code[i] < 10)
					{
						CurrentCode = CurrentCode + "0" + Code[i];
					}
					else
					{
						CurrentCode += Code[i];
					}
				}
			}
			return CurrentCode;
		}

		public string ToSearchableWord()
		{
			bool[] LettersDone = new bool[Letters];
			char[] FixedLetters = new char[Letters];
			string FixedWord = "";
			int CurrentBlank = 1;
			for (int i = 0; i < Letters; i++)
			{
				if (Char.IsLetter(DecodedLetters[i]))
				{
					FixedLetters[i] = DecodedLetters[i];
				}
				else if (!LettersDone[i])
				{
					FixedLetters[i] = ("" + CurrentBlank).ToCharArray()[0];
					LettersDone[i] = true;
					for (int j = 0; j < Letters; j++)
					{
						if (Code[j] == Code[i] && j != i)
						{
							FixedLetters[j] = ("" + CurrentBlank).ToCharArray()[0];
							LettersDone[j] = true;
						}
					}
					CurrentBlank++;
				}
			}
			for (int i = 0; i < Letters; i++)
			{
				FixedWord += FixedLetters[i];
			}

			return FixedWord.ToLower();
		}

		public static int FindSolutions(string CodedWord, bool[] LettersAvailable)
		{
		Word Dummy = new Word();
			return Dummy.FindPossibleSolutions(CodedWord, LettersAvailable);
		}
	
		public int FindPossibleSolutions(string CodedWord, bool[] LettersAvailable)
		{
			int WordLength = CodedWord.Length;
			if (WordLength > 10) WordLength = 10;
			var allWords = System.IO.File.ReadAllText($@"C:\Users\Dominic\Desktop\cc\Words{WordLength}.txt").Split("\r\n", StringSplitOptions.None);
			int Solutions = 0;
			PossibleSolutions.RemoveRange(0, PossibleSolutions.Count);

			foreach (var DictionaryWord in allWords) //Does the CodedWord match up with the dictionary word?
			{
				if (DictionaryWord.Length == 0) break;
				char[] BlankChar = { ' ', ' ', ' ', ' ' };
				bool Match = true;
				for (int i = 0; i<WordLength; i++)
				{
					if (Char.IsLetter(CodedWord.ToCharArray()[i]))
					{
						if (CodedWord.ToCharArray()[i] != DictionaryWord.ToCharArray()[i])
						{
							Match = false;
							break;
						}
					}else
					{
						int BlankNumber = int.Parse("" + CodedWord.ToCharArray()[i]) - 1;
						if (BlankChar[BlankNumber] == ' ')
						{
							if (!LettersAvailable[Program.AlphabetLetterToNum(DictionaryWord.ToCharArray()[i]) - 1])
							{
								Match = false;
								break;
							}
							BlankChar[BlankNumber] = DictionaryWord.ToCharArray()[i];
						}else
						{
							if (BlankChar[BlankNumber] != DictionaryWord.ToCharArray()[i] || !LettersAvailable[Program.AlphabetLetterToNum(DictionaryWord.ToCharArray()[i]) - 1])
							{
								Match = false;
								break;
							}
						}
					}
				}
				if (Match)
				{
					Solutions++;
					if (!Dummy)
					{
						PossibleSolutions.Add(DictionaryWord);
					}else
					{
						if (!DictionaryWord.Equals(CodedWord))
						{
							System.Console.WriteLine(DictionaryWord);
						}
					}
				}
			}

			return Solutions;
		}
	
		//Private	
		private int LettersInWord(string Word)
		{
			if (Word.Length % 2 == 0)
			{
				int LettersInWord = Word.Length / 2;
				return LettersInWord;
			}
			else
			{
				return -1;
			}
		}

		private int[] SeparateNumbers(string Word, int Letters)
		{
			int[] Code = new int[Letters];
			for (int i = 0, j = 0; i < Letters; i++, j += 2)
			{
				string Numberstring = "" + Word.ToCharArray()[j] + Word.ToCharArray()[j + 1];
				int NumberInt = int.Parse(Numberstring);
				Code[i] = NumberInt;
			}
			return Code;
		}

		private bool[] CheckNumbers(int[] Code)
		{
			bool[] HasNumbers = new bool[26];
			for (int i = 0; i < Code.Length; i++)
			{
				HasNumbers[(Code[i] - 1)] = true;
			}
			return HasNumbers;
		}
	}
}
