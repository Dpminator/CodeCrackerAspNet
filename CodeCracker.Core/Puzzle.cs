using System;
using System.Collections.Generic;

namespace CodeCracker.Core
{
    public class Puzzle
    {
        private readonly string Title;
        private readonly (int Height, int Width) GridDimensions;
        private readonly DuoKeyDictionary<int, int, int> Grid;
        private Dictionary<int, char> NumToLetter;
        private Dictionary<int, char> NumToLetterBackup;

        public Puzzle(PuzzleImporter import)
        {
            Title = import.GetTitle();
            GridDimensions = (import.GetGridHeight(), import.GetGridWidth()); 
            Grid = import.IsUsingNewCode() ? CreateGrid(import.GetNewCode()) : CreateGrid(import.GetCodedLines());
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

        public List<CodedWord> GetWordsFromGrid()
        {
            var wordList = new List<CodedWord>();

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

                    wordList.Add(new CodedWord(codeword));
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
            NumToLetterBackup = new Dictionary<int, char>();
            NumToLetterBackup = NumToLetter;
        }

        public void RestoreDecodedLetters()
        {
            NumToLetter = NumToLetterBackup;
            NumToLetterBackup = new Dictionary<int, char>();
        }

        public bool IsGridSquareBlank(int i, int j)
        {
            return !Grid.Exists(i, j) || Grid.Get(i,j).Equals(0);
        }

        public void CreateHtmlResults(string directory)
        {
            if (!IsSolved()) 
                throw new InvalidOperationException("You cannot create a results file when the puzzle is yet to be solved!");

            using var output = new System.IO.StreamWriter($@"{directory}\Results.html");
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
            output.WriteLine($"<h1>{GetTitle()}</h1>");
            output.WriteLine("<table class='result'>");
            for (int i = 0; i < GetGridHeight(); i++)
            {
                output.WriteLine("<tr>");
                for (int j = 0; j < GetGridWidth(); j++)
                {
                    if (IsGridSquareBlank(i, j))
                    {
                        output.WriteLine("<td class = 'blank'></td>");
                    }
                    else
                    {
                        output.WriteLine("<td><div class = 'small'>" + GetGridNumberAsString(i, j) + "</div><br><div class = 'regular'>" + GetEncodedLetter(i, j) + "</div></td>");
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
                output.WriteLine("<td><div class = 'small'>" + i + "</div><br><div class = 'regular'>" + GetEncodedLetter(i) + "</div></td>");
            }
            output.WriteLine("</tr></table>");
            output.WriteLine("</table>");
            output.WriteLine("</body>");
            output.WriteLine("</html>");
        }

        public void CreateTextFileResults(string directory)
        {
            if (!IsSolved())
                throw new InvalidOperationException("You cannot create a results file when the puzzle is yet to be solved!");

            using var output = new System.IO.StreamWriter($@"{directory}\Export.txt");
            output.WriteLine(GetTitle());

            output.WriteLine("----------------------------------------------------------------------------");
            for (int i = 0; i < GetGridHeight(); i++)
            {
                for (int j = 0; j < GetGridWidth(); j++)
                {
                    if (IsGridSquareBlank(i, j))
                    {
                        output.Write("| ");
                    }
                    else
                    {
                        output.Write("|" + GetEncodedLetter(i, j));
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
                output.Write((i + 1) + ": " + GetEncodedLetter(i + 1) + "  ");
                if (i == 25)
                {
                    output.WriteLine();
                }
            }
        }

        private DuoKeyDictionary<int, int, int> CreateGrid(List<string> codedLines)
        {
            if (codedLines.Count != GridDimensions.Height)
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

        private DuoKeyDictionary<int, int, int> CreateGrid(string codedLine)
        {
            var grid = new DuoKeyDictionary<int, int, int>();
            (int i, int j) = (0, 0);
            (int, int) incrementCount()
            {
                if (++j == GridDimensions.Width)
                {
                    j = 0;
                    i++;
                }
                return (i, j);
            }


            foreach (var codedChar in codedLine)
            {
                if (char.IsLower(codedChar))
                {
                    //If its a number of blanks
                    for (var _ = 0; _ < PuzzleSolver.AlphabetLetterToNum(codedChar); _++)
                    {
                        grid.Set(i, j, 0);
                        (i, j) = incrementCount();
                    }
                }
                else 
                {   
                    //If its a coded number
                    grid.Set(i, j, PuzzleSolver.AlphabetLetterToNum(codedChar));
                    (i, j) = incrementCount();
                }
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
