using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCracker.Console
{
    public class PuzzleImporter
    {
        private readonly int GridHieght;
        private readonly int GridWidth;
        private readonly string[] CodedLines;
        private readonly (int num, char letter)[] GivenLeters;
        private readonly string Title;

        public PuzzleImporter(string puzzleImportText)
        {
            var lines = puzzleImportText.Split("\r\n");

            var (height, width) = ImportGridDimensions(lines);

            GridHieght = height;
            GridWidth = width;
            CodedLines = ImportEncodedNumbers(lines);
            GivenLeters = ImportGivenLetters(lines);
            Title = ImportTitle(lines);
        }

        public int GetGridHeight()
        {
            return GridHieght;
        }

        public int GetGridWidth()
        {
            return GridWidth;
        }

        public string[] GetCodedLines()
        {
            return CodedLines;
        }

        public (int num, char letter)[] GetGivenLetters()
        {
            return GivenLeters;
        }

        public string GetTitle()
        {
            return Title;
        }

        private (int h, int w) ImportGridDimensions(string[] lines)
        {
            var gridDimensions = lines[0].Trim().Split('x');
            var gridHieght = int.Parse(gridDimensions[0]);
            var gridWidth = int.Parse(gridDimensions[1]);

            return (gridHieght, gridWidth);
        }

        private string[] ImportEncodedNumbers(string[] lines)
        {
            return lines.Skip(1).Take(GridHieght).ToArray();
        }

        private (int num, char letter)[] ImportGivenLetters(string[] lines)
        {
            var givenLetters = new List<(int, char)>();

            foreach (var letterCode in lines[GridHieght + 1].Split(' '))
            {
                if (letterCode.Length != 3)
                    throw new ArgumentException("Given letter code was not exactly 3 characters");

                var num = int.Parse(letterCode.Substring(0, 2));
                var letter = char.Parse(letterCode.Substring(2).Trim().ToUpper());

                givenLetters.Add((num, letter));
            }

            return givenLetters.ToArray();
        }

        private string ImportTitle(string[] lines)
        {
            return lines.Length == GridHieght + 3 ? $"{lines[GridHieght + 2].Trim()}" : "Results" ;
        }
    }
}
