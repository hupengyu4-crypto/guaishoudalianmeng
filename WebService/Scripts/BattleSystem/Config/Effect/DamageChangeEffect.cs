#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;

namespace BattleSystem
{
    /// <summary>
    /// 检查条件，改变伤害
    /// </summary>
    public class DamageChangeEffect : BaseEffect
    {
        /// <summary>
        /// 检查状态
        /// </summary>
        public BattleObject.State[] checkState;

        /// <summary>
        /// 检查效果类型
        /// </summary>
        public EffectSys.EffectType[] checkTypes;

        /// <summary>
        /// 检查职业
        /// </summary>
        public BattleDef.EJobType[] checkJobs;

        /// <summary>
        /// 检测的属性
        /// </summary>
        public BattleDef.Property property;
        /// <summary>
        /// 检测的类型
        /// </summary>
        public BattleDef.CompareType checkType;
        /// <summary>
        /// 属性值(百分比)
        /// </summary>
        public double value;

        /// <summary>
        /// 是否同时满足条件
        /// </summary>
        public bool isCombine;

        /// <summary>
        /// 是否计算数量
        /// </summary>
        public bool isCalculateCount;

        /// <summary>
        /// 最大数量
        /// </summary>
        public int maxCount;

        /// <summary>
        /// 单个增伤百分比
        /// </summary>
        public double factor;

        /// <summary>
        /// 差异的百分之多少算一份（百分比）
        /// </summary>
        public double step;

        /// <summary>
        /// 一份改变多少值
        /// </summary>
        public double stepChangePerValue;

        /// <summary>
        /// 是否数值类型
        /// </summary>
        public bool isNumber;

        /// <summary>
        /// 被击时改变伤害
        /// </summary>
        public bool beDamaged;

        private long mOldDamage, mNewDamage;
        private int mCount;

        public override void OnBegin()
        {
            mOldDamage = 0;
            mNewDamage = 0;
            mCount = 0;
            if (maxCount == 0)
            {
                maxCount = int.MaxValue;
            }

            target.AddEvent(beDamaged ? BattleObject.Event.BeDamagePre : BattleObject.Event.AttackDamagePre, CheckHandler, false, 1);
            base.OnBegin();
        }

