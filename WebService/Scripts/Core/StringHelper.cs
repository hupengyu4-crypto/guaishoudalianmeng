using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RootScript.Config
{
    //===============================================================================
    public static class StringHelper
    {
        #region Public Methods
        //---------------------------------------------------------------------------
        public static bool CheckIsUrl(string url)
        {
            return RegexMatch(url, @"^(http|https|ftp)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&amp;%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&amp;%\$#\=~_\-]+))*$");
        }

        //---------------------------------------------------------------------------
        public static bool RegexMatch(string input, string pattern)
        {
            if (input != null && input.Trim() != "")
            {
                return Regex.IsMatch(input, pattern);
            }

            return false;
        }
        
        //---------------------------------------------------------------------------
        public static bool IEquals(string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }
        
        //---------------------------------------------------------------------------
        public static bool IContains(string str1, string str2)
        {
            return IIndexOf(str1, str2) >= 0;
        }

        //---------------------------------------------------------------------------
        public static int IIndexOf(string str1, string str2)
        {
            return str1.IndexOf(str2, StringComparison.OrdinalIgnoreCase);
        }

        //---------------------------------------------------------------------------
        public static bool StartsWith(string str, string val)
        {
            if (str.Length < val.Length)
            {
                return false;
            }

            for (int i = 0; i < val.Length; i++)
            {
                if (str[i] != val[i])
                {
                    return false;
                }
            }

            return true;
        }

        //---------------------------------------------------------------------------
        public static bool IStartsWith(string str, string val)
        {
            if (str.Length < val.Length)
            {
                return false;
            }

            for (int i = 0; i < val.Length; i++)
            {
                char c1 = str[i];
                char c2 = val[i];

                if (lowerCharMapArray[c1] != lowerCharMapArray[c2])
                {
                    return false;
                }
            }

            return true;
        }

        //---------------------------------------------------------------------------
        public static bool EndsWith(string str, string val)
        {
            if (str.Length < val.Length)
            {
                return false;
            }
            
            for (int i = val.Length - 1; i >= 0; i--)
            {
                char c1 = str[str.Length - (val.Length - i)];
                char c2 = val[i];

                if (c1 != c2)
                {
                    return false;
                }
            }

            int count = str.Length - val.Length;
            for (int i = str.Length - 1; i >= count; i--)
            {
                if (str[i] != val[i])
                {
                    return false;
                }
            }

            return true;
        }

        //---------------------------------------------------------------------------
        public static bool IEndsWith(string str, string val)
        {
            if (str.Length < val.Length)
            {
                return false;
            }
            
            for (int i = val.Length - 1; i >= 0; i--)
            {
                char c1 = str[str.Length - (val.Length - i)];
                char c2 = val[i];

                if (lowerCharMapArray[c1] != lowerCharMapArray[c2])
                {
                    return false;
                }
            }

            return true;
        }
        
        //---------------------------------------------------------------------------
        public static int LowerHashCode(string str)
        {
            int num = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                num = ((num << 5) - num) + lowerCharMapArray[str[i]];
            }

            return num;
        }
        
        //---------------------------------------------------------------------------
        public static string AdjustWrap(string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n");
        }
        
        //---------------------------------------------------------------------------
        public static string TrimBom(string str, Encoding ec)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            byte[] results = null;
            if (!TrimBom(ec.GetBytes(str), ec, out results))
            {
                return str;
            }

            return ec.GetString(results);
        }

        //---------------------------------------------------------------------------
        public static byte[] TrimBom(byte[] bytes, Encoding ec)
        {
            if (bytes == null || bytes.Length < 2)
            {
                return bytes;
            }

            byte[] results = bytes;
            if (!TrimBom(bytes, ec, out results))
            {
                return bytes;
            }

            return results;
        }

        //---------------------------------------------------------------------------
        public static byte[] TrimBom(byte[] bytes, Encoding[] ecs)
        {
            if (bytes == null || bytes.Length < 2)
            {
                return bytes;
            }

            for (int i = 0; i < ecs.Length; i++)
            {
                byte[] results = bytes;
                if (TrimBom(bytes, ecs[i], out results))
                {
                    return results;
                }
            }

            return bytes;
        }
        #endregion

        #region Internal Methods
        //---------------------------------------------------------------------------
        static StringHelper()
        {
            for (int i = 0; i < lowerCharMapArray.Length; ++i)
            {
                char c = (char) i;
                if (c >= 'A' && c <= 'Z')
                {
                    c = (char) (c + 32);
                }

                lowerCharMapArray[i] = c;
            }
        }

        //---------------------------------------------------------------------------
        private static string StringUniqueName(string name)
        {
            return name;
        }

        //---------------------------------------------------------------------------
        private static bool TrimBom(byte[] bytes, Encoding ec, out byte[] results)
        {
            results = bytes;

            byte[] preamble = ec.GetPreamble();
            if (bytes.Length < preamble.Length)
            {
                return false;
            }

            for (int i = 0; i < preamble.Length; i++)
            {
                if (preamble[i] != bytes[i])
                {
                    return false;
                }
            }

            int length = preamble.Length;
            results = new byte[bytes.Length - length];
            Array.Copy(bytes, length, results, 0, results.Length);

            return true;
        }
        #endregion

        #region Internal Fields
        //---------------------------------------------------------------------------
        private static char[] lowerCharMapArray = new char[char.MaxValue];

        //---------------------------------------------------------------------------
        private static Func<string, string> stringUniqueName = StringUniqueName;
        #endregion
    }
}
