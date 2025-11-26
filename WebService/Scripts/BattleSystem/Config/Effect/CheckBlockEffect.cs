#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

namespace BattleSystem
{
    /// <summary>
    /// 检测格挡效果
    /// ps:如果目标是自己，那么监听的是给自己，此时施法者为自己，目标为伤害制造者(修正后)
    /// 如果效果给对手加，那么监听的是对手的格挡次数，此时施法者为自己，目标为伤害制造者(修正后)
    /// </summary>
    public class CheckBlockEffect : BaseEffect
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
        /// 通过给攻击索敌目标的人加效果
        /// </summary>
        public long[] passedAttackTargetEffects;
        /// <summary>
        /// 未通过给自己加效果
        /// </summary>
        public long[] failedSelfEffects;
        /// <summary>
        /// 未通过给目标加效果
        /// </summary>
        public long[] failedTargetEffects;
        /// <summary>
        /// 未通过给攻击目标的人加效果
        /// </summary>
        public long[] failedAttackTargetEffects;
        /// <summary>
        /// 检测格挡次数
        /// </summary>
        public int count;
        /// <summary>
        /// 切换回合是否重置次数统计
        /// </summary>
        public bool clearOnBout;
        /// <summary>
        /// 已格挡次数
        /// </summary>
        private int mBlockCount;

        /// <summary>
        /// 监听者产生格挡的伤害制造者
        /// </summary>
        private Fighter mFighter;

        public override void OnBegin()
        {
            mBlockCount = 0;

            target.AddEvent(BattleObject.Event.Block, OnBlock);

            if (clearOnBout)
            {
                battle.AddEvent(BaseBattle.Event.BoutEnd, OnBlockBoutEnd);
            }

            base.OnBegin();
        }

        private void OnBlockBoutEnd(EventParams eventParams)
        {
            mBlockCount = 0;
        }

        /// <summary>
        /// 暴击时
        /// </summary>
        private void OnBlock(EventParams eventParams)
        {
            mBlockCount++;
            if (eventParams is DamageParams param)
            {
                mFighter = battle.GetSceneObject<Fighter>(param.attackUid); //修正目标，为伤害制造者，而不是效果持有者或施法者自己
            }
            if (count <= 0 || mBlockCount >= count)
            {
#if UNITY_EDITOR
                battle.AddInfo(target.GetBaseDesc()).AddInfo("PropertyEffect-").AddInfo(sid).AddInfo("格挡,[").AddInfo(count).AddInfo("]次，触发效果", true);
#endif
                mBlockCount = 0;
                target.Effect.AddEffects(passedTargetEffects, authorUid, effectLevel);
                mFighter.Effect.AddEffects(passedAttackTargetEffects, authorUid, effectLevel);
                author.Effect.AddEffects(passedSelfEffects, authorUid, effectLevel);
            }
            else
            {
                target.Effect.AddEffects(failedTargetEffects, authorUid, effectLevel);
                mFighter.Effect.AddEffects(failedAttackTargetEffects, authorUid, effectLevel);
                author.Effect.AddEffects(failedSelfEffects, authorUid, effectLevel);
            }

            //触发次数提供
            TriggerBegin();
        }

        public override void Trigger()
        {

        }

        public override void OnEnd()
        {
            target.RemoveEvent(BattleObject.Event.Block, OnBlock);
            if (clearOnBout)
            {
                battle.RemoveEvent(BaseBattle.Event.BoutEnd, OnBlockBoutEnd);
            }
            base.OnEnd();
        }
    }
}