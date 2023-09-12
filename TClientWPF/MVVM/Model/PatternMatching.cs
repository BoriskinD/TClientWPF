using System.Text.RegularExpressions;

namespace TClientWPF.Model
{
    class PatternMatching
    {
        private Regex expression;

        public PatternMatching(string pattern) =>
               expression = new Regex(pattern, RegexOptions.IgnoreCase);

        public bool IsMatch(string text)
        {
            MatchCollection matches = expression.Matches(text);
            if (matches.Count != 0) return true;
            else return false;
        }
    }
}
