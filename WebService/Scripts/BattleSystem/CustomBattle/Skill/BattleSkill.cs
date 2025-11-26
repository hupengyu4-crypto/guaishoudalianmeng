#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;
using System.Collections.Generic;
using RootScript.Config;

namespace BattleSystem
{
    public class BattleSkill
    {
        /// <summary>
        /// 技能拥有者
        /// </summary>
        public Fighter Owner { get; private set; }
        /// <summary>
        /// 配置表
        /// </summary>
        public BattleSkillCfg Cfg { get; private set; }
        /// <summary>
        /// 技能Sid
        /// </summary>
        public long Sid { get; private set; }
        /// <summary>
        /// 消耗的怒气
        /// </summary>
        public int Rage { get; private set; }
        /// <summary>
        /// 技能类型
        /// </summary>
        public BattleDef.ESkillType SkillType { get; private set; }
        /// <summary>
        /// 先给自己的效果
        /// </summary>
        public BaseEffectCfg[] SelfEffects { get; private set; }
        /// <summary>
        /// 给目标的效果
        /// </summary>
        public BaseEffectCfg[] TargetEffects { get; private set; }
        /// <summary>
        /// 后给自己的效果
        /// </summary>
        public BaseEffectCfg[] SelfEndEffects { get; private set; }
        /// <summary>
        /// 查找范围
        /// </summary>
        public BattleDef.EFindType FindType { get; private set; }
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
        /// 技能等级
        /// </summary>
        private int mSkillLevel;
        public int skillLevel => mSkillLevel;
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


