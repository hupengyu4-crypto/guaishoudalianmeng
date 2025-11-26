namespace BattleSystem
{
    /// <summary>
    /// 护盾承伤反弹
    /// </summary>
    public class ShieldAbsorptionReflectiveEffect : BaseEffect
    {
        /// <summary>
        /// 反伤百分比(1-100)
        /// </summary>
        public double value;

        /// <summary>
        /// 索敌(反伤的目标)
        /// </summary>
        public long findEffects;

        private long _shieldAbsorptionValue = 0;
        private bool _haveShield = false;
        private bool _triggered = false;


        public override void OnBegin()
        {
            var targetFighter = (Fighter)target;
            _haveShield = targetFighter.IsState(BattleObject.State.Shield);
            if (_haveShield) //添加上的时候目标有护盾才生效
            {
                target.AddEvent(BattleObject.Event.ShieldAbsorption, OnShieldAbsorption, false, 1);
                target.AddEvent(BattleObject.Event.ShieldRemove, OnShieldRemove, false, 1);
            }

            base.OnBegin();
        }

        public override void Trigger()
        {
        }

        public override void OnEnd()
        {
            if (_haveShield)
            {
                target.RemoveEvent(BattleObject.Event.ShieldAbsorption, OnShieldAbsorption);
                target.RemoveEvent(BattleObject.Event.ShieldRemove, OnShieldRemove);
                //TriggerReflective(); //结束不触发
            }
            _shieldAbsorptionValue = 0;
            _haveShield = false;
            _triggered = false;

            base.OnEnd();
        }

        private void OnShieldAbsorption(EventParams arg0)
        {
            if (arg0 is ShieldAbsorptionParams absorptionParams)
            {
                _shieldAbsorptionValue += absorptionParams.shieldAbsorptionValue;
            }
        }

        private void OnShieldRemove(EventParams arg0)
        {
            TriggerReflective();
        }

        private void TriggerReflective()
        {
            if (_triggered)
            {
                return;
            }
            _triggered = true;

            if (_shieldAbsorptionValue <= 0 || findEffects == 0)
            {
                return;
            }

            var damage = (long)YKMath.Ceiling(_shieldAbsorptionValue * value * BattleDef.Percent);
            if (damage > 0)
            {
#if UNITY_EDITOR
                var targetFighter = (Fighter)target;
                battle.AddInfo($"护盾承伤反弹---触发者：{targetFighter.Data.Cfg?.name}---护盾承伤：{_shieldAbsorptionValue}---反弹伤害：{damage}", true);
#endif
                target.Effect.AddEffect(findEffects, target.Uid, effectLevel, out var effect);
                if (effect is FindTargetEffect findEffect)
                {
                    var allTargets = findEffect.GetFighters();
                    var targetsCount = allTargets.Count;
                    if (targetsCount > 0)
                    {
                        var fakeDamageEffectCfg = new DamageEffectCfg();
                        fakeDamageEffectCfg.id = -999999;
                        for (int i = 0; i < targetsCount; i++)
                        {
                            var beDamagedTarget = allTargets[i];
                            //beDamagedTarget.AddProp(BattleDef.Property.hp, -damage, target.Uid);
                            //var beDamageEffect = cfg.Create(beAddTarget.Effect.Owner, target.Uid);
                            var beDamageEffect = fakeDamageEffectCfg.Create(beDamagedTarget.Effect.Owner, target.Uid) as DamageEffect;
                            if (beDamageEffect == null)
                            {
                                continue;
                            }
                            beDamageEffect.damageType = BattleDef.DamageType.Indirect;
                            beDamageEffect.calculatedDamage = damage;
                            beDamageEffect.rate = 1.0;
                            beDamageEffect.isRealDamage = true;
                            beDamagedTarget.Effect.Battle.BattleInfo?.ListenEffectInfo(beDamageEffect);
                            beDamagedTarget.Effect.AddEffect(beDamageEffect);
                        }
                    }
                }
                target.Effect.ClearEffect(effect);
            }
        }
    }
}