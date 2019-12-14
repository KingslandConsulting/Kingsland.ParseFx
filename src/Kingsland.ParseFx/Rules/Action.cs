﻿using Kingsland.ParseFx.Lexing;
using Kingsland.ParseFx.Text;
using System;

namespace Kingsland.ParseFx.Rules
{

    public sealed class Action : IAction
    {

        public Action(Func<SourceReader, (Token, SourceReader)> statement)
        {
            this.Statement = statement ?? throw new ArgumentNullException(nameof(statement));
        }

        private Func<SourceReader, (Token, SourceReader)> Statement
        {
            get;
            set;
        }

        public (Token Token, SourceReader NextReader) Invoke(SourceReader reader)
        {
            return this.Statement.Invoke(reader);
        }

    }

}
