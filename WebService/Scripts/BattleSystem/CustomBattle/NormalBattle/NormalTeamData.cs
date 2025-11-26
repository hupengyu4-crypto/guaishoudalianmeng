#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;


namespace BattleSystem
{
    /// <summary>
    /// 队伍数据
    /// </summary>
    public class NormalTeamData : BattleEvent
    {
        /// <summary>
        /// 满员状态数量
        /// </summary>
        public const int Const_MaxCount = 5;
        /// <summary>
        /// 被动对位出战者，从1号位到5号位置
        /// </summary>
        public Fighter Combatant
        {
            get
            {
                for (var i = 0; i < Const_MaxCount; i++)
                {
                    if (posFighters.TryGetValue(i, out var fighter))
                    {
                        if (fighter == null)
                            continue;
                        if (!fighter.IsDead && !fighter.IsFakeDead)
                        {
                            return fighter;
                        }
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// 参战人员(可按照不同玩法定义为波数，轮换等)
        /// </summary>
        public List<battler> TeamData { get; protected set; }
        /// <summary>
        /// 所有参战人员的战斗者汇总(包含未出战)
        /// </summary>
        public List<fighter_info> AllFighterInfos { get; protected set; }
        /// <summary>
        /// 当前处于出战的战斗者
        /// </summary>
        public List<Fighter> AllFighters { get; protected set; }
        /// <summary>
        /// 有站位的战斗者
        /// </summary>
        public Dictionary<int, Fighter> posFighters;
        /// <summary>
        /// 宠物数据
        /// </summary>
        public fighter_info petInfo;
        /// <summary>
        /// 宠物单位（不计入出战单位 无法被攻击和反击 不用于结算）
        /// </summary>
        public Fighter pet;
        /// <summary>
        /// 战场
        /// </summary>
        public NormalBattle Battle { get; protected set; }
        /// <summary>
        /// 阵营类型
        /// </summary>
        public BattleDef.TeamCampType CampType { get; protected set; }
        /// <summary>
        /// 场中的人是否团灭
        /// </summary>
        public virtual bool IsAllDead
        {
            get
            {
                foreach (var fighter in AllFighters)
                {
                    if (fighter != null && !fighter.IsDoDeadTag)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// 战斗次数，也可以作为队伍轮换数
        /// </summary>
        public int FightTimes { get; protected set; }
        /// <summary>
        /// 战斗者取值下标，用于上阵
        /// </summary>
        public int FighterIndex { get; protected set; }
        /// <summary>
        /// 集结人数
        /// </summary>
        public int AllFighterCount { get; protected set; }
        /// <summary>
        /// 是否所有集结战斗者都阵亡
        /// </summary>
        public bool IsTeamDead => FighterIndex >= AllFighterCount && IsAllDead;

        public NormalTeamData(BattleDef.TeamCampType campType, NormalBattle battle)
        {
            CampType = campType;
            Battle = battle;
        }

        public override void Dispose()
        {
            base.Dispose();
            TeamData?.Clear();
            AllFighterInfos?.Clear();
            AllFighters?.Clear();
            posFighters?.Clear();
            pet = null;
            petInfo = null;
        }

        /// <summary>
        /// 初始化战斗者
        /// </summary>
        /// <param name="battlers">所有战斗者</param>
        public void InitBattlers(RepeatedField<battler> battlers)
        {
            var count = battlers.Count;
            if (TeamData == null)
            {
                TeamData = new List<battler>(5);
            }
            else
            {
                TeamData.Clear();
            }

            if (AllFighterInfos == null)
            {
                AllFighterInfos = new List<fighter_info>(5);
            }
            else
            {
                AllFighterInfos.Clear();
            }

            for (var i = 0; i < count; i++)
            {
                var battler = battlers[i];
                if (!Battle.CanInitBattlerNow(battler, i))
                {
                    continue;
                }
                TeamData.Add(battler);
                var fighters = battler.Fighter;
                var fCount = fighters.Count;
                for (var j = 0; j < fCount; j++)
                {
                    AllFighterInfos.Add(fighters[j]);
                }
                var pets = battler.PetFighter;
                //宠物不会参与轮换,这里只记录第一个
                if (petInfo == null && pets.Count > 0)
                {
                    petInfo = pets[0];
                }
            }

            InitFighter();
        }

        public void ClearFighters()
        {
            if (AllFighters == null || AllFighters.Count <= 0)
            {
                return;
            }

            //从第二轮开始，清理已经死亡的
            foreach (var fighter in CollectionForeachSyncUtility.Foreach(AllFighters))
            {
                posFighters[fighter.Data.Pos - 1] = null;
                Battle.RemoveSceneObject(fighter.Uid);
            }

            AllFighters.Clear();

            if (pet != null)
            {
                Battle.RemoveSceneObject(pet.Uid);
                pet = null;
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected virtual void InitFighter()
        {
#if UNITY_EDITOR
            Battle.AddInfo(CampType == BattleDef.TeamCampType.Attacker ? "初始化 进攻方 队伍" : "初始化 防守方 队伍");
#endif

            if (AllFighters == null)
            {
                AllFighters = new List<Fighter>(Const_MaxCount);
            }
            else
            {
                AllFighters.Clear();
            }

            if (posFighters == null)
            {
                posFighters = new Dictionary<int, Fighter>(Const_MaxCount);
            }
            else
            {
                posFighters.Clear();
            }

            //默认空位
            for (int pos = 0; pos < Const_MaxCount; pos++)
            {
                posFighters[pos] = null;
            }

            FightTimes = 0;
            FighterIndex = 0;

            AllFighterCount = AllFighterInfos.Count;
            if (petInfo != null)
            {
                AddPet(petInfo);
            }
            UpdateNextFighter(null);
        }

        /// <summary>
        /// 更新一轮战斗者
        /// </summary>
        public virtual void UpdateNextFighter(List<long> toRemoveFighters)
        {
            if (AllFighters.Count > 0)
            {
                //从第二轮开始，清理已经死亡的
                foreach (var fighter in CollectionForeachSyncUtility.Foreach(AllFighters))
                {
                    if (fighter.IsDead || fighter.IsState(BattleObject.State.Dead))
                    {
                        AllFighters.Remove(fighter);
                        posFighters[fighter.Data.Pos - 1] = null;

                        //延迟移除（RemoveSceneObject），不然后面的fighter.ResetInit()里处理effect的时候
                        //可能会用到这个fighter，这里移除后就找不到fighter了，会空异常。等所有的attacker和defender都reset了再移除  by ww
                        if (toRemoveFighters == null)
                        {
                            Battle.RemoveSceneObject(fighter.Uid);
                        }
                        else
                        {
                            toRemoveFighters.Add(fighter.Uid);
                        }

#if UNITY_EDITOR
                        Battle.AddInfo(fighter.GetBaseDesc()).AddInfo(" 已死亡，退出战斗", true);
#endif
                    }
                    else
                    {
                        //没死亡的回归初始化
                        fighter.ResetInit();
                    }
                }
            }
            if (FighterIndex >= AllFighterCount)
            {
                //所有人都出战了
                return;
            }
            FightTimes++;
            int times = 0;
            while (AllFighters.Count < Const_MaxCount && FighterIndex < AllFighterCount)
            {
                var nextFighter = AllFighterInfos[FighterIndex];
                if (nextFighter != null)
                {
                    AddFighter(nextFighter);
                    FighterIndex++;
                }
                else
                {
                    FighterIndex++;
                }

                times++;
                if (times >= 200)
                {
                    Log.error($"逻辑或数据异常！！！{times}");
                    break;
                }
            }
        }

        /// <summary>
        /// 添加宠物
        /// </summary>
        /// <param name="fighter">宠物数据数据</param>
        public virtual Fighter AddPet(fighter_info fighter)
        {
            var f = new Fighter(Battle);
            f.SetData(fighter, this);
            f.OnStart();
            pet = f;
#if UNITY_EDITOR
            Battle.AddInfo("添加宠物:").AddInfo(f);
#endif
            return f;
        }

        /// <summary>
        /// 添加战斗者
        /// </summary>
        /// <param name="fighter">战斗者数据</param>
        public virtual Fighter AddFighter(fighter_info fighter)
        {
            var f = new Fighter(Battle);
            f.SetData(fighter, this);
            f.OnStart();

            AllFighters.Add(f);
            posFighters[(int)fighter.Pos - 1] = f;

#if UNITY_EDITOR
            Battle.AddInfo("添加战斗者:").AddInfo(f);
#endif
            return f;
        }
        /// <summary>
        /// 添加战斗者到空闲位置
        /// </summary>
        /// <returns></returns>
        public virtual Fighter AddFighterInEmptyPos(fighter_info fighter)
        {
            for (int i = 0; i < Const_MaxCount; i++)
            {
                if (posFighters[i] == null)
                {
                    fighter.Pos = i + 1;
                    return AddFighter(fighter);
                }
            }
#if UNITY_EDITOR
            Battle.AddInfo("没有空位无法添加", true);
#endif
            return null;
        }

        /// <summary>
        /// 获取队伍总速度
        /// </summary>
        /// <returns></returns>
        public double GetTotalSpeed()
        {
            double speed = 0;
            foreach (var fighter in AllFighters)
            {
                speed += fighter.Data.GetProp(BattleDef.Property.speed);
            }
            return speed;
        }

    }
}
