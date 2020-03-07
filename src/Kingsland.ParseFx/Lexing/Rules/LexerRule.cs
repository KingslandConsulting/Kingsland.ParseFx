using Kingsland.ParseFx.Lexing.Text;
using System;

namespace Kingsland.ParseFx.Lexing.Rules
{

    public sealed class LexerRule
    {

        #region Constructors

        internal LexerRule(IMatchRule match, Func<SourceReader, (Token, SourceReader)> action)
        {
            this.Match = match ?? throw new ArgumentNullException(nameof(match));
            this.Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        #endregion

        #region Properties

        public IMatchRule Match
        {
            get;
            private set;
        }

        public Func<SourceReader, (Token, SourceReader)> Action
        {
            get;
            private set;
        }

        #endregion

    }

}
