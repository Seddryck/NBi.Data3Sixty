using NBi.NUnit.Runtime.Embed.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Data3Sixty.Engine.Filter
{
    [Serializable]
    public class IdentifierFilter : PropertyFilter
    {
        public IdentifierFilter(string identifier)
            : base("Identifier", identifier)
        { }
    }
}
