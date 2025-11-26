#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace BattleSystem
{
    /// <summary>
    /// 战斗基类
    /// </summary>
    public abstract class BaseBattle : BattleEvent
    {
        /// <summary>
        /// 战斗唯一ID
        /// </summary>
        public long Uid { get; private set; }
        /// <summary>
        /// 战斗帧
        /// </summary>
        public uint Frame { get; private set; }
        /// <summary>
        /// 战斗时长
        /// </summary>
        public double Time { get; private set; }
        /// <summary>
        /// 大回合
        /// </summary>
        public int Bout { get; protected set; }
        /// <summary>
        /// 最大回合限制
        /// </summary>
        public int LimitBout { get; protected set; }
        /// <summary>
        /// 随机控制器
        /// </summary>
        internal YKRandom Random { get; private set; }
        /// <summary>
        /// 对象池
        /// </summary>
        internal AnyObjectPool<IObjectPool> Pool { get; private set; }
        /// <summary>
        /// 战斗指令集
        /// </summary>
        public BattleCommand Cmd { get; protected set; }
        /// <summary>
        /// 战斗是否结束
        /// </summary>
        public bool IsBattleOver { get; protected set; }
        /// <summary>
        /// 所有战斗对象
        /// </summary>
        public FastIterationDictionary<long, BattleObject> allSceneObjects = new FastIterationDictionary<long, BattleObject>();
        /// <summary>
        /// 战斗状态
        /// </summary>
        public BattleDef.BattleResult gameOverState;
        /// <summary>
        /// 战场配置
        /// </summary>
        public BattleSceneCfg Cfg { get; }
        /// <summary>
        /// 战报
        /// </summary>
        public BattleInfoSys BattleInfo { get; }
        /// <summary>
        /// 是否全死亡标记，用于全死亡后，下一帧再处理结算
        /// </summary>
        protected bool IsAllDeadTag { get; set; }
        /// <summary>
        /// 战斗是否结束标记，用于下一帧再处理
        /// </summary>
        public bool IsBattleOverTag { get; protected set; }
        /// <summary>
        /// 战斗中不能回血
        /// </summary>
        public bool CanNotHeal = false;

        protected BaseBattle(long uid, BattleSceneCfg cfg)
        {
            this.Uid = uid;
            Cfg = cfg;
            LimitBout = Cfg.maxBout;
            Random = new YKRandom();
            Cmd = new BattleCommand(this);
            Pool = new AnyObjectPool<IObjectPool>();
            EventParamPool = new AnyObjectPool<EventParams>();
            BattleInfo = new BattleInfoSys(this);
            BattleUtils.InitPropertyName();
        }

        public override void Dispose()
        {
            base.Dispose();
            Pool.Dispose();
            BattleInfo.Dispose();
            foreach (var battleObject in allSceneObjects.Values)
            {
                battleObject.Dispose();
            }
            allSceneObjects.Clear();
            EventParamPool.Dispose();
        }

        public void InitCmd(byte[] fight_cmd)
        {
            Cmd.SetCmd(fight_cmd);
        }

        public abstract void InitData(object battleData);

        /// <summary>
        /// 是否为PVE战斗
        /// </summary>
        public abstract bool IsPve();

        /// <summary>
        /// 更新逻辑
        /// </summary>
        public virtual void UpdateLogic()
        {
            if (IsBattleOverTag)
            {
                BattleOver();
            }
            if (IsAllDeadTag)
            {
                AllDead();
                return;
            }

            if (IsBattleOver)
                return;

            Frame++;
            Time += BattleDef.FrameDelta;
            Cmd.OnUpdate();

            OnUpdateLogic();
        }

        /// <summary>
        /// 更新逻辑，子类实现
        /// </summary>
        protected abstract void OnUpdateLogic();
        /// <summary>
        /// 战损
        /// </summary>
        public abstract void BeginDead();
        /// <summary>
        /// 团灭
        /// </summary>
        public abstract void AllDead();
        /// <summary>
        /// 结束
        /// </summary>
        public abstract void OnBattleOver();

        protected virtual void AddBattleInfoOnBattleOver()
        {
        }

        public void BattleOver()
        {
            OnBattleOver();

            if (IsBattleOver)
            {
                DispatchEvent(this, BaseBattle.Event.BattleOver);

#if UNITY_EDITOR
                if (Cmd.externalActions.Count > 0)
                {
                    AddInfo("剩余指令未执行:", true);
                    foreach (var action in Cmd.externalActions)
                    {
                        AddInfo("第{").AddInfo(action.Key).AddInfo("}帧，count=").AddInfo(action.Value.Count).AddInfo(">>>");
                        if (action.Value.Count > 0)
                        {
                            foreach (var handler in action.Value)
                            {
                                AddInfo(handler.ToString());
                            }
                        }
                        AddInfo("", true);
                    }
                }
#endif

                AddBattleInfoOnBattleOver();
            }
        }

        #region 战报

        /// <summary>
        /// 添加战报
        /// </summary>
        /// <param name="info">内容</param>
        /// <param name="isNewLine">是否结束换行</param>
        /// <returns>BattleInfoSys</returns>>
        public abstract BattleInfoSys AddInfo(string info, bool isNewLine = false);

        public void SaveInfo(string path)
        {
            if (BattleDef.DebugLog)
            {
                File.AppendAllText(path, BattleInfo.ToString(), Encoding.UTF8);
            }
        }

        #endregion

        #region 战场/对象

        /// <summary>
        /// 添加战斗者
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="obj"></param>
        public void AddSceneObject(BattleObject obj)
        {
            allSceneObjects.Add(obj.Uid, obj);
            DispatchEvent(this, BaseBattle.Event.AddSceneObject, EventParams<BattleObject>.Create(this, obj));
        }

        /// <summary>
        ///查找战斗者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public T GetSceneObject<T>(long uid) where T : BattleObject
        {
            if (allSceneObjects.TryGetValue(uid, out BattleObject obj))
            {
                return (T)obj;
            }
            return null;
        }

        public BattleObject[] GetAllSceneObject()
        {
            return allSceneObjects.Values.ToArray();
        }

        public void RemoveSceneObject(long uid)
        {
            if (allSceneObjects.ContainsKey(uid))
            {
                var obj = allSceneObjects[uid];
                allSceneObjects.Remove(uid);
                DispatchEvent(this, BaseBattle.Event.RemoveSceneObject, EventParams<BattleObject>.Create(this, obj));
                //obj.Dispose();
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 事件参数对象池
        /// </summary>
        public AnyObjectPool<EventParams> EventParamPool { get; protected set; }

        public EventParams CreateEventParam() => CreateEventParam(typeof(EventParams));

        public T CreateEventParam<T>() where T : EventParams => (T)CreateEventParam(typeof(T));

        public EventParams CreateEventParam(Type t)
        {
            if (EventParamPool == null) EventParamPool = new AnyObjectPool<EventParams>();
            var obj = EventParamPool.Create(t);
            return obj;
        }

        public void ReleaseEventParam(EventParams e)
        {
            EventParamPool.Release(e);
        }

        #endregion

        /// <summary>
        /// 战场事件
        /// </summary>
        public enum Event
        {
            /// <summary>
            /// 空
            /// </summary>
            None,
            /// <summary>
            /// 初始化战斗者结束，进场动画
            /// </summary>
            OpeningAnimation,
            /// <summary>
            /// 初始化结束
            /// </summary>
            InitEnd,
            /// <summary>
            /// 使用开场技
            /// </summary>
            DoOpenSkills,
            /// <summary>
            /// 使用被动技
            /// </summary>
            DoPassiveSkills,
            /// <summary>
            /// 某人使用普攻
            /// </summary>
            [Description("某人使用普攻")]
            SomeoneDoNormalSkill,
            /// <summary>
            /// 某人使用技能
            /// </summary>
            [Description("某人使用技能")]
            SomeoneDoRageSkill,
            /// <summary>
            /// 添加战场对象
            /// </summary>
            AddSceneObject,
            /// <summary>
            /// 新战斗者中途上场
            /// </summary>
            AddNewFighter,
            /// <summary>
            /// 移除战场对象
            /// </summary>
            RemoveSceneObject,
            /// <summary>
            /// 换人
            /// </summary>
            ChangeFighter,
            /// <summary>
            /// 操作指令开始事件
            /// </summary>
            CmdBegin,
            /// <summary>
            /// 大回合开始预报(表现层用)
            /// </summary>
            BoutStartShow,
            /// <summary>
            /// 大回合开始
            /// </summary>
            [Description("大回合开始")]
            BoutStart,
            /// <summary>
            /// 大回合开始预报完成(表现层用)
            /// </summary>
            BoutStartShowEnd,
            /// <summary>
            /// 大回合结束
            /// </summary>
            [Description("大回合结束")]
            BoutEnd,
            /// <summary>
            /// 大回合结束(表现层用)
            /// </summary>
            BoutEndShow,
            /// <summary>
            /// 大回合结束完成(表现层用)
            /// </summary>
            BoutEndShowEnd,
            /// <summary>
            /// 小回合开始
            /// </summary>
            [Description("小回合开始")]
            RoundStart,
            /// <summary>
            /// 小回合结束
            /// </summary>
            [Description("小回合结束")]
            RoundEnd,
            /// <summary>
            /// 战斗结束
            /// </summary>
            BattleOver,
            /// <summary>
            /// 伤害指令之前
            /// </summary>
            CmdDamagePre,
            /// <summary>
            /// 伤害指令
            /// </summary>
            CmdDamage,
            /// <summary>
            /// 任意死亡
            /// </summary>
            [Description("任意死亡")]
            AnyDead,
            /// <summary>
            /// 任意战斗者指定状态添加
            /// </summary>
            [Description("任意战斗者指定状态添加")]
            AnyFighterAddState,
            /// <summary>
            /// 任意战斗者指定状态移除
            /// </summary>
            [Description("任意战斗者指定状态移除")]
            AnyFighterRemoveState,
            /// <summary>
            /// 任意战斗者指定状态添加之后
            /// </summary>
            [Description("任意战斗者指定状态添加之后")]
            AnyFighterAddStateEnd,
            /// <summary>
            /// 任意战斗者指定状态移除之后
            /// </summary>
            [Description("任意战斗者指定状态移除之后")]
            AnyFighterRemoveStateEnd,
            /// <summary>
            /// 战斗中队伍战斗结束
            /// </summary>
            BattleTeamOver,
        }

        #region Effect.AdditionData
        public class EffectAdditionData
        {
            private readonly BaseBattle _belongBattle;
            private int _refCount;
            public double stepChangeValue;
            public BattleObject specificTarget;
            public int propertyName;
            public long propertyValue;
            /// <summary>
            /// 下一次造成的伤害
            /// </summary>
            public double damageValue;
            /// <summary>
            /// 已经造成的伤害
            /// </summary>
            public double causedDamage;

            public EffectAdditionData(BaseBattle belongBattle)
            {
                _belongBattle = belongBattle;
            }

            private void Reset()
            {
                _refCount = 0;
                stepChangeValue = 0.0;
                specificTarget = null;
                propertyName = 0;
                propertyValue = 0;
                damageValue = 0;
                causedDamage = 0;
            }

            public void Use()
            {
                _refCount++;
            }

            public void Release()
            {
                _refCount--;
                if (_refCount <= 0)
                {
                    Reset();
                    if (_belongBattle != null)
                    {
                        _belongBattle._additionDataBuffer.Push(this);
                    }
                }
            }
        }

        private readonly RingBuffer<EffectAdditionData> _additionDataBuffer = new(8);

        public EffectAdditionData AllocAdditionData(EffectAdditionData existData = null)
        {
            if (existData != null)
            {
                return existData;
            }

            _additionDataBuffer.Pop(out var data);
            if (data == null)
            {
                data = new EffectAdditionData(this);
            }

            return data;
        }
        #endregion

        #region BattleData
        private Dictionary<string, object> _battleData;

        public void SetBattleData(string key, object value)
        {
            if (_battleData == null)
            {
                _battleData = new Dictionary<string, object>();
            }

            _battleData[key] = value;
        }

        public bool GetBattleData(string key, out object value)
        {
            value = null;
            if (_battleData == null)
            {
                return false;
            }

            return _battleData.TryGetValue(key, out value);
        }
        #endregion
    }
}
