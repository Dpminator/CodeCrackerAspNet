using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeCracker.Pages
{
    public class ImportMakerModel : PageModel
    {
        public int GridHeight = 19;
        public int GridWidth = 15;

        public void OnGet()
        {
            var query = Request.Query;

            if (query.ContainsKey("height")) GridHeight = int.Parse(query["height"].First());
            if (query.ContainsKey("width")) GridWidth = int.Parse(query["width"].First());
        }
    }
}
