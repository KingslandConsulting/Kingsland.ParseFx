using System;
using Kingsland.ParseFx.Lexing;
using Kingsland.ParseFx.Text;

namespace Kingsland.ParseFx.Rules
{

    public sealed class CharRule : LexerRule
    {

        #region Constructors

        public CharRule(char value, Func<SourceReader, (Token, SourceReader)> action)
            : base(action)
        {
            this.Value = value;
        }

        #endregion

        #region Properties

        public char Value
        {
            get;
            private set;
        }

        #endregion

        #region LexerRule Members

        public override bool Matches(char value)
        {
            return (value == this.Value);
        }

        #endregion

    }

}
