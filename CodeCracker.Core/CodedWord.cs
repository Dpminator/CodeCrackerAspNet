﻿using System;
using System.Collections.Generic;

namespace CodeCracker.Core
{
	public class CodedWord
	{
		private static readonly AllWords allWords = AllWords.GetInstance();

		private readonly string OriginalCode;                                 //The original code inputed (e.g: "0224180415")
		private readonly Dictionary<int, (int Number, char Letter)> Code;     //A dicitonary which matches word position to its original code number and letter (Defaulted to '?')

		//TOD: Remove this as a field
		private readonly List<string> PossibleSolutions;                      //A List of all the possible solutions


		private string SolvedWord;                                   //The string of the word after it has been found

		public CodedWord(string codedWord)
		{
			OriginalCode = codedWord;
			Code = new Dictionary<int, (int, char)>();
			PossibleSolutions = new List<string>();
			SolvedWord = "";

			var codeNumber = "";
			foreach ((var num, var index) in OriginalCode.ToListWithIndex())
			{
				codeNumber += num;

				if (index % 2 == 1)
				{
					Code.Add((index-1)/2, (int.Parse(codeNumber.Substring(index-1)), '?'));
				}
			}
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
			foreach (var letterPosition in Code)
			{
				if (letterPosition.Value.Number == codedNumber)
					return letterPosition.Value.Letter != '?';
			}
			return false;
		}

		public IEnumerator<string> GetPossibleSolutions()
		{
			var enumerator = PossibleSolutions.GetEnumerator();
			enumerator.MoveNext();
			return enumerator;
		}

		public int GetEncodedNumber(int position)
		{
			return Code[position].Number;
		}

		public static bool CompareCodes(CodedWord w1, CodedWord w2)
		{
			return w1.OriginalCode == w2.OriginalCode;
		}

		public int UniqueBlanks()
		{
			var blanks = 0;
			for (int i = 1; i <= 26; i++)
			{
				if (CodeContainsNumber(i) && !IsNumberDecoded(i))
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
				if (Code[i].Number == letterCode)
				{
					Code[i] = (letterCode, realLetter);
				}
			}
			UpdateIfFound();
		}

		public bool CodeContainsNumber(int num)
		{
			foreach (var letterPosition in Code)
			{
				if (letterPosition.Value.Number == num)
					return true;
			}
			return false;
		}

		private void UpdateIfFound()
		{
			var word = "";
			foreach ((var index, (var num, var letter)) in Code)
			{
				if (letter.Equals('?')) return;
				word += letter;
			}
			SolvedWord = word;
		}

		public string ToCurrentCode()
		{
			var currentCode = "";

			foreach ((var index, (var num, var letter)) in Code)
				currentCode += letter.Equals('?') ? $"{(num < 10 ? "0" : "")}{num}" : $"{letter.ToString()}";

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
				if (char.IsLetter(Code[i].Letter))
				{
					fixedLetters[i] = Code[i].Letter;
				}
				else if (!lettersDone[i])
				{
					fixedLetters[i] = ("" + currentBlank).ToCharArray()[0];
					lettersDone[i] = true;
					for (int j = 0; j < letters; j++)
					{
						if (Code[j].Number == Code[i].Number && j != i)
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

			return fixedWord.ToUpper();
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
			var codedWord = ToSearchableWord().ToLower();
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
						match = true;
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
	}
}
