using System.Collections.Generic;

namespace BattleSystem
{
    using State = BattleObject.State;

    /// <summary>
    /// 检测效果
    /// </summary>
    public class CheckEffect : BaseEffect
    {
        /// <summary>
        /// 通过给自己的效果
        /// </summary>
        public long[] passedSelfEffects;

        /// <summary>
        /// 通过给目标的效果
        /// </summary>
        public long[] passedTargetEffects;

        /// <summary>
        /// 未通过给自己的效果
        /// </summary>
        public long[] failedSelfEffects;

        /// <summary>
        /// 未通过给目标的效果
        /// </summary>
        public long[] failedTargetEffects;

        /// <summary>
        /// 检查条件
        /// </summary>
        public BattleDef.CheckType checkType;

        /// <summary>
        /// 扩展信息
        /// </summary>
        public double[] extraDouble;

        /// <summary>
        /// 扩展信息
        /// </summary>
        public long[] extraLong;

        #region 检测状态层数
        private EffectSys.EffectType _effectType;
        private int _stateOverlayCount = 0;
        private bool _stateOverlayCountFullFighter = false;
        //private Dictionary<BaseEffect, int> _effectOverlayCountDict = null;
        #endregion

        public override void OnBegin()
        {
            _stateOverlayCountFullFighter = true;
            _effectType = EffectSys.EffectType.Null;
            _stateOverlayCount = 0;
            if (checkType == BattleDef.CheckType.StateOverlayCount && extraLong != null && extraLong.Length > 0 && extraDouble != null &&
                extraDouble.Length > 0)
            {
                _stateOverlayCountFullFighter = true;
                if (extraLong.Length > 1)
                {
                    _stateOverlayCountFullFighter = extraLong[1] <= 0;
                }

                if (_stateOverlayCountFullFighter)
                {
                    battle.AddEvent(EffectSys.Event.Begin, OnEffectBegin, false, -100);
                    battle.AddEvent(EffectSys.Event.End, OnEffectEnd, false, -100);
                    battle.AddEvent(EffectSys.Event.OverlayAdd, OnEffectOverlayAdd, false, -100);
                    battle.AddEvent(EffectSys.Event.OverlayRed, OnEffectOverlayRed, false, -100);
                }
                else
                {
                    target.AddEvent(EffectSys.Event.Begin, OnEffectBegin, false, -100);
                    target.AddEvent(EffectSys.Event.End, OnEffectEnd, false, -100);
                    target.AddEvent(EffectSys.Event.OverlayAdd, OnEffectOverlayAdd, false, -100);
                    target.AddEvent(EffectSys.Event.OverlayRed, OnEffectOverlayRed, false, -100);
                }
            }

            base.OnBegin();
        }

        public override void OnEnd()
        {
            if (checkType == BattleDef.CheckType.StateOverlayCount && extraLong != null && extraLong.Length > 0 && extraDouble != null &&
                extraDouble.Length > 0)
            {
                if (_stateOverlayCountFullFighter)
                {
                    battle.RemoveEvent(EffectSys.Event.Begin, OnEffectBegin);
                    battle.RemoveEvent(EffectSys.Event.End, OnEffectEnd);
                    battle.RemoveEvent(EffectSys.Event.OverlayAdd, OnEffectOverlayAdd);
                    battle.RemoveEvent(EffectSys.Event.OverlayRed, OnEffectOverlayRed);
                }
                else
                {
                    target.RemoveEvent(EffectSys.Event.Begin, OnEffectBegin);
                    target.RemoveEvent(EffectSys.Event.End, OnEffectEnd);
                    target.RemoveEvent(EffectSys.Event.OverlayAdd, OnEffectOverlayAdd);
                    target.RemoveEvent(EffectSys.Event.OverlayRed, OnEffectOverlayRed);
                }
            }

            base.OnEnd();
        }

        public override void Trigger()
        {
            switch (checkType)
            {
                case BattleDef.CheckType.DebuffTypeNumAddFactors:
                    DebuffNumAddFactors(true);
                    break;
                case BattleDef.CheckType.DebuffNumAddFactors:
                    DebuffNumAddFactors(false);
                    break;
                case BattleDef.CheckType.RandomOneEffect:
                    RandomOneEffect();
                    break;
                case BattleDef.CheckType.DesignatedBout:
                    DesignatedBout();
                    break;
                case BattleDef.CheckType.StateOverlayCount:
                    StateOverlayCount();
                    break;
            }
        }

