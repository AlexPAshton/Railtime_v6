using System;
using System.Collections.Generic;
using System.Text;

namespace RtRoutePlanner
{
    //Contains operator overloads
    static class RouteOperands
    {
        public static string NullSplit(this string input, string splitby, int index)
        {
            try
            {
                return input.Split(new string[] { splitby }, 0)[index];
            }
            catch
            {
                return "null";
            }
        }
    }
}
