#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;
using System.Collections.Generic;

namespace BattleSystem
{
    /// <summary>
    /// 清除效果
    /// </summary>
    public class ClearEffect : BaseEffect
    {
        /// <summary>
        /// 清除效果的sid
        /// </summary>
        public long[] clearSids;
        /// <summary>
        /// (选择类型清理时)禁止清理效果类型
        /// </summary>
        public EffectSys.EffectType[] forbidType;
        /// <summary>
        /// (选择类型清理时)清理效果类型
        /// </summary>
        public EffectSys.EffectType[] checkType;
        /// <summary>
        /// 清理数量
        /// </summary>
        public int count;
        /// <summary>
        /// 清理前是否引爆伤害
        /// </summary>
        public bool triggerAllDamage;

        public override void Trigger()
        {
            var effectList = target.Effect.AllEffectList;
            List<BaseEffect> clearList = new List<BaseEffect>();
            if (clearSids.Length > 0)
            {
                foreach (var effect in effectList)
                {
                    if (effect == null)
                        continue;
                    if (System.Array.IndexOf(clearSids, effect.sid) >= 0)
                    {
                        clearList.Add(effect);
                    }
                }
            }
            else
            {
                foreach (var item in effectList)
                {
                    foreach (var t in checkType)
                    {
                        if (item.IsType(t))
                        {
                            clearList.Add(item);
                        }
                    }
                }

                for (var i = 0; i < clearList.Count; i++)
                {
                    foreach (var t in forbidType)
                    {
                        if (clearList[i].IsType(t))
                        {
                            clearList.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }
            }

            int num = 0;
            for (var i = 0; i < clearList.Count; i++)
            {
                var eff = clearList[i];
                if (eff == null || eff.isEnd)
                {
                    continue;
                }
                TryTriggerEffect(eff);
                target.Effect.ClearEffect(eff);
                num++;
                if (count > 0 && num >= count)
                {
                    break;
                }
            }

            clearList.Clear();
        }

        /// <summary>
        /// 尝试触发后续效果
        /// </summary>
        /// <param name="effect"></param>
        private void TryTriggerEffect(BaseEffect effect)
        {
            if (!triggerAllDamage)
                return;

            //剩余回合数=可持续回合-已生效回合
            var surplusBout = effect.bout - effect.GetActiveBout();
            if (surplusBout <= 0 || !IsCanTriggerDamage(effect))
                return;
            bool burn = false, bleed = false, poison=false;
            if (effect is AddStateEffect addState)
            {
                var sids = addState.targetEffects;
                foreach (var l in sids)
                {
                    TriggerDamage(l, effect, surplusBout);
                    if (!poison && effect.IsType(EffectSys.EffectType.Poison))
                        poison = true;
                    if (!bleed && effect.IsType(EffectSys.EffectType.Bleed))
                        bleed = true;
                    if (!burn && effect.IsType(EffectSys.EffectType.Burn))
                        burn = true;
                }
            }

            if (effect is DamageEffect)
            {
                TriggerDamage(effect.sid, effect, surplusBout);
                if (!poison && effect.IsType(EffectSys.EffectType.Poison))
                    poison = true;
                if (!bleed && effect.IsType(EffectSys.EffectType.Bleed))
                    bleed = true;
                if (!burn && effect.IsType(EffectSys.EffectType.Burn))
                    burn = true;
            }
            if (poison) 
            {
                author.DispatchEvent(battle, BattleObject.Event.TriggerAllPoisonDmg);
            }
            if (bleed)
            {
                author.DispatchEvent(battle, BattleObject.Event.TriggerAllBleedDmg);
            }
            if (burn)
            {
                author.DispatchEvent(battle, BattleObject.Event.TriggerAllBurnDmg);
            }
        }

        /// <summary>
        /// 引爆效果
        /// </summary>
        /// <param name="effSid">效果Sid</param>
        /// <param name="effect"></param>
        /// <param name="surplusBout">剩余回合</param>
        private void TriggerDamage(long effSid, BaseEffect effect, int surplusBout)
        {
            if (effSid == 0)
            {
                Log.error($"看到这个堆栈，大概率配置出问题了,{effect}");
                return;
            }
            var eff = effect.target.Effect.CreateEffect(effSid, effect.authorUid, effectLevel);
            eff.effectTypeMark = EffectSys.EffectType.Null;
            eff.bout = 0; //为了秒算
            eff.overlayCount = effect.overlayCount;//叠加层级共享
            if (eff.triggerCount > 0)
            {
                surplusBout = Math.Min(surplusBout, eff.triggerCount - eff.triggerNum);//触发次数也参与计算
                eff.triggerCount = 0;//为了秒算
            }
            //目前只有伤害可以引爆
            if (surplusBout > 0 && eff is DamageEffect damageEffect)
            {
#if UNITY_EDITOR
                battle.AddInfo(effect.target.GetBaseDesc()).AddInfo(" 被引爆伤害:").AddInfo(eff.sid).AddInfo(",叠层:").AddInfo(eff.overlayCount).AddInfo("剩余回合/剩余触发次数：").AddInfo(surplusBout, true);
#endif
                damageEffect.rate *= surplusBout * eff.overlayCount;//通过修改系数去汇总所有伤害
                effect.target.Effect.AddEffect(eff);
            }
            else
            {
                battle.Pool.Release(eff);
            }

        }

        /// <summary>
        /// 是否能引爆伤害
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        private bool IsCanTriggerDamage(BaseEffect effect)
        {
            return effect.IsType(EffectSys.EffectType.Poison) || effect.IsType(EffectSys.EffectType.Bleed) ||
                   effect.IsType(EffectSys.EffectType.Burn);
        }
    }
}