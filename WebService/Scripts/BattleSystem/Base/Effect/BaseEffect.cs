
namespace BattleSystem
{
    public abstract class BaseEffect : BattleEvent, IObjectPool
    {
        /// <summary>
        /// 唯一UID
        /// </summary>
        public long uid;

        /// <summary>
        /// 表现层配置Sid
        /// </summary>
        public long viewCfgSid;

        /// <summary>
        /// 战场
        /// </summary>
        public BaseBattle battle;

        /// <summary>
        /// 效果Sid
        /// </summary>
        public long sid;

        /// <summary>
        /// 描述
        /// </summary>
        public string describe;

        /// <summary>
        /// 效果持有者(效果的目标)
        /// </summary>
        public BattleObject target;

        /// <summary>
        /// 施法者
        /// </summary>
        public BattleObject author;

        /// <summary>
        /// 该效果来源战斗者Uid
        /// </summary>
        public long authorUid;

        /// <summary>
        /// 层级
        /// </summary>
        public int overlayCount = 1;

        /// <summary>
        /// 效果类型可能会包含多个类型
        /// </summary>
        public EffectSys.EffectType effectTypeMark;

        /// <summary>
        /// 可持续大回合
        /// </summary>
        public int bout;

        /// <summary>
        /// 最大叠加层级
        /// </summary>
        public int addMax;

        /// <summary>
        /// 第几个回合被添加
        /// </summary>
        public int boutAdd;

        /// <summary>
        /// 触发次数
        /// </summary>
        public int triggerNum;

        /// <summary>
        /// 效果结束附加效果
        /// </summary>
        public long[] endEffects;

        /// <summary>
        /// 生效最大次数
        /// </summary>
        public int triggerCount;

        /// <summary>
        /// 效果等级
        /// </summary>
        public int effectLevel;

        /// <summary>
        /// 当前生效次数 注意这里被添加后会马上执行一次
        /// </summary>
        public int TriggerNum
        {
            get => triggerNum;
            private set => triggerNum = value;
        }

        /// <summary>
        /// 状态标脏
        /// </summary>
        public bool typeDirty = true;

        /// <summary>
        /// 是否已结束
        /// </summary>
        public bool isEnd = false;

        /// <summary>
        /// 触发的回合数
        /// </summary>
        private int _triggerBout = -1;

        /// <summary>
        /// 是否是被传导的
        /// </summary>
        public bool conducted = false;

        protected BaseBattle.EffectAdditionData _additionData;

        protected BaseBattle.EffectAdditionData AllocAdditionData(BaseBattle.EffectAdditionData existData = null)
        {
            return battle.AllocAdditionData(existData);
        }

        public void UseAdditionData(BaseBattle.EffectAdditionData additionData)
        {
            if (_additionData != null)
            {
                _additionData.Release();
            }

            _additionData = additionData;
            if (additionData != null)
            {
                additionData.Use();
            }
        }

        public virtual void OnAwake()
        {
        }

        /// <summary>
        /// 检查效果是否可用
        /// </summary>
        public virtual bool Check()
        {
            return target != null && !target.IsDead && !isEnd;
        }

        /// <summary>
        /// 是否持续，可叠加，可多触发的
        /// </summary>
        /// <returns></returns>
        public virtual bool IsNeedAddList()
        {
            return bout > 0 || triggerCount > 0 || addMax > 0;
        }

        public void AddType(EffectSys.EffectType state)
        {
            if (!IsType(state))
            {
                effectTypeMark |= state;
                typeDirty = true;
            }
        }

        public void RemoveType(EffectSys.EffectType state)
        {
            if (IsType(state))
            {
                effectTypeMark &= ~state;
                typeDirty = true;
            }
        }

        public bool IsType(EffectSys.EffectType state)
        {
            return (effectTypeMark & state) != 0;
        }

        public virtual void OnBegin()
        {
            isEnd = false;
            if (bout > 0)
            {
                boutAdd = battle.Bout;
            }

            if (!IsType(EffectSys.EffectType.BoutStart) && !IsType(EffectSys.EffectType.BoutEnd) &&
                !IsType(EffectSys.EffectType.RoundStart) && !IsType(EffectSys.EffectType.RoundEnd))
            {
                TriggerBegin();
            }
        }

        /// <summary>
        /// 获取生效回合(当前回合-添加的回合)
        /// </summary>
        /// <returns></returns>
        public int GetActiveBout()
        {
            if (bout > 0)
            {
                //return battle.Bout - boutAdd;
                if (_triggerBout >= 0)
                {
                    return battle.Bout - _triggerBout + 1;
                }

                return 0;
            }

            return 9999;
        }

        /// <summary>
        /// 大回合开始
        /// </summary>
        /// <param name="arg"></param>
        public virtual void OnBoutStart(EventParams arg)
        {
            if (IsType(EffectSys.EffectType.BoutStart))
            {
                TriggerBegin();
            }
        }

