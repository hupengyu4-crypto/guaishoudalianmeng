namespace BattleSystem
{
    /// <summary>
    /// 添加状态效果
    /// </summary>
    public class AddStateEffect : BaseEffect
    {
        /// <summary>
        /// 自己触发效果
        /// </summary>
        public long[] selfEffects;
        /// <summary>
        /// 目标触发效果
        /// </summary>
        public long[] targetEffects;

        private void AddEffects()
        {
            if (selfEffects != null && selfEffects.Length > 0)
            {
                author.Effect.AddEffects(selfEffects, authorUid, effectLevel);
            }

            if (targetEffects != null && targetEffects.Length > 0)
            {
                target.Effect.AddEffects(targetEffects, authorUid, effectLevel);
            }
        }

        public override void Trigger()
        {
            AddEffects();
        }

        public override void OnOverlayAdd()
        {
            if (overlayCount < addMax)
            {
                AddEffects();
            }

            base.OnOverlayAdd();
        }
    }
}