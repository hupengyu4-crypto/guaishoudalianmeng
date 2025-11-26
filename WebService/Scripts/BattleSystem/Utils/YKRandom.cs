namespace BattleSystem
{
    public class YKRandom : BaseRandom
    {
        private const int A = 16807;
        private const int M = 2147483647;
        private const int Q = 127773;
        private const int R = 2836;
        private const int MASK = 123459876;

        public YKRandom() : base()
        {
        }

        public YKRandom(int seed) : base(seed)
        {
        }


        public override int RandomInt()
        {
            var r = seed;
            // 防止种子为0
            r ^= MASK;
            var k = r / Q;
            r = A * (r - k * Q) - R * k;
            if (r < 0) r += M;
            seed = r;
            return r;
        }
    }
}

