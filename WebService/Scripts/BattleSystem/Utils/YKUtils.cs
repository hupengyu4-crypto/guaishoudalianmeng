using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BattleSystem
{
    /// <summary>
    /// YoukiaUnity的工具类
    /// </summary>
    public static class YKUtils
    {
        /// <summary>
        /// 字符串转换枚举
        /// </summary>
        public static T EnumParse<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        /// <summary>
        /// 枚举转字符串
        /// </summary>
        public static string StringEnumParse<T>(object value)
        {
            return Enum.GetName(typeof(T), value);
        }

        /// <summary>
        /// int转换枚举
        /// </summary>
        public static T EnumParse<T>(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
        public static bool FlagOn<T>(this T t, T flag) { return (Convert.ToInt32(t) & Convert.ToInt32(flag)) == Convert.ToInt32(flag); }
        public static int Modulo(this int x, int m, int add = 0) { return ((x + add) % m + m) % m; }


        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">容器</param>
        /// <param name="func">判断方法</param>
        public static void BubbleSort<T>(this List<T> list, System.Func<T, T, bool> func)
        {
            T temp = default(T);
            for (int i = list.Count; i > 0; i--)
            {
                for (int j = 0; j < i - 1; j++)
                {
                    if (func(list[j], list[j + 1]))
                    {
                        temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }
        }

        public static void BubbleSort<T>(this T[] array, Comparison<T> comparison)
        {
            T temp;//存储临时变量
            for (int i = 0; i < array.Length; i++)
                for (int j = i - 1; j >= 0; j--)
                    //if (intArray[j + 1] < intArray[j])
                    if (comparison(array[j + 1], array[j]) < 0)
                    {
                        temp = array[j + 1];
                        array[j + 1] = array[j];
                        array[j] = temp;
                    }
        }

        public static void BubbleSort<T>(this List<T> array, Comparison<T> comparison)
        {
            T temp;//存储临时变量
            for (int i = 0; i < array.Count; i++)
                for (int j = i - 1; j >= 0; j--)
                    //if (intArray[j + 1] < intArray[j])
                    if (comparison(array[j + 1], array[j]) < 0)
                    {
                        temp = array[j + 1];
                        array[j + 1] = array[j];
                        array[j] = temp;
                    }
        }

        public static uint CombineMask<T>(T[] enums) where T : IConvertible
        {
            uint flag = 0;
            for (int i = 0; i < enums.Length; i++)
            {
                flag |= enums[i].ToUInt32(null);
            }
            return flag;
        }

        public static T[] MaskToEnums<T>(this uint mask) where T : IConvertible
        {
            Array enums = Enum.GetValues(typeof(T));
            List<T> res = new List<T>();
            for (int i = 0; i < enums.Length; i++)
            {
                uint frag = (uint)enums.GetValue(i);
                if ((frag & mask) == frag)
                {
                    res.Add((T)enums.GetValue(i));
                }
            }
            return res.ToArray();
        }

        #region prop format, prop can is 100 or 100%

        public static float PropFormat(string v, float nowV, float baseV, float percentage = 0.01f)
        {
            bool reduce = v.Contains("-");
            bool add = v.Contains("+");
            if (reduce)
                v = v.Replace("-", "");
            else if (add)
                v = v.Replace("+", "");

            float value = 0;
            if (v.IndexOf("%") == -1)
            {
                value = float.Parse(v);
            }
            else if (v.IndexOf("%%") == -1)
            {
                value = (float)Math.Round(float.Parse(v.Replace("%%", "")) * percentage * baseV);
            }
            else
            {
                value = (float)Math.Round(float.Parse(v.Replace("%", "")) * percentage * nowV);
            }
            if (reduce)
                value = nowV - value;
            if (add)
                value = nowV + value;

            return value;
        }

        public static float PropFormat(string prop)
        {
            float value = 0;
            if (prop.IndexOf("%") == -1)
            {
                value = float.Parse(prop);
            }
            else
            {
                value = float.Parse(prop.Replace("%", ""));
            }
            return value;
        }

        public static string PropAdd(string a1, string a2)
        {
            if (a1.Substring(a1.Length - 2, 1) == "%")
            {
                return (float.Parse(a1.Replace("%", "")) + float.Parse(a2.Replace("%", ""))).ToString() + "%";
            }
            return (float.Parse(a1) + float.Parse(a2)).ToString();
        }

        #endregion

        #region 随机打乱list

        /// <summary>
        /// 随即打乱列表
        /// </summary>
        public static void RamdomSort<T>(this List<T> arr)
        {
            System.Random ran = new System.Random();
            int k = 0;
            T strtmp;
            for (int i = 0; i < arr.Count; i++)
            {
                k = ran.Next(0, arr.Count - 1);
                if (k != i)
                {
                    strtmp = arr[i];
                    arr[i] = arr[k];
                    arr[k] = strtmp;
                }
            }
        }

        public static void RamdomSort<T>(this List<T> arr, YKRandom random)
        {
            int k;
            T strtmp;
            for (int i = 0; i < arr.Count; i++)
            {
                k = (int)random.RandomValue(0, arr.Count - 1);
                if (k != i)
                {
                    strtmp = arr[i];
                    arr[i] = arr[k];
                    arr[k] = strtmp;
                }
            }
        }

        /// <summary>
        /// 随即打乱数组
        /// </summary>
        public static void RamdomSort<T>(this T[] arr)
        {
            System.Random ran = new System.Random();
            int k = 0;
            T strtmp;
            for (int i = 0; i < arr.Length; i++)
            {
                k = ran.Next(0, arr.Length - 1);
                if (k != i)
                {
                    strtmp = arr[i];
                    arr[i] = arr[k];
                    arr[k] = strtmp;
                }
            }
        }

        public static void RamdomSort<T>(this T[] arr, YKRandom random)
        {
            long k = 0;
            T strtmp;
            for (int i = 0; i < arr.Length; i++)
            {
                k = (int)random.RandomValue(0, arr.Length - 1);
                if (k != i)
                {
                    strtmp = arr[i];
                    arr[i] = arr[k];
                    arr[k] = strtmp;
                }
            }
        }

        #endregion

        public static string ToHexString(this byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append($"{bytes[i]:X2} ");
            }
            return sb.ToString();
        }

        #region string扩展
        public static byte[] FromHexString(this string str)
        {
            string[] byteStrings = str.Split(" ".ToCharArray());
            byte[] byteOut = new byte[byteStrings.Length - 1];
            for (int i = 0; i < byteStrings.Length - 1; i++)
            {
                byteOut[i] = byte.Parse(byteStrings[i], NumberStyles.HexNumber);
            }
            return byteOut;
        }

        public const float Epsilon = 0.0001f;

        /// <summary>
        /// 可控的误差比较
        /// </summary>
        /// <param name="f1">浮点源1</param>
        /// <param name="f2">浮点源2</param>
        /// <param name="epsilon">公差范围</param>
        /// <returns>是否在误差内</returns>
        public static bool FloatEquals(this float f1, float f2, float epsilon = float.Epsilon)
        {
            return System.Math.Abs(f1 - f2) < epsilon;
        }


        public static bool toBool(this string str)
        {
            if (!str.valid()) return false;
            else if (str.ToLower() == "true") return true;
            else return false;
        }

        public static int ToInt(this string str)
        {
            int result = 0;
            bool negitave = false;
            if (string.IsNullOrEmpty(str)) return 0;
            int i = 0;
            for (; i < str.Length && !char.IsDigit(str[i]); i++)
                ;
            if (i > 0 && str[i - 1] == '-') negitave = true;
            for (; i < str.Length && char.IsDigit(str[i]); i++)
                result = 10 * result + (str[i] - 48);
            return negitave ? -result : result;
        }
        public static int[] ToIntArray(this string str, char symbol)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new int[0];
            }
            string[] strArray = str.Split(symbol);
            if (strArray.Length < 1)
            {
                return new int[0];
            }
            int[] value = new int[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                value[i] = ToInt(strArray[i]);
            }
            return value;
        }
        public static uint ToUInt(this string str)
        {
            uint result = 0;
            if (string.IsNullOrEmpty(str)) return 0;
            int i = 0;
            for (; i < str.Length && !char.IsDigit(str[i]); i++)
                ;
            for (; i < str.Length && char.IsDigit(str[i]); i++)
                result = 10 * result + (uint)(str[i] - 48);
            return result;
        }
        public static byte ToByte(this string str)
        {
            byte rt;
            byte.TryParse(str, out rt);
            return rt;
        }
        public static short ToShort(this string str)
        {
            short rt;
            short.TryParse(str, out rt);
            return rt;
        }
        public static float ToFloat(this string str)
        {
            float rt;
            float.TryParse(str, out rt);
            return rt;
        }
        public static float[] ToFloatArray(this string str, char symbol)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new float[0];
            }
            string[] strArray = str.Split(symbol);
            if (strArray.Length < 1)
            {
                return new float[0];
            }
            float[] value = new float[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                value[i] = ToFloat(strArray[i]);
            }
            return value;
        }
        public static double ToDouble(this string str)
        {
            double rt;
            double.TryParse(str, out rt);
            return rt;
        }
        public static double[] ToDoubleArray(this string str, char symbol)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new double[0];
            }
            string[] strArray = str.Split(symbol);
            if (strArray.Length < 1)
            {
                return new double[0];
            }
            double[] value = new double[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                value[i] = ToDouble(strArray[i]);
            }
            return value;
        }
        public static long ToLong(this string str)
        {
            long result = 0;
            bool negitave = false;
            if (string.IsNullOrEmpty(str)) return 0;
            int i = 0;
            for (; i < str.Length && !char.IsDigit(str[i]); i++)
                ;
            if (i > 0 && str[i - 1] == '-') negitave = true;
            for (; i < str.Length && char.IsDigit(str[i]); i++)
                result = 10 * result + (str[i] - 48);
            return negitave ? -result : result;
        }
        public static long[] ToLongArray(this string str, char symbol)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new long[0];
            }
            string[] strArray = str.Split(symbol);
            if (strArray.Length < 1)
            {
                return new long[0];
            }
            long[] value = new long[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                value[i] = ToLong(strArray[i]);
            }
            return value;
        }
        /// <summary>
        /// 判断字符串是否 不为 null 或者 空 或者 不等于指定值
        /// </summary>
        public static bool valid(this string str, string not_equ = "")
        {
            return !string.IsNullOrEmpty(str) && str != not_equ;
        }

        #endregion

        #region Array扩展
        /// <summary>
        /// 克隆一个数组
        /// </summary>
        public static T[] Clone<T>(this T[] Array) where T : class
        {
            if (Array == null) return null;

            int i, len = Array.Length;
            T[] newArray = new T[len];

            for (i = 0; i < len; i++)
            {
                newArray[i] = Array[i];
            }

            return newArray;
        }

        public static T[] Add<T>(this T[] Array, T add) where T : class
        {
            if (Array == null) Array = new T[0];
            int len = Array.Length;
            T[] nList = new T[len + 1];
            for (int i = 0; i < len; i++)
            {
                nList[i] = Array[i];
            }
            nList[len] = add;
            return nList;
        }

        public static bool Contains<T>(this T[] Array, T key) where T : class
        {
            if (Array != null)
            {
                for (int i = 0; i < Array.Length; i++)
                {
                    if (key == Array[i]) return true;
                }
            }
            return false;
        }

        public static int IndexOf<T>(this T[] Array, T key) where T : class
        {
            if (Array != null)
            {
                for (int i = 0; i < Array.Length; i++)
                {
                    if (key == Array[i]) return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 判断不为空，并且长度大于0
        /// </summary>
        public static bool valid<T>(this T[] array)
        {
            return array != null && array.Length > 0;
        }
        /// <summary>
        /// 二分查找
        /// </summary>
        public static int indexOf<T>(this T[] array, Predicate<T> compare)
        {
            if (array == null || array.Length == 0) return -1;
            for (int i = 0; i < array.Length / 2 + 1; i++)
            {
                if (compare(array[i])) return i;
                if (compare(array[array.Length - i - 1])) return array.Length - i - 1;
            }
            return -1;
        }
        public static T get<T>(this object[] args, int index)
        {
            if (args == null || args.Length <= index) return default(T);
            if (args[index].GetType() == typeof(T)) return (T)args[index];
            return default(T);
        }
        public static T[] Concat<T>(this T[] v, T[] arr)
        {
            int oldLen = v.Length;
            Array.Resize(ref v, v.Length + arr.Length);
            for (int i = oldLen; i < v.Length; i++)
            {
                v[i] = arr[i - oldLen];
            }
            return v;
        }
        public static T[] ShallowClone<T>(this T[] v)
        {
            if (v == null)
                return v;
            T[] rt = new T[v.Length];
            v.CopyTo(rt, 0);
            return rt;
        }
        public static T[][] ShallowClone<T>(this T[][] v)
        {
            if (v == null)
                return v;
            T[][] rt = new T[v.Length][];
            for (int i = 0; i < v.Length; i++)
            {
                rt[i] = ShallowClone(v[i]);
            }
            return rt;
        }
        public static T[,] ShallowClone<T>(this T[,] v)
        {
            if (v == null)
                return v;
            T[,] rt = new T[v.GetLength(0), v.GetLength(1)];
            for (int i = 0; i < rt.GetLength(0); i++)
                for (int j = 0; j < rt.GetLength(1); j++)
                    rt[i, j] = v[i, j];
            return rt;
        }

        public static void sort<T>(this T[] array, Comparison<T> comparison)
        {
            array.sort(0, array.Length - 1, comparison);
        }
        /// <summary>
        /// 直接反编译c#的复制过来的
        /// </summary>
        public static void sort<T>(this T[] array, int low0, int high0, Comparison<T> comparison)
        {
            int num;
            int num2;
            int num3;
            T local;
            if (low0 < high0)
            {
                goto Label_0008;
            }
            return;
        Label_0008:
            num = low0;
            num2 = high0;
            num3 = num + ((num2 - num) / 2);
            local = array[num3];
        Label_001C:
            goto Label_0025;
        Label_0021:
            num += 1;
        Label_0025:
            if (num >= high0)
            {
                goto Label_0049;
            }
            if (comparison(array[num], local) < 0)
            {
                goto Label_0021;
            }
            goto Label_0049;
        Label_0045:
            num2 -= 1;
        Label_0049:
            if (num2 <= low0)
            {
                goto Label_0064;
            }
            if (comparison(local, array[num2]) < 0)
            {
                goto Label_0045;
            }
        Label_0064:
            if (num > num2)
            {
                goto Label_008A;
            }
            swap<T>(array, num, num2);
            num += 1;
            num2 -= 1;
            goto Label_0085;
        Label_0085:
            goto Label_001C;
        Label_008A:
            if (low0 >= num2)
            {
                goto Label_009A;
            }
            sort<T>(array, low0, num2, comparison);
        Label_009A:
            if (num >= high0)
            {
                goto Label_00AA;
            }
            sort<T>(array, num, high0, comparison);
        Label_00AA:
            return;

        }

        public static void swap<T>(this T[] array, int i, int j)
        {
            T local = array[i];
            array[i] = array[j];
            array[j] = local;
        }

        public static bool IsSame<T>(this T[] v, T[] target) where T : class
        {
            if (target == null || v.Length != target.Length)
                return false;
            for (int i = 0; i < v.Length; i++)
            {
                if (v[i] != target[i])
                    return false;
            }
            return true;
        }
        #endregion

        #region List扩展
        public static void SafeSort<T>(this List<T> array, Comparison<T> comparison)
        {
            array.SafeSort(0, array.Count - 1, comparison);
        }

        public static void SafeSort<T>(this List<T> array, int low0, int high0, Comparison<T> comparison)
        {
            int num;
            int num2;
            int num3;
            T local;
            if (low0 < high0)
            {
                goto Label_0008;
            }
            return;
        Label_0008:
            num = low0;
            num2 = high0;
            num3 = num + ((num2 - num) / 2);
            local = array[num3];
        Label_001C:
            goto Label_0025;
        Label_0021:
            num += 1;
        Label_0025:
            if (num >= high0)
            {
                goto Label_0049;
            }
            if (comparison(array[num], local) < 0)
            {
                goto Label_0021;
            }
            goto Label_0049;
        Label_0045:
            num2 -= 1;
        Label_0049:
            if (num2 <= low0)
            {
                goto Label_0064;
            }
            if (comparison(local, array[num2]) < 0)
            {
                goto Label_0045;
            }
        Label_0064:
            if (num > num2)
            {
                goto Label_008A;
            }
            swap<T>(array, num, num2);
            num += 1;
            num2 -= 1;
            goto Label_0085;
        Label_0085:
            goto Label_001C;
        Label_008A:
            if (low0 >= num2)
            {
                goto Label_009A;
            }
            SafeSort<T>(array, low0, num2, comparison);
        Label_009A:
            if (num >= high0)
            {
                goto Label_00AA;
            }
            SafeSort<T>(array, num, high0, comparison);
        Label_00AA:
            return;
        }

        public static void swap<T>(this List<T> array, int i, int j)
        {
            T local;
            local = array[i];
            array[i] = array[j];
            array[j] = local;
            return;
        }

        public static string ToString<T>(this List<T> v, char split)
        {
            string str = "";
            for (int i = 0; i < v.Count; i++)
            {
                str += v[i].ToString() + split.ToString();
            }
            if (v.Count > 0)
                str = str.Substring(0, str.Length - 1);
            return str;
        }
        /// <summary>
        /// 判断不为空，并且长度大于0
        /// </summary>
        public static bool valid<T>(this List<T> list)
        {
            return list != null && list.Count > 0;
        }
        #endregion
    }
}
