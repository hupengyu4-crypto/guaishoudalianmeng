using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace RootScript.Config
{
    /// <summary>
    /// 字符串相关的辅助方法集合
    /// </summary>
    public static class StringUtils
    {
        #region Methods
        /// <summary>
        /// 判断两个字符串是否相等，忽略大小写；
        /// </summary>
        /// <param name="str">字符串1</param>
        /// <param name="other">字符串2</param>
        /// <returns>相等返回True，否则返回False</returns>
        public static bool IEquals(this string str, string other)
        {
            return str.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 判断字符串中是否包含指定字符串，忽略大小写；
        /// </summary>
        /// <param name="str">字符串1</param>
        /// <param name="other">字符串2</param>
        /// <returns>包含返回True，否则返回False</returns>
        public static bool IContains(this string str, string other)
        {
            return str.IndexOf(other, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// 获取指定字符串在给定字符串中的起始位置，忽略大小写；
        /// </summary>
        /// <param name="str">字符串1</param>
        /// <param name="other">字符串2</param>
        /// <returns>包含则返回起始位置，否则返回-1</returns>
        public static int IIndexOf(this string str, string other)
        {
            return str.IndexOf(other, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 判断字符串中是否以给定字符串开始，忽略大小写；
        /// </summary>
        /// <param name="str">字符串1</param>
        /// <param name="other">字符串2</param>
        /// <returns>如果是则返回True，否则返回False</returns>
        public static bool IStartsWith(this string str, string val)
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

        /// <summary>
        /// 判断字符串中是否以给定字符串结尾，忽略大小写；
        /// </summary>
        /// <param name="str">字符串1</param>
        /// <param name="other">字符串2</param>
        /// <returns>如果是则返回True，否则返回False</returns>
        public static bool IEndsWith(this string str, string val)
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

        /// <summary>
        /// 获取字符串的哈希值，自定义算法与系统有区别，不能混用。
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回哈希值</returns>
        public static int HashCode(this string str)
        {
            int num = 0;
            int totalLength = str.Length;
            for (int i = 0; i < totalLength; ++i)
            {
                num = ((num << 5) - num) + str[i];
            }

            return num;
        }

        /// <summary>
        /// 获取字符串的哈希值，自定义算法与系统有区别，不能混用。
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="startIndex">哈希值计算的起始位置</param>
        /// <returns>返回哈希值</returns>
        public static int HashCode(this string str, int startIndex)
        {
            int num = 0;
            int totalLength = str.Length;
            for (int i = startIndex; i < totalLength; ++i)
            {
                num = ((num << 5) - num) + str[i];
            }

            return num;
        }

        /// <summary>
        /// 获取字符串的哈希值，自定义算法与系统有区别，不能混用。
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="startIndex">哈希值计算的起始位置</param>
        /// <param name="length">哈希值计算的字符串总长度</param>
        /// <returns>返回哈希值</returns>
        public static int HashCode(this string str, int startIndex, int length)
        {
            int num = 0;
            int totalLength = startIndex + length;
            for (int i = startIndex; i < totalLength; ++i)
            {
                num = ((num << 5) - num) + str[i];
            }

            return num;
        }

        /// <summary>
        /// 获取字符串的哈希值，忽略大小写，自定义算法与系统有区别，不能混用。
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回哈希值</returns>
        public static int IHashCode(this string str)
        {
            int num = 0;
            int totalLength = str.Length;
            for (int i = 0; i < totalLength; ++i)
            {
                var c = str[i];
                if (c < lowerCharMapArray.Length)
                {
                    num = ((num << 5) - num) + lowerCharMapArray[c];
                }
                else
                {
                    num = ((num << 5) - num) + c;
                }
            }

            return num;
        }

        /// <summary>
        /// 获取字符串的哈希值，忽略大小写，自定义算法与系统有区别，不能混用。
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="startIndex">哈希值计算的起始位置</param>
        /// <returns>返回哈希值</returns>
        public static int IHashCode(this string str, int startIndex)
        {
            int num = 0;
            int totalLength = str.Length;
            for (int i = startIndex; i < totalLength; ++i)
            {
                var c = str[i];
                if (c < lowerCharMapArray.Length)
                {
                    num = ((num << 5) - num) + lowerCharMapArray[c];
                }
                else
                {
                    num = ((num << 5) - num) + c;
                }
            }

            return num;
        }

        /// <summary>
        /// 获取字符串的哈希值，忽略大小写，自定义算法与系统有区别，不能混用。
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="startIndex">哈希值计算的起始位置</param>
        /// <param name="length">哈希值计算的字符串总长度</param>
        /// <returns>返回哈希值</returns>
        public static int IHashCode(this string str, int startIndex, int length)
        {
            int num = 0;
            int totalLength = startIndex + length;
            for (int i = startIndex; i < totalLength; ++i)
            {
                var c = str[i];
                if (c < lowerCharMapArray.Length)
                {
                    num = ((num << 5) - num) + lowerCharMapArray[c];
                }
                else
                {
                    num = ((num << 5) - num) + c;
                }
            }

            return num;
        }

        /// <summary>
        /// 移除给定字符串最前面包含的Bom数据，如果没有则返回原本字符串。
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="encoding">字符集编码</param>
        /// <returns>移除后的字节数组</returns>
        public static string TrimBom(string str, Encoding encoding)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            byte[] results = null;
            if (!TrimBom(encoding.GetBytes(str), encoding, out results))
            {
                return str;
            }

            return encoding.GetString(results);
        }

        /// <summary>
        /// 移除给定字节数组最前面包含的Bom数据，如果没有则返回原本数组。
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="encoding">字符集编码</param>
        /// <returns>移除后的字节数组</returns>
        public static byte[] TrimBom(byte[] bytes, Encoding encoding)
        {
            if (bytes == null || bytes.Length < 2)
            {
                return bytes;
            }

            byte[] results = bytes;
            if (!TrimBom(bytes, encoding, out results))
            {
                return bytes;
            }

            return results;
        }

        /// <summary>
        /// 移除给定字节数组最前面包含的Bom数据，如果没有则返回原本数组。
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="encodings">字符集编码集合</param>
        /// <returns>移除后的字节数组</returns>
        public static byte[] TrimBom(byte[] bytes, Encoding[] encodings)
        {
            if (bytes == null || bytes.Length < 2)
            {
                return bytes;
            }

            for (int i = 0; i < encodings.Length; i++)
            {
                byte[] results = bytes;
                if (TrimBom(bytes, encodings[i], out results))
                {
                    return results;
                }
            }

            return bytes;
        }

        /// <summary>
        /// 在传入的字符串后拼接索引，确保拼接后的字符串在传入的列表中唯一。
        /// </summary>
        /// <param name="str">字符串前缀</param>
        /// <param name="strs">已存在的字符串列表</param>
        /// <returns>拼接后的字符串</returns>
        public static string Unique(string str, ICollection<string> strs)
        {
            return Unique(str, strs, null);
        }

        /// <summary>
        /// 在传入的字符串后拼接索引，确保拼接后的字符串在传入的列表中唯一。
        /// 通过字符串获取器来获得对象对应的字符串，如果字符串获取器为空则使用对象的ToString。
        /// </summary>
        /// <typeparam name="T">列表对象类型</typeparam>
        /// <param name="str">字符串前缀</param>
        /// <param name="strs">已存在的字符串列表</param>
        /// <param name="strGetter">字符串获取器</param>
        /// <returns>拼接后的字符串</returns>
        public static string Unique<T>(
            string str, ICollection<T> strs, Func<T, string> strGetter)
        {
            int index = 0;
            string result = string.Empty;
            while (string.IsNullOrEmpty(result))
            {
                string newName = str;
                if (index != 0)
                {
                    newName = string.Format("{0} {1}", str, index);
                }

                bool founded = false;
                var iter = strs.GetEnumerator();
                while (iter.MoveNext())
                {
                    string oldName = string.Empty;
                    if (strGetter != null)
                    {
                        oldName = strGetter(iter.Current);
                    }
                    else
                    {
                        oldName = iter.Current.ToString();
                    }

                    if (oldName == newName)
                    {
                        founded = true;
                        break;
                    }
                }

                if (!founded)
                {
                    result = newName;
                }

                index++;
            }

            return result;
        }

        /// <summary>
        /// 测试字符串是否为有效的Url
        /// </summary>
        /// <param name="str">测试字符串</param>
        /// <returns>如果字符串为有效的Url返回True，否则返回False</returns>
        public static bool IsUrl(string str)
        {
            return RegexMatch(str,
                @"^[a-zA-Z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$");
        }

        /// <summary>
        /// 使用正则表达式来匹配字符串
        /// </summary>
        /// <param name="str">测试字符串</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns>如果正则表达式能匹配测试字符串返回True，否则返回False</returns>
        public static bool RegexMatch(string str, string pattern)
        {
            if (str == pattern)
            {
                return true;
            }

            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// 使用通配符表达式来匹配字符串
        /// </summary>
        /// <param name="str">测试字符串</param>
        /// <param name="pattern">含有通配符的表达式</param>
        /// <returns>如果匹配成功返回True，否则返回False</returns>
        public static bool WildcardMatch(string str, string pattern)
        {
            if (str == pattern)
            {
                return true;
            }

            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            pattern = Regex.Escape(pattern);
            pattern = pattern.Replace(@"\*", ".*?");

            return Regex.IsMatch(str, "^" + pattern + "$");
        }

        /// <summary>
        /// 使用通配符表达式来匹配通配符表达式
        /// </summary>
        /// <param name="str">含有通配符的表达式</param>
        /// <param name="pattern">含有通配符的表达式</param>
        /// <returns>如果匹配成功返回True，否则返回False</returns>
        public static bool WildcardMatchTwoWay(string str, string pattern)
        {
            if (str == pattern)
            {
                return true;
            }

            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            int firstStrWcPos = str.IndexOf('*');
            if (firstStrWcPos == -1)
            {
                return WildcardMatch(str, pattern);
            }

            var patterns = pattern.Split('*');
            if (patterns.Length == 1)
            {
                return WildcardMatch(pattern, str);
            }

            string firstPattern = patterns[0];
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(firstPattern))
            {
                int patternWcPos = firstPattern.Length;
                int minWcPos = Math.Min(firstStrWcPos, patternWcPos);
                sb.Append('^');
                if (firstPattern.Length <= minWcPos)
                {
                    sb.Append(Regex.Escape(firstPattern));
                }
                else
                {
                    sb.Append(Regex.Escape(firstPattern.Substring(0, minWcPos)));
                    sb.Append(@"(\*|");
                    sb.Append(Regex.Escape(firstPattern.Substring(
                        minWcPos, firstPattern.Length - minWcPos)));
                    sb.Append(")");
                }
            }

            sb.Append(".*?");
            int patternLength = patterns.Length;
            for (int i = 1; i < patternLength - 1; i++)
            {
                if (string.IsNullOrEmpty(patterns[i]))
                {
                    continue;
                }

                sb.Append(@"(\*|");
                sb.Append(Regex.Escape(patterns[i]));
                sb.Append(").*?");
            }

            string lastPattern = patterns[patternLength - 1];
            if (!string.IsNullOrEmpty(lastPattern))
            {
                int lastStrWcLength = str.Length - str.LastIndexOf('*');
                int lastPatternWcLength = pattern.Length - pattern.LastIndexOf('*');
                int maxWcLength = Math.Max(lastStrWcLength, lastPatternWcLength);
                if (lastStrWcLength < lastPatternWcLength)
                {
                    sb.Append(@"(\*|");
                    sb.Append(Regex.Escape(lastPattern.Substring(0, lastStrWcLength - 1)));
                    sb.Append(")");

                    sb.Append(@"(\*|");
                    sb.Append(Regex.Escape(lastPattern.Substring(lastStrWcLength - 1,
                        lastPattern.Length - (lastStrWcLength - 1))));
                    sb.Append(")$");
                }
                else
                {
                    sb.Append(@"(\*|");
                    sb.Append(Regex.Escape(lastPattern));
                    sb.Append(")$");
                }
            }

            return Regex.IsMatch(str, sb.ToString());
        }

        /// <summary>
        /// 解析Url询问参数到集合
        /// </summary>
        /// <param name="query">询问的字符串</param>
        /// <returns>解析后的参数集合</returns>
        public static NameValueCollection QueryToCollection(string query)
        {
            var col = new NameValueCollection();

            if (string.IsNullOrEmpty(query))
            {
                return col;
            }

            int l = query.Length;
            int i = 0;
            if (query[0] == '?')
            {
                i = 1;
            }

            while (i < l)
            {
                int si = i;
                int ti = -1;

                while (i < l)
                {
                    char ch = query[i];

                    if (ch == '=')
                    {
                        if (ti < 0)
                        {
                            ti = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }

                    i++;
                }

                string name = null;
                string value = null;

                if (ti >= 0)
                {
                    name = query.Substring(si, ti - si);
                    value = query.Substring(ti + 1, i - ti - 1);
                }
                else
                {
                    value = query.Substring(si, i - si);
                }

                col.Add(name, value);

                i++;
            }

            return col;
        }
        #endregion

        #region Internal Methods
        static StringUtils()
        {
            for (int i = 0; i < lowerCharMapArray.Length; ++i)
            {
                char c = (char)i;
                if (c >= 'A' && c <= 'Z')
                {
                    c = (char)(c + 32);
                }

                lowerCharMapArray[i] = c;
            }
        }

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
        private static char[] lowerCharMapArray = new char[char.MaxValue];
        #endregion
    }
}
