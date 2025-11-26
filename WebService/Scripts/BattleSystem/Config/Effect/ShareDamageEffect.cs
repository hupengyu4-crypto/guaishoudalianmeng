using System.Collections.Generic;

namespace BattleSystem
{
    /// <summary>
    /// 分摊伤害
    /// </summary>
    public class ShareDamageEffect : BaseEffect
    {
        /// <summary>
        /// 索敌(至少小回合结束)
        /// </summary>
        public long findEffects;

        /// <summary>
        /// 伤害效果
        /// </summary>
        public long damageEffects;

        /// <summary>
        /// 伤害效果先作用于目标，如果目标死亡后再分摊
        /// </summary>
        public bool bekilled;

        /// <summary>
        /// 施法者
        /// </summary>
        private Fighter mAuthor;

        public override void OnBegin()
        {
            mAuthor = (Fighter)author;
            base.OnBegin();
        }

        public override void Trigger()
        {
            List<Fighter> allTargets = null; //先索敌，target死了就索不到了 by ww
            if (findEffects != 0)
            {
                //配置了就重新索敌
                target.Effect.AddEffect(findEffects, target.Uid, effectLevel, out var ef);
                if (ef is FindTargetEffect findEffect)
                {
                    allTargets = findEffect.GetFighters();
                }

                target.Effect.ClearEffect(ef);
            }
            else
            {
                //不配置就用主技能的索敌
                allTargets = mAuthor.CacheTargets;
            }

            DamageEffect killDamage = null;
            var killDamageOverflow = 0.0d;
            if (bekilled)
            {
                var oldHp = 0.0d;
                if (target is Fighter targetFighter)
                {
                    oldHp = targetFighter.Data[BattleDef.Property.hp];
                }
                else
                {
                    return;
                }

                target.Effect.AddEffect(damageEffects, author.Uid, effectLevel, out var ef);
                if (ef is DamageEffect d)
                {
                    killDamage = d;
                    if (target.IsDead || target.IsFakeDead)
                    {
                        killDamageOverflow = d.GetThisDamageValue() - oldHp;
                        if (killDamageOverflow <= 0.0d)
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            if (allTargets == null || allTargets.Count == 0)
            {
                return;
            }

            var count = allTargets.Count;
            //var shareKillDamage = 0L;
            var shareScaleFactor = 1.0d;
            if (killDamage != null)
            {
                //shareKillDamage = (long)YKMath.Ceiling(killDamageOverflow / count);
                shareScaleFactor = killDamageOverflow / count / killDamage.GetThisDamageValue();
            }
            else
            {
                shareScaleFactor = 1.0d / count;
            }

            if (findEffects != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var shareTarget = allTargets[i];
                    // if (killDamage != null)
                    // {
                    //     var thisDamage = killDamageOverflow / count;
                    //     var dmgArg = BattleUtils.CreateSimpleDamageParams(battle, sid, target.Uid, target.Uid, shareKillDamage, killDamage.damageType, killDamage.isRealDamage, true);
                    //     shareTarget.DispatchEvent(battle, BattleObject.Event.BeDamageWithoutDamageEffect, dmgArg);
                    //     shareTarget.AddProp(BattleDef.Property.hp, -shareKillDamage);
                    // }
                    // else
                    {
                        var eff = shareTarget.Effect.CreateEffect(damageEffects, author.Uid, effectLevel);
                        if (eff is DamageEffect damageEffect)
                        {
                            damageEffect.rate *= shareScaleFactor; //通过修改系数去伤害分摊
                            shareTarget.Effect.AddEffect(eff);
                        }
                        else
                        {
                            battle.Pool.Release(eff);
                        }
                    }
                }
            }
            else
            {
                var eff = target.Effect.CreateEffect(damageEffects, author.Uid, effectLevel);
                if (eff is DamageEffect damageEffect)
                {
                    damageEffect.rate *= shareScaleFactor; //通过修改系数去伤害分摊
                    target.Effect.AddEffect(eff);
                }
                else
                {
                    battle.Pool.Release(eff);
                }
            }
        }

        public override void OnEnd()
        {
            mAuthor = null;
            base.OnEnd();
        }
    }
}