        #region 目标异常状态数量增加伤害基础百分比
        private HashSet<BattleDef.Property> _isAdd = new HashSet<BattleDef.Property>();
        private static readonly State[] _debuffStates;
        //private static readonly State _debuffStateType;
        private static readonly EffectSys.EffectType _debuffEffectType;

        static CheckEffect()
        {
            _debuffStates = EffectSys.debuffStates;
            _debuffEffectType = EffectSys.allDebuffEffectType & ~EffectSys.EffectType.DeBuff;
        }

        void DebuffNumAddFactors(bool careType = true)
        {
            if(extraDouble.Length== 0)
            {
                Log.error("目标异常状态数量增加伤害基础百分比 配置错误");
                return;
            }
            int num = 0;
            if (careType)
            {
                //target.IsState(_debuffStateType) //需要计算个数
                foreach (var state in _debuffStates)
                {
                    if (target.IsState(state))
                    {
                        num++;
                    }
                }
            }

            _isAdd.Clear();
            var effectList = target.Effect.AllEffectList;
            foreach (var effect in effectList)
            {
                bool added = false;
                if (effect is PropertyEffect propertyEffect)
                {
                    if (propertyEffect.IsType(EffectSys.EffectType.DeBuff))
                    {
                        if (!careType || _isAdd.Add(propertyEffect.property))
                        {
                            num++;
                            added = true;
                        }
                    }
                }

                if (!added)
                {
                    if (!careType && effect.IsType(_debuffEffectType))
                    {
                        num++;
                    }
                }
            }

            if (num > 0)
            {
                author.Effect.AddEffects(passedSelfEffects, authorUid, effectLevel);
                if (passedTargetEffects != null)
                {
                    foreach (var sid in passedTargetEffects)
                    {
                        var effect = target.Effect.CreateEffect(sid, authorUid, effectLevel);
                        if (effect != null && effect is DamageEffect damageEffect)
                        {
                            for (int i = 0; i < damageEffect.factors.Length; i++)
                            {
                                damageEffect.otherFactor = extraDouble[0] * num;
                            }
                        }
                        target.Effect.AddEffect(effect);
                    }
                }
            }
            else
            {
                author.Effect.AddEffects(passedSelfEffects, authorUid, effectLevel);
                target.Effect.AddEffects(passedTargetEffects, authorUid, effectLevel);
            }
        }
        #endregion

        #region 随机添加一个效果

        private void RandomOneEffect()
        {
            if (passedSelfEffects?.Length > 0)
            {
                int index = (int)battle.Random.RandomValue(0, passedSelfEffects.Length - 1);
                author.Effect.AddEffect(passedSelfEffects[index], authorUid, effectLevel, out _);
            }

            if (passedTargetEffects?.Length > 0)
            {
                int index = (int)battle.Random.RandomValue(0, passedTargetEffects.Length - 1);
                target.Effect.AddEffect(passedTargetEffects[index], authorUid, effectLevel, out _);
            }
        }

        #endregion

        #region 指定回合释放
        private void DesignatedBout()
        {
            if (extraLong.Length == 0)
            {
                Log.error("指定回合触发 未配置指定回合");
                return;
            }
            for (int i = 0; i < extraLong.Length; i++)
            {
                if (battle.Bout == extraLong[i])
                {
                    author.Effect.AddEffects(passedSelfEffects, authorUid, effectLevel);
                    target.Effect.AddEffects(passedTargetEffects, authorUid, effectLevel);
                    return;
                }
            }
        }
        #endregion

