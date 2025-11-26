namespace BattleSystem
{
    /// <summary>
    /// 触发器效果
    /// </summary>
    public class TriggerEffect : BaseEffect
    {
        /// <summary>
        /// 目标触发效果
        /// </summary>
        public long[] selfEffects;
        /// <summary>
        /// 目标触发效果
        /// </summary>
        public long[] targetEffects;

        public override void Trigger()
        {
            author.Effect.AddEffects(selfEffects, authorUid, effectLevel);
            target.Effect.AddEffects(targetEffects, authorUid, effectLevel);
        }
    }
}