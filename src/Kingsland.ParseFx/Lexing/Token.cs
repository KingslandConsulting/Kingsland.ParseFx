using Kingsland.ParseFx.Text;
using System;

namespace Kingsland.ParseFx.Lexing
{

    public abstract class Token
    {

        protected Token(SourceExtent extent)
        {
            this.Extent = extent;
        }

        public SourceExtent Extent
        {
            get;
            private set;
        }

        public bool IsEqualTo(Token obj)
        {
            return object.ReferenceEquals(obj, this) ||
                   ((obj != null) &&
                    (obj.GetType() == this.GetType()) &&
                    obj.Extent.IsEqualTo(this.Extent));
        }

        public override string ToString()
        {
            return this.Extent.Text;
        }

    }

}
