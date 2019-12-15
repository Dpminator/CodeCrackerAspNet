using System;
using System.Collections.Generic;

namespace CodeCracker.Console
{
	public class CodedWord
	{
		private static readonly AllWords allWords = AllWords.GetInstance();

		private string OriginalCode;                                 //The original code inputed (e.g: "0224180415")
		private Dictionary<int, int> Code;                           //A dicitonary which matches word position to its original code number
		private char[] DecodedLetters;                               //An Array (size = letters) that has the decoded letters of the code (or a ? where it isn't solved)
		private Dictionary<int, bool> LetterFound;                   //A dictionary that links coded numbers to a bool of if it is found
		private List<string> PossibleSolutions;                      //An List of all the possible solutions
		private string SolvedWord;                                   //The string of the word after it has been found

		public CodedWord(string codedWord)
		{
			OriginalCode = codedWord;
			PossibleSolutions = new List<string>();
			LetterFound = new Dictionary<int, bool>();
			SolvedWord = "";
			Code = SeparateCodedNumbers();

			var letters = WordLength();
			DecodedLetters = new char[letters];
			for (int i = 0; i < letters; i++)
				DecodedLetters[i] = '?';
		}

		public string GetOriginalCode()
		{ 
			return OriginalCode;
		}

		public bool IsSolved()
		{
			return SolvedWord != "";
		}

		public string GetSolvedWord()
		{
			return SolvedWord;
		}

		public bool IsNumberDecoded(int codedNumber)
		{
			return LetterFound.GetValueOrDefault(codedNumber);
		}

		public IEnumerator<string> GetPossibleSolutions()
		{
			var enumerator = PossibleSolutions.GetEnumerator();
			enumerator.MoveNext();
			return enumerator;
		}

		public int GetEncodedNumber(int position)
		{
			return Code[position];
		}

		public static bool CompareCodes(CodedWord w1, CodedWord w2)
		{
			return w1.OriginalCode == w2.OriginalCode;
		}

		public void Copy(CodedWord OtherWord)
		{
			OriginalCode = OtherWord.OriginalCode;
			Code = OtherWord.Code;
			DecodedLetters = OtherWord.DecodedLetters;
			LetterFound = OtherWord.LetterFound;
			PossibleSolutions = OtherWord.PossibleSolutions;
			SolvedWord = OtherWord.SolvedWord;
		}

		public int UniqueBlanks()
		{
			var blanks = 0;
			for (int i = 1; i <= 26; i++)
			{
				if (Code.ContainsValue(i) && !LetterFound.GetValueOrDefault(i))
				{
					blanks++;
				}
			}
			return blanks;
		}

		public static bool DoesWordExist(string testingWord)
		{
			foreach (var word in allWords.GetWordList(testingWord.Length))
			{
				if (word.Equals(testingWord.ToLower()))
				{
					return true;
				}
			}

			return false;
		}
	
		public void UpdateLetterDecoding(int letterCode, char realLetter)
		{
			for (int i = 0; i < WordLength(); i++)
			{
				if (Code[i] == letterCode)
				{
					DecodedLetters[i] = realLetter;
					LetterFound[letterCode] = true;
				}
			}
			UpdateIfFound();
		}


		private void UpdateIfFound()
		{
			var hasUnkownLetter = false;
			var word = "";
			for (int i = 0; i < WordLength(); i++)
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
				SolvedWord = word;
			}
		}

		public string ToCurrentCode()
		{
			var currentCode = "";
			for (int i = 0; i < WordLength(); i++)
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
			var letters = WordLength();
			var lettersDone = new bool[letters];
			var fixedLetters = new char[letters];
			var fixedWord = "";
			var currentBlank = 1;

			for (int i = 0; i < letters; i++)
			{
				if (char.IsLetter(DecodedLetters[i]))
				{
					fixedLetters[i] = DecodedLetters[i];
				}
				else if (!lettersDone[i])
				{
					fixedLetters[i] = ("" + currentBlank).ToCharArray()[0];
					lettersDone[i] = true;
					for (int j = 0; j < letters; j++)
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
			for (int i = 0; i < letters; i++)
			{
				fixedWord += fixedLetters[i];
			}

			return fixedWord.ToLower();
		}

		public static int FindSolutions(string codedWord, List<char> lettersAvailable)
		{
			var words = new CodedWord(codedWord).FindPossibleSolutions(lettersAvailable);

			foreach (var word in words)
			{
				System.Console.WriteLine(word);
			}

			return words.Count;
		}


		public List<string> FindPossibleSolutions(List<char> lettersAvailable)
		{
			var letters = WordLength();
			var codedWord = ToSearchableWord();
			PossibleSolutions.RemoveRange(0, PossibleSolutions.Count);

			foreach (var dictionaryWord in allWords.GetWordList(letters)) //Does the CodedWord match up with the dictionary word?
			{
				var blankChar = new char[]{ ' ', ' ', ' ', ' ' };
				var match = true;
				for (int i = 0; i < letters; i++)
				{
					if (char.IsLetter(codedWord[i]))
					{
						if (codedWord[i] != dictionaryWord[i])
						{
							match = false;
							break;
						}
					}else
					{
						var blankNumber = int.Parse("" + codedWord[i]) - 1;
						if (blankChar[blankNumber] == ' ')
						{
							if (!lettersAvailable.Contains(char.ToUpper(dictionaryWord[i])))
							{
								match = false;
								break;
							}
							blankChar[blankNumber] = dictionaryWord[i];
						}else
						{
							if (blankChar[blankNumber] != dictionaryWord[i] || !lettersAvailable.Contains(char.ToUpper(dictionaryWord[i])))
							{
								match = false;
								break;
							}
						}
					}
				}

				if (match)
				{
					PossibleSolutions.Add(dictionaryWord);
				}
			}

			return PossibleSolutions;
		}
		
		private int WordLength()
		{
			if (OriginalCode.Length % 2 == 0)
				return OriginalCode.Length / 2;
			throw new InvalidOperationException();
			
		}

		private Dictionary<int, int> SeparateCodedNumbers()
		{
			var code = new Dictionary<int, int>();
			var originalCodeArray = OriginalCode.ToCharArray();
			for (int i = 0, j = 0; i < WordLength(); i++, j += 2)
			{
				code.Add(i, int.Parse($"{originalCodeArray[j]}{originalCodeArray[j + 1]}"));
			}
			return code;
		}
	}
}
