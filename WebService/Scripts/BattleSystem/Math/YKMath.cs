using System;

namespace BattleSystem
{
    public static class YKMath
    {
        /// <summary>
        /// 向上进位取整,Ceiling(3.1)=4
        /// </summary>
        public static double Ceiling(double v)
        {
            return Math.Ceiling(v);
        }

        /// <summary>
        /// 向上进位取整,Ceiling(3.1)=4
        /// </summary>
        public static float Ceiling(float v)
        {
            return (float)Math.Ceiling(v);
        }

        /// <summary>
        /// 向下舍位取整,Floor(3.6)=3
        /// </summary>
        public static double Floor(double v)
        {
            return Math.Floor(v);
        }

        /// <summary>
        /// 向下舍位取整,Floor(3.6)=3
        /// </summary>
        public static float Floor(float v)
        {
            return (float)Math.Floor(v);
        }

        public static int Clamp(int v, int min, int max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        public static float Clamp01(float v)
        {
            return v < 0 ? 0 : (v > 1 ? 1 : v);
        }

        public static double Clamp02(double v)
        {
            return v < 0 ? 0 : (v > 1 ? 1 : v);
        }

        public static float Clamp(float v, float min, float max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        public static double Clamp(double v, double min, double max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        public static float Lerp(float from, float to, float t)
        {
            return (from + ((to - from) * Clamp01(t)));
        }

        public static float Repeat(float t, float length)
        {
            return (t - ((float)(Math.Floor(t / length)) * length));
        }

        public static float LerpAngle(float a, float b, float t)
        {
            float num;
            num = Repeat(b - a, 360f);
            if (num <= 180f)
            {
                goto Label_0021;
            }
            num -= 360f;
        Label_0021:
            return (a + (num * Clamp01(t)));
        }

        public static float Distance(float a, float b)
        {
            return Math.Abs(a - b);
        }

        public static float Ln(float a)
        {
            return (float)Math.Log(a, Math.E);
        }

        public static double Ln(double a)
        {
            return Math.Log(a, Math.E);
        }

        public static bool IsRange(float value, float min, float max)
        {
            return value < min ? false : (value > max ? false : true);
        }

        public static YKVector2d ToVector2(this YKVector3d v)
        {
            return new YKVector2d(v.x, v.z);
        }

        public static YKVector2f ToVector2(this YKVector3f v)
        {
            return new YKVector2f(v.x, v.z);
        }

        public static YKVector3d ToVector3(this YKVector2d v, double y = 0)
        {
            return new YKVector3d(v.x, y, v.y);
        }

        public static YKVector3f ToVector3(this YKVector2f v, float y = 0)
        {
            return new YKVector3f(v.x, y, v.y);
        }
    }
}

