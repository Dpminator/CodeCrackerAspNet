using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCracker.Console
{

    public class Puzzle
    {
        private readonly string Title;
        private readonly (int Height, int Width) GridDimensions;
        private readonly DuoKeyDictionary<int, int, int> Grid;
        private Dictionary<int, char> NumToLetter;
        private Dictionary<int, char> NumToLetter2;

        public Puzzle(PuzzleImporter import)
        {
            Title = import.GetTitle();
            GridDimensions = (import.GetGridHeight(), import.GetGridWidth()); 
            Grid = CreateGrid(import.GetCodedLines());
            NumToLetter = new Dictionary<int, char>();

            foreach (var (num, letter) in import.GetGivenLetters()) 
                DecodeNumber(num, letter);
        }

        public bool IsSolved()
        {
            return NumToLetter.Count == 26;
        }

        public string GetTitle()
        {
            return Title;
        }

        public int GetGridHeight()
        {
            return GridDimensions.Height;
        }

        public int GetGridWidth()
        {
            return GridDimensions.Width;
        }

        public void DecodeNumber(int num, char letter)
        {
            letter = char.ToUpper(letter);

            if (num < 1 || num > 26 || !char.IsLetter(letter))
                throw new InvalidOperationException("Either number is out of bounds or the character is not a letter");

            if (IsNumberDecoded(num) && GetEncodedLetter(num)!=letter)
                throw new InvalidOperationException($"{num} already exist with the value of {GetEncodedLetter(num)}");
            
            NumToLetter[num] = letter;
        }

        public bool IsNumberDecoded(int num)
        {
            return NumToLetter.ContainsKey(num);
        }

        public bool IsLetterDecoded(char letter)
        {
            return NumToLetter.ContainsValue(letter);
        }

        public char GetEncodedLetter(int num, char defaultChar = '?')
        {
           return IsNumberDecoded(num) ? NumToLetter.GetValueOrDefault(num) : defaultChar;
        }

        public char GetEncodedLetter(int i, int j, char defaultChar = '?')
        {
            return GetEncodedLetter(Grid.Get(i, j), defaultChar);
        }

        public string GetGridNumberAsString(int i, int j, bool trailingZero = false)
        {
            var num = Grid.Get(i, j);
            return (num < 10 && trailingZero) ? $"0{num}": $"{num}";
        }

        public void DisplayGrid(bool showLetters = false)
        {
            for (int i = 0; i < GridDimensions.Height; i++)
            {
                for (int j = 0; j < GridDimensions.Width; j++)
                {
                    System.Console.Write(
                        !IsGridSquareBlank(i, j)
                        ? (showLetters ? $"| {GetEncodedLetter(Grid.Get(i, j))} " : $"| {GetGridNumberAsString(i, j, true)} ") 
                        : (showLetters ? "|   " : "|    ")
                    );
                }
                System.Console.WriteLine("|");
            }
        }

        public void DisplayEncodedLetters()
        {
            System.Console.WriteLine("The current letter/number code is:");
            for (int i = 1; i <= 26; i++)
            {
                if (i == 14) System.Console.WriteLine();
                System.Console.Write($"{i.ToString("D2")}: {GetEncodedLetter(i, ' ')} "); 
            }
            System.Console.WriteLine();
        }

        public List<Word> GetWordsFromGrid()
        {
            var wordList = new List<Word>();

            void CheckDirection(int x, int y, bool vertical)
            {
                var v = vertical ? 1 : 0;
                var h = vertical ? 0 : 1;

                if (IsGridSquareBlank(x-(1*v), y-(1*h)) && !IsGridSquareBlank(x+(1*v), y+(1*h)))
                {
                    var wordLen = 2;
                    var codeword = "";

                    while (!IsGridSquareBlank(x+(v*wordLen), y+(h*wordLen))) 
                        wordLen++;

                    for (int i = 0; i < wordLen; i++) 
                        codeword += Grid.Get(x+(i*v), y+(i*h)).ToString("D2");

                    wordList.Add(new Word(codeword));
                }
            }

            for (int i = 0; i < GridDimensions.Height; i++)
            {
                for (int j = 0; j < GridDimensions.Width; j++)
                {
                    if (IsGridSquareBlank(i, j)) continue;
                    CheckDirection(i, j, false);
                    CheckDirection(i, j, true);
                }
            }

            return wordList;
        }

        public void BackupDecodedLetters()
        { 
            NumToLetter2 = new Dictionary<int, char>();
            NumToLetter2 = NumToLetter;
        }

        public void RestoreDecodedLetters()
        {
            NumToLetter = NumToLetter2;
            NumToLetter2 = new Dictionary<int, char>();
        }

        public bool IsGridSquareBlank(int i, int j)
        {
            return !Grid.Exists(i, j) || Grid.Get(i,j).Equals(0);
        }

        private DuoKeyDictionary<int, int, int> CreateGrid(string[] codedLines)
        {
            if (codedLines.Length != GridDimensions.Height)
                throw new InvalidOperationException("The ammount of Coded Lines does not match the Height of the Grid");
            
            var grid = new DuoKeyDictionary<int, int, int>();

            var lineNum = 0;
            foreach (var line in codedLines)
            {
                if (line.Length/2 != GridDimensions.Width)
                    throw new InvalidOperationException("The length of a Coded Line does not match the Width of the Grid");

                for (int i = 0; i < GridDimensions.Width; i++)
                {
                    var numCodeStr = line.Substring(i*2, 2);

                    if (!int.TryParse(numCodeStr, out var numCodeInt))
                        throw new InvalidOperationException("A character in the Coded Lines was not a number");

                    grid.Set(lineNum, i, numCodeInt);
                }
                lineNum++;
            }

            return grid;
        }
    }

    

    class DuoKeyDictionary<K1, K2, V>
    {
        private readonly Dictionary<K1, Dictionary<K2, V>> Dict;

        public DuoKeyDictionary()
        {
            Dict = new Dictionary<K1, Dictionary<K2, V>>();
        }

        public void Set(K1 key1, K2 key2, V value)
        {
            Dict.TryAdd(key1, new Dictionary<K2, V>());
            Dict.GetValueOrDefault(key1).Remove(key2);
            Dict.GetValueOrDefault(key1).Add(key2, value);
        }

        public V Get(K1 key1, K2 key2)
        {
            return Dict.ContainsKey(key1) ? Dict.GetValueOrDefault(key1).GetValueOrDefault(key2) : default;
        }

        public bool Comapre(K1 key1, K2 key2, V comparingValue)
        {
            return Get(key1, key2).Equals(comparingValue);
        }

        public bool Exists(K1 key1, K2 key2)
        {
            return Dict.ContainsKey(key1) && Dict.GetValueOrDefault(key1).ContainsKey(key2);
        }

        public bool Remove(K1 key1, K2 key2)
        {
            return Dict.ContainsKey(key1) && Dict.GetValueOrDefault(key1).Remove(key2);
        }
    }
}
