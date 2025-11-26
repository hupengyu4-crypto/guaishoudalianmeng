using System;
using System.Configuration;

namespace BattleSystem
{
    /// <summary>
    /// 检测事件效果
    /// </summary>
    public class CheckEventEffect : BaseEffect
    {
        /// <summary>
        /// 通过给自己的效果
        /// </summary>
        public long[] passedSelfEffects;
        /// <summary>
        /// 通过给索敌目标的效果
        /// </summary>
        public long[] passedFindTargetEffects;
        /// <summary>
        /// 通过给造成目标事件的人的效果
        /// </summary>
        public long[] passedTargetEffects;
        /// <summary>
        /// 角色事件
        /// </summary>
        public BattleObject.Event fighterEvent;
        /// <summary>
        /// 战场事件
        /// </summary>
        public BaseBattle.Event battleEvent;
        /// <summary>
        /// 角色效果事件
        /// </summary>
        public EffectSys.Event effectEvent;
        /// <summary>
        /// 扩展信息
        /// </summary>
        public double extraOne;
        /// <summary>
        /// 扩展信息2
        /// </summary>
        public double extraTwo;
        /// <summary>
        /// 扩展信息3
        /// </summary>
        public double extraThree;
        /// <summary>
        /// 是否停止监听事件执行
        /// </summary>
        public bool isStopEvent;
        /// <summary>
        /// 大回合内最多检测几次
        /// </summary>
        public int boutCheckCount;
        /// <summary>
        /// 小回合内最多检测几次
        /// </summary>
        public int roundCheckCount;
        /// <summary>
        /// 通过几次后删除自身
        /// </summary>
        public int passCount;
        /// <summary>
        /// 满足几次后才通过
        /// </summary>
        public int checkCount;

        private bool _damageEventTrigger; //防止后续的DamageEffect又触发damage事件
        private int _doNormalSkillCount = 0;
        private int _boutCheckCount = 0;
        private int _roundCheckCount = 0;
        private int _passCount = 0;
        private int _checkCount = 0;

        public override void OnBegin()
        {
            _doNormalSkillCount = 0;
            _damageEventTrigger = false;
            _boutCheckCount = 0;
            _roundCheckCount = 0;
            _passCount = 0;
            _checkCount = 0;

            if (fighterEvent != BattleObject.Event.None)
            {
                target.AddEvent(fighterEvent, CheckHandler, false, 1);
            }

            if (battleEvent != BaseBattle.Event.None)
            {
                battle.AddEvent(battleEvent, CheckHandler, false, 1);
            }

            if (effectEvent != EffectSys.Event.None)
            {
                target.AddEvent(effectEvent, CheckHandler, false, 1);
            }

            if (boutCheckCount > 0)
            {
                battle.AddEvent(BaseBattle.Event.BoutStart, OnAnyBoutStart, false, 1);
            }

            if (roundCheckCount > 0)
            {
                battle.AddEvent(BaseBattle.Event.RoundStart, OnAnyRoundStart, false, 1);
            }

            base.OnBegin();
        }

