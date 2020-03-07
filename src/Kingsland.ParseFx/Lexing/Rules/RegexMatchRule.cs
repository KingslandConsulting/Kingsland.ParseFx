using System.Text.RegularExpressions;

namespace Kingsland.ParseFx.Lexing.Rules
{

    public sealed class RegexMatchRule : IMatchRule
    {

        #region Constructors

        public RegexMatchRule(string pattern)
        {
            this.Pattern = pattern;
            this.Regex = new Regex(pattern, RegexOptions.Compiled);
        }

        #endregion

        #region Properties

        public string Pattern
        {
            get;
            private set;
        }

        public Regex Regex
        {
            get;
            private set;
        }

        #endregion

        #region LexerRule Members

        public bool Matches(char value)
        {
            return this.Regex.IsMatch(
                new string(value, 1)
            );
        }

        #endregion

    }

}
