using Kingsland.ParseFx.Lexing;
using Kingsland.ParseFx.Text;
using System;

namespace Kingsland.ParseFx.Rules
{

    public sealed class LexerRule
    {

        #region Constructors

        internal LexerRule(IMatch match, IAction action)
        {
            this.Match = match ?? throw new ArgumentNullException(nameof(match));
            this.Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        #endregion

        #region Properties

        public IMatch Match
        {
            get;
            private set;
        }

        public IAction Action
        {
            get;
            private set;
        }

        #endregion

    }

}
