using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeCracker.Core
{
    public class PuzzleImporter
    {
        private readonly int GridHieght;
        private readonly int GridWidth;
        private readonly List<string> CodedLines;
        private readonly List<(int num, char letter)> GivenLeters;
        private readonly string Title;
        private readonly bool IsUsingNewWay;
        private readonly string NewCode;

        public PuzzleImporter(string puzzleImportText)
        {
            var lines = puzzleImportText.Split("\r\n");

            var (height, width) = ImportGridDimensions(lines);

            GridHieght = height;
            GridWidth = width;
            CodedLines = ImportEncodedNumbers(lines, height);
            GivenLeters = ImportGivenLetters(lines[height + 1]);
            Title = ImportTitle(lines, height);
            IsUsingNewWay = false;
        }

        public PuzzleImporter(int height, int width, string code, string letters, string name)
        {
            GridHieght = height;
            GridWidth = width;
            NewCode = code;
            GivenLeters = ImportGivenLetters(letters, ',');
            Title = name;
            IsUsingNewWay = true;
        }

        public static (int, int, string, string, string) ConvertOldToNew(string puzzleImportText)
        {
            var lines = puzzleImportText.Split("\r\n");

            var (height, width) = ImportGridDimensions(lines);
            var letters = lines[height + 1].Trim().Replace(' ', ',');
            var name = ImportTitle(lines, height);
            var code = "";

            var currentConsecutiveBlanks = 0;
            foreach (string line in ImportEncodedNumbers(lines, height))
            {
                for (int i = 0; i < width; i++)
                {
                    var numCodeStr = line.Substring(i * 2, 2);

                    if (!int.TryParse(numCodeStr, out var numCodeInt))
                        throw new InvalidOperationException("A character in the Coded Lines was not a number");

                    if (numCodeInt == 0)
                    {
                        currentConsecutiveBlanks++;
                        continue;
                    }

                    if (currentConsecutiveBlanks > 0)
                    {
                        if (currentConsecutiveBlanks > 26) 
                            throw new InvalidOperationException("There are more than 26 blank spaces in a row!");
                        code += char.ToLower(PuzzleSolver.NumToAlphabetLetter(currentConsecutiveBlanks));
                        currentConsecutiveBlanks = 0;
                    }

                    code += PuzzleSolver.NumToAlphabetLetter(numCodeInt);
                }
            }
            if (currentConsecutiveBlanks > 0) code += char.ToLower(PuzzleSolver.NumToAlphabetLetter(currentConsecutiveBlanks));

            return (height, width, code, letters, name);
        }

        public int GetGridHeight()
        {
            return GridHieght;
        }

        public int GetGridWidth()
        {
            return GridWidth;
        }

        public List<string> GetCodedLines()
        {
            if (!IsUsingNewWay)
                return CodedLines;
            throw new InvalidOperationException("'CodedLines' does not exist for the new way...");
        }

        public bool IsUsingNewCode()
        {
            return IsUsingNewWay;
        }

        public string GetNewCode()
        {
            if (IsUsingNewWay)
                return NewCode;
            throw new InvalidOperationException("'NewCode' does not exist for the old way...");
        }

        public List<(int num, char letter)> GetGivenLetters()
        {
            return GivenLeters;
        }

        public string GetTitle()
        {
            return Title;
        }

        private static (int h, int w) ImportGridDimensions(string[] lines)
        {
            var gridDimensions = lines[0].Trim().Split('x');
            var gridHieght = int.Parse(gridDimensions[0]);
            var gridWidth = int.Parse(gridDimensions[1]);

            return (gridHieght, gridWidth);
        }

        private static List<string> ImportEncodedNumbers(string[] lines, int gridHeight)
        {
            return lines.Skip(1).Take(gridHeight).ToList();
        }

        private List<(int num, char letter)> ImportGivenLetters(string lettersLine, char separator = ' ')
        {
            var givenLetters = new List<(int, char)>();

            foreach (var letterCode in lettersLine.Split(separator))
            {
                if (letterCode.Length != 3)
                    throw new ArgumentException("Given letter code was not exactly 3 characters");

                var num = int.Parse(letterCode.Substring(0, 2));
                var letter = char.Parse(letterCode.Substring(2).Trim().ToUpper());

                givenLetters.Add((num, letter));
            }

            return givenLetters;
        }

        private static string ImportTitle(string[] lines, int gridHeight)
        {
            return lines.Length == gridHeight + 3 ? $"{lines[gridHeight + 2].Trim()}" : "Results" ;
        }
    }
}
