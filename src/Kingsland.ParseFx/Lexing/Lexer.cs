using Kingsland.ParseFx.Rules;
using Kingsland.ParseFx.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kingsland.ParseFx.Lexing
{

    public sealed class Lexer
    {

        #region Constructors

        public Lexer(IEnumerable<ILexerRule> rules)
        {
            this.Rules = new ReadOnlyCollection<ILexerRule>(
                (rules ?? throw new ArgumentNullException(nameof(rules))).ToList()
            );
            this.RuleCache = new Dictionary<char, ILexerRule>();
        }

        #endregion

        #region Properties

        public ReadOnlyCollection<ILexerRule> Rules
        {
            get;
            private set;
        }

        private Dictionary<char, ILexerRule> RuleCache
        {
            get;
            set;
        }

        #endregion

        #region Lexing Methods

        public List<Token> Lex(string sourceText)
        {
            var reader = SourceReader.From(sourceText);
            return this.ReadToEnd(reader).ToList();
        }

        public IEnumerable<Token> ReadToEnd(SourceReader reader)
        {
            var thisReader = reader;
            while (!thisReader.Eof())
            {
                (var nextToken, var nextReader) = this.ReadToken(thisReader);
                yield return nextToken;
                thisReader = nextReader;
            }
        }

        public (Token Token, SourceReader NextRader) ReadToken(SourceReader reader)
        {
            var peek = reader.Peek();
            // make sure the rule for the next character is in the rule cache
            if (!this.RuleCache.ContainsKey(peek.Value))
            {
                this.RuleCache.Add(
                    peek.Value,
                    this.Rules.FirstOrDefault(r => r.Matches(peek.Value))
                        ?? throw new UnexpectedCharacterException(peek)
                );
            }
            // apply the rule for the next character
            return this.RuleCache[peek.Value].Scan(reader);
        }

        #endregion

    }

}