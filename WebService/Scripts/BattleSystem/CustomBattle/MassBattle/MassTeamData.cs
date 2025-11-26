#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System.Collections.Generic;

namespace BattleSystem
{
    public class MassTeamData : NormalTeamData
    {
        public MassTeamData(BattleDef.TeamCampType campType, NormalBattle battle) : base(campType, battle)
        {
        }

        public override void UpdateNextFighter(List<long> toRemoveFighters)
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
                        fighter.ResetInit(true);
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
                    //判断有没有空位
                    for (var i = 0; i < Const_MaxCount; i++)
                    {
                        if (posFighters[i] != null)
                        {
                            continue;//有人，下一个
                        }
                        nextFighter.Pos = i + 1;//修正位置，用策划规则动态填
                        AddFighter(nextFighter);
                        FighterIndex++;
                        break;
                    }
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
    }
}