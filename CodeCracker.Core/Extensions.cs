using System.Collections.Generic;
using System.Linq;

namespace CodeCracker.Core
{
	public static class Extensions
	{
		public static IEnumerable<(char, int)> ToListWithIndex(this string str)
		{
			int num = 0;
			return from ch in str select (ch, num++);
		}
	}
}
