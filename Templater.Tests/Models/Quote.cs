using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templater.Tests.Models
{
    public class Quote
    {
        public string Title { get; set; }
        public List<Line> Lines { get; private set; }

        public Quote()
        {
            this.Lines = new List<Line>();
        }
    }
}
