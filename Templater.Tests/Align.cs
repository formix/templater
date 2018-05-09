using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templater.Tests
{
    public class Align
    {
        public string Left(string text, int totalWidth)
        {
            if (text.Length > totalWidth)
            {
                return text.Substring(0, totalWidth);
            }
            return text.PadRight(totalWidth);
        }

        public string Right(string text, int totalWidth)
        {
            if (text.Length > totalWidth)
            {
                return text.Substring(text.Length - totalWidth);
            }
            return text.PadLeft(totalWidth);
        }


        public string Center(string text, int totalWidth)
        {
            int halfWidth = (text.Length + totalWidth) / 2;
            return Right(Left(text, halfWidth), totalWidth);
        }
    }
}
