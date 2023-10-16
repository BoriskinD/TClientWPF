using System.Text.RegularExpressions;

namespace TClientWPF.Model
{
    class PatternMatching
    {
        private string expression;

        public string Expression
        {
            get => expression;
            set => expression = value;
        }

        public bool IsMatch(string text) => Regex.Match(text, expression).Success;
    }
}
