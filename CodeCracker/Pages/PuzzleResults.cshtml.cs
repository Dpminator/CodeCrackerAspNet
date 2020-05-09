using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeCracker.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeCracker.Pages
{
    public class PuzzleResultsModel : PageModel
    {
        public Puzzle solvedPuzzle;

        public void OnGet()
        {
            var query = Request.Query;

            string GetQueryValue(string key)
            {
                if (!query.ContainsKey(key))
                    throw new InvalidOperationException($"GET parameters does not contain the \"{key}\" key!");
                return query[key].First();
            }

            var height = int.Parse(GetQueryValue("height"));
            var width = int.Parse(GetQueryValue("width"));
            var code = GetQueryValue("code");
            var letters = GetQueryValue("letters");
            var name = GetQueryValue("name");

            solvedPuzzle = new PuzzleSolver(height, width, code, letters, name).SolveCodeCracker();
        }
    }
}
