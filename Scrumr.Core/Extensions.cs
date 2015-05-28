using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Core.Extensions
{
    public static class Extensions
    {
        public static string ToSubcode(this Guid target, int length = 7)
        {
            var fullCode = target.ToString();
            return fullCode.Substring(fullCode.Length - length, length);
        }
    }
}
