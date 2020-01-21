using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeCracker.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeCracker.Pages
{
    public class ImportMakerModel : PageModel
    {
        public string lines;

        public void OnGet()
        {
        }

        public void OnPost()
        {
            new PuzzleSolver(lines).SolveCodeCracker();
        }

        public void ToggleSquare(int id)
        {
            System.Console.WriteLine("I MADE IT HERE!!");
        }
    }
}
