using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Data3Sixty.Engine.Result
{
    public class InvalidMessageException : InvalidOperationException
    {
        public InvalidMessageException(string msg)
            : base(msg) { }
    }
}
