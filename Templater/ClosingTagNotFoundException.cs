using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templater
{
    public class ClosingTagNotFoundException : Exception
    {
        public ClosingTagNotFoundException() : base("End of script reached without a closing tag.") { }
        public ClosingTagNotFoundException(string message) : base(message) { }
        public ClosingTagNotFoundException(string message, Exception cause) : base(message, cause) { }
    }
}
