using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoukiaSDKSpace
{
    public class UrlEncodeUtil
    {

    
        // Fields

        private static char[] _htmlEntityEndingChars = new char[] { ';', '&' };
        private const char HIGH_SURROGATE_START = '\ud800';
        private const char LOW_SURROGATE_END = '\udfff';
        private const char LOW_SURROGATE_START = '\udc00';
        private const int UNICODE_PLANE00_END = 0xffff;
        private const int UNICODE_PLANE01_START = 0x10000;
        private const int UNICODE_PLANE16_END = 0x10ffff;
        private const int UnicodeReplacementChar = 0xfffd;



        private static void ConvertSmpToUtf16(uint smpChar, out char leadingSurrogate, out char trailingSurrogate)
        {
            int num = ((int)smpChar) - 0x10000;
            leadingSurrogate = (char)((num / 0x400) + 0xd800);
            trailingSurrogate = (char)((num % 0x400) + 0xdc00);
        }

        private static int HexToInt(char h)
        {
            if ((h >= '0') && (h <= '9'))
            {
                return (h - '0');
            }
            if ((h >= 'a') && (h <= 'f'))
            {
                return ((h - 'a') + 10);
            }
            if ((h >= 'A') && (h <= 'F'))
            {
                return ((h - 'A') + 10);
            }
            return -1;
        }





        private static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 0x30);
            }
            return (char)((n - 10) + 0x41);
        }

        private static bool IsUrlSafeChar(char ch)
        {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9')))
            {
                return true;
            }
            switch (ch)
            {
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                case '!':
                    return true;
            }
            return false;
        }

        public static string UrlEncode(string value)
        {
            if (value == null)
            {
                return null;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Encoding.UTF8.GetString(UrlEncode(bytes, 0, bytes.Length, false));
        }
        private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
        {
            if ((bytes == null) && (count == 0))
            {
                return false;
            }
            if (bytes == null)
            {
                //throw new ArgumentNullException("bytes");
            }
            if ((offset < 0) || (offset > bytes.Length))
            {
                //throw new ArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || ((offset + count) > bytes.Length))
            {
                //throw new ArgumentOutOfRangeException("count");
            }
            return true;
        }



        private static byte[] UrlEncode(byte[] bytes, int offset, int count)
        {
            if (!ValidateUrlEncodingParameters(bytes, offset, count))
            {
                return null;
            }
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];
                if (ch == ' ')
                {
                    num++;
                }
                else if (!IsUrlSafeChar(ch))
                {
                    num2++;
                }
            }
            if ((num == 0) && (num2 == 0))
            {
                return bytes;
            }
            byte[] buffer = new byte[count + (num2 * 2)];
            int num4 = 0;
            for (int j = 0; j < count; j++)
            {
                byte num6 = bytes[offset + j];
                char ch2 = (char)num6;
                if (IsUrlSafeChar(ch2))
                {
                    buffer[num4++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num4++] = 0x2b;
                }
                else
                {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte)IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        private static byte[] UrlEncode(byte[] bytes, int offset, int count, bool alwaysCreateNewReturnValue)
        {
            byte[] buffer = UrlEncode(bytes, offset, count);
            if ((alwaysCreateNewReturnValue && (buffer != null)) && (buffer == bytes))
            {
                return (byte[])buffer.Clone();
            }
            return buffer;
        }


        public static byte[] UrlEncodeToBytes(byte[] value, int offset, int count)
        {
            return UrlEncode(value, offset, count, true);
        }



    }
}