        private void CheckHandler(EventParams arg0)
        {
            if (arg0 is DamageParams { newValue: > 0 } param)
            {
                var factorAdd = 0.0;
                //查找攻击目标
                var damageTarget = beDamaged ? battle.GetSceneObject<Fighter>(param.attackUid) : battle.GetSceneObject<Fighter>(param.defendUid);
                mCount = 0;
                bool isCheckPassed = false;
                if (checkState?.Length > 0)
                {
                    foreach (var state in checkState)
                    {
                        if (damageTarget.IsState(state))
                        {
                            isCheckPassed = true;
                            if (!isCalculateCount)
                            {
                                break; //不统计数量，发现满足其中之一就跳出循环
                            }

                            mCount++;
                        }
                        else if (isCombine)
                        {
                            isCheckPassed = false;
                            break; //不满足同时条件，跳出
                        }
                    }
                }
                else if (checkTypes?.Length > 0)
                {
                    var all = damageTarget.Effect.AllEffectList;
                    foreach (var effect in all)
                    {
                        foreach (var effectType in checkTypes)
                        {
                            if (effect.IsType(effectType))
                            {
                                isCheckPassed = true;
                                if (!isCalculateCount)
                                {
                                    break; //不统计数量，发现满足其中之一就跳出循环
                                }

                                mCount++;
                            }
                            else if (isCombine)
                            {
                                isCheckPassed = false;
                                break; //不满足同时条件，跳出
                            }
                        }
                    }
                }
                else if (checkJobs?.Length > 0)
                {
                    foreach (var job in checkJobs)
                    {
                        if (damageTarget.Data.jobType == job)
                        {
                            isCheckPassed = true;
                            if (!isCalculateCount)
                            {
                                break; //不统计数量，发现满足其中之一就跳出循环
                            }

                            mCount++;
                        }
                        else if (isCombine)
                        {
                            isCheckPassed = false;
                            break; //不满足同时条件，跳出
                        }
                    }
                }
                else if (property != BattleDef.Property.None)
                {
                    if (checkType == BattleDef.CompareType.Small || checkType == BattleDef.CompareType.Equal || checkType == BattleDef.CompareType.Big
                        || checkType == BattleDef.CompareType.SmallAndEqual || checkType == BattleDef.CompareType.BigAndEqual)
                    {
                        double curPropValue = damageTarget.Data.nowProps[(int)property];
                        double maxPropValue = 0;
                        if (property == BattleDef.Property.hp || property == BattleDef.Property.max_hp)
                        {
                            maxPropValue = damageTarget.Data.nowProps[(int)BattleDef.Property.max_hp];
                        }
                        else
                        {
                            maxPropValue = Math.Max(damageTarget.Data.initProps[(int)property],
                                BattleUtils.GetMaxPropertyValue(damageTarget, property));
                        }

                        double result = isNumber ? curPropValue : curPropValue * 1.0d / maxPropValue;
                        switch (checkType)
                        {
                            case BattleDef.CompareType.Small:
                                isCheckPassed = result < (isNumber ? value : value * BattleDef.Percent);
                                break;
                            case BattleDef.CompareType.SmallAndEqual:
                                isCheckPassed = result <= (isNumber ? value : value * BattleDef.Percent);
                                break;
                            case BattleDef.CompareType.Equal:
                                isCheckPassed = Math.Abs(result - (isNumber ? value : value * BattleDef.Percent)) < BattleDef.Percent;
                                break;
                            case BattleDef.CompareType.Big:
                                isCheckPassed = result > (isNumber ? value : value * BattleDef.Percent);
                                break;
                            case BattleDef.CompareType.BigAndEqual:
                                isCheckPassed = result >= (isNumber ? value : value * BattleDef.Percent);
                                break;
                        }

                        if (step > 0.0 && stepChangePerValue != 0.0)
                        {
                            var diff = (int)Math.Floor(result / (isNumber ? step : step * BattleDef.Percent));
                            if (diff > 0)
                            {
                                factorAdd = YKMath.Clamp(diff * stepChangePerValue, -1.0d, 1.0d);
                            }
                        }
                    }
                    else
                    {
                        var self = (Fighter)target;
                        var selfValue = self.Data.GetProp(property);
                        var otherValue = damageTarget.Data.GetProp(property);
                        switch (checkType)
                        {
                            case BattleDef.CompareType.LessThanMe:
                                isCheckPassed = otherValue < selfValue;
                                break;
                            case BattleDef.CompareType.EqualToMe:
                                isCheckPassed = Math.Abs(otherValue - selfValue) < BattleDef.Percent;
                                break;
                            case BattleDef.CompareType.GreaterThanMe:
                                isCheckPassed = otherValue > selfValue;
                                break;
                        }

                        if (step > 0.0 && stepChangePerValue != 0.0)
                        {
                            var diff = 0.0d;
                            if (isNumber)
                            {
                                diff = (int)Math.Floor(Math.Abs(otherValue - selfValue) / step);
                            }
                            else
                            {
                                diff = (int)Math.Floor(((double)Math.Abs(otherValue - selfValue) / selfValue) / (step * BattleDef.Percent));
                            }

                            if (diff > 0)
                            {
                                factorAdd = YKMath.Clamp(diff * stepChangePerValue, -1.0d, 1.0d);
                            }
                        }
                    }
                }

                if (isCheckPassed)
                {
                    mCount = YKMath.Clamp(mCount, 1, maxCount);
                    mOldDamage = param.newValue;
                    //最终值=当前伤害+当前伤害*系数*数量
                    mNewDamage = (long)YKMath.Ceiling(mOldDamage + (mOldDamage * (factor * BattleDef.Percent + factorAdd) * mCount));
                    mNewDamage = Math.Max(mNewDamage, 0);
                    param.newValue = mNewDamage;

                    //触发次数提供
                    TriggerBegin();

#if UNITY_EDITOR
                    if (mOldDamage == 0)
                        return;
                    battle.AddInfo(GetType().Name).AddInfo("-").AddInfo(sid).AddInfo(",原伤害:").AddInfo(mOldDamage).AddInfo(",系数:").AddInfo(factor * BattleDef.Percent)
                        .AddInfo(",数量:").AddInfo(mCount).AddInfo(",增伤:")
                        .AddInfo(mNewDamage - mOldDamage).AddInfo(",修正伤害:").AddInfo(mNewDamage, true);
#endif

                    mOldDamage = 0;
                    mNewDamage = 0;
                    mCount = 0;
                }
            }
        }

        public override void Trigger()
        {

        }

        public override void OnEnd()
        {
            target.RemoveEvent(beDamaged ? BattleObject.Event.BeDamagePre : BattleObject.Event.AttackDamagePre, CheckHandler);
            base.OnEnd();
        }
    }
}