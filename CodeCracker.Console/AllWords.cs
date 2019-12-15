using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCracker.Console
{
    public class AllWords
    {
        public static AllWords _instance;
        private readonly static int _maxLetters = 15; 

        private Dictionary<int, List<string>> WordLists;

        public static AllWords GetInstance()
        {
            if (_instance == null)
                _instance = new AllWords();

            return _instance;
        }

        public List<string> GetWordList(int letterCount)
        {
            if (letterCount < 3) 
                throw new InvalidOperationException("Words less than 3 letters aren't stored!");

            if (letterCount > _maxLetters) letterCount = _maxLetters;

            return WordLists[letterCount];
        }

        private AllWords()
        {
            var url = "http://app.aspell.net/create?max_size=60&spelling=AU&max_variant=0&diacritic=strip&download=wordlist&encoding=utf-8&format=inline";
            System.Console.WriteLine($"\n\nGetting all English words from:\n{url}\nThis will take a little while...");

            WordLists = new Dictionary<int, List<string>>();
            for (int i = 3; i <= _maxLetters; i++)
                WordLists.Add(i, new List<string>());

            var enumerator = new System.Net.WebClient().DownloadString(url).Split("\n").GetEnumerator();
            for (int i = 0; i < 44; i++) enumerator.MoveNext();

            while(enumerator.MoveNext())
            {
                string word = enumerator.Current.ToString().Trim();

                if (word.Length < 3 || word.Contains("'") || char.IsUpper(word[1])) 
                    continue;

                var letterCount = word.Length > _maxLetters ? _maxLetters : word.Length;

                WordLists[letterCount].Add(word.ToLower());
            }

            for (int i = 3; i <= _maxLetters; i++)
                WordLists[i].Sort();

            System.Console.WriteLine($"Completed the retrieval of all words!\n\n");
        }

    }
}
