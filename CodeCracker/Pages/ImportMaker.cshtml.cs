using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeCracker.Pages
{
    public class ImportMakerModel : PageModel
    {
        public int GridHeight = 19;
        public int GridWidth = 15;
        public string Title = "Puzzle 1";
        public string Letters = "";
        public string Code = "";

        public void OnGet()
        {
            var query = Request.Query;

            if (query.ContainsKey("height")) GridHeight = int.Parse(query["height"].First());
            if (query.ContainsKey("width")) GridWidth = int.Parse(query["width"].First());
            if (query.ContainsKey("name")) Title = query["name"].First();
            if (query.ContainsKey("title")) Title = query["title"].First();
            if (query.ContainsKey("code")) Code = query["code"].First();
            if (query.ContainsKey("letters")) Letters = query["letters"].First();
        }
    }
}
