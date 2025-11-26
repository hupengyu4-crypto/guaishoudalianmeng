#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System.Collections.Generic;

namespace BattleSystem
{
    public class CheckFighterEffect : BaseEffect
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
        private long[] mSid;
        /// <summary>
        /// 检测筛选出的种族
        /// </summary>
        private BattleDef.ERaceType mCheckRaceType;
        /// <summary>
        /// 通过给自己的效果
        /// </summary>
        private long[] mPassedSelfEffects;
        /// <summary>
        /// 通过给目标的效果
        /// </summary>
        private long[] mPassedTargetEffects;
        /// <summary>
        /// 通过给施法者的效果
        /// </summary>
        private long[] mPassedAuthorEffects;
        /// <summary>
        /// 目标中是否包含自己
        /// </summary>
        private bool mTargetIncludeSelf;
        /// <summary>
        /// 筛选数量
        /// </summary>
        private int mFiltrateCount;
        /// <summary>
        /// 比较类型
        /// </summary>
        private BattleDef.SimpleCompareType mCompareType;

        public override void OnBegin()
        {
            if (mFiltrateCount > 0)
            {
                battle.AddEvent(BaseBattle.Event.AddSceneObject, Recheck, false, 1);
                battle.AddEvent(BaseBattle.Event.RemoveSceneObject, Recheck, false, 1);
                battle.AddEvent(BaseBattle.Event.AnyDead, OnAnyDead, false, 1);
            }

            base.OnBegin();
        }

        public override void OnEnd()
        {
            if (mFiltrateCount > 0)
            {
                battle.RemoveEvent(BaseBattle.Event.AddSceneObject, Recheck);
                battle.RemoveEvent(BaseBattle.Event.RemoveSceneObject, Recheck);
                battle.RemoveEvent(BaseBattle.Event.AnyDead, OnAnyDead);
            }

            base.OnEnd();
        }

        public override void Trigger()
        {
            Recheck(null);
        }

        private void OnAnyDead(EventParams args)
        {
            if (!(args is EventParams<long> deadParams))
            {
                return;
            }

            var deadFighter = battle.GetSceneObject<Fighter>(deadParams.data);
            if (deadFighter == null || !deadFighter.IsDead) //可能是假死
            {
                return;
            }

            Recheck(null);
        }

        private void Recheck(EventParams _)
        {
            var mFighterList = BattleUtils.GetFighters((Fighter)author, mFindType, mRaceType, mJobType, 0, 1000, null,
                                                       BattleDef.Property.None, BattleDef.OtherType.Null, BattleDef.CompareGradeType.Highest, null);
            var fighterCount = mFighterList.Count;
            if (fighterCount <= 0)
            {
#if UNITY_EDITOR
                Log.info($"检查战斗者目标为空,请检查配置和逻辑,CheckFighterEffect:{sid},FindType:{mFindType},{author.GetBaseDesc()}");
#endif
                return;
            }

            var passed = true;
            do
            {
                var sidLength = mSid != null ? mSid.Length : 0;
                if (sidLength > 0)
                {
                    HashSet<long> fighters = new HashSet<long>();
                    for (int i = 0; i < fighterCount; i++)
                    {
                        var f = mFighterList[i];
                        fighters.Add(f.Data.Sid);
                    }

                    for (int i = 0; i < sidLength; i++)
                    {
                        if (!fighters.Contains(mSid[i]))
                        {
                            passed = false;
                            break;
                        }
                    }
                }

                if (!passed)
                {
                    break;
                }

                if (mCheckRaceType != BattleDef.ERaceType.All)
                {
                    for (int i = 0; i < fighterCount; i++)
                    {
                        var f = mFighterList[i];
                        if (f == null)
                        {
                            continue;
                        }

                        if (f.Data.raceType != mCheckRaceType)
                        {
                            passed = false;
                            break;
                        }
                    }
                }
            }
            while (false);

#if UNITY_EDITOR
            battle.AddInfo("检查战斗者结果(").AddInfo(mFighterList.Count).AddInfo("):");
            foreach (var fighter in mFighterList)
            {
                battle.AddInfo($"{fighter.GetBaseDesc()}、");
            }
            battle.AddInfo("", true);
#endif

            if (passed && mFiltrateCount > 0)
            {
                switch (mCompareType)
                {
                    case BattleDef.SimpleCompareType.Equal:
                        passed = fighterCount == mFiltrateCount;
                        break;
                    case BattleDef.SimpleCompareType.Small:
                        passed = fighterCount < mFiltrateCount;
                        break;
                    case BattleDef.SimpleCompareType.Big:
                        passed = fighterCount > mFiltrateCount;
                        break;
                    case BattleDef.SimpleCompareType.SmallEqual:
                        passed = fighterCount <= mFiltrateCount;
                        break;
                    case BattleDef.SimpleCompareType.BigEqual:
                        passed = fighterCount >= mFiltrateCount;
                        break;
                }
            }

            if (passed)
            {
                target.Effect.AddEffects(mPassedSelfEffects, authorUid, effectLevel);
                for (int i = 0; i < mFighterList.Count; i++)
                {
                    var f = mFighterList[i];
                    if (!mTargetIncludeSelf && f == target)
                    {
                        continue;
                    }

                    f.Effect.AddEffects(mPassedTargetEffects, authorUid, effectLevel);
                }
                if (_additionData!=null && _additionData.specificTarget is Fighter maker)
                {
                    maker.Effect.AddEffects(mPassedAuthorEffects, authorUid, effectLevel);
                }
            }
        }

        public void SetData(CheckFighterEffectConfig cfg)
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
            mSid = cfg.roleSid;
            mCheckRaceType = cfg.checkRaceType;
            mPassedSelfEffects = cfg.passedSelfEffects;
            mPassedTargetEffects = cfg.passedTargetEffects;
            mPassedAuthorEffects = cfg.passedAuthorEffects;
            mTargetIncludeSelf = cfg.targetIncludeSelf;
            mFiltrateCount = cfg.filtrateCount;
            mCompareType = cfg.compareType;
        }
    }
}