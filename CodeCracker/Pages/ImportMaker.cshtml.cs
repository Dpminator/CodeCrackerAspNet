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
        public string lines = "dfsfggfdggfgsfd";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            //new PuzzleSolver(lines).SolveCodeCracker();
            System.Console.WriteLine("We did it!");
        }
    }
}
