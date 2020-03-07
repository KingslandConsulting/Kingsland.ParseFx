using Kingsland.ParseFx.Lexing.Text;

namespace Kingsland.ParseFx.Lexing.Rules
{

    public interface IAction
    {

        public (Token Token, SourceReader NextReader) Invoke(SourceReader reader);

    }

}
