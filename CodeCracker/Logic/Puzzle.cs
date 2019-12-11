using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCracker.Logic
{
    public class Puzzle
    {
        private readonly int Height;
        private readonly int Width;
        private Dictionary<(int, int), int> GridNumbers;


        public Puzzle(int height, int width, Dictionary<(int, int), int> grid)
        {
            Height = height;
            Width = width;
            GridNumbers = grid;
        }

        
    }
}
