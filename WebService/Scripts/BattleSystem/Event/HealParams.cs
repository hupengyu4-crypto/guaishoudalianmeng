namespace BattleSystem
{
    public class HealParams : EventParams
    {
        /// <summary>
        /// 施法者Uid
        /// </summary>
        public long casterkUid;
        /// <summary>
        /// 目标Uid
        /// </summary>
        public long targetUid;
        /// <summary>
        /// 新值
        /// </summary>
        public long newValue;
        /// <summary>
        /// 旧值
        /// </summary>
        public long oldValue;
        /// <summary>
        /// 是否暴击
        /// </summary>
        public bool isCrit;

        public override void Dispose()
        {
            casterkUid = 0;
            targetUid = 0;
            newValue = 0;
            oldValue = 0;
            isCrit = false;
            base.Dispose();

        }
    }
}