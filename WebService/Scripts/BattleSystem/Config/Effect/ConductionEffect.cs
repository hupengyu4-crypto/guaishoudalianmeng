using System.Collections.Generic;
using RootScript.Config;

namespace BattleSystem
{
    /// <summary>
    /// 传导效果
    /// </summary>
    public class ConductionEffect : BaseEffect
    {
        /// <summary>
        /// 添加的效果类型(不配表示所有debuff)
        /// </summary>
        public EffectSys.EffectType[] addedEffectType;

        /// <summary>
        /// 概率(0-100)
        /// </summary>
        public double probability;

        /// <summary>
        /// 索敌(传导给谁，0是传给队友)
        /// </summary>
        public long findEffect;

        private EffectSys.EffectType _addedEffectType;

        public override void Trigger()
        {
        }

        public override void OnBegin()
        {
            var typeLength = addedEffectType.Length;
            if (typeLength > 0)
            {
                for (int i = 0, l = typeLength; i < typeLength; i++)
                {
                    var t = addedEffectType[i];
                    if (t != EffectSys.EffectType.Conduction)
                    {
                        _addedEffectType |= t;
                    }
                }
            }
            else
            {
                _addedEffectType = EffectSys.allDebuffEffectType & ~EffectSys.EffectType.Conduction;
            }

            if (_addedEffectType != EffectSys.EffectType.Null)
            {
                target.AddEvent(EffectSys.Event.Begin, OnEffectAdd, false, 1);
            }
        }

        public override void OnEnd()
        {
            if (_addedEffectType != EffectSys.EffectType.Null)
            {
                target.RemoveEvent(EffectSys.Event.Begin, OnEffectAdd);
            }

            base.OnEnd();
        }

        private void OnEffectAdd(EventParams arg0)
        {
            if (!(arg0 is EventParams<BaseEffect> effectArg))
            {
                return;
            }

            var addedEffect = effectArg.data;
            if (addedEffect == null || addedEffect.conducted || !addedEffect.IsType(_addedEffectType))
            {
                return;
            }

            List<Fighter> allTargets = null;
            if (findEffect <= 0)
            {
                allTargets = BattleUtils.GetFighters((Fighter)target, BattleDef.EFindType.AllSelf, null, null, 0, 1000, null,
                                                     BattleDef.Property.None, BattleDef.OtherType.Null, BattleDef.CompareGradeType.Highest, null);
            }
            else
            {
                target.Effect.AddEffect(findEffect, target.Uid, effectLevel, out var e);
                if (e is FindTargetEffect findTargetEffect)
                {
                    allTargets = findTargetEffect.GetFighters();
                }
                target.Effect.ClearEffect(e);
            }

            if (allTargets != null)
            {
                BaseEffectCfg cfg = ConfigManagerNew.Instance.Get<BaseEffectCfg>(ConfigHashCodeDefine.BaseEffectCfg, addedEffect.sid);
                if (cfg != null)
                {
                    for (int i = 0, l = allTargets.Count; i < l; i++)
                    {
                        var beAddTarget = allTargets[i];
                        if (beAddTarget == target || !beAddTarget.IsState(BattleObject.State.Conduction))
                        {
                            continue;
                        }

                        double randomValue = battle.Random.RandomValue(0d, 100d);
                        if (randomValue > probability)
                        {
#if UNITY_EDITOR
                            battle.AddInfo($"ConductionEffect({sid}),因几率没有生效:{probability},随机结果{randomValue}", true);
#endif
                            continue;
                        }

                        //beAddTarget.Effect.AddEffect(addedEffect.sid, authorUid);
                        var toConductionEffect = cfg.Create(beAddTarget.Effect.Owner, authorUid);
                        if (toConductionEffect == null)
                        {
                            continue;
                        }

#if UNITY_EDITOR
                        battle.AddInfo($"ConductionEffect({sid}),传导到:{beAddTarget.GetBaseDesc()}", true);
#endif
                        toConductionEffect.conducted = true;
                        beAddTarget.Effect.Battle.BattleInfo?.ListenEffectInfo(toConductionEffect);
                        beAddTarget.Effect.AddEffect(toConductionEffect);
                    }
                }
            }
        }
    }
}