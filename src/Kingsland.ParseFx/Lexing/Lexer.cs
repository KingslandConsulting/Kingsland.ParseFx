using Kingsland.ParseFx.Lexing.Rules;
using Kingsland.ParseFx.Lexing.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kingsland.ParseFx.Lexing
{

    public sealed class Lexer
    {

        #region Constructors

        public Lexer()
            : this(new List<LexerRule>())
        {
        }

        public Lexer(IEnumerable<LexerRule> rules)
        {
            this.Rules = new ReadOnlyCollection<LexerRule>(
                (rules ?? throw new ArgumentNullException(nameof(rules))).ToList()
            );
            this.RuleCache = new Dictionary<char, LexerRule>();
        }

        #endregion

        #region Properties

        public ReadOnlyCollection<LexerRule> Rules
        {
            get;
            private set;
        }

        private Dictionary<char, LexerRule> RuleCache
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
                    this.Rules.FirstOrDefault(r => r.Match.Matches(peek.Value))
                        ?? throw new UnexpectedCharacterException(peek)
                );
            }
            // apply the rule for the next character
            return this.RuleCache[peek.Value].Action.Invoke(reader);
        }

        #endregion

        #region LexerRule Methods

        //public Lexer AddRule(char value, Func<SourceExtent, Token> constructor)
        //{
        //    return this.AddRule(
        //        new CharMatchRule(value),
        //        reader =>
        //        {
        //            (var sourceChar, var nextReader) = reader.Read(value);
        //            var extent = SourceExtent.From(sourceChar);
        //            return (constructor(extent), nextReader);
        //        }
        //    );
        //}

        public Lexer AddRule(char value, Func<SourceReader, (Token, SourceReader)> action)
        {
            return this.AddRule(new CharMatchRule(value), action);
        }

        public Lexer AddRule(char fromValue, char toValue, Func<SourceReader, (Token, SourceReader)> action)
        {
            return this.AddRule(new RangeMatchRule(fromValue, toValue), action);
        }

        public Lexer AddRule(string pattern, Func<SourceReader, (Token, SourceReader)> action)
        {
            return this.AddRule(new RegexMatchRule(pattern), action);
        }

        public Lexer AddRule(IMatchRule match, Func<SourceReader, (Token, SourceReader)> action)
        {
            var newRules = this.Rules.ToList();
            newRules.Add(new LexerRule(match, action));
            return new Lexer(newRules);
        }

        #endregion

    }

}