#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System.Collections.Generic;

namespace BattleSystem
{
    public class FindTargetEffect : BaseEffect
    {
        /// <summary>
        /// 查找范围
        /// </summary>
        private BattleDef.EFindType mFindType;
        /// <summary>
        /// 种族选择
        /// </summary>
        private BattleDef.ERaceType[] mRaceType;
        /// <summary>
        /// 职业选择
        /// </summary>
        private BattleDef.EJobType[] mJobType;
        /// <summary>
        /// 角色Sid 如果指定角色 BattleDef.EFindType 只能我方全体 或者敌方全体
        /// </summary>
        private long mSid;
        /// <summary>
        /// 命中数量
        /// </summary>
        private int mCount;
        /// <summary>
        /// 状态选择
        /// </summary>
        private BattleObject.State[] mCheckState;
        /// <summary>
        /// 属性选择
        /// </summary>
        private BattleDef.Property mProperty;
        /// <summary>
        /// 其他类型
        /// </summary>
        private BattleDef.OtherType mOtherType;
        /// <summary>
        /// 比较类型
        /// </summary>
        private BattleDef.CompareGradeType mCompareType;
        /// <summary>
        /// 其他类型的参数
        /// </summary>
        private long[] mOtherArgs;
        /// <summary>
        /// 索敌结果
        /// </summary>
        private List<Fighter> mFighterList;
        /// <summary>
        /// 不换排搜索
        /// </summary>
        private bool mNotSwitchRow;

        public override void Trigger()
        {
            mFighterList = BattleUtils.GetFighters((Fighter)author, mFindType, mRaceType, mJobType, mSid, mCount, mCheckState, mProperty, mOtherType,
                                                   mCompareType, mOtherArgs, mNotSwitchRow);

#if UNITY_EDITOR
            if (mFighterList.Count == 0)
            {
                Log.info($"筛选技能目标为空,请检查配置和逻辑,FindTargetEffect:{sid},FindType:{mFindType},{author.GetBaseDesc()}");
            }
            battle.AddInfo("索敌结果(").AddInfo(mFighterList.Count).AddInfo("):");
            foreach (var fighter in mFighterList)
            {
                battle.AddInfo($"{fighter.GetBaseDesc()}、");
            }
            battle.AddInfo("", true);
#endif

            for (int i = 0; i < mFighterList.Count; i++)
            {
                mFighterList[i].Effect.AddEffects(endEffects, authorUid, effectLevel);
            }
        }

        /// <summary>
        /// 获取索敌列表
        /// </summary>
        /// <returns></returns>
        public List<Fighter> GetFighters()
        {
            return mFighterList;
        }

        public override void OnEnd()
        {
            //不执行父类方法，因为目标已经改变
            //OnEndRemoveEvent();
        }

        public void SetData(FindTargetEffectConfig cfg)
        {
            mFindType = cfg.findType;
            if (cfg.raceTypes != null)
            {
                if (cfg.raceTypes.Length == 0)
                {
                    mRaceType = new BattleDef.ERaceType[1] { BattleDef.ERaceType.All };
                }
                else
                {
                    mRaceType = cfg.raceTypes;
                }
            }
            else
            {
                mRaceType = new BattleDef.ERaceType[1] { BattleDef.ERaceType.All };
            }
            if (cfg.jobTypes != null)
            {
                if (cfg.jobTypes.Length == 0)
                {
                    mJobType = new BattleDef.EJobType[1] { BattleDef.EJobType.All };
                }
                else
                {
                    mJobType = cfg.jobTypes;
                }
            }
            else
            {
                mJobType = new BattleDef.EJobType[1] { BattleDef.EJobType.All };
            }
            mCount = cfg.count;
            mCheckState = cfg.checkState;
            mProperty = cfg.property;
            mOtherType = cfg.otherType;
            mCompareType = cfg.compareType;
            mOtherArgs = cfg.otherArgs; 
            mNotSwitchRow = cfg.notSwitchRow;
            mSid = cfg.roleSid; 
        }
    }
}