        private void CheckHandler(EventParams arg0)
        {
            if (!CheckPass(arg0))
            {
                return;
            }

            //先判断小回合，这里return了就不加大回合次数了
            if (roundCheckCount > 0 && ++_roundCheckCount > roundCheckCount)
            {
                return;
            }

            if (boutCheckCount > 0 && ++_boutCheckCount > boutCheckCount)
            {
                return;
            }

            bool isPass = true;//理论上监听了都要通过
            BaseBattle.EffectAdditionData additionData = null;

            //造成目标事件的人
            Fighter makeEventFighter = null;
            var damageEventTrigger = false;

            if (fighterEvent != BattleObject.Event.None)
            {
                switch (fighterEvent)
                {
                    case BattleObject.Event.AttackDamagePre:
                    case BattleObject.Event.AttackDamage:
                    case BattleObject.Event.Crit:
                    case BattleObject.Event.BeBlock:
                    case BattleObject.Event.NormalDamage:
                    case BattleObject.Event.NormalCritDamage:
                    case BattleObject.Event.SkillDamage:
                    case BattleObject.Event.SkillCritDamage:
                    case BattleObject.Event.DeBuffDamage:
                    //case BattleObject.Event.IndirectDamagePre:
                    case BattleObject.Event.IndirectDamage:
                        if (arg0 is DamageParams param)
                        {
                            damageEventTrigger = true;
                            makeEventFighter = battle.GetSceneObject<Fighter>(param.defendUid);//监听者，攻击了 目标，所以为了给目标加效果，就要修正目标为受到攻击的人
                            if (extraThree == 999 && _additionData != null && _additionData.specificTarget != null)
                            {
                                additionData = _additionData;
                            }
                            else
                            {
                                additionData = AllocAdditionData(additionData);
                                additionData.specificTarget = target;
                            }
                        }
                        break;
                    case BattleObject.Event.BeCrit:
                    case BattleObject.Event.Block:
                    case BattleObject.Event.BeDamagePre:
                    case BattleObject.Event.BeDamage:
                    case BattleObject.Event.BeNormalDamage:
                    case BattleObject.Event.BeNormalCritDamage:
                    case BattleObject.Event.BeSkillDamage:
                    case BattleObject.Event.BeSkillCritDamage:
                    case BattleObject.Event.BeDeBuffDamage:
                    //case BattleObject.Event.BeIndirectDamagePre:
                    case BattleObject.Event.BeIndirectDamage:
                        if (arg0 is DamageParams param2)
                        {
                            damageEventTrigger = true;
                            makeEventFighter = battle.GetSceneObject<Fighter>(param2.attackUid);//目标 攻击了 监听者，所以为了给目标加效果，就要修正目标为攻击的人
                        }
                        break;
                    case BattleObject.Event.Kill:
                        if (arg0 is EventTwoParams<Fighter, PropParams> beKiller)
                        {
                            makeEventFighter = battle.GetSceneObject<Fighter>(beKiller.data1.Uid);//监听者，击杀了 谁
                        }
                        break;
                    case BattleObject.Event.BeKill:
                    case BattleObject.Event.DeadPre:
                        if (arg0 != null && arg0 is EventTwoParams<Fighter, PropParams> killer)
                        {
                            if (killer != null && killer.data1 != null)
                                makeEventFighter = battle.GetSceneObject<Fighter>(killer.data1.Uid);//谁 击杀了 监听者
                        }
                        break;
                }
            }

            if (extraOne > 0)
            {
                isPass = false;//理论上有要求，默认都不通过
                if (fighterEvent != BattleObject.Event.None)
                {
                    switch (fighterEvent)
                    {
                        case BattleObject.Event.DoSkill:
                            //值1是技能Sid
                            isPass = arg0 is EventParams<BattleSkill> skill && skill.data.Sid != (long)extraOne;
                            break;
                        case BattleObject.Event.AddState:
                        case BattleObject.Event.RemoveState:
                            //值1是状态枚举值
                            isPass = arg0 is FighterStateParams stateParams && ((int)extraOne & (int)stateParams.state) != 0;
                            break;
                        case BattleObject.Event.Prop: //下面处理
                            break;
                        case BattleObject.Event.ImmuneDamageTrigger:
                            isPass = arg0 is EventParams<ImmuneDamageType> immuneDamageParams && (int)extraOne == (int)immuneDamageParams.data;
                            break;
                        case BattleObject.Event.AttackDamagePre:
                        case BattleObject.Event.AttackDamage:
                        case BattleObject.Event.Crit:
                        case BattleObject.Event.BeBlock:
                        case BattleObject.Event.NormalDamage:
                        case BattleObject.Event.NormalCritDamage:
                        case BattleObject.Event.SkillDamage:
                        case BattleObject.Event.SkillCritDamage:
                        case BattleObject.Event.DeBuffDamage:
                        //case BattleObject.Event.IndirectDamagePre:
                        case BattleObject.Event.IndirectDamage:
                        case BattleObject.Event.BeCrit:
                        case BattleObject.Event.Block:
                        case BattleObject.Event.BeDamagePre:
                        case BattleObject.Event.BeDamage:
                        case BattleObject.Event.BeNormalDamage:
                        case BattleObject.Event.BeNormalCritDamage:
                        case BattleObject.Event.BeSkillDamage:
                        case BattleObject.Event.BeSkillCritDamage:
                        case BattleObject.Event.BeDeBuffDamage:
                        //case BattleObject.Event.BeIndirectDamagePre:
                        case BattleObject.Event.BeIndirectDamage:
                        {
                            if (arg0 is DamageParams damageParams)
                            {
                                isPass = (int)damageParams.damageType == (int)extraOne - 1;

                                if (extraTwo > 0)
                                {
                                    var damage = damageParams.oldValue * extraTwo;
                                    additionData = AllocAdditionData(additionData);
                                    additionData.damageValue = damage;
                                }
                            }
                            break;
                        }
                        case BattleObject.Event.Kill:
                        case BattleObject.Event.BeKill:
                        case BattleObject.Event.DeadPre:
                            isPass = arg0 is EventTwoParams<Fighter, PropParams> param && param.data2 != null && (int)param.data2.damageType == (int)extraOne - 1;
                            break;
                        case BattleObject.Event.ShieldChange:
                        {
                            if (arg0 is EventTwoParams<long, long> shieldParam)
                            {
                                isPass = shieldParam.data1 > 0;
                                if (isPass && extraTwo > 0)
                                {
                                    additionData = AllocAdditionData(additionData);
                                    additionData.propertyName = (int)extraOne;
                                    additionData.propertyValue = (long)YKMath.Ceiling(shieldParam.data1 * extraTwo);
                                }
                            }
                            break;
                        }
                        default:
                            Log.error($"新的战斗者事件没有逻辑处理：{fighterEvent},CheckEventEffect:{sid}");
                            break;
                    }
                }

                if (battleEvent != BaseBattle.Event.None)
                {
                    switch (battleEvent)
                    {
                        case BaseBattle.Event.AnyFighterAddState:
                        case BaseBattle.Event.AnyFighterRemoveState:
                        case BaseBattle.Event.AnyFighterAddStateEnd:
                        case BaseBattle.Event.AnyFighterRemoveStateEnd:
                            if (arg0 is FighterStateParams stateParams)
                            {
                                var isSample = ((int)extraOne & (int)stateParams.state) != 0;
                                if (isSample)
                                {
                                    //值2是判断是否同阵营的。0无所谓，1我方，2敌对，3是author添加的（author可能是0，见EffectSys::TryRemoveState），4是target添加的
                                    if ((int)extraTwo == 1 && stateParams.teamCampType == author.CampType)
                                    {
                                        isPass = true;
                                    }
                                    else if ((int)extraTwo == 2 && stateParams.teamCampType != author.CampType)
                                    {
                                        isPass = true;
                                    }
                                    else if ((int)extraTwo == 3 && stateParams.authorUid == author.Uid)
                                    {
                                        isPass = true;
                                    }
                                    else if ((int)extraTwo == 4 && stateParams.targetUid == target.Uid)
                                    {
                                        isPass = true;
                                    }
                                    else if ((int)extraTwo == 0)
                                    {
                                        isPass = true;
                                    }
                                }
                            }
                            break;
                        case BaseBattle.Event.AnyDead:
                            if (arg0 is EventParams<long> deadParams)
                            {
                                if (extraOne == 0)
                                {
                                    isPass = true;
                                }
                                else
                                {
                                    var deadF = battle.GetSceneObject<Fighter>(deadParams.data);
                                    //值1是判断是否同阵营的。0无所谓，1我方，2敌对
                                    if ((int)extraOne == 1 && deadF.CampType == author.CampType)
                                    {
                                        isPass = true;
                                    }
                                    else if ((int)extraOne == 2 && deadF.CampType != author.CampType)
                                    {
                                        isPass = true;
                                    }
                                }
                            }
                            break;
                        case BaseBattle.Event.SomeoneDoNormalSkill:
                        case BaseBattle.Event.SomeoneDoRageSkill:
                            if (arg0 is EventTwoParams<Fighter, BattleSkill> doNormalSkillParams &&
                                doNormalSkillParams.data1.CampType == target.CampType)
                            {
                                if (extraTwo <= 0.0d || doNormalSkillParams.data1.Uid == target.Uid)
                                {
                                    _doNormalSkillCount++;
                                    var count = (int)extraOne;
                                    if (_doNormalSkillCount >= count)
                                    {
                                        _doNormalSkillCount -= count;
                                        isPass = true;
                                    }
                                }
                            }
                            break;
                        default:
                            Log.error($"新的战场事件没有逻辑处理：{battleEvent},CheckEventEffect:{sid}");
                            break;
                    }
                }

                if (effectEvent != EffectSys.Event.None)
                {
                    if (extraTwo == 0)
                    {
                        Log.error($"CheckEventEffect:{sid} 需要配置对应的extraTwo值！");
                        return;
                    }
                    //值1是判定类型：
                    //1：效果类型判断，此时2为类型值
                    //2：效果Sid判断，此时2为效果Sid
                    switch (effectEvent)
                    {
                        case EffectSys.Event.Begin:
                        case EffectSys.Event.OverlayAdd:
                        case EffectSys.Event.End:
                        case EffectSys.Event.OverlayRed:
                        case EffectSys.Event.BeginOrOverlayAdd:
                            if (arg0 is EventParams<BaseEffect> effect)
                            {
                                switch ((int)extraOne)
                                {
                                    case 1:
                                        if (effect.data.IsType((EffectSys.EffectType)(int)extraTwo))
                                        {
                                            isPass = true;
                                        }
                                        break;
                                    case 2:
                                        if (effect.data.sid == sid)
                                        {
                                            isPass = true;
                                        }
                                        break;
                                }
                            }
                            break;
                        default:
                            Log.error($"新的效果事件没有逻辑处理：{effectEvent},CheckEventEffect:{sid}");
                            break;
                    }
                }
            }

            if (fighterEvent == BattleObject.Event.Prop && extraOne > 0.0d && arg0 is PropParams propParams &&
                (int)propParams.property == (int)extraOne)
            {
                var changedValue = propParams.newValue - propParams.dataValue;
                if (extraTwo != 0.0d)
                {
                    if (extraTwo < 0.0d)
                    {
                        isPass = changedValue < extraTwo;
                    }
                    else if (extraTwo > 0.0d)
                    {
                        isPass = changedValue > extraTwo;
                    }
                }
                else if (extraThree != 0.0d)
                {
                    if (extraThree < 0.0d)
                    {
                        isPass = changedValue < extraThree;
                    }
                    else if (extraThree > 0.0d)
                    {
                        isPass = changedValue > extraThree;
                    }

                    if (isPass)
                    {
                        var a = Math.Floor(changedValue / extraThree);
                        if (a >= 2.0d)
                        {
                            additionData = AllocAdditionData(additionData);
                            additionData.stepChangeValue = a - 1.0d;
                        }
                    }
                }
            }

            if (!isPass)
                return;

            if (checkCount > 0 && ++_checkCount < checkCount)
            {
                return;
            }

            _checkCount = 0;
            if (isStopEvent && arg0 != null)
            {
                arg0.IsBlockEvent = true;
            }

            if (damageEventTrigger)
            {
                _damageEventTrigger = damageEventTrigger;
            }
            author.Effect.AddEffects(passedSelfEffects, authorUid, effectLevel, additionData);
            target.Effect.AddEffects(passedFindTargetEffects, authorUid, effectLevel, additionData);
            makeEventFighter?.Effect.AddEffects(passedTargetEffects, authorUid, effectLevel, additionData);

#if UNITY_EDITOR
            if (makeEventFighter == null && passedTargetEffects != null && passedTargetEffects.Length != 0)
            {
                Log.error($"CheckEventEffect({sid})配置了造成事件{fighterEvent}的对象效果，但实际上未实现或者无法获取这个目标，找程序和策划一起核对这个配置！");
            }
#endif

            //触发次数提供
            TriggerBegin();
            _damageEventTrigger = false;

            if (passCount > 0 && ++_passCount >= passCount)
            {
                if (!isEnd)
                {
                    target.Effect.ClearEffect(this);
                }
            }
        }

