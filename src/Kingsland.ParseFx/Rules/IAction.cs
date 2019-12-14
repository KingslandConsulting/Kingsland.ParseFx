using Kingsland.ParseFx.Lexing;
using Kingsland.ParseFx.Text;

namespace Kingsland.ParseFx.Rules
{

    public interface IAction
    {

        public (Token Token, SourceReader NextReader) Invoke(SourceReader reader);

    }

}
