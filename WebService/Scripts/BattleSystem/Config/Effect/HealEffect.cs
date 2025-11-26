using System;

namespace BattleSystem
{
    public class HealEffect : BaseEffect
    {
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
        public double[] factors;
        /// <summary>
        /// 是否参照损失血量
        /// </summary>
        public bool lossHp;
        /// <summary>
        /// 损失血量系数(百分比)
        /// </summary>
        public double lossHpFactor;
        /// <summary>
        /// 溢出治疗量转换为护盾百分比
        /// </summary>
        public double overHealToShield;
        /// <summary>
        /// 基础治疗量
        /// </summary>
        public double baseHealValue;

        public override void Trigger()
        {
            if (battle.CanNotHeal)
            {
                return;
            }

            var mAuthor = (Fighter)author;
            var mTarget = (Fighter)target;
            if (mTarget.Data[BattleDef.Property.hp] <= 0)
            {
                //防止检测血量变更回血技能所导致的无法死亡BUG
                return;
            }
            double healValue = baseHealValue;
            var tempPropTarget = propTarget == BattleDef.TargetType.Target ? mTarget : mAuthor;
            if (factors != null)
            {
                for (var i = 0; i < properties.Length; i++)
                {
                    healValue += Math.Max(tempPropTarget.Data[properties[i]] * factors[i] * BattleDef.Percent, 0);
#if UNITY_EDITOR
                    if (BattleDef.IsPropertyCoefficient[properties[i]])
                    {
                        Log.error($"不允许配置系数类型的属性作为参考值！{properties[i]}, HealEffectConfig-{sid}");
                    }
#endif
                }
            }

            if (lossHp)
            {
                healValue += Math.Max(tempPropTarget.LossHp * lossHpFactor * BattleDef.Percent, 0);
            }

            var heal = BattleUtils.CalcHealValue(mAuthor, mTarget);

            //#if UNITY_EDITOR
            //            if (BattleDef.DebugLog)
            //                Log.debug($"{sid},基础治疗:{healValue},治疗强度:{heal},最终治疗:{YKMath.Ceiling(healValue * heal)}," +
            //                          $"剩余血量:{tempPropTarget.lossHp},参照目标:{propTarget}>>{tempPropTarget.GetBaseDesc()},目标:{mTarget.GetBaseDesc()},施法者:{mAuthor.GetBaseDesc()}");
            //#endif

            healValue = YKMath.Ceiling(healValue * heal);
            if (healValue > 0.0)
            {
                var cureParams = battle.CreateEventParam<HealParams>();
                cureParams.casterkUid = authorUid;
                cureParams.targetUid = mTarget.Uid;
                cureParams.oldValue = (long)healValue;
                cureParams.newValue = (long)healValue;
                cureParams.IsAutoRelease = false;


                if (!mTarget.DispatchEvent(battle, BattleObject.Event.HealPre, cureParams).IsBlockEvent)
                {
                    var curHP = mTarget.Data[BattleDef.Property.hp];
                    var maxHP = mTarget.Data[BattleDef.Property.max_hp];
                    mTarget.AddProp(BattleDef.Property.hp, cureParams.newValue > 0 ? cureParams.newValue : 0, authorUid);

                    mTarget.DispatchEvent(battle, BattleObject.Event.Heal, cureParams);

                    cureParams.IsAutoRelease = true;
                    battle.ReleaseEventParam(cureParams);

                    if (overHealToShield > 0 && curHP + healValue > maxHP)
                    {
                        mTarget.AddShield((long)(overHealToShield * BattleDef.Percent * (curHP + healValue - maxHP)));
                    }
                }
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            baseHealValue = 0;
        }
    }
}