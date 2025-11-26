namespace BattleSystem
{
    /// <summary>
    /// 将施法者切换为效果持有者
    /// </summary>
    public class SwitchTargetToAuthorEffect : BaseEffect
    {
        /// <summary>
        /// 传递施法者
        /// </summary>
        public bool keepAuthor;
        /// <summary>
        /// 切换后施加效果
        /// </summary>
        public long[] addEffects;
        public override void Trigger()
        {
            BaseBattle.EffectAdditionData newAdditionData = null;
            if (keepAuthor)
            {
                newAdditionData = AllocAdditionData(newAdditionData);
                newAdditionData.specificTarget = author;
            }
            author = target;
            authorUid = target.Uid;

            target.Effect.AddEffects(addEffects, authorUid, effectLevel, newAdditionData);
        }

        public void SetData(SwitchTargetToAuthorEffectConfig cfg)
        {
            keepAuthor = cfg.keepAuthor;
            addEffects = cfg.addEffects;
        }
    }
}