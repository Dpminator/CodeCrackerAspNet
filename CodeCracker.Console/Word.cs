using System;
using System.Collections.Generic;
using System.IO;

namespace CodeCracker.Console
{
	public class Word
	{
		private string OriginalCode;                                 //The original code inputed (e.g: "0224180415")
		public int[] Code;                                           //An array (size = letters) that has each int of the code
		public char[] DecodedLetters;                                //An Array (size = letters) that has the decoded letters of the code (or a ? where it isn't solved)
		private HashSet<int> HasNumber;  //Get rid of this once Code isn't an array       //A Hashset of the coded numbers in the word
		private Dictionary<int, bool> LetterFound;                   //A dictionary that links coded numbers to a bool of if it is found
		private List<string> PossibleSolutions;                      //An List of all the possible solutions
		private string SolvedWord;                                   //The string of the word after it has been found

		public Word(string codedWord)
		{
			OriginalCode = codedWord;
			PossibleSolutions = new List<string>();
			LetterFound = new Dictionary<int, bool>();
			SolvedWord = "";

			var letters = LettersInWord();
			Code = SeparateNumbers(letters);
			
			DecodedLetters = new char[letters];
			for (int i = 0; i < letters; i++)
				DecodedLetters[i] = '?';

			HasNumber = new HashSet<int>();
			for (int i = 0; i < Code.Length; i++)
				HasNumber.Add(Code[i]);
			
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

		public void Copy(Word OtherWord)
		{
			OriginalCode = OtherWord.OriginalCode;
			Code = OtherWord.Code;
			DecodedLetters = OtherWord.DecodedLetters;
			HasNumber = OtherWord.HasNumber;
			LetterFound = OtherWord.LetterFound;
			PossibleSolutions = OtherWord.PossibleSolutions;
			SolvedWord = OtherWord.SolvedWord;
		}

		public int UniqueBlanks()
		{
			var blanks = 0;
			for (int i = 1; i <= 26; i++)
			{
				if (HasNumber.Contains(i) && !LetterFound.GetValueOrDefault(i))
				{
					blanks++;
				}
			}
			return blanks;
		}

		public static bool DoesWordExist(string testingWord)
		{
			//var input = File.ReadAllText($@"C:\Users\Dominic\Desktop\cc\Words{wordLength}.txt");
			//var allWords = input.Split("\r\n", StringSplitOptions.None);

			foreach (var word in AllWords.GetInstance().GetWordList(testingWord.Length))
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
			for (int i = 0; i < Code.Length; i++)
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
			for (int i = 0; i < DecodedLetters.Length; i++)
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
			for (int i = 0; i < DecodedLetters.Length; i++)
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
			var letters = Code.Length;
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
			var words = new Word(codedWord).FindPossibleSolutions(lettersAvailable);

			foreach (var word in words)
			{
				System.Console.WriteLine(word);
			}

			return words.Count;
		}


		public List<string> FindPossibleSolutions(List<char> lettersAvailable)
		{
			var codedWord = ToSearchableWord();
			//var wordLength = codedWord.Length;
			//if (wordLength > 10) wordLength = 10;
			//var allWords = File.ReadAllText($@"C:\Users\Dominic\Desktop\cc\Words{wordLength}.txt").Split("\r\n");
			//var solutions = 0;
			PossibleSolutions.RemoveRange(0, PossibleSolutions.Count);

			foreach (var dictionaryWord in AllWords.GetInstance().GetWordList(codedWord.Length)) //Does the CodedWord match up with the dictionary word?
			{
				//if (dictionaryWord.Length == 0) break;
				var blankChar = new char[]{ ' ', ' ', ' ', ' ' };
				var match = true;
				for (int i = 0; i < codedWord.Length; i++)
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
					//solutions++;
					PossibleSolutions.Add(dictionaryWord);
				}
			}

			return PossibleSolutions;
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

		private int[] SeparateNumbers(int letters)
		{
			var code = new int[letters];
			var originalCodeArray = OriginalCode.ToCharArray();
			for (int i = 0, j = 0; i < letters; i++, j += 2)
			{
				code[i] = int.Parse($"{originalCodeArray[j]}{originalCodeArray[j + 1]}");
			}
			return code;
		}
	}
}
