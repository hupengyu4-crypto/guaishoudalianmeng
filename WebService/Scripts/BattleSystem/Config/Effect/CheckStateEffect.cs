namespace BattleSystem
{
    /// <summary>
    /// 检测状态效果
    /// </summary>
    public class CheckStateEffect : BaseEffect
    {
        /// <summary>
        /// 通过给自己加效果
        /// </summary>
        public long[] passedSelfEffects;
        /// <summary>
        /// 通过给目标加效果
        /// </summary>
        public long[] passedTargetEffects;
        /// <summary>
        /// 未通过给自己加效果
        /// </summary>
        public long[] failedSelfEffects;
        /// <summary>
        /// 未通过给目标加效果
        /// </summary>
        public long[] failedTargetEffects;
        /// <summary>
        /// 检查状态
        /// </summary>
        public BattleObject.State[] checkState;

        public override void Trigger()
        {
            var caster = battle.GetSceneObject<Fighter>(authorUid);
            if (caster == null)
                return;

            var toCheckState = BattleObject.State.Normal;
            if (checkState.Length > 0)
            {
                for (int i = 0, l = checkState.Length; i < l; i++)
                {
                    toCheckState |= checkState[i];
                }
            }
            if (target.IsState(toCheckState))
            {
                caster.Effect?.AddEffects(passedSelfEffects, authorUid, effectLevel);
                target.Effect?.AddEffects(passedTargetEffects, authorUid, effectLevel);
            }
            else
            {
                caster.Effect?.AddEffects(failedSelfEffects, authorUid, effectLevel);
                target.Effect?.AddEffects(failedTargetEffects, authorUid, effectLevel);
            }
        }
    }
}