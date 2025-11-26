using System.Collections.Generic;

namespace BattleSystem
{
    /// <summary>
    /// 火把进攻方队伍
    /// </summary>
    public class FireAtkTeamData : NormalTeamData
    {
        public FireAtkTeamData(BattleDef.TeamCampType campType, NormalBattle battle) : base(campType, battle)
        {
            AllFighters = new List<Fighter>(0);
            posFighters = new Dictionary<int, Fighter>(0);
            FightTimes = 0;
            AllFighterCount = 0;
        }

    }
}