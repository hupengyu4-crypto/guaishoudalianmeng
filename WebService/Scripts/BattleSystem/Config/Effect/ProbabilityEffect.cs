#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;

namespace BattleSystem
{
    /// <summary>
    /// 概率效果
    /// </summary>
    public class ProbabilityEffect : BaseEffect
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
        /// 不通过给自己加效果
        /// </summary>
        public long[] failedSelfEffects;
        /// <summary>
        /// 不通过给目标加效果
        /// </summary>
        public long[] failedTargetEffects;
        /// <summary>
        /// 目标抵抗属性
        /// </summary>
        public BattleDef.Property targetProperty = BattleDef.Property.None;
        /// <summary>
        /// 概率(0-100)
        /// </summary>
        public double[] probability;
        /// <summary>
        /// 是否随机其中一个效果
        /// </summary>
        public bool isRandomOne;
        /// <summary>
        /// 星级概率
        /// </summary>
        public double starValue;

        public override void Trigger()
        {
            var mAuthor = (Fighter)author;
            var cfgProbability = 0.0d;
            if (BattleUtils.GetLevelValue(probability, effectLevel, out var cfgValue))
            {
                cfgProbability = cfgValue;
            }
            double newProbability = cfgProbability + mAuthor.Data.Star * starValue;
            double randomValue = battle.Random.RandomValue(0d, 100d);
#if UNITY_EDITOR
            battle.AddInfo($"ProbabilityEffect({sid}),配置概率:{cfgProbability},随机结果{randomValue}");
#endif
            if (randomValue <= newProbability)
            {
                var mTarget = (Fighter)target;

                if (authorUid != target.Uid && targetProperty != BattleDef.Property.None)
                {
#if UNITY_EDITOR
                    battle.AddInfo(",抵抗检测", true);
#endif
                    double control = 0;
                    //控制类型的
                    switch (targetProperty)
                    {
                        case BattleDef.Property.freeze_cut:
                        case BattleDef.Property.palsy_cut:
                        case BattleDef.Property.silent_cut:
                        case BattleDef.Property.vertigo_cut:
                            control = (BattleDef.TenThousand - mTarget.Data[BattleDef.Property.control_cut] +
                                       mAuthor.Data[BattleDef.Property.control_add] - mTarget.Data[targetProperty]) * BattleDef.Percent;
                            break;
                        case BattleDef.Property.control_cut:
                        case BattleDef.Property.control_add:
                            control = (BattleDef.TenThousand - mTarget.Data[BattleDef.Property.control_cut] +
                                       mAuthor.Data[BattleDef.Property.control_add]) * BattleDef.Percent;
                            break;
                        case BattleDef.Property.burn_cut:
                            control = (BattleDef.TenThousand - mTarget.Data[targetProperty] + mAuthor.Data[BattleDef.Property.burn_hit_add]) *
                                      BattleDef.Percent;
                            break;
                        case BattleDef.Property.poison_cut:
                            control = (BattleDef.TenThousand - mTarget.Data[targetProperty] + mAuthor.Data[BattleDef.Property.poison_hit_add]) *
                                      BattleDef.Percent;
                            break;
                        case BattleDef.Property.bleed_cut:
                            control = (BattleDef.TenThousand - mTarget.Data[targetProperty] + mAuthor.Data[BattleDef.Property.bleed_hit_add]) *
                                      BattleDef.Percent;
                            break;
                        default:
                            control = (BattleDef.TenThousand - mTarget.Data[targetProperty]) * BattleDef.Percent;
                            break;
                    }
                    control = YKMath.Clamp(control, 0, 100);
                    double random = battle.Random.RandomValue(0d, 100d);
#if UNITY_EDITOR
                    battle.AddInfo($"计算抵抗概率:{control},随机结果{random}");
#endif
                    if (random < control)
                    {
                        Pass();
                    }
                    else
                    {
                        NotPass();
                        //目标抵抗成功
                        mTarget.DispatchEvent(battle, BattleObject.Event.Resistance);
                    }
                }
                else
                {
                    Pass();
                }
            }
            else
            {
                NotPass();
            }
        }

        private void Pass()
        {
#if UNITY_EDITOR
            battle.AddInfo(",通过", true);
#endif
            AddEffect(author, passedSelfEffects);
            AddEffect(target, passedTargetEffects);
        }
        private void NotPass()
        {
#if UNITY_EDITOR
            battle.AddInfo(",不通过", true);
#endif
            AddEffect(author, failedSelfEffects);
            AddEffect(target, failedTargetEffects);
        }
        /// <summary>
        /// 添加效果
        /// </summary>
        /// <param name="bo">战斗对象</param>
        /// <param name="addEffects">效果</param>
        private void AddEffect(BattleObject bo, long[] addEffects)
        {
            if (bo != null && addEffects != null)
            {
                int count = addEffects.Length;
                if (isRandomOne && count > 1)
                {
                    var newIndex = battle.Random.RandomValue(0, count - 1);
                    bo.Effect.AddEffect(addEffects[newIndex], authorUid, effectLevel, out _, false, _additionData);
                }
                else
                {
                    bo.Effect.AddEffects(addEffects, authorUid, effectLevel, _additionData);
                }
            }
        }
    }
}