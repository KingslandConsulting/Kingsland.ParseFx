using Kingsland.ParseFx.Lexing;
using Kingsland.ParseFx.Text;
using System;

namespace Kingsland.ParseFx.Rules
{

    public abstract class LexerRule : ILexerRule
    {

        #region Constructors

        public LexerRule(Func<SourceReader, (Token, SourceReader)> action)
        {
            this.Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        #endregion

        #region Properties

        public abstract bool Matches(char value);

        public Func<SourceReader, (Token, SourceReader)> Action
        {
            get;
            private set;
        }

        #endregion

        #region ILexerRule Interface

        public (Token, SourceReader) Scan(SourceReader reader)
        {
            return this.Action(reader);
        }

        #endregion

    }

}