        #region 检测状态层数
        private void OnEffectChanged(EventParams arg0, EffectSys.Event effectEvent)
        {
            BaseEffect effect = null;
            if (_stateOverlayCountFullFighter)
            {
                if (!(arg0 is EventTwoParams<BattleObject, BaseEffect> effectArg))
                {
                    return;
                }

                if (!(effectArg.data1 is Fighter fighter) || fighter.CampType != target.CampType)
                {
                    return;
                }

                effect = effectArg.data2;
            }
            else
            {
                if (!(arg0 is EventParams<BaseEffect> effectArg))
                {
                    return;
                }

                effect = effectArg.data;
            }

            if (effect == null || !effect.IsType(_effectType))
            {
                return;
            }

            var oldOverlayCount = _stateOverlayCount;
            switch (effectEvent)
            {
                case EffectSys.Event.Begin:
                {
                    _stateOverlayCount += effect.overlayCount;
                    break;
                }

                case EffectSys.Event.End:
                {
                    _stateOverlayCount = System.Math.Max(0, _stateOverlayCount - effect.overlayCount);
                    break;
                }

                case EffectSys.Event.OverlayAdd:
                {
                    _stateOverlayCount++;
                    break;
                }

                case EffectSys.Event.OverlayRed:
                {
                    _stateOverlayCount = System.Math.Max(0, _stateOverlayCount - 1);
                    break;
                }

                default:
                    return;
            }

            if (oldOverlayCount == _stateOverlayCount)
            {
                return;
            }

            CheckStateOverlayCount();
        }

        private void OnEffectBegin(EventParams arg0)
        {
            OnEffectChanged(arg0, EffectSys.Event.Begin);
        }

        private void OnEffectEnd(EventParams arg0)
        {
            OnEffectChanged(arg0, EffectSys.Event.End);
        }

        private void OnEffectOverlayAdd(EventParams arg0)
        {
            OnEffectChanged(arg0, EffectSys.Event.OverlayAdd);
        }

        private void OnEffectOverlayRed(EventParams arg0)
        {
            OnEffectChanged(arg0, EffectSys.Event.OverlayRed);
        }

        private void CheckStateOverlayCount()
        {
            var count = (int)extraDouble[0];
            if (count <= 0)
            {
                Log.error("检测状态层数 不能配置0层哦！！！（扩展信息（小数）0）");
                return;
            }

            BaseBattle.EffectAdditionData additionData = null;
            while (_stateOverlayCount >= count)
            {
                _stateOverlayCount -= count;
                if (additionData == null)
                {
                    additionData = AllocAdditionData();
                    additionData.specificTarget = target;
                }

                author.Effect.AddEffects(passedSelfEffects, authorUid, effectLevel, additionData);
                target.Effect.AddEffects(passedTargetEffects, authorUid, effectLevel, additionData);
            }
        }

        private void StateOverlayCount()
        {
            if (extraLong.Length <= 0)
            {
                Log.error("检测状态层数 未配置状态");
                return;
            }

            if (!(target is Fighter targetFighter))
            {
                return;
            }

            // if (_effectOverlayCountDict == null)
            // {
            //     _effectOverlayCountDict = new();
            // }

            _stateOverlayCount = 0;
            // var state = EffectSys.EffectType.Null;
            // for (int i = 0, l = extraLong.Length; i < l; i++)
            // {
            //     state |= EffectSys.StateToEffectType((State)extraLong[i]);
            // }

            var state = EffectSys.StateToEffectType((State)extraLong[0]);
            if (state == EffectSys.EffectType.Null)
            {
                return;
            }

            _effectType = state;
            if (_stateOverlayCountFullFighter)
            {
                var teamFighters = targetFighter.TeamData.AllFighters;
                for (int i = 0, l = teamFighters.Count; i < l; i++)
                {
                    var teamFighter = teamFighters[i];
                    if (teamFighter.IsDead && teamFighter.IsDoDeadTag)
                    {
                        continue;
                    }

                    var allEffectList = teamFighter.Effect.AllEffectList;
                    for (int j = 0, jl = allEffectList.Count; j < jl; j++)
                    {
                        var effect = allEffectList[j];
                        if (effect == null)
                        {
                            continue;
                        }

                        if (effect.IsType(state))
                        {
                            _stateOverlayCount += effect.overlayCount;
                        }
                    }
                }
            }
            else
            {
                if (!targetFighter.IsDead && !targetFighter.IsDoDeadTag)
                {
                    var allEffectList = targetFighter.Effect.AllEffectList;
                    for (int j = 0, jl = allEffectList.Count; j < jl; j++)
                    {
                        var effect = allEffectList[j];
                        if (effect == null)
                        {
                            continue;
                        }

                        if (effect.IsType(state))
                        {
                            _stateOverlayCount += effect.overlayCount;
                        }
                    }
                }
            }

            CheckStateOverlayCount();
        }
        #endregion
    }
}