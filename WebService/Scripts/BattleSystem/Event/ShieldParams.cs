namespace BattleSystem
{
    public class ShieldAbsorptionParams : EventParams
    {
        /// <summary>
        /// 进攻方Uid
        /// </summary>
        public long attackUid;
        /// <summary>
        /// 防守方Uid
        /// </summary>
        public long defendUid;
        /// <summary>
        /// 新值
        /// </summary>
        public long shieldAbsorptionValue;

        public override void Dispose()
        {
            attackUid = 0;
            defendUid = 0;
            shieldAbsorptionValue = 0;
            base.Dispose();

        }
    }
}