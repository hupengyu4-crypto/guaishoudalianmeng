using System.Collections.Generic;

namespace BattleSystem
{
    /// <summary>
    /// 按回合分摊伤害
    /// </summary>
    public class ApportionDamageEffect : BaseEffect
    {
        /// <summary>
        /// 首次伤害百分比
        /// </summary>
        private double _firstFactor;
        /// <summary>
        /// 分摊回合
        /// </summary>
        private int _apportionBout;
        /// <summary>
        /// 分摊伤害是否忽略护盾
        /// </summary>
        private bool _ignoreShield;

        private Fighter _targetFighter;
        private List<KeyValuePair<long, int>> _waitToApportionDamage;

        public override void OnBegin()
        {
            _targetFighter = (Fighter)target;
            target.AddEvent(BattleObject.Event.BeDamagePre, OnDamagePre, false, 2); //要在shield的BeDamagePre之前执行，事件是倒序执行，所以要比shield的BeDamagePre的1大才行 by ww

            base.OnBegin();
        }

        private void OnDamagePre(EventParams eventParams)
        {
            if (!eventParams.IsBlockEvent && eventParams is DamageParams args)
            {
                var damage = args.newValue;

                var firstDamage = (long)YKMath.Ceiling(_firstFactor * BattleDef.Percent * damage);
                var leftDamage = damage - firstDamage;
                args.newValue = firstDamage;

                if (leftDamage > 0 && _apportionBout > 0)
                {
                    var eachDamage = (long)YKMath.Ceiling(leftDamage * 1.0 / _apportionBout);
                    if (eachDamage > 0)
                    {
                        if (_waitToApportionDamage == null)
                        {
                            _waitToApportionDamage = new List<KeyValuePair<long, int>>();
                        }

                        _waitToApportionDamage.Add(new KeyValuePair<long, int>(eachDamage, _apportionBout));
                    }
                }
            }
        }

        public override void Trigger()
        {
        }

        public override void OnBoutEnd(EventParams arg)
        {
            var damage = 0L;
            if (_waitToApportionDamage != null)
            {
                for (int i = _waitToApportionDamage.Count - 1; i >= 0; i--)
                {
                    var toDamage = _waitToApportionDamage[i];
                    var leftCount = toDamage.Value - 1;
                    damage += toDamage.Key;
                    if (leftCount <= 0)
                    {
                        _waitToApportionDamage.RemoveAt(i);
                    }
                    else
                    {
                        _waitToApportionDamage[i] = new KeyValuePair<long, int>(toDamage.Key, leftCount);
                    }
                }
            }

            if (damage > 0)
            {
#if UNITY_EDITOR
                battle.AddInfo($"{_targetFighter.GetBaseDesc()} 分摊伤害---：{damage}", true);
#endif

                if (!_ignoreShield)
                {
                    damage = _targetFighter.ShieldEffect(damage, 0, 0);
                }

                if (damage > 0)
                {
                    var dmgArg = BattleUtils.CreateSimpleDamageParams(battle, sid, target.Uid, target.Uid, damage, BattleDef.DamageType.Normal, true, true);
                    _targetFighter.DispatchEvent(battle, BattleObject.Event.BeDamageWithoutDamageEffect, dmgArg);
                    _targetFighter.AddProp(BattleDef.Property.hp, -damage);
                }
            }

            base.OnBoutEnd(arg);
        }

        public void SetData(ApportionDamageEffectConfig cfg)
        {
            _firstFactor = cfg.firstFactor;
            _apportionBout = cfg.apportionBout;
            _ignoreShield = cfg.ignoreShield;
        }

        public override void OnEnd()
        {
            target.RemoveEvent(BattleObject.Event.BeDamagePre, OnDamagePre);
            _targetFighter = null;

            base.OnEnd();
        }
    }
}