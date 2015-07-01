using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluffinMuffin.Server.DataTypes.Helper
{
    public static class CardStringRepresentationHelper
    {
        public static string ConvertForHandEvaluator(string c)
        {
            return c.Replace("T", "10");
        }
        public static string ConvertForProtocol(string c)
        {
            return c.Replace("10", "T");
        }
    }
}