        /// <summary>
        /// 大回合结束
        /// </summary>
        /// <param name="arg"></param>
        public virtual void OnBoutEnd(EventParams arg)
        {
            if (IsType(EffectSys.EffectType.BoutEnd))
            {
                TriggerBegin();
            }

            if (GetActiveBout() >= bout)
            {
                target?.Effect.ClearEffect(this);
            }
        }

        /// <summary>
        /// 小回合开始
        /// </summary>
        /// <param name="arg"></param>
        public virtual void OnRoundStart(EventParams arg)
        {
            if (IsType(EffectSys.EffectType.RoundStart))
            {
                TriggerBegin();
            }
        }

        /// <summary>
        /// 小回合结束
        /// </summary>
        /// <param name="arg"></param>
        public virtual void OnRoundEnd(EventParams arg)
        {
            if (IsType(EffectSys.EffectType.RoundEnd))
            {
                TriggerBegin();
            }
        }

        /// <summary>
        /// 触发之前
        /// </summary>
        public void TriggerBegin()
        {
            if (!Check())
            {
                return;
            }

            if (sid == 0)
            {
                Log.warning($"效果已经被移除了，但还尝试触发:" + ToString());
                return;
            }

            if (_triggerBout < 0 && bout > 0)
            {
                _triggerBout = System.Math.Max(1, battle.Bout); //小于1的，从1回合开始算（不要从0回合开始算）
            }

            if (!target.DispatchEvent(battle, EffectSys.Event.TriggerPre, EventParams<BaseEffect>.Create(battle, this)).IsBlockEvent)
            {
                Trigger();
                target.DispatchEvent(battle, EffectSys.Event.TriggerEnd, EventParams<BaseEffect>.Create(battle, this));
                if (triggerNum >= triggerCount && triggerCount > 0)
                {
                    target.Effect.ClearEffect(this);
                }
            }

            //不是持续的,又不可叠加，生效完毕及时回收
            if (!isEnd && !IsNeedAddList())
            {
                target.Effect.ClearEffect(this);
            }
        }

        /// <summary>
        /// 触发内容(注意外部额外触发需要调用TriggerBegin)
        /// </summary>
        public abstract void Trigger();

        public virtual void OnEnd()
        {
            target.Effect.AddEffects(endEffects, authorUid, effectLevel);
        }

        /// <summary>
        /// 增加层级
        /// </summary>
        public virtual void OnOverlayAdd()
        {
            overlayCount++;
            if (overlayCount >= addMax)
            {
                overlayCount = addMax;
            }
        }

        /// <summary>
        /// 消减层级
        /// </summary>
        public virtual void OnOverlayReduce()
        {
            overlayCount--;
            if (overlayCount >= 0)
            {
                var targetBattle = target.Battle;
                target.DispatchEvent(targetBattle, EffectSys.Event.OverlayRed, EventParams<BaseEffect>.Create(targetBattle, this));
                targetBattle.DispatchEvent(targetBattle, EffectSys.Event.OverlayRed, EventTwoParams<BattleObject, BaseEffect>.Create(targetBattle, target, this));
            }

            if (overlayCount <= 0 && IsType(EffectSys.EffectType.Overlays))
            {
                target.Effect.ClearEffect(this);
            }
        }

        public new virtual void Dispose()
        {
            target = null;
            author = null;
            authorUid = 0;
            sid = 0;
            triggerNum = 0;
            overlayCount = 1;
            typeDirty = true;
            isEnd = true;
            boutAdd = 0;
            triggerNum = 0;
            _triggerBout = -1;
            conducted = false;
            if (_additionData != null)
            {
                _additionData.Release();
                _additionData = null;
            }
        }

        public void SetBaseData(BaseEffectCfg cfg, BattleObject obj, long authorUid)
        {
            uid = authorUid;
            battle = obj.Battle;
            this.authorUid = authorUid;
            target = obj;
            author = obj.Battle.GetSceneObject<BattleObject>(authorUid);
            //配置
            triggerNum = 0;
            overlayCount = 1;
            viewCfgSid = cfg.viewCfg;
            sid = cfg.id;
            describe = cfg.describe;
            bout = cfg.bout;
            triggerCount = cfg.triggerCount;
            effectTypeMark = EffectSys.EffectType.Null;
            endEffects = cfg.endEffects;
            addMax = cfg.addMax;
            if (cfg.effectType != null && cfg.effectType.Length > 0)
            {
                effectTypeMark = 0;
                foreach (var t in cfg.effectType)
                {
                    effectTypeMark |= t;
                }
            }

            isEnd = false;
            typeDirty = true;
        }

        public override string ToString()
        {
            return $"[{target.GetBaseDesc()},Sid:{sid},Level:{effectLevel} {GetType().Name}]";
        }
    }
}