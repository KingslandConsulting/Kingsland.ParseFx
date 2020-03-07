using System;

namespace Kingsland.ParseFx.Lexing.Rules
{

    public sealed class RangeMatchRule : IMatchRule
    {

        #region Constructors

        public RangeMatchRule(char fromValue, char toValue)
        {
            if (fromValue > toValue)
            {
                throw new ArgumentException($"{nameof(fromValue)} must be less than {nameof(toValue)}.");
            }
            this.FromValue = fromValue;
            this.ToValue = toValue;
        }

        #endregion

        #region Properties

        public char FromValue
        {
            get;
            private set;
        }

        public char ToValue
        {
            get;
            private set;
        }

        #endregion

        #region LexerRule Members

        public bool Matches(char value)
        {
            return (value >= this.FromValue) && (value <= this.ToValue);
        }

        #endregion

    }

}
