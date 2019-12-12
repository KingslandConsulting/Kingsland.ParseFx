using Kingsland.ParseFx.Lexing;
using Kingsland.ParseFx.Text;
using System;
using System.Text.RegularExpressions;

namespace Kingsland.ParseFx.Rules
{

    public sealed class RegexRule : LexerRule
    {

        #region Constructors

        public RegexRule(string pattern, Func<SourceReader, (Token, SourceReader)> action)
            : base(action)
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

        public override bool Matches(char value)
        {
            return this.Regex.IsMatch(
                new string(value, 1)
            );
        }

        #endregion

    }

}
