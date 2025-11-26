using System;

namespace BattleSystem
{
    public struct YKAABB3d
    {
        public YKVector3d Max;
        public YKVector3d Min;

        public YKAABB3d(YKVector3d min, YKVector3d max)
        {
            Max = max;
            Min = min;
        }

        public YKAABB3d Reset()
        {
            Max.Set(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            Min.Set(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);

            return this;
        }

        public YKAABB3d AddPoint(YKVector3d v)
        {
            Max.Set(Math.Max(Max.x, v.x), Math.Max(Max.y, v.y), Math.Max(Max.z, v.z));
            Min.Set(Math.Min(Min.x, v.x), Math.Min(Min.y, v.y), Math.Min(Min.z, v.z));
            return this;
        }

        public YKVector3d Center
        {
            get { return (Max + Min) / 2; }
        }

        public YKVector3d PointClamp(YKVector3d point)
        {
            Clamp(ref point, ref Min, ref Max);
            return point;
        }

        /// <summary>
        /// 这里在用 直接写这里了 后面有需要的可以写成一个静态工具方法 某些效率特别敏感的地方,直接传ref 防止struct copy, 也没有返回值
        /// </summary>
        private void Clamp(ref YKVector3d value, ref YKVector3d min, ref YKVector3d max)
        {
            value.x = YKMath.Clamp(value.x, min.x, max.x);
            value.y = YKMath.Clamp(value.y, min.y, max.y);
            value.z = YKMath.Clamp(value.z, min.z, max.z);
        }
    }
}
