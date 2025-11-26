using System;

namespace BattleSystem
{
    public class DamageEffect : BaseEffect
    {
        /// <summary>
        /// 伤害类型
        /// </summary>
        public BattleDef.DamageType damageType = BattleDef.DamageType.Normal;
        /// <summary>
        /// 参照目标类型
        /// </summary>
        public BattleDef.TargetType propTarget;
        /// <summary>
        /// 参照属性
        /// </summary>
        public BattleDef.Property[] properties;
        /// <summary>
        /// 属性系数(百分比)
        /// </summary>
        public double[][] factors;
        /// <summary>
        /// 最大值参照目标类型
        /// </summary>
        public BattleDef.TargetType maxPropTarget;
        /// <summary>
        /// 最大值参照属性
        /// </summary>
        public BattleDef.Property maxProperty;
        /// <summary>
        /// 最大值参照属性(百分比)
        /// </summary>
        public double maxValue;
        /// <summary>
        /// 伤害倍率
        /// </summary>
        public double rate;
        /// <summary>
        /// 是否真实伤害
        /// </summary>
        public bool isRealDamage;
        /// <summary>
        /// 伤害回血系数(百分比)
        /// </summary>
        public double healFactor;
        /// <summary>
        /// 额外百分比加成
        /// </summary>
        public double otherFactor;
        /// <summary>
        /// 技能固定伤害
        /// </summary>
        public double fixedValue;
        /// <summary>
        /// 星级伤害
        /// </summary>
        public double starValue;
        /// <summary>
        /// 等级伤害
        /// </summary>
        public double lvValue;
        /// <summary>
        /// 乘效果的层数
        /// </summary>
        public EffectSys.EffectType multiplyByEffect;
        /// <summary>
        /// 附带累计受到普攻伤害百分比
        /// </summary>
        public double allNormalDmg;
        /// <summary>
        /// 回血索敌
        /// </summary>
        public long healFindEffects;

        /// <summary>
        /// 已经算好的伤害
        /// </summary>
        public double calculatedDamage;

        private double _thisDamageValue;

        /// <summary>
        /// 是否是转移伤害
        /// </summary>
        public bool isTransferDamage;
        /// <summary>
        /// 进攻方
        /// </summary>
        private Fighter mAuthor;
        /// <summary>
        /// 防守方
        /// </summary>
        private Fighter mTarget;

        public override void OnBegin()
        {
            mAuthor = (Fighter)author;
            mTarget = (Fighter)target;
            base.OnBegin();
        }

