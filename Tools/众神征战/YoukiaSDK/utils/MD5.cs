using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoukiaSDKSpace
{
    public class MD5Utils
    {
        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string MD5String(String str)
        {
            if (null == str)
            {
                str = "";
            }

            String word = str;
            StringBuilder pwd = new StringBuilder();
            //System.Security.Cryptography.MD5
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(word));
            for (int i = 0; i < s.Length; i++)
            {
                //pwd = pwd + s[i].ToString("x");
                pwd.Append(s[i].ToString("x2"));
            }
            return pwd.ToString();
        }


    }
}
