using System;

namespace BattleSystem
{
    public abstract class BaseRandom
    {
        /// <summary>
        /// 随机种子
        /// </summary>
        protected int seed = 0;

        protected BaseRandom()
        {
            seed = (int)((new TimeSpan(DateTime.Now.Ticks).TotalSeconds));
        }

        /// <summary>
        /// 以指定的种子构造随机数生成器
        /// </summary>
        /// <param name="seed">种子</param>
        protected BaseRandom(int seed)
        {
            this.seed = seed;
        }

        /// <summary>
        /// 获得随机种子
        /// </summary>
        /// <returns></returns>
        public long GetSeed()
        {
            return seed;
        }

        /// <summary>
        /// 设置随机种子
        /// </summary>
        /// <param name="seed"></param>
        public void SetSeed(int seed)
        {
            this.seed = seed;
        }

        public abstract int RandomInt();

        public float RandomFloat()
        {
            var r = RandomInt();
            return r / (float)int.MaxValue;
        }

        public double RandomDouble()
        {
            var r = RandomInt();
            return r / (double)int.MaxValue;
        }

        /// <summary>
        /// 获得指定范围的随机整数
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public int RandomValue(int v1, int v2)
        {
            if (v2 > v1)
            {
                v2 += 1;
                return RandomInt() % (v2 - v1) + v1;
            }
            if (v1 > v2)
            {
                v1 += 1;
                return RandomInt() % (v1 - v2) + v2;
            }

            return v1;
        }

        /// <summary>
        /// 获得指定范围的随机浮点数
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float RandomValue(float v1, float v2)
        {
            if (v2 > v1)
                return RandomFloat() * (v2 - v1) + v1;
            if (v1 > v2)
                return RandomFloat() * (v1 - v2) + v2;
            return v1;
        }

        /// <summary>
        /// 获得指定范围的随机double数
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double RandomValue(double v1, double v2)
        {
            if (v2 > v1)
                return RandomDouble() * (v2 - v1) + v1;
            if (v1 > v2)
                return RandomDouble() * (v1 - v2) + v2;
            return v1;
        }
    }
}

