namespace BattleSystem
{
    public class DamageParams : EventParams
    {
        /// <summary>
        /// 触发的效果Sid
        /// </summary>
        public long effectSid;
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
        public long newValue;
        /// <summary>
        /// 旧值
        /// </summary>
        public long oldValue;
        /// <summary>
        /// 是否暴击
        /// </summary>
        public bool isCrit;
        /// <summary>
        /// 是否有格挡
        /// </summary>
        public bool isBlock;
        /// <summary>
        /// 伤害类型
        /// </summary>
        public BattleDef.DamageType damageType;
        /// <summary>
        /// 是否是转移伤害
        /// </summary>
        public bool isTransferDamage;

        /// <summary>
        /// 是否真实伤害
        /// </summary>
        public bool isRealDamage;

        public override void Dispose()
        {
            attackUid = 0;
            defendUid = 0;
            newValue = 0;
            oldValue = 0;
            isCrit = false;
            isRealDamage = false;
            isTransferDamage = false;
            base.Dispose();

        }
    }
}