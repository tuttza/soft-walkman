using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Soft_Walkman.Lib
{
    class StringFormatter
    {
        public static string RemoveNumbersFromBeginningOfString(string str)
        {
            string formattedString = "";

            string numberPattern = @"(^([0-9]+)(\s?)(-?))";

            var regex = new Regex(numberPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (regex.IsMatch(str))
            {
                formattedString = Regex.Replace(str, numberPattern, "");
                Debug.WriteLine($"RemoveNumbersFromBeginningOfString - formattedString: {0}", formattedString);
            } 

            if (regex.IsMatch(formattedString))
            {
                formattedString = Regex.Replace(formattedString, numberPattern, "");
                Debug.WriteLine($"RemoveNumbersFromBeginningOfString - formattedString: {0}", formattedString);
            }

            if (formattedString.Length > 0)
            {
                return formattedString;
            } 
            else
            {
                return str;
            }
        }

        public static string RemoveExtension(string str)
        {
            string formattedString = "";

            string extPattern = @"(\.[a-zA-Z0-9]+)";

            var regex = new Regex(extPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (regex.IsMatch(str))
            {
                var matches = regex.Matches(str);
                var lastMatch = matches[matches.Count - 1];
                formattedString = str.Remove(lastMatch.Index);
                Debug.WriteLine($"RemoveExtention - formattedString: {0}", formattedString);
            }

            if (formattedString.Length > 0)
            {
                return formattedString;
            }
            else
            {
                return str;
            }
        }

        public static string RemoveSpecialCharNearBeginningOfString(string str, char special)
        {
            string formattedString = "";

            int specialCharIndex = str.IndexOf(special);

            if (specialCharIndex == -1)
            {
                return str;
            }
            
            if (specialCharIndex <= 4)
            {
                try
                {
                    formattedString = str.Remove(specialCharIndex, specialCharIndex + 1);

                } catch(ArgumentOutOfRangeException)
                {
                    return str;
                }
            }

            Debug.WriteLine($"RemoveSpecialCharNearBeginningOfString - formattedString: {0}", formattedString);

            if (formattedString.Length > 0)
            {
                return formattedString;
            } 
            else
            {
                return str;
            }
        }
    }
}