        public override void Trigger()
        {
            double damage = 0;
            if (_additionData != null && _additionData.damageValue > 0)
            {
                CauseDamage(_additionData.damageValue);
                return;
            }
            Fighter tempPropTarget = GetPropTarget();
#if UNITY_EDITOR
            if (calculatedDamage <= 0.0)
            {
                if (properties == null)
                {
                    Log.error($"未配置目标属性:DamageEffect>>>{sid}");
                }

                var factorLength = factors == null ? 0 : factors.Length;
                if (properties.Length != factorLength)
                {
                    Log.error($"目标属性数量不匹配:DamageEffect>>>{sid}");
                }
            }
#endif
            if (calculatedDamage > 0.0)
            {
                damage = calculatedDamage;
            }
            else
            {
                var addedFactor = mAuthor.Data.Star * starValue + mAuthor.Data.Level * lvValue;
                for (var i = 0; i < properties.Length; i++)
                {
                    double factor = 0.0d;
                    if (BattleUtils.GetLevelValue(factors[i], effectLevel, out var cfgFactor))
                    {
                        factor = cfgFactor;
                    }

                    damage += Math.Max(tempPropTarget.Data[properties[i]] * ((factor + otherFactor + addedFactor) * BattleDef.Percent), 0);
#if UNITY_EDITOR
                    if (BattleDef.IsPropertyCoefficient[properties[i]])
                    {
                        Log.error($"不允许配置系数类型的属性作为参考值！{properties[i]}, DamageEffectConfig-{sid}");
                    }
#endif
                }
            }
            if (allNormalDmg > 0)
            {
                damage += mAuthor.causeNormalDamage * allNormalDmg * BattleDef.Percent;
            }

#if UNITY_EDITOR
            double initDamage = damage;
#endif

            if (_additionData != null)
            {
                rate *= _additionData.stepChangeValue + 1.0;
            }

            if (isRealDamage)
            {
                damage *= rate;
                CauseDamage(damage);
                //#if UNITY_EDITOR
                //                if (BattleDef.DebugLog)
                //                    Log.debug($"基础伤害值{initDamage},倍率系数{rate},真实伤害{damage}");
                //#endif
                return;
            }


            //破甲与防御公式
            double defDef = mTarget.Data[BattleDef.Property.defense];//防守方防御
            double brokenAemorAdd = mAuthor.Data[BattleDef.Property.broken_add];//攻击方破甲率
            double brokenAemorCut = mTarget.Data[BattleDef.Property.broken_reduce];//防守方破甲抵抗率
            double defActive = Math.Min(defDef / (defDef + 5000d) * BattleDef.TenThousand, 7000d);//生效值=(防守方防御/(防守方防御+5000)*10000),上限值7000
            double value1 = (BattleDef.TenThousand + YKMath.Clamp(brokenAemorAdd - brokenAemorCut, 0, BattleDef.AntiDefMax) - defActive) * BattleDef.Percent100;
            value1 = Math.Max(value1, 0.3d);//修正不低于

            //VIP增伤减伤
            double hurtAddVip = mAuthor.Data[BattleDef.Property.vip_damage_add];//攻击方vip增伤
            double hurtCutVip = mTarget.Data[BattleDef.Property.vip_damage_reduce];//防守方vip减伤
            double value2 = (BattleDef.TenThousand + hurtAddVip - hurtCutVip) * BattleDef.Percent100;
            value2 = Math.Max(value2, 0.3d);//修正不低于

            //增伤减伤
            double hurtAdd = mAuthor.Data[BattleDef.Property.damage_add];//进攻方增伤
            double hurtCut = mTarget.Data[BattleDef.Property.damage_reduce];//防守方减伤
            double value3 = (BattleDef.TenThousand + hurtAdd - hurtCut) * BattleDef.Percent100;
            value3 = YKMath.Clamp(value3, 0.3d, 1.2d);//修正区间

            //乘效果层数
            if (multiplyByEffect != EffectSys.EffectType.Null)
            {
                int checkCount = 0;
                foreach (BaseEffect e in mTarget.Effect.AllEffectList)
                {
                    if (e.IsType(multiplyByEffect))
                    {
                        checkCount += e.overlayCount;
                    }
                }
                damage *= checkCount;
            }

            //职业抵抗
            double jobValue = 0;
            switch (mAuthor.Data.jobType)
            {
                case BattleDef.EJobType.Soldier:
                    jobValue = mTarget.Data[BattleDef.Property.soldier_hurt_cut];
                    break;
                case BattleDef.EJobType.Sorcerer:
                    jobValue = mTarget.Data[BattleDef.Property.master_hurt_cut];
                    break;
                case BattleDef.EJobType.Assassin:
                    jobValue = mTarget.Data[BattleDef.Property.assassin_hurt_cut];
                    break;
                case BattleDef.EJobType.Controller:
                    jobValue = mTarget.Data[BattleDef.Property.control_hurt_cut];
                    break;
                case BattleDef.EJobType.Meatshield:
                    jobValue = mTarget.Data[BattleDef.Property.tank_hurt_cut];
                    break;
                case BattleDef.EJobType.Supporter:
                    jobValue = mTarget.Data[BattleDef.Property.support_hurt_cut];
                    break;
                case BattleDef.EJobType.MapBoss:
                case BattleDef.EJobType.Pet:
                    jobValue = 0;
                    break;
                default:
                    //Log.error($"Sid:{mAuthor.Data.Sid},Uid:{mAuthor.Uid},错误的职业类型配置:{mAuthor.Data.jobType}");
                    break;
            }

            //职业的增伤
            switch (mAuthor.Data.jobType)
            {
                case BattleDef.EJobType.Soldier:
                    jobValue -= mAuthor.Data[BattleDef.Property.soldier_hurt_add];
                    break;
                case BattleDef.EJobType.Sorcerer:
                    jobValue -= mAuthor.Data[BattleDef.Property.master_hurt_add];
                    break;
                case BattleDef.EJobType.Assassin:
                    jobValue -= mAuthor.Data[BattleDef.Property.assassin_hurt_add];
                    break;
                case BattleDef.EJobType.Controller:
                    jobValue -= mAuthor.Data[BattleDef.Property.control_hurt_add];
                    break;
                case BattleDef.EJobType.Meatshield:
                    jobValue -= mAuthor.Data[BattleDef.Property.tank_hurt_add];
                    break;
                case BattleDef.EJobType.Supporter:
                    jobValue -= mAuthor.Data[BattleDef.Property.support_hurt_add];
                    break;
                default:
                    break;
            }

            double value4 = Math.Max((BattleDef.TenThousand - jobValue) * BattleDef.Percent100, 0.3d);//修正不低于

            switch (damageType)
            {
                case BattleDef.DamageType.DeBuff:
                    damage = YKMath.Ceiling(damage * value1 * value2 * value3 * value4 * rate);
                    CauseDamage(damage);
                    break;
                case BattleDef.DamageType.BleedBuff:
                case BattleDef.DamageType.PoisonBuff:
                case BattleDef.DamageType.FiringBuff:
                    int debuffDmgMin = BattleDef.MinDebuffDMGMultiple;
                    int debuffDmgMax = BattleDef.MaxDebuffDMGMultiple;
                    double value_debuff = YKMath.Clamp(10000 + (mTarget.Data[BattleDef.Property.debuff_damage_add] - mTarget.Data[BattleDef.Property.debuff_damage_reduce]),
                        debuffDmgMin, debuffDmgMax) * BattleDef.Percent100;//灼烧、中毒、流血的增伤减伤
                    double value_debuff2 = 0;
                    switch (damageType)
                    {
                        case BattleDef.DamageType.BleedBuff:
                            value_debuff2 = YKMath.Clamp(10000 + (mTarget.Data[BattleDef.Property.bleed_damage_add] - mTarget.Data[BattleDef.Property.bleed_damage_reduce]),
                                debuffDmgMin, debuffDmgMax) * BattleDef.Percent100;
                            break;
                        case BattleDef.DamageType.PoisonBuff:
                            value_debuff2 = YKMath.Clamp(10000 + (mTarget.Data[BattleDef.Property.poison_damage_add] - mTarget.Data[BattleDef.Property.poison_damage_reduce]),
                                debuffDmgMin, debuffDmgMax) * BattleDef.Percent100;
                            break;
                        case BattleDef.DamageType.FiringBuff:
                            value_debuff2 = YKMath.Clamp(10000 + (mTarget.Data[BattleDef.Property.burn_damage_add] - mTarget.Data[BattleDef.Property.burn_damage_reduce]),
                                debuffDmgMin, debuffDmgMax) * BattleDef.Percent100;
                            break;
                    }
                    value_debuff = YKMath.Clamp(value_debuff * value_debuff2, debuffDmgMin * BattleDef.Percent100, debuffDmgMax * BattleDef.Percent100);

                    damage = YKMath.Ceiling(damage * value1 * value2 * value3 * value4 * value_debuff * rate);
                    CauseDamage(damage);
                    //#if UNITY_EDITOR
                    //                if (BattleDef.DebugLog)
                    //                    Log.debug($"持续伤害：基础伤害值{initDamage},破甲与防御公式{value1},VIP增伤减伤{value2},增伤减伤{value3},职业抵抗{value4}debuff减伤{value_debuff}");
                    //#endif
                    break;
                default:
                    //暴击伤害
                    double value5 = 1;
                    //暴击几率
                    double criticalAdd = mAuthor.Data[BattleDef.Property.crit];//进攻方暴击
                    double criticalCut = mTarget.Data[BattleDef.Property.crit_reduce];//防守方暴击抵抗
                    double criticalPer = (criticalAdd - criticalCut) * BattleDef.Percent;
                    criticalPer = YKMath.Clamp(criticalPer, 0, 100d);//修正

                    bool isCrit = damageType != BattleDef.DamageType.Indirect && criticalPer >= mAuthor.Battle.Random.RandomValue(0d, 100d);
                    if (isCrit)
                    {
                        double criticalHurtAdd = mAuthor.Data[BattleDef.Property.crit_hurt_add];//进攻方爆伤
                        double criticalHurtCut = mTarget.Data[BattleDef.Property.crit_hurt_reduce];//防守方爆伤抵抗
                        value5 = (15000d + criticalHurtAdd - criticalHurtCut) * BattleDef.Percent100;//爆伤公式计算
                        value5 = YKMath.Clamp(value5, BattleDef.CriticalHurtMin, BattleDef.CriticalHurtMax);//修正
                    }

                    //技能伤害
                    double hurtAddSkill = mAuthor.Data[BattleDef.Property.ability_dmg_add];//进攻方技能增伤
                    double hurtCutSkill = mTarget.Data[BattleDef.Property.ability_dmg_reduce];//防守方技能减伤
                    double value6 = (BattleDef.TenThousand + hurtAddSkill - hurtCutSkill) * BattleDef.Percent100;//技能伤害公式计算
                    value6 = YKMath.Clamp(value6, 1d, 3d);//修正

                    //格挡
                    double accurate = mAuthor.Data[BattleDef.Property.accurate];//进攻方精准
                    double block = mTarget.Data[BattleDef.Property.block];//防守方格挡
                    double blockPer = (block - accurate) * BattleDef.Percent;
                    blockPer = YKMath.Clamp(blockPer, 0d, 100d);//修正

                    bool isBlock = damageType != BattleDef.DamageType.Indirect && blockPer >= mAuthor.Battle.Random.RandomValue(0d, 100d);
                    double value7 = 1;
                    if (isBlock)
                    {
                        value7 = 0.67d;
                    }


                    //战功压制系数 =向上取整(10000+进攻方战功压制系数-防守方战功压制系数)*0.0001  此值不可高于1.5 不可低于0.5
                    double value8 = YKMath.Ceiling(10000d + mAuthor.Data[BattleDef.Property.service_pressing] - mTarget.Data[BattleDef.Property.service_pressing]) * BattleDef.Percent100;
                    value8 = YKMath.Clamp(value8, 0.5d, 1.5d);//修正

                    //种族压制 =向上取整(10000+进攻方定向阵容压制系数-防守方定向阵容抵抗系数)*0.0001  此值不可高于1.5 不可低于0.5
                    double value9 = 1;
                    switch (mAuthor.Data.raceType)
                    {
                        case BattleDef.ERaceType.Wei:
                            value9 = YKMath.Ceiling(10000d + mAuthor.Data[BattleDef.Property.greece__dmg_add] -
                                                    mTarget.Data[BattleDef.Property.greece__dmg_reduce] +
                                                    mTarget.Data[BattleDef.Property.be_first_hit_damage_add]) * BattleDef.Percent100;
                            break;
                        case BattleDef.ERaceType.Shu:
                            value9 = YKMath.Ceiling(10000d + mAuthor.Data[BattleDef.Property.nordic__dmg_add] -
                                                    mTarget.Data[BattleDef.Property.nordic__dmg_reduce] +
                                                    mTarget.Data[BattleDef.Property.be_second_hit_damage_add]) * BattleDef.Percent100;
                            break;
                        case BattleDef.ERaceType.Wu:
                            value9 = YKMath.Ceiling(10000d + mAuthor.Data[BattleDef.Property.chinese__dmg_add] -
                                                    mTarget.Data[BattleDef.Property.chinese__dmg_reduce] +
                                                    mTarget.Data[BattleDef.Property.be_third_hit_damage_add]) * BattleDef.Percent100;
                            break;
                        case BattleDef.ERaceType.Qun:
                            value9 = YKMath.Ceiling(10000d + mAuthor.Data[BattleDef.Property.fourth__dmg_add] -
                                                    mTarget.Data[BattleDef.Property.fourth__dmg_reduce] +
                                                    mTarget.Data[BattleDef.Property.be_forth_hit_damage_add]) * BattleDef.Percent100;
                            break;
                        default:
                            value9 = 1;
                            break;
                    }

                    switch (mTarget.Data.raceType)
                    {
                        case BattleDef.ERaceType.Wei:
                            value9 += YKMath.Ceiling(mAuthor.Data[BattleDef.Property.to_first_dmg_add]) * BattleDef.Percent100;
                            break;
                        case BattleDef.ERaceType.Shu:
                            value9 += YKMath.Ceiling(mAuthor.Data[BattleDef.Property.to_second_dmg_add]) * BattleDef.Percent100;
                            break;
                        case BattleDef.ERaceType.Wu:
                            value9 += YKMath.Ceiling(mAuthor.Data[BattleDef.Property.to_third_dmg_add]) * BattleDef.Percent100;
                            break;
                        case BattleDef.ERaceType.Qun:
                            value9 += YKMath.Ceiling(mAuthor.Data[BattleDef.Property.to_fourth_dmg_add]) * BattleDef.Percent100;
                            break;
                        default:
                            break;
                    }

                    value9 += mTarget.Data[BattleDef.Property.be_hit_damage_add] * BattleDef.Percent100;
                    value9 = YKMath.Clamp(value9, 0.5d, 1.5d);//修正

                    //进攻方即时伤害全公式
                    if (damageType == BattleDef.DamageType.Normal || damageType == BattleDef.DamageType.Indirect)
                    {
                        var normalAdd = mAuthor.Data[BattleDef.Property.normal_damage_add] * BattleDef.Percent100;
                        var normalReduce = mTarget.Data[BattleDef.Property.normal_damage_reduce] * BattleDef.Percent100;
                        var normalScale = YKMath.Clamp(1 + normalAdd - normalReduce, 0.4d, 2.5d);
                        var beNormalHitAdd = YKMath.Clamp(1 + mTarget.Data[BattleDef.Property.be_hit_normal_damage_add] * BattleDef.Percent100, 0.5d,
                                                          2.0d);
                        damage = YKMath.Ceiling(damage * value1 * value2 * value3 * value4 * value5 * value7 * value8 * value9 * rate * normalScale *
                                                beNormalHitAdd);
                        //#if UNITY_EDITOR
                        //                    if (BattleDef.DebugLog)
                        //                        Log.debug($"普攻：基础伤害值{initDamage},破甲与防御公式{value1},VIP增伤减伤{value2},增伤减伤{value3},职业抵抗{value4},暴击{criticalPer},暴击伤害{value5},格挡{blockPer},格挡减伤{value7},伤害倍率{rate}");
                        //#endif
                    }
                    else
                    {
                        damage = YKMath.Ceiling(damage * value1 * value2 * value3 * value4 * value5 * value6 * value7 * value8 * value9 * rate);
                        //#if UNITY_EDITOR
                        //                    if (BattleDef.DebugLog)
                        //                        Log.debug($"技能：基础伤害值{initDamage},破甲与防御公式{value1},VIP增伤减伤{value2},增伤减伤{value3},职业抵抗{value4},暴击{criticalPer},暴击伤害{value5},技能伤害{value6},格挡{blockPer},格挡减伤{value7},伤害倍率{rate}");
                        //#endif
                    }

                    //星级压制
                    var roleEquipScale =
                        YKMath.Clamp(1.0d + (mAuthor.Data[BattleDef.Property.role_equip_star] - mTarget.Data[BattleDef.Property.role_equip_star]) * BattleDef.Percent100,
                                     1.0d, 3.0d);

                    //星级减伤
                    roleEquipScale *=
                        YKMath.Clamp(1.0d + (mAuthor.Data[BattleDef.Property.anti_role_equip_star] - mTarget.Data[BattleDef.Property.anti_role_equip_star]) * BattleDef.Percent100,
                                     0.3d, 1.0d);

                    // 最终伤害减免
                    damage *= YKMath.Clamp(1.0d - mAuthor.Data[BattleDef.Property.final_attack_damage_reduce] * BattleDef.Percent100, 0.01d, 1.0d);
                    damage *= YKMath.Clamp(1.0d - mTarget.Data[BattleDef.Property.final_damage_reduce] * BattleDef.Percent100, 0.01d, 1.0d);

                    damage *= roleEquipScale;

                    //伤害DeBuff叠加
                    damage *= overlayCount;

                    //现在只有地图boss会受到这个伤害系数影响
                    if (mTarget.Data.jobType == BattleDef.EJobType.MapBoss)
                    {
                        //额外伤害影响，如星级系数等
                        damage *= mAuthor.GetDamageFactor();
                    }
                    damage *= mAuthor.GetDamageFactorAlways();

                    damage *= 1.0 - mTarget.GetDamageReduce();

                    if (mAuthor.Data.jobType == BattleDef.EJobType.Pet)
                    {
                        damage *= mAuthor.Battle.IsPve() ? BattleDef.PvePetExtraDamageAddFactor : BattleDef.PvpPetExtraDamageAddFactor;
                    }
                    damage = YKMath.Ceiling(damage);

                    CauseDamage(damage, isCrit, isBlock);
                    break;
            }
        }

