using RootScript.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BattleSystem
{
    public class EffectSys
    {
        /// <summary>
        /// 持有者
        /// </summary>
        public BattleObject Owner { get; private set; }
        /// <summary>
        /// 战场
        /// </summary>
        public BaseBattle Battle { get; private set; }
        /// <summary>
        /// 全部效果
        /// </summary>
        public List<BaseEffect> AllEffectList { get; private set; } = new List<BaseEffect>();
        /// <summary>
        /// 等待清理效果
        /// </summary>
        public List<BaseEffect> WaitEffectList { get; private set; } = new List<BaseEffect>();

        private bool _effectsLock = false;
        /// <summary>
        /// 等待删除的效果
        /// </summary>
        private List<BaseEffect> _waitToRemoveEffectList = new List<BaseEffect>();
        /// <summary>
        /// 效果标记
        /// </summary>
        private EffectType mAllEffectTypeMask;
        private Array mEnumObjStates = Enum.GetValues(typeof(BattleObject.State));
        private Array mEnumEffectTypes = Enum.GetValues(typeof(EffectType));

        public EffectSys(BaseBattle battle, BattleObject owner)
        {
            Owner = owner;
            Battle = battle;
        }

        public static readonly EffectType[] debuffEffectTypes = new[]
            { EffectType.DeBuff, EffectType.Dizzy, EffectType.Silence, EffectType.Poison, EffectType.Frozen, EffectType.Bleed, EffectType.Burn, EffectType.Paralysis, EffectType.UnableAddAnger, EffectType.Disarm, EffectType.Conduction, EffectType.UnableAddShield };
        public static readonly EffectType[] hasIconDebuffEffectTypes = new[]
            { EffectType.Dizzy, EffectType.Silence, EffectType.Poison, EffectType.Frozen, EffectType.Bleed, EffectType.Burn, EffectType.Paralysis, EffectType.UnableAddAnger, EffectType.Disarm };
        public static readonly BattleObject.State[] debuffStates;
        //private static State _debuffStateType;
        public static readonly EffectType allDebuffEffectType;
        public static readonly BattleObject.State allDebuffState;
        public static readonly BattleObject.State[] hasIconDebuffStates;
        public static readonly EffectType allHasIconDebuffEffectType;
        public static readonly BattleObject.State allHasIconDebuffState;

        static EffectSys()
        {
            List<BattleObject.State> debuffState = new List<BattleObject.State>(debuffEffectTypes.Length);
            for (int i = 0, l = debuffEffectTypes.Length; i < l; i++)
            {
                var effectType = debuffEffectTypes[i];
                var state = EffectTypeToState(effectType);
                allDebuffEffectType |= effectType;
                allDebuffState |= state;
                if (state != BattleObject.State.Normal)
                {
                    debuffState.Add(state);
                }
            }
            debuffStates = debuffState.ToArray();

            debuffState.Clear();
            for (int i = 0, l = hasIconDebuffEffectTypes.Length; i < l; i++)
            {
                var effectType = hasIconDebuffEffectTypes[i];
                var state = EffectTypeToState(effectType);
                allHasIconDebuffEffectType |= effectType;
                allHasIconDebuffState |= state;
                if (state != BattleObject.State.Normal)
                {
                    debuffState.Add(state);
                }
            }
            hasIconDebuffStates = debuffState.ToArray();
        }

        /// <summary>
        /// 回合开始
        /// </summary>
        public void OnBoutStart()
        {
            _effectsLock = true;
            for (int i = 0; i < AllEffectList.Count; i++)
            {
                if (AllEffectList.Count == 0 || i > AllEffectList.Count)
                    break;
                var effect = AllEffectList[i];
                if (effect == null || effect.isEnd)
                    continue;
                if (effect.IsType(EffectType.BoutStart))
                {
                    effect.OnBoutStart(null);
                }
            }
            _effectsLock = false;

            ReleaseEffect();
        }

        /// <summary>
        /// 回合结束
        /// </summary>
        public void OnBoutEnd()
        {
            _effectsLock = true;
            for (int i = 0; i < AllEffectList.Count; i++)
            {
                if (AllEffectList.Count == 0 || i > AllEffectList.Count)
                    break;
                var effect = AllEffectList[i];
                if (effect == null || effect.isEnd)
                    continue;
                if (effect.bout > 0 || effect.IsType(EffectType.BoutEnd))
                {
                    effect.OnBoutEnd(null);
                }
            }
            _effectsLock = false;

            ReleaseEffect();
        }

        /// <summary>
        /// 小回合开始
        /// </summary>
        public void OnRoundStart()
        {
            _effectsLock = true;
            for (int i = 0; i < AllEffectList.Count; i++)
            {
                if (AllEffectList.Count == 0 || i > AllEffectList.Count)
                    break;
                var effect = AllEffectList[i];
                if (effect == null || effect.isEnd)
                    continue;
                if (effect.IsType(EffectType.RoundStart))
                {
                    effect.OnRoundStart(null);
                }
            }
            _effectsLock = false;

            ReleaseEffect();
        }

        /// <summary>
        /// 小回合结束
        /// </summary>
        public void OnRoundEnd()
        {
            _effectsLock = true;
            for (int i = 0; i < AllEffectList.Count; i++)
            {
                if (AllEffectList.Count == 0 || i > AllEffectList.Count)
                    break;
                var effect = AllEffectList[i];
                if (effect == null || effect.isEnd)
                    continue;
                if (effect.IsType(EffectType.RoundEnd))
                {
                    effect.OnRoundEnd(null);
                }
            }
            _effectsLock = false;

            ReleaseEffect();
        }

        /// <summary>
        /// 创建效果
        /// </summary>
        /// <param name="sid">效果Sid</param>
        /// <param name="authorUid">施法者Uid</param>
        /// <param name="level"></param>
        /// <param name="additionData"></param>
        /// <returns></returns>
        public BaseEffect CreateEffect(long sid, long authorUid, int level, BaseBattle.EffectAdditionData additionData = null)
        {
            BaseEffectCfg cfg = ConfigManagerNew.Instance.Get<BaseEffectCfg>(ConfigHashCodeDefine.BaseEffectCfg, sid);
            var effect = cfg?.Create(Owner, authorUid);
            if (effect != null)
            {
                effect.effectLevel = level;
                effect.UseAdditionData(additionData);
            }
            return effect;
        }

        /// <summary>
        /// 创建效果
        /// </summary>
        /// <param name="cfg">效果配置</param>
        /// <param name="authorUid">施法者Uid</param>
        /// <param name="level"></param>
        /// <param name="additionData"></param>
        /// <returns></returns>
        public BaseEffect CreateEffect(BaseEffectCfg cfg, long authorUid, int level, BaseBattle.EffectAdditionData additionData = null)
        {
            var effect = cfg?.Create(Owner, authorUid);
            if (effect != null)
            {
                effect.effectLevel = level;
                effect.UseAdditionData(additionData);
            }
            return effect;
        }

        /// <summary>
        /// 添加多个效果
        /// </summary>
        /// <param name="cfgs">效果列表</param>
        /// <param name="authorUid">施法者Uid</param>
        /// <param name="level"></param>
        /// <param name="additionData"></param>
        public void AddEffects(BaseEffectCfg[] cfgs, long authorUid, int level, BaseBattle.EffectAdditionData additionData = null)
        {
            if (Owner.isOnlySelfAddEffect && authorUid != Owner.Uid)
            {
                return;
            }
            if (cfgs != null)
            {
                bool isAdd = false;
                for (int i = 0; i < cfgs.Length; i++)
                {
                    var add = CreateWithAddEffect(cfgs[i], authorUid, level, out _, false, additionData);
                    if (add) isAdd = true;
                }
                if (isAdd)
                {
                    UpdateState();
                }
            }
        }

        /// <summary>
        /// 添加一个效果
        /// </summary>
        /// <param name="cfg">效果</param>
        /// <param name="authorUid">施法者Uid</param>
        /// <param name="level"></param>
        /// <param name="additionData"></param>
        public BaseEffect AddEffect(BaseEffectCfg cfg, long authorUid, int level, BaseBattle.EffectAdditionData additionData = null)
        {
            if (Owner.isOnlySelfAddEffect && authorUid != Owner.Uid)
            {
                return null;
            }

            BaseEffect addedEffect = null;
            if (cfg != null)
            {
                CreateWithAddEffect(cfg, authorUid, level, out addedEffect, true, additionData);
            }
            return addedEffect;
        }

        /// <summary>
        /// 添加多个效果
        /// </summary>
        /// <param name="sids">效果Sid列表</param>
        /// <param name="authorUid">施法者Uid</param>
        /// <param name="level"></param>
        /// <param name="additionData"></param>
        /// <returns>是否添加到列表</returns>
        public void AddEffects(long[] sids, long authorUid, int level, BaseBattle.EffectAdditionData additionData = null)
        {
            if (Owner.isOnlySelfAddEffect && authorUid != Owner.Uid)
            {
                return;
            }
            if (sids != null)
            {
                bool isAdd = false;
                foreach (var item in sids)
                {
                    var add = AddEffect(item, authorUid, level, out _, false, additionData);
                    if (add) isAdd = true;
                }
                if (isAdd)
                {
                    UpdateState();
                }
            }
        }

        /// <summary>
        /// 添加一个效果
        /// </summary>
        /// <param name="sid">效果Sid</param>
        /// <param name="authorUid">施法者Uid</param>
        /// <param name="level"></param>
        /// <param name="addedEffect"></param>
        /// <param name="isNeedUpdateState">是否需要更新状态。默认要，如果不false，需要手动更新</param>
        /// <param name="additionData"></param>
        /// <returns>是否添加到列表</returns>
        public bool AddEffect(long sid, long authorUid, int level, out BaseEffect addedEffect, bool isNeedUpdateState = true, BaseBattle.EffectAdditionData additionData = null)
        {
            addedEffect = null;
            if (Owner.isOnlySelfAddEffect && authorUid != Owner.Uid)
            {
                return false;
            }
            BaseEffectCfg cfg = ConfigManagerNew.Instance.Get<BaseEffectCfg>(ConfigHashCodeDefine.BaseEffectCfg, sid);
            if (cfg != null)
            {
                return CreateWithAddEffect(cfg, authorUid, level, out addedEffect, isNeedUpdateState, additionData);
            }
            return false;
        }

        /// <summary>
        /// 创建并添加一个效果
        /// </summary>
        /// <param name="cfg">效果配置</param>
        /// <param name="authorUid">施法者Uid</param>
        /// <param name="level"></param>
        /// <param name="addedEffect"></param>
        /// <param name="isNeedUpdateState">是否需要更新状态。默认要，如果不false，需要手动更新</param>
        /// <param name="additionData"></param>
        /// <returns>是否进列表</returns>
        private bool CreateWithAddEffect(BaseEffectCfg cfg, long authorUid, int level, out BaseEffect addedEffect, bool isNeedUpdateState = true, BaseBattle.EffectAdditionData additionData = null)
        {
            addedEffect = null;
            if (Owner.isOnlySelfAddEffect && authorUid != Owner.Uid)
            {
                return false;
            }
            var effect = cfg?.Create(Owner, authorUid);
            addedEffect = effect;
            if (effect != null)
            {
                effect.effectLevel = level;
                effect.UseAdditionData(additionData);
                Battle.BattleInfo?.ListenEffectInfo(effect);
                return AddEffect(effect, isNeedUpdateState);
            }
            return false;
        }

        private bool CanAddEffect(BaseEffect effect)
        {
            if (effect.IsType(EffectType.Isolate))
            {
                var isolateMaxOverlayCount = BattleDef.IsolateMaxOverlayCount;
                var isolateOverlayCount = 0;
                var allEffectList = AllEffectList;
                for (int i = 0, l = allEffectList.Count; i < l; i++)
                {
                    var e = allEffectList[i];
                    if (e == null)
                    {
                        continue;
                    }

                    if (e.IsType(EffectType.Isolate))
                    {
                        isolateOverlayCount += effect.overlayCount;

                        if (isolateOverlayCount >= isolateMaxOverlayCount)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 添加效果到字典
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="isNeedUpdateState">是否需要更新状态。默认要，如果不false，需要手动更新</param>
        /// <returns>是否进列表</returns>
        public bool AddEffect(BaseEffect effect, bool isNeedUpdateState = true)
        {
            if (effect == null)
                return false;
            if (Owner.isOnlySelfAddEffect && effect.authorUid != Owner.Uid)
            {
                Owner.Battle.Pool.Release(effect);
                return false;
            }

            if (!CanAddEffect(effect))
            {
                Owner.Battle.Pool.Release(effect);
                return false;
            }

            var isAdd = effect.IsNeedAddList();

            if (isAdd)
            {
                //只有在需求在列表中的，才需要进行覆盖，叠加判断
                var uid = effect.authorUid;
                var sid = effect.sid;
                var oldEff = GetEffect(uid, sid);
                var isEx = oldEff != null;

                if (isEx && !effect.IsType(EffectType.Overlays))
                {
                    ClearEffect(oldEff, isNeedUpdateState); //不叠加就先清理再重新加
                }
                if (isEx && effect.IsType(EffectType.Overlays))
                {
                    if (!Owner.DispatchEvent(Battle, Event.OverlayAddPre, EventParams<BaseEffect>.Create(Battle, oldEff)).IsBlockEvent)
                    {
                        oldEff.typeDirty = true;
                        oldEff.OnOverlayAdd();
                        Owner.Battle.Pool.Release(effect);//叠加旧的，回收刚创建的
                        Owner.DispatchEvent(Battle, Event.OverlayAdd, EventParams<BaseEffect>.Create(Battle, oldEff));
                        Battle.DispatchEvent(Battle, Event.OverlayAdd, EventTwoParams<BattleObject, BaseEffect>.Create(Battle, Owner, oldEff));
                        Owner.DispatchEvent(Battle, Event.BeginOrOverlayAdd, EventParams<BaseEffect>.Create(Battle, oldEff));
                        Battle.DispatchEvent(Battle, Event.BeginOrOverlayAdd, EventTwoParams<BattleObject, BaseEffect>.Create(Battle, Owner, oldEff));
                        return false;
                    }

                    Owner.Battle.Pool.Release(effect); //OverlayAddPre block了后，不应该继续往下面走BeginPre的逻辑了啊 by ww
                    return false;
                }
            }

            if (effect.Check() && !Owner.DispatchEvent(Battle, Event.BeginPre, EventParams<BaseEffect>.Create(Battle, effect)).IsBlockEvent)
            {
                if (effect.IsNeedAddList())
                {
                    AllEffectList.Add(effect);
                }

                effect.OnBegin();
                Owner.DispatchEvent(Battle, Event.Begin, EventParams<BaseEffect>.Create(Battle, effect));
                Battle.DispatchEvent(Battle, Event.Begin, EventTwoParams<BattleObject, BaseEffect>.Create(Battle, Owner, effect));
                Owner.DispatchEvent(Battle, Event.BeginOrOverlayAdd, EventParams<BaseEffect>.Create(Battle, effect));
                Battle.DispatchEvent(Battle, Event.BeginOrOverlayAdd, EventTwoParams<BattleObject, BaseEffect>.Create(Battle, Owner, effect));

                if (isAdd && isNeedUpdateState)
                {
                    UpdateState();//只有在需求在列表中的，才需要更新状态，否则都是临时效果，更新无意义
                }
                return isAdd;
            }
            Owner.Battle.Pool.Release(effect);

            return false;
        }

        /// <summary>
        /// 获取指定sid效果
        /// </summary>
        /// <param name="uid">施法者uid</param>
        /// <param name="sid">Sid</param>
        /// <returns>效果</returns>
        public BaseEffect GetEffect(long uid, long sid)
        {
            for (int i = 0; i < AllEffectList.Count; i++)
            {
                if (AllEffectList.Count == 0 || i > AllEffectList.Count)
                    break;
                var effect = AllEffectList[i];
                if (effect == null || effect.isEnd)
                    continue;
                if (effect.uid == uid && effect.sid == sid)
                {
                    return effect;
                }
            }
            return null;
        }

        /// <summary>
        /// 清理指定类型的效果
        /// </summary>
        /// <param name="effectType"></param>
        public void ClearEffectByType(EffectType effectType)
        {
            List<BaseEffect> clearList = new List<BaseEffect>();
            for (int i = 0; i < AllEffectList.Count; i++)
            {
                if (AllEffectList.Count == 0 || i > AllEffectList.Count)
                    break;
                var effect = AllEffectList[i];
                if (effect == null || effect.isEnd)
                    continue;
                if (effect.IsType(effectType))
                {
                    clearList.Add(effect);
                }
            }

            for (int i = clearList.Count - 1; i >= 0; i--)
            {
                ClearEffect(clearList[i], false);
            }

            UpdateState();
        }

        /// <summary>
        /// 清理指定效果
        /// </summary>
        /// <param name="effect">效果</param>
        /// <param name="isNeedUpdateState">是否需要更新状态。默认要，如果不false，需要手动更新</param>
        public void ClearEffect(BaseEffect effect, bool isNeedUpdateState = true)
        {
            if (effect == null || effect.isEnd)
            {
                return;
            }

            if (!effect.IsNeedAddList())
            {
                if (!Owner.DispatchEvent(Battle, Event.EndPre, EventParams<BaseEffect>.Create(Battle, effect)).IsBlockEvent)
                {
                    WaitEffectList.Add(effect);
                    effect.isEnd = true;
                    effect.OnEnd();
                    Owner.DispatchEvent(Battle, Event.End, EventParams<BaseEffect>.Create(Battle, effect));
                    Battle.DispatchEvent(Battle, Event.End, EventTwoParams<BattleObject, BaseEffect>.Create(Battle, Owner, effect));
                    if (isNeedUpdateState)
                    {
                        UpdateState();
                    }
                }
                return;
            }

            for (int i = 0; i < AllEffectList.Count; i++)
            {
                if (AllEffectList.Count == 0 || i > AllEffectList.Count)
                    break;
                var eff = AllEffectList[i];
                if (eff == null || eff.isEnd || eff.uid != effect.uid || eff.sid != effect.sid)
                    continue;
                if (!Owner.DispatchEvent(Battle, Event.EndPre, EventParams<BaseEffect>.Create(Battle, eff)).IsBlockEvent)
                {
                    if (_effectsLock)
                    {
                        _waitToRemoveEffectList.Add(eff);
                    }
                    else
                    {
                        AllEffectList.RemoveAt(i);
                    }
                    WaitEffectList.Add(eff);
                    eff.isEnd = true;
                    eff.OnEnd();
                    Owner.DispatchEvent(Battle, Event.End, EventParams<BaseEffect>.Create(Battle, eff));
                    Battle.DispatchEvent(Battle, Event.End, EventTwoParams<BattleObject, BaseEffect>.Create(Battle, Owner, eff));
                    if (isNeedUpdateState)
                    {
                        UpdateState();
                    }
                }

                break;
            }
        }
        /// <summary>
        /// 结束效果回池
        /// </summary>
        public void ReleaseEffect()
        {
            for (int i = 0, l = _waitToRemoveEffectList.Count; i < l; i++)
            {
                AllEffectList.Remove(_waitToRemoveEffectList[i]);
            }
            _waitToRemoveEffectList.Clear();

            if (WaitEffectList.Count > 0)
            {
                foreach (var effect in WaitEffectList)
                {
                    Owner.Battle.Pool.Release(effect);
                }
                WaitEffectList.Clear();
            }
        }

        /// <summary>
        /// 清理所有效果
        /// </summary>
        public void ClearAll(params EffectType[] excludeTypes)
        {
            ReleaseEffect();

            List<BaseEffect> clearList = new List<BaseEffect>();
            for (int i = 0; i < AllEffectList.Count; i++)
            {
                if (AllEffectList.Count == 0 || i > AllEffectList.Count)
                    break;
                var effect = AllEffectList[i];
                if (effect == null || effect.isEnd)
                    continue;
                var isClear = true;
                if (excludeTypes != null)
                {
                    foreach (var type in excludeTypes)
                    {
                        if (!effect.IsType(type))
                            continue;
                        isClear = false;
                        break;
                    }
                }
                if (isClear)
                {
                    clearList.Add(effect);
                }
            }

            for (int i = clearList.Count - 1; i >= 0; i--)
            {
                var toClearEffect = clearList[i];
                var endEffects = toClearEffect.endEffects;
                toClearEffect.endEffects = null; //ClearAllEffect的时候，不要又触发“效果结束附加效果” by ww
                ClearEffect(toClearEffect, false);
                toClearEffect.endEffects = endEffects;
            }

            UpdateState();
        }

        /// <summary>
        /// 通过效果类型，实时更新战斗者状态
        /// </summary>
        public void UpdateState()
        {
            mAllEffectTypeMask = EffectType.Null;
            //var newStateMark = BattleObject.State.Normal;
            for (int i = 0; i < AllEffectList.Count; i++)
            {
                if (AllEffectList.Count == 0 || i > AllEffectList.Count)
                    break;
                var effect = AllEffectList[i];
                if (effect == null || effect.isEnd)
                    continue;
                TryAddState(effect);
                if (effect.effectTypeMark != EffectType.Null)
                {
                    mAllEffectTypeMask |= effect.effectTypeMark;
                }
            }

            //TryRemoveState(BattleObject.State.Shield);
            TryRemoveState(BattleObject.State.Poison);
            TryRemoveState(BattleObject.State.Bleed);
            TryRemoveState(BattleObject.State.Burn);
            TryRemoveState(BattleObject.State.Frozen);
            TryRemoveState(BattleObject.State.Dizzy);
            TryRemoveState(BattleObject.State.Silence);
            TryRemoveState(BattleObject.State.Paralysis);
            TryRemoveState(BattleObject.State.UnableAddAnger);
            TryRemoveState(BattleObject.State.Relive);
            TryRemoveState(BattleObject.State.Disarm);
            TryRemoveState(BattleObject.State.Conduction);
            TryRemoveState(BattleObject.State.UnableAddShield);
            TryRemoveState(BattleObject.State.IgnoreImmuneDead);
            TryRemoveState(BattleObject.State.Taunte);
            TryRemoveState(BattleObject.State.Isolate);
            TryRemoveState(BattleObject.State.ActAgain);
            TryRemoveState(BattleObject.State.Arrow);
            TryRemoveState(BattleObject.State.IgnoreRelive);
            TryRemoveState(BattleObject.State.ShenZhu);
            TryRemoveState(BattleObject.State.ShenGeSuiPian);
            TryRemoveState(BattleObject.State.ShenWei);
            TryRemoveState(BattleObject.State.CannotSelectByRageSkill);
            TryRemoveState(BattleObject.State.NuJi);
            TryRemoveState(BattleObject.State.ZhenShi);
            TryRemoveState(BattleObject.State.ImmuneDamage);
            TryRemoveState(BattleObject.State.YuWei);
            TryRemoveState(BattleObject.State.Weakness);
            TryRemoveState(BattleObject.State.ShengSiYuGong);
        }

        /// <summary>
        /// 尝试移除指定英雄状态
        /// </summary>
        /// <param name="state">英雄状态</param>
        private void TryRemoveState(BattleObject.State state)
        {
            if (Owner.IsState(state) && !IsState(state))
            {
                Owner.RemoveState(state, 0);
            }
        }

        /// <summary>
        /// 通过效果类型，尝试给英雄加状态
        /// </summary>
        /// <param name="effect"></param>
        private void TryAddState(BaseEffect effect)
        {
            if (!effect.typeDirty)
                return;
            effect.typeDirty = false; //避免重复标记

            //if (effect.IsType(EffectType.Shield))
            //{
            //    Owner.AddState(EffectTypeToState(EffectType.Shield), effect.authorUid);
            //}
            if (effect.IsType(EffectType.Poison))
            {
                Owner.AddState(EffectTypeToState(EffectType.Poison), effect.authorUid);
            }
            if (effect.IsType(EffectType.Bleed))
            {
                Owner.AddState(EffectTypeToState(EffectType.Bleed), effect.authorUid);
            }
            if (effect.IsType(EffectType.Burn))
            {
                Owner.AddState(EffectTypeToState(EffectType.Burn), effect.authorUid);
            }
            if (effect.IsType(EffectType.Frozen))
            {
                Owner.AddState(EffectTypeToState(EffectType.Frozen), effect.authorUid);
            }
            if (effect.IsType(EffectType.Dizzy))
            {
                Owner.AddState(EffectTypeToState(EffectType.Dizzy), effect.authorUid);
            }
            if (effect.IsType(EffectType.Silence))
            {
                Owner.AddState(EffectTypeToState(EffectType.Silence), effect.authorUid);
            }
            if (effect.IsType(EffectType.Paralysis))
            {
                Owner.AddState(EffectTypeToState(EffectType.Paralysis), effect.authorUid);
            }
            if (effect.IsType(EffectType.UnableAddAnger))
            {
                Owner.AddState(EffectTypeToState(EffectType.UnableAddAnger), effect.authorUid);
            }
            if (effect.IsType(EffectType.Relive))
            {
                Owner.AddState(EffectTypeToState(EffectType.Relive), effect.authorUid,effect.sid);
            }
            if (effect.IsType(EffectType.Disarm))
            {
                Owner.AddState(EffectTypeToState(EffectType.Disarm), effect.authorUid);
            }
            if (effect.IsType(EffectType.Conduction))
            {
                Owner.AddState(EffectTypeToState(EffectType.Conduction), effect.authorUid);
            }
            if (effect.IsType(EffectType.UnableAddShield))
            {
                Owner.AddState(EffectTypeToState(EffectType.UnableAddShield), effect.authorUid);
            }
            if (effect.IsType(EffectType.IgnoreImmuneDead))
            {
                Owner.AddState(EffectTypeToState(EffectType.IgnoreImmuneDead), effect.authorUid);
            }
            if (effect.IsType(EffectType.Taunte))
            {
                Owner.AddState(EffectTypeToState(EffectType.Taunte), effect.authorUid);
            }
            if (effect.IsType(EffectType.Isolate))
            {
                Owner.AddState(EffectTypeToState(EffectType.Isolate), effect.authorUid);
            }
            if (effect.IsType(EffectType.ActAgain))
            {
                Owner.AddState(EffectTypeToState(EffectType.ActAgain), effect.authorUid);
            }
            if (effect.IsType(EffectType.Arrow))
            {
                Owner.AddState(EffectTypeToState(EffectType.Arrow), effect.authorUid);
            }
            if (effect.IsType(EffectType.IgnoreRelive))
            {
                Owner.AddState(EffectTypeToState(EffectType.IgnoreRelive), effect.authorUid);
            }
            if (effect.IsType(EffectType.ShenZhu))
            {
                Owner.AddState(EffectTypeToState(EffectType.ShenZhu), effect.authorUid);
            }
            if (effect.IsType(EffectType.ShenGeSuiPian))
            {
                Owner.AddState(EffectTypeToState(EffectType.ShenGeSuiPian), effect.authorUid);
            }
            if (effect.IsType(EffectType.ShenWei))
            {
                Owner.AddState(EffectTypeToState(EffectType.ShenWei), effect.authorUid);
            }
            if (effect.IsType(EffectType.CannotSelectByRageSkill))
            {
                Owner.AddState(EffectTypeToState(EffectType.CannotSelectByRageSkill), effect.authorUid);
            }
            if (effect.IsType(EffectType.NuJi))
            {
                Owner.AddState(EffectTypeToState(EffectType.NuJi), effect.authorUid);
            }
            if (effect.IsType(EffectType.ZhenShi))
            {
                Owner.AddState(EffectTypeToState(EffectType.ZhenShi), effect.authorUid);
            }
            if (effect.IsType(EffectType.ImmuneDamage))
            {
                Owner.AddState(EffectTypeToState(EffectType.ImmuneDamage), effect.authorUid, effect.sid);
            }
            if (effect.IsType(EffectType.YuWei))
            {
                Owner.AddState(EffectTypeToState(EffectType.YuWei), effect.authorUid);
            }
            if (effect.IsType(EffectType.Weakness))
            {
                Owner.AddState(EffectTypeToState(EffectType.Weakness), effect.authorUid);
            }
            if (effect.IsType(EffectType.ShengSiYuGong))
            {
                Owner.AddState(EffectTypeToState(EffectType.ShengSiYuGong), effect.authorUid);
            }
            if (effect.IsType(EffectType.ImmuneDead))
            {
                Owner.AddState(EffectTypeToState(EffectType.ImmuneDead), effect.authorUid, effect.sid);
            }
            if (effect.IsType(EffectType.LongZiYiGui))
            {
                Owner.AddState(EffectTypeToState(EffectType.LongZiYiGui), effect.authorUid);
            }
            if (effect.IsType(EffectType.LieBieZhiZu))
            {
                Owner.AddState(EffectTypeToState(EffectType.LieBieZhiZu), effect.authorUid);
            }
        }

        /// <summary>
        /// 是否包括某个效果类型
        /// </summary>
        /// <param name="effectType">效果类型</param>
        /// <returns></returns>
        public bool IsType(EffectSys.EffectType effectType)
        {
            return (mAllEffectTypeMask & effectType) != 0;
        }

        /// <summary>
        /// 是否包含某个英雄状态
        /// </summary>
        /// <param name="state">英雄状态</param>
        /// <returns></returns>
        public bool IsState(BattleObject.State state)
        {
            return (mAllEffectTypeMask & StateToEffectType(state)) != 0;
        }

        /// <summary>
        /// 英雄状态转效果类型
        /// </summary>
        /// <param name="state">英雄状态</param>
        /// <returns>效果类型</returns>
        public static EffectType StateToEffectType(BattleObject.State state)
        {
            switch (state)
            {
                case BattleObject.State.Dizzy:
                    return EffectType.Dizzy;
                case BattleObject.State.Silence:
                    return EffectType.Silence;
                case BattleObject.State.Poison:
                    return EffectType.Poison;
                case BattleObject.State.Frozen:
                    return EffectType.Frozen;
                case BattleObject.State.Bleed:
                    return EffectType.Bleed;
                case BattleObject.State.Burn:
                    return EffectType.Burn;
                case BattleObject.State.Paralysis:
                    return EffectType.Paralysis;
                case BattleObject.State.Shield:
                    return EffectType.Shield;
                case BattleObject.State.UnableAddAnger:
                    return EffectType.UnableAddAnger;
                case BattleObject.State.Relive:
                    return EffectType.Relive;
                case BattleObject.State.Disarm:
                    return EffectType.Disarm;
                case BattleObject.State.Conduction:
                    return EffectType.Conduction;
                case BattleObject.State.UnableAddShield:
                    return EffectType.UnableAddShield;
                case BattleObject.State.IgnoreImmuneDead:
                    return EffectType.IgnoreImmuneDead;
                case BattleObject.State.Taunte:
                    return EffectType.Taunte;
                case BattleObject.State.Isolate:
                    return EffectType.Isolate;
                case BattleObject.State.ActAgain:
                    return EffectType.ActAgain;
                case BattleObject.State.Arrow:
                    return EffectType.Arrow;
                case BattleObject.State.IgnoreRelive:
                    return EffectType.IgnoreRelive;
                case BattleObject.State.ShenZhu:
                    return EffectType.ShenZhu;
                case BattleObject.State.ShenGeSuiPian:
                    return EffectType.ShenGeSuiPian;
                case BattleObject.State.ShenWei:
                    return EffectType.ShenWei;
                case BattleObject.State.CannotSelectByRageSkill:
                    return EffectType.CannotSelectByRageSkill;
                case BattleObject.State.NuJi:
                    return EffectType.NuJi;
                case BattleObject.State.ZhenShi:
                    return EffectType.ZhenShi;
                case BattleObject.State.ImmuneDamage:
                    return EffectType.ImmuneDamage;
                case BattleObject.State.YuWei:
                    return EffectType.YuWei;
                case BattleObject.State.Weakness:
                    return EffectType.Weakness;
                case BattleObject.State.ShengSiYuGong:
                    return EffectType.ShengSiYuGong;
                case BattleObject.State.LongZiYiGui:
                    return EffectType.LongZiYiGui;
                case BattleObject.State.LieBieZhiZu:
                    return EffectType.LieBieZhiZu;
                default:
                    return EffectType.Null;
            }
        }

        /// <summary>
        /// 效果类型转英雄状态
        /// </summary>
        /// <param name="effectType">效果类型</param>
        /// <returns>英雄状态</returns>
        public static BattleObject.State EffectTypeToState(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.Shield:
                    return BattleObject.State.Shield;
                case EffectType.Poison:
                    return BattleObject.State.Poison;
                case EffectType.Bleed:
                    return BattleObject.State.Bleed;
                case EffectType.Burn:
                    return BattleObject.State.Burn;
                case EffectType.Frozen:
                    return BattleObject.State.Frozen;
                case EffectType.Dizzy:
                    return BattleObject.State.Dizzy;
                case EffectType.Silence:
                    return BattleObject.State.Silence;
                case EffectType.Paralysis:
                    return BattleObject.State.Paralysis;
                case EffectType.UnableAddAnger:
                    return BattleObject.State.UnableAddAnger;
                case EffectType.Relive:
                    return BattleObject.State.Relive;
                case EffectType.Disarm:
                    return BattleObject.State.Disarm;
                case EffectType.Conduction:
                    return BattleObject.State.Conduction;
                case EffectType.UnableAddShield:
                    return BattleObject.State.UnableAddShield;
                case EffectType.IgnoreImmuneDead:
                    return BattleObject.State.IgnoreImmuneDead;
                case EffectType.Taunte:
                    return BattleObject.State.Taunte;
                case EffectType.Isolate:
                    return BattleObject.State.Isolate;
                case EffectType.ActAgain:
                    return BattleObject.State.ActAgain;
                case EffectType.Arrow:
                    return BattleObject.State.Arrow;
                case EffectType.IgnoreRelive:
                    return BattleObject.State.IgnoreRelive;
                case EffectType.ShenZhu:
                    return BattleObject.State.ShenZhu;
                case EffectType.ShenGeSuiPian:
                    return BattleObject.State.ShenGeSuiPian;
                case EffectType.ShenWei:
                    return BattleObject.State.ShenWei;
                case EffectType.CannotSelectByRageSkill:
                    return BattleObject.State.CannotSelectByRageSkill;
                case EffectType.NuJi:
                    return BattleObject.State.NuJi;
                case EffectType.ZhenShi:
                    return BattleObject.State.ZhenShi;
                case EffectType.ImmuneDamage:
                    return BattleObject.State.ImmuneDamage;
                case EffectType.YuWei:
                    return BattleObject.State.YuWei;
                case EffectType.Weakness:
                    return BattleObject.State.Weakness;
                case EffectType.ShengSiYuGong:
                    return BattleObject.State.ShengSiYuGong;
                case EffectType.ImmuneDead:
                    return BattleObject.State.ImmuneDead;
                case EffectType.LongZiYiGui:
                    return BattleObject.State.LongZiYiGui;
                case EffectType.LieBieZhiZu:
                    return BattleObject.State.LieBieZhiZu;
                default:
                    return BattleObject.State.Normal;
            }
        }

        /// <summary>
        /// 效果类型 最大支持到 1UL<<62
        /// </summary>
        [Flags]
        public enum EffectType : ulong
        {
            [Description("无,忽略")]
            Null = 0,
            /// <summary>
            /// 叠加层级
            /// </summary>
            [Description("叠加")]
            Overlays = 1UL << 0,
            /// <summary>
            /// 覆盖
            /// </summary>
            [Description("覆盖")]
            Override = 1UL << 1,
            /// <summary>
            /// 大回合开始时触发一次
            /// </summary>
            [Description("大回合开始时触发一次")]
            BoutStart = 1UL << 2,
            /// <summary>
            /// 大回合结束时触发一次
            /// </summary>
            [Description("大回合结束时触发一次")]
            BoutEnd = 1UL << 3,
            /// <summary>
            /// 小回合开始触发一次
            /// </summary>
            [Description("小回合开始触发一次")]
            RoundStart = 1UL << 4,
            /// <summary>
            /// 小回合结束触发一次
            /// </summary>
            [Description("小回合结束触发一次")]
            RoundEnd = 1UL << 5,
            /// <summary>
            /// 死亡后是否保留
            /// </summary>
            [Description("死后保留")]
            DieRetain = 1UL << 6,
            /// <summary>
            /// 护盾
            /// </summary>
            [Description("护盾")]
            Shield = 1UL << 7,
            /// <summary>
            /// 增益效果(属性提高)
            /// </summary>
            [Description("增益效果(属性提高)")]
            Buff = 1UL << 8,
            /// <summary>
            /// 减益效果(属性降低)
            /// </summary>
            [Description("减益效果(属性降低)")]
            DeBuff = 1UL << 9,
            /// <summary>
            /// 中毒
            /// </summary>
            [Description("中毒")]
            Poison = 1UL << 10,
            /// <summary>
            /// 流血
            /// </summary>
            [Description("流血")]
            Bleed = 1UL << 11,
            /// <summary>
            /// 灼烧
            /// </summary>
            [Description("灼烧")]
            Burn = 1UL << 12,
            /// <summary>
            /// 冰冻
            /// </summary>
            [Description("冰冻")]
            Frozen = 1UL << 13,
            /// <summary>
            /// 眩晕
            /// </summary>
            [Description("眩晕")]
            Dizzy = 1UL << 14,
            /// <summary>
            /// 沉默
            /// </summary>
            [Description("沉默")]
            Silence = 1UL << 15,
            /// <summary>
            /// 麻痹
            /// </summary>
            [Description("麻痹")]
            Paralysis = 1UL << 16,
            /// <summary>
            /// 免疫伤害
            /// </summary>
            [Description("免疫伤害")]
            ImmuneDamage = 1UL << 17,
            /// <summary>
            /// 无法回怒
            /// </summary>
            [Description("无法回怒")]
            UnableAddAnger = 1UL << 18,
            /// <summary>
            /// 复活
            /// </summary>
            [Description("复活")]
            Relive = 1UL << 19,
            /// <summary>
            /// 缴械
            /// </summary>
            [Description("缴械")]
            Disarm = 1UL << 20,
            /// <summary>
            /// 传导
            /// </summary>
            [Description("传导")]
            Conduction = 1UL << 21,
            /// <summary>
            /// 无法获得护盾
            /// </summary>
            [Description("无法获得护盾")]
            UnableAddShield = 1UL << 22,
            /// <summary>
            /// 无视免死
            /// </summary>
            [Description("无视免死")]
            IgnoreImmuneDead = 1UL << 23,
            /// <summary>
            /// 嘲讽
            /// </summary>
            [Description("嘲讽")]
            Taunte = 1UL << 24,
            /// <summary>
            /// 孤立
            /// </summary>
            [Description("孤立")]
            Isolate = 1UL << 25,
            /// <summary>
            /// 再动
            /// </summary>
            [Description("再动")]
            ActAgain = 1UL << 26,
            /// <summary>
            /// 箭矢
            /// </summary>
            [Description("箭矢")]
            Arrow = 1UL << 27,
            /// <summary>
            /// 无法复活
            /// </summary>
            [Description("无法复活")]
            IgnoreRelive = 1UL << 28,
            /// <summary>
            /// 神主
            /// </summary>
            [Description("神主")]
            ShenZhu = 1UL << 29,
            /// <summary>
            /// 神格碎片
            /// </summary>
            [Description("神格碎片")]
            ShenGeSuiPian = 1UL << 30,
            /// <summary>
            /// 神威
            /// </summary>
            [Description("神威")]
            ShenWei = 1UL << 31,
            /// <summary>
            /// 无法被怒气技选中
            /// </summary>
            [Description("无法被怒气技选中")]
            CannotSelectByRageSkill = 1UL << 32,
            /// <summary>
            /// 怒击
            /// </summary>
            [Description("怒击")]
            NuJi = 1UL << 33,
            /// <summary>
            /// 真视
            /// </summary>
            [Description("真视")]
            ZhenShi = 1UL << 34,
            /// <summary>
            /// 余威
            /// </summary>
            [Description("余威")]
            YuWei = 1UL << 35,
            /// <summary>
            /// 弱点
            /// </summary>
            [Description("弱点")]
            Weakness = 1UL << 36,
            /// <summary>
            /// 生死与共
            /// </summary>
            [Description("生死与共")]
            ShengSiYuGong = 1UL << 37,
            /// <summary>
            /// 免疫死亡
            /// </summary>
            [Description("免疫死亡")]
            ImmuneDead = 1UL << 38,
            /// <summary>
            /// 龙子仪轨
            /// </summary>
            [Description("龙子仪轨")]
            LongZiYiGui = 1UL << 39,
            /// <summary>
            /// 裂碑之诅
            /// </summary>
            [Description("裂碑之诅")]
            LieBieZhiZu = 1UL << 40,

        }

        /// <summary>
        /// 效果事件
        /// </summary>
        public enum Event
        {
            [Description("无,忽略")]
            None = 0,
            /// <summary>
            /// 效果层级叠加之前
            /// </summary>
            [Description("效果层级叠加之前")]
            OverlayAddPre,
            /// <summary>
            /// 效果层级叠加
            /// </summary>
            [Description("效果层级叠加")]
            OverlayAdd,
            /// <summary>
            /// 层级减少 ps:现在用不上，因为不像魔兽世界流血那种叠加，这里回合数共同使用的，所以回合一到就清理了，不存在减少层级一说
            /// </summary>
            OverlayRed,
            /// <summary>
            /// 效果开始添加效果
            /// </summary>
            [Description("效果开始添加效果")]
            BeginPre,
            /// <summary>
            /// 效果开始(已经添加上)
            /// </summary>
            [Description("效果开始(已经添加上)")]
            Begin,
            /// <summary>
            /// 效果生效之前
            /// </summary>
            [Description("效果生效之前")]
            TriggerPre,
            /// <summary>
            /// 生效之后
            /// </summary>
            [Description("生效之后")]
            TriggerEnd,
            /// <summary>
            /// 效果结束之前
            /// </summary>
            [Description("效果结束之前")]
            EndPre,
            /// <summary>
            /// 效果结束
            /// </summary>
            [Description("效果结束")]
            End,
            /// <summary>
            /// 效果开始(已经添加上)或层级叠加
            /// </summary>
            [Description("效果开始(已经添加上)或层级叠加")]
            BeginOrOverlayAdd,
        }
    }
}