        public BattleSkill(long sid, int level, Fighter owner)
        {
            this.Sid = sid;
            mSkillLevel = level;
            if (mSkillLevel < 1)
            {
                mSkillLevel = 1;
            }
            Owner = owner;
            LoadData();
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="specificTargetUid"></param>
        public void Do(long specificTargetUid = 0)
        {
            if (Owner.Battle.IsBattleOverTag || Owner.Battle.IsBattleOver)
            {
#if UNITY_EDITOR
                Owner.Battle.AddInfo("战斗结束，不释放技能:").AddInfo(Sid).AddInfo("，施法者:").AddInfo(Owner.GetBaseDesc(), true);
#endif
                return;
            }

            if (SkillType == BattleDef.ESkillType.Normal && Owner.IsState(BattleObject.State.Disarm))
            {
#if UNITY_EDITOR
                Owner.Battle.AddInfo("缴械中，不能释放普攻:").AddInfo(Sid).AddInfo("，施法者:").AddInfo(Owner.GetBaseDesc(), true);
#endif
                return;
            }

            if (SkillType == BattleDef.ESkillType.Rage && Owner.IsState(BattleObject.State.Silence))
            {
#if UNITY_EDITOR
                Owner.Battle.AddInfo("沉默中，不能释放怒气技能:").AddInfo(Sid).AddInfo("，施法者:").AddInfo(Owner.GetBaseDesc(), true);
#endif
                return;
            }

            var alreadyFindTarget = false;
            if (specificTargetUid > 0)
            {
                var specificTarget = Owner.Battle.GetSceneObject<Fighter>(specificTargetUid);
                if (specificTarget != null && !specificTarget.IsDead && !specificTarget.IsFakeDead)
                {
                    Owner.CacheTargets = new()
                    {
                        specificTarget
                    };
                }
                else
                {
                    if (Owner.CacheTargets != null)
                    {
                        Owner.CacheTargets.Clear();
                    }
                    else
                    {
                        Owner.CacheTargets = new();
                    }
                }

                alreadyFindTarget = true;
            }

            if (!alreadyFindTarget && SkillType == BattleDef.ESkillType.Normal)
            {
                var tauntUid = Owner.GetTauntedUid();
                if (tauntUid > 0)
                {
                    var taunter = Owner.Battle.GetSceneObject<Fighter>(tauntUid);
                    if (taunter != null && !taunter.IsDead && !taunter.IsFakeDead)
                    {
                        alreadyFindTarget = true;
#if UNITY_EDITOR
                        Owner.Battle.AddInfo("被嘲讽中，攻击目标:").AddInfo(taunter.GetBaseDesc(), true);
#endif
                        Owner.CacheTargets = new List<Fighter>()
                        {
                            taunter
                        };
                        Owner.DispatchEvent(Owner.Battle, BattleObject.Event.AttackTaunter, EventParams<long>.Create(Owner.Battle, tauntUid));
                        taunter.DispatchEvent(taunter.Battle, BattleObject.Event.TaunterBeAttacked, EventParams<long>.Create(taunter.Battle, Owner.Uid));
                    }
                }
            }

            if (!alreadyFindTarget)
            {
                Owner.CacheTargets = BattleUtils.GetFighters(Owner, FindType, mRaceType, mJobType, mSid, mCount, mCheckState, mProperty, mOtherType,
                                                             mCompareType, mOtherArgs);
            }

            if (Owner.CacheTargets.Count == 0)
            {
#if UNITY_EDITOR
                Log.info($"筛选技能目标为空,请检查配置和逻辑,BattleSkill:{Sid},FindType:{FindType},{Owner.GetBaseDesc()}");
#endif
                if ((SelfEffects == null || SelfEffects.Length == 0))
                {
#if UNITY_EDITOR
                    Owner.Battle.AddInfo("技能索敌失败，不释放技能:").AddInfo(Sid).AddInfo("，施法者:").AddInfo(Owner.GetBaseDesc(), true);
#endif
                    return;
                }
            }

            List<Fighter> removedTargets = null;
            if (SkillType == BattleDef.ESkillType.Rage)
            {
                for (int i = Owner.CacheTargets.Count - 1; i >= 0; i--)
                {
                    var target = Owner.CacheTargets[i];
                    if (target.IsState(BattleObject.State.CannotSelectByRageSkill))
                    {
                        Owner.CacheTargets.RemoveAt(i);

                        if (removedTargets == null)
                        {
                            removedTargets = new();
                        }
                        removedTargets.Add(target);
                    }
                }
            }

            if (SkillType == BattleDef.ESkillType.Normal)
            {
                //普攻加怒气
                Owner.AddProp(BattleDef.Property.anger, BattleDef.AttackAddRage);
                Owner.DispatchEvent(Owner.Battle, BattleObject.Event.DoNormalSkill, EventParams<BattleSkill>.Create(Owner.Battle, this));
            }
            else if (SkillType == BattleDef.ESkillType.Rage)
            {
                Owner.statisticsData.skillNum++;
                //先扣后放，避免使用后又加，超出最大值无法增加
                Owner.AddProp(BattleDef.Property.anger, -Cfg.rage);
                Owner.DispatchEvent(Owner.Battle, BattleObject.Event.DoRageSkill, EventParams<BattleSkill>.Create(Owner.Battle, this));
            }
            Owner.DispatchEvent(Owner.Battle, BattleObject.Event.DoSkill, EventParams<BattleSkill>.Create(Owner.Battle, this));

            Owner.Effect.AddEffects(SelfEffects, Owner.Uid, mSkillLevel);

            for (int i = 0; i < Owner.CacheTargets.Count; i++)
            {
                var target = Owner.CacheTargets[i];
                target.Effect.AddEffects(TargetEffects, Owner.Uid, mSkillLevel);
            }

            Owner.Effect.AddEffects(SelfEndEffects, Owner.Uid, mSkillLevel);

            if (SkillType == BattleDef.ESkillType.Normal || SkillType == BattleDef.ESkillType.Rage)
            {
                var beSkillAttackEndEvent = SkillType == BattleDef.ESkillType.Normal ? BattleObject.Event.BeNormalSkillAttackEnd :
                                                BattleObject.Event.BeRageSkillAttackEnd;
                for (int i = 0; i < Owner.CacheTargets.Count; i++)
                {
                    var target = Owner.CacheTargets[i];
                    target.DispatchEvent(target.Battle, beSkillAttackEndEvent, EventParams<BattleSkill>.Create(target.Battle, this));
                }

                if (removedTargets != null)
                {
                    for (int i = 0; i < removedTargets.Count; i++)
                    {
                        var target = removedTargets[i];
                        target.DispatchEvent(target.Battle, beSkillAttackEndEvent, EventParams<BattleSkill>.Create(target.Battle, this));
                    }
                }
            }

            if (SkillType == BattleDef.ESkillType.Normal)
            {
                Owner.DispatchEvent(Owner.Battle, BattleObject.Event.DoNormalSkillEnd, EventParams<BattleSkill>.Create(Owner.Battle, this));
            }
            else if (SkillType == BattleDef.ESkillType.Rage)
            {
                Owner.DispatchEvent(Owner.Battle, BattleObject.Event.DoRageSkillEnd, EventParams<BattleSkill>.Create(Owner.Battle, this));
            }
            Owner.DispatchEvent(Owner.Battle, BattleObject.Event.DoSkillEnd, EventParams<BattleSkill>.Create(Owner.Battle, this));
            Owner.Effect.ReleaseEffect();
        }

        /// <summary>
        /// 替换技能效果
        /// </summary>
        /// <param name="effects"></param>
        /// <param name="selfEffects"></param>
        /// <param name="selfEndEffects"></param>
        public void ReplaceEffects(long[] effects, long[] selfEffects, long[] selfEndEffects)
        {
            TargetEffects = null;
            if (effects != null)
            {
                TargetEffects = new BaseEffectCfg[effects.Length];
                for (int i = 0; i < TargetEffects.Length; i++)
                {
                    TargetEffects[i] = ConfigManagerNew.Instance.Get<BaseEffectCfg>(ConfigHashCodeDefine.BaseEffectCfg, effects[i]);
                }
            }

            SelfEffects = null;
            if (selfEffects != null)
            {
                SelfEffects = new BaseEffectCfg[selfEffects.Length];
                for (int i = 0; i < SelfEffects.Length; i++)
                {
                    SelfEffects[i] = ConfigManagerNew.Instance.Get<BaseEffectCfg>(ConfigHashCodeDefine.BaseEffectCfg, selfEffects[i]);
                }
            }

            SelfEndEffects = null;
            if (selfEndEffects != null)
            {
                SelfEndEffects = new BaseEffectCfg[selfEndEffects.Length];
                for (int i = 0; i < SelfEndEffects.Length; i++)
                {
                    SelfEndEffects[i] = ConfigManagerNew.Instance.Get<BaseEffectCfg>(ConfigHashCodeDefine.BaseEffectCfg, selfEndEffects[i]);
                }
            }
        }

        /// <summary>
        /// 还原技能默认效果
        /// </summary>
        public void ReductionEffects()
        {
            ReplaceEffects(Cfg.effects, Cfg.selfEffects, Cfg.selfEndEffects);
        }

        /// <summary>
        /// 加载配置数据
        /// </summary>
        private void LoadData()
        {
            Cfg = ConfigManagerNew.Instance.Get<BattleSkillCfg>(ConfigHashCodeDefine.BattleSkillCfg, Sid);
            Rage = Cfg.rage;
            SkillType = Cfg.skillType;
            ReplaceEffects(Cfg.effects, Cfg.selfEffects, Cfg.selfEndEffects);
            FindType = Cfg.findType;
            if (Cfg.raceTypes != null)
            {
                if (Cfg.raceTypes.Length == 0)
                {
                    mRaceType = new BattleDef.ERaceType[1] { BattleDef.ERaceType.All };
                }
                else
                {
                    mRaceType = Cfg.raceTypes;
                }
            }
            else
            {
                mRaceType = new BattleDef.ERaceType[1] { BattleDef.ERaceType.All };
            }
            if (Cfg.jobTypes != null)
            {
                if (Cfg.jobTypes.Length == 0)
                {
                    mJobType = new BattleDef.EJobType[1] { BattleDef.EJobType.All };
                }
                else
                {
                    mJobType = Cfg.jobTypes;
                }
            }
            else
            {
                mJobType = new BattleDef.EJobType[1] { BattleDef.EJobType.All };
            }
            mSid = Cfg.roleSid;
            mCount = Cfg.count;
            mCheckState = Cfg.checkState;
            mProperty = Cfg.property;
            mOtherType = Cfg.otherType;
            mCompareType = Cfg.compareType;
            mOtherArgs = Cfg.otherArgs;
        }

        /// <summary>
        /// 初始化给目标的技能效果
        /// </summary>
        /// <param name="skillEffects">指定效果组</param>
        /// <param name="cfgSids">效果sid数组</param>
        private void InitSkillEffects(BaseEffectCfg[] skillEffects, long[] cfgSids)
        {
            if (cfgSids != null)
            {
                skillEffects = new BaseEffectCfg[cfgSids.Length];
                for (int i = 0; i < skillEffects.Length; i++)
                {
                    skillEffects[i] = ConfigManagerNew.Instance.Get<BaseEffectCfg>(ConfigHashCodeDefine.BaseEffectCfg,cfgSids[i]);
                }
            }
        }
    }
}
