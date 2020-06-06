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
        public Dictionary<int, string> Letters = new Dictionary<int, string>();
        public string LettersCode = "";
        public string Code = "";

        public void OnGet()
        {
            var query = Request.Query;

            if (query.ContainsKey("height")) GridHeight = int.Parse(query["height"].First());
            if (query.ContainsKey("width")) GridWidth = int.Parse(query["width"].First());
            if (query.ContainsKey("name")) Title = query["name"].First();
            if (query.ContainsKey("title")) Title = query["title"].First();
            if (query.ContainsKey("code")) Code = query["code"].First();
            if (query.ContainsKey("letters"))
            {
                LettersCode = query["letters"].First();
                var letterPairs = LettersCode.Split(",");
                foreach (var letterPair in letterPairs)
                {
                    var num = int.Parse(letterPair.Substring(0, 2));
                    var letter = letterPair.Substring(2, 1).ToUpper();
                    Letters.Add(num, letter);
                }
            }
        }
    }
}
