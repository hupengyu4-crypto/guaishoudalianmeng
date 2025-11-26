namespace BattleSystem
{
    /// <summary>
    /// 替换技能
    /// </summary>
    public class ReplaceSkillEffect : BaseEffect
    {
        /// <summary>
        /// 目标技能sid
        /// </summary>
        public long targetSid;
        /// <summary>
        /// 替换sid
        /// </summary>
        public long replaceSid;
        /// <summary>
        /// 效果结束是否还原技能
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
            mIsReplaceSuccess = mTarget.Data.ReplaceSkill(targetSid, replaceSid);
        }

        public override void OnEnd()
        {
            if (isRestore && mIsReplaceSuccess)
            {
                mTarget.Data.ReplaceSkill(replaceSid, targetSid);
            }

            mIsReplaceSuccess = false;
            mTarget = null;
            base.OnEnd();
        }
    }
}