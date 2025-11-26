namespace BattleSystem
{
    /// <summary>
    /// 替换技能的结束效果
    /// </summary>
    public class ReplaceEndEffect : BaseEffect
    {
        /// <summary>
        /// 目标技能sid
        /// </summary>
        public long targetSid;
        /// <summary>
        /// 先给自己的新效果
        /// </summary>
        public long[] newSelfEffects;
        /// <summary>
        /// 给目标的新效果
        /// </summary>
        public long[] newEndEffects;
        /// <summary>
        /// 后给自己的新效果
        /// </summary>
        public long[] newSelfEndEffects;
        /// <summary>
        /// 效果结束是否还原
        /// </summary>
        public bool isRestore;

        /// <summary>
        /// 目标
        /// </summary>
        private Fighter mTarget;
        /// <summary>
        /// 是否替换成功
        /// </summary>
        private bool mIsReplaceSuccess;

        public override void Trigger()
        {
            mTarget = (Fighter)target;
            mIsReplaceSuccess = mTarget.Data.ReplaceSkillEffect(targetSid, newEndEffects, newSelfEffects, newSelfEndEffects);
        }

        public override void OnEnd()
        {
            if (isRestore && mIsReplaceSuccess)
            {
                mTarget.Data.ReductionEffects(targetSid);
            }

            mIsReplaceSuccess = false;
            mTarget = null;
            base.OnEnd();
        }
    }
}