        /// <summary>
        /// 造成伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <param name="isCrit">是否暴击</param>
        /// <param name="isBlock">是否格挡</param>
        private void CauseDamage(double damage, bool isCrit = false, bool isBlock = false)
        {
            if (calculatedDamage <= 0.0)
            {
                // 技能固定值伤害
                damage += fixedValue;
                //最大值限制
                Fighter tempPropTarget = GetPropTarget();
                var max = tempPropTarget.Data[maxProperty] * maxValue * BattleDef.Percent;
                if (max <= 0)
                {
                    max = damage;
                }

                if (damage > max)
                {
                    //#if UNITY_EDITOR
                    //                if (BattleDef.DebugLog)
                    //                    Log.debug($"触发伤害上限限制,伤害{damage}>最大伤害{max}");
                    //#endif
                    damage = max;
                }
            }

            _thisDamageValue = damage;
            DamageParams args = battle.CreateEventParam<DamageParams>();
            args.effectSid = sid;
            args.attackUid = mAuthor.Uid;
            args.defendUid = mTarget.Uid;
            var lastDamage = (long)damage;
            args.oldValue = lastDamage;
            args.newValue = lastDamage;
            args.isCrit = isCrit;
            args.isBlock = isBlock;
            args.IsAutoRelease = false;
            args.damageType = damageType;
            args.isRealDamage = isRealDamage;
            args.isTransferDamage = isTransferDamage;
            var attackIsBlockEvent = false;
            var defendIsBlockEvent = false;
            if (damageType == BattleDef.DamageType.Normal || damageType == BattleDef.DamageType.Indirect || damageType == BattleDef.DamageType.Skill)
            {
                attackIsBlockEvent = mAuthor.DispatchEvent(battle, BattleObject.Event.AttackDamagePre, args).IsBlockEvent;
                defendIsBlockEvent = mTarget.DispatchEvent(battle, BattleObject.Event.BeDamagePre, args).IsBlockEvent;
            }
            // else if (damageType == BattleDef.DamageType.Indirect)
            // {
            //     attackIsBlockEvent = mAuthor.DispatchEvent(battle, BattleObject.Event.IndirectDamagePre, args).IsBlockEvent;
            //     defendIsBlockEvent = mTarget.DispatchEvent(battle, BattleObject.Event.BeIndirectDamagePre, args).IsBlockEvent;
            // }
            else
            {
                attackIsBlockEvent = mAuthor.DispatchEvent(battle, BattleObject.Event.OtherAttackDamagePre, args).IsBlockEvent;
                defendIsBlockEvent = mTarget.DispatchEvent(battle, BattleObject.Event.OtherBeDamagePre, args).IsBlockEvent;
            }
            if (!attackIsBlockEvent && !defendIsBlockEvent)
            {
                var par = battle.CreateEventParam<PropParams>();
                par.isCrit = isCrit;
                par.isBlock = isBlock;
                par.damageType = damageType;

                mAuthor.recordDamage += args.newValue;
                mTarget.recordSubHp += args.newValue;
                RecordDamage(damageType, mAuthor, mTarget, args.newValue);
                mTarget.AddProp(BattleDef.Property.hp, -args.newValue, authorUid, par);

                //当伤害回血有配置时
                if (healFactor != 0 && !battle.CanNotHeal)
                {
                    var healValue = (long)YKMath.Ceiling(args.newValue * healFactor * BattleDef.Percent);
                    if (healFindEffects > 0)
                    {
                        author.Effect.AddEffect(healFindEffects, author.Uid, effectLevel, out var fEffect);
                        if (fEffect is FindTargetEffect findEffect)
                        {
                            var allTargets = findEffect.GetFighters();
                            var targetsCount = allTargets.Count;
                            if (targetsCount > 0)
                            {
                                var fakeHeal = new HealEffectConfig();
                                fakeHeal.id = -999999;
                                for (int i = 0; i < targetsCount; i++)
                                {
                                    var healTarget = allTargets[i];
                                    var healEffect = fakeHeal.Create(healTarget.Effect.Owner, target.Uid) as HealEffect;
                                    if (healEffect == null)
                                    {
                                        continue;
                                    }
                                    healEffect.baseHealValue = (long)YKMath.Ceiling(args.newValue * healFactor * BattleDef.Percent);
                                    healTarget.Effect.Battle.BattleInfo?.ListenEffectInfo(healEffect);
                                    healTarget.Effect.AddEffect(healEffect);
                                }
                            }
                        }
                        author.Effect.ClearEffect(fEffect);
                    }
                    else
                    {
                        var oldProp = mAuthor.Data[BattleDef.Property.hp];
                        mAuthor.AddProp(BattleDef.Property.hp, healValue, authorUid);
                        healValue = mAuthor.Data[BattleDef.Property.hp] - oldProp;
                        if (healValue > 0)
                        {
                            var healParams = battle.CreateEventParam<HealParams>();
                            healParams.casterkUid = authorUid;
                            healParams.targetUid = authorUid;
                            healParams.oldValue = healValue;
                            healParams.newValue = healValue;
                            healParams.IsAutoRelease = false;
                            mAuthor.DispatchEvent(battle, BattleObject.Event.Heal, healParams);
                            healParams.IsAutoRelease = true;
                            battle.ReleaseEventParam(healParams);
                        }
                    }
                }
                var lifeStealData = mAuthor.Data[BattleDef.Property.life_steal];
                if (lifeStealData > 0 && !battle.CanNotHeal)
                {
                    var lifeSteal = lifeStealData * BattleDef.Percent100;
                    var heal = BattleUtils.CalcHealValue(mAuthor, mAuthor);
                    var stealValue = (long)YKMath.Ceiling(args.newValue * lifeSteal * heal);
                    if (stealValue > 0)
                    {
                        RecordDamage(damageType, mAuthor, mTarget, args.newValue);
                        mAuthor.AddProp(BattleDef.Property.hp, stealValue, authorUid);
                        var stealParams = battle.CreateEventParam<HealParams>();
                        stealParams.casterkUid = authorUid;
                        stealParams.targetUid = authorUid;
                        stealParams.oldValue = stealValue;
                        stealParams.newValue = stealValue;
                        stealParams.IsAutoRelease = false;
                        mAuthor.DispatchEvent(battle, BattleObject.Event.Heal, stealParams);
                        stealParams.IsAutoRelease = true;
                        battle.ReleaseEventParam(stealParams);
                    }
                }

                if (damageType == BattleDef.DamageType.Normal || damageType == BattleDef.DamageType.Indirect || damageType == BattleDef.DamageType.Skill)
                {
                    mAuthor.DispatchEvent(battle, BattleObject.Event.AttackDamage, args);
                    mTarget.DispatchEvent(battle, BattleObject.Event.BeDamage, args);

                    if (isCrit)
                    {
                        mAuthor.statisticsData.critNum++;
                        mAuthor.DispatchEvent(battle, BattleObject.Event.Crit, args);
                        mTarget.DispatchEvent(battle, BattleObject.Event.BeCrit, args);
                    }
                    if (isBlock)
                    {
                        mTarget.statisticsData.blockNum++;
                        mAuthor.DispatchEvent(battle, BattleObject.Event.BeBlock, args);
                        mTarget.DispatchEvent(battle, BattleObject.Event.Block, args);
                    }
                }

                switch (damageType)
                {
                    case BattleDef.DamageType.Normal:
                        mAuthor.DispatchEvent(battle, BattleObject.Event.NormalDamage, args);
                        mTarget.DispatchEvent(battle, BattleObject.Event.BeNormalDamage, args);
                        if (isCrit)
                        {
                            mAuthor.DispatchEvent(battle, BattleObject.Event.NormalCritDamage, args);
                            mTarget.DispatchEvent(battle, BattleObject.Event.BeNormalCritDamage, args);
                        }
                        RecordDamage(damageType, mAuthor, mTarget, args.newValue);
                        mTarget.AddProp(BattleDef.Property.anger, BattleDef.BeHitAddRage); //受击加怒气
                        break;
                    case BattleDef.DamageType.Indirect:
                        mAuthor.DispatchEvent(battle, BattleObject.Event.IndirectDamage, args);
                        mTarget.DispatchEvent(battle, BattleObject.Event.BeIndirectDamage, args);
                        RecordDamage(damageType, mAuthor, mTarget, args.newValue);
                        mTarget.AddProp(BattleDef.Property.anger, BattleDef.BeHitAddRage); //受击加怒气
                        break;
                    case BattleDef.DamageType.Skill:
                        mAuthor.DispatchEvent(battle, BattleObject.Event.SkillDamage, args);
                        mTarget.DispatchEvent(battle, BattleObject.Event.BeSkillDamage, args);
                        if (isCrit)
                        {
                            mAuthor.DispatchEvent(battle, BattleObject.Event.SkillCritDamage, args);
                            mTarget.DispatchEvent(battle, BattleObject.Event.BeSkillCritDamage, args);
                        }
                        RecordDamage(damageType, mAuthor, mTarget, args.newValue);
                        mTarget.AddProp(BattleDef.Property.anger, BattleDef.BeHitAddRage); //受击加怒气
                        break;
                    case BattleDef.DamageType.DeBuff:
                    case BattleDef.DamageType.PoisonBuff:
                    case BattleDef.DamageType.FiringBuff:
                    case BattleDef.DamageType.BleedBuff:
                        mAuthor.DispatchEvent(battle, BattleObject.Event.DeBuffDamage, args);
                        mTarget.DispatchEvent(battle, BattleObject.Event.BeDeBuffDamage, args);
                        break;
                }
            }

            //回收
            args.IsAutoRelease = true;
            battle.ReleaseEventParam(args);

        }

        private void RecordDamage(BattleDef.DamageType damageType, Fighter mAuthor, Fighter mTarget, double dmg)
        {
            if (damageType == BattleDef.DamageType.Normal)
            {
                mAuthor.causeNormalDamage += dmg;
                mTarget.hitNormalDamage += dmg;
            }
        }

        public override void OnEnd()
        {
            BaseBattle.EffectAdditionData additionData = null;
            additionData = AllocAdditionData(additionData);
            additionData.causedDamage = _thisDamageValue;
            mAuthor = null;
            mTarget = null;
            calculatedDamage = 0.0d;
            _thisDamageValue = 0.0d;
            isTransferDamage = false;
            target.Effect.AddEffects(endEffects, authorUid, effectLevel, additionData);
        }

        public double GetThisDamageValue()
        {
            return _thisDamageValue;
        }

        private Fighter GetPropTarget()
        {
            if (_additionData != null && _additionData.specificTarget != null && _additionData.specificTarget is Fighter spFighter)
            {
                return spFighter;
            }
            else
            {
                return propTarget == BattleDef.TargetType.Target ? mTarget : mAuthor;
            }
        }
    }
}