        public override void Trigger()
        {

        }

        public override void OnEnd()
        {
            if (fighterEvent != BattleObject.Event.None)
            {
                target.RemoveEvent(fighterEvent, CheckHandler);
            }

            if (battleEvent != BaseBattle.Event.None)
            {
                battle.RemoveEvent(battleEvent, CheckHandler);
            }

            if (effectEvent != EffectSys.Event.None)
            {
                target.RemoveEvent(effectEvent, CheckHandler);
            }

            if (boutCheckCount > 0)
            {
                battle.RemoveEvent(BaseBattle.Event.BoutStart, OnAnyBoutStart);
            }

            if (roundCheckCount > 0)
            {
                battle.RemoveEvent(BaseBattle.Event.RoundStart, OnAnyRoundStart);
            }

            base.OnEnd();
        }

        private void OnAnyBoutStart(EventParams eventParams)
        {
            _boutCheckCount = 0;
        }

        private void OnAnyRoundStart(EventParams eventParams)
        {
            _roundCheckCount = 0;
        }

        private bool CheckPass(EventParams arg0)
        {
            if (arg0 is DamageParams damageParams)
            {
                if (_damageEventTrigger)
                {
                    return false;
                }

                if (passedSelfEffects != null)
                {
                    foreach (var effectSid in passedSelfEffects)
                    {
                        if (damageParams.effectSid == effectSid)
                        {
                            return false;
                        }
                    }
                }
                if (passedFindTargetEffects != null)
                {
                    foreach (var effectSid in passedFindTargetEffects)
                    {
                        if (damageParams.effectSid == effectSid)
                        {
                            return false;
                        }
                    }
                }

                if (passedTargetEffects != null)
                {
                    foreach (var effectSid in passedTargetEffects)
                    {
                        if (damageParams.effectSid == effectSid)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}