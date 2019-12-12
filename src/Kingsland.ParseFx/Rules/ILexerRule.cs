using Kingsland.ParseFx.Lexing;
using Kingsland.ParseFx.Text;

namespace Kingsland.ParseFx.Rules
{

    public interface ILexerRule
    {

        public bool Matches(char value);

        public (Token, SourceReader) Scan(SourceReader reader);

    }

}
