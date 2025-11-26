namespace BattleSystem
{
    /// <summary>
    /// 属性改变事件
    /// </summary>
    public class PropParams : EventParams
    {
        /// <summary>
        /// 新的值
        /// </summary>
        public long newValue;
        /// <summary>
        /// 谁修改的属性
        /// </summary>
        public long authorUid;
        /// <summary>
        /// 属性
        /// </summary>
        public BattleDef.Property property;
        /// <summary>
        /// 之前的值
        /// </summary>
        public long dataValue;
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
        /// 是否显示
        /// </summary>
        public bool isShow = true;

        public override void Dispose()
        {
            newValue = 0;
            authorUid = 0;
            property = BattleDef.Property.None;
            dataValue = 0;
            isCrit = false;
            isBlock = false;
            isShow = true;
            base.Dispose();
        }
    }
}