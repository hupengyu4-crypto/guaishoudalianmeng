using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace BattleSystem
{
    /// <summary>
    /// Boss战斗
    /// </summary>
    public class BoBattle : PVPBattle
    {
        // List<Fighter> _atkFighters = new List<Fighter>();
        // List<Fighter> _defFighters = new List<Fighter>();
        protected int _attackTeam = 0;
        protected int _defendTeam = 0;
        protected int _attackTeamCount = 0;
        protected int _defenceTeamCount = 0;

        public int attackTeam => _attackTeam;
        public int defendTeam => _defendTeam;

        protected bool _allOver = false;

        private readonly Dictionary<long, fighter_info> _fighterInfoDict = new();

        public BoBattle(long uid, BattleSceneCfg cfg) : base(uid, cfg)
        {
        }

        public override void InitData(object battleData)
        {
            base.InitData(battleData);

            _attackTeamCount = BattleData.AtkCamp.Count;
            _defenceTeamCount = BattleData.DfsCamp.Count;
            _attackTeam = 0;
            _defendTeam = 0;
            _allOver = false;
        }

        internal override bool CanInitBattlerNow(battler battler, int index)
        {
            //return index == 0; //test to delete
            //if (battler.Camp == (int)BattleDef.TeamCampType.Attacker)
            if (battler.Camp == 1) //proto里的camp是： 1:进攻方|2:防守方
            {
                return index == _attackTeam;
            }

            return index == _defendTeam;
        }

        private void ResetFighter(Fighter fighter)
        {
            if (fighter == null)
            {
                return;
            }

            fighter.ResetInit(true);
        }

        private void ResetFighters(List<Fighter> fighters)
        {
            if (fighters == null)
            {
                return;
            }

            for (int i = 0, l = fighters.Count; i < l; i++)
            {
                var fighter = fighters[i];
                ResetFighter(fighter);
            }
        }

        private void CleanFighter(Fighter fighter)
        {
            if (fighter == null)
            {
                return;
            }

            var data = fighter.Data;
            const int carePropertyCount = 2;
            int dealPropertyCount = 0;
            if (_fighterInfoDict.TryGetValue(fighter.Uid, out var fighterInfo))
            {
                foreach (var attr in fighterInfo.Attr)
                {
                    if (System.Enum.TryParse<BattleDef.Property>(attr.AttrName, out var property))
                    {
                        if (property == BattleDef.Property.hp)
                        {
                            ++dealPropertyCount;
                            //attr.Value = data.GetProp(BattleDef.Property.hp);
                            attr.Value = data[BattleDef.Property.hp]; //web那边是用[]取值的，还是保持一致吧
                        }
                        else if (property == BattleDef.Property.dead_hp)
                        {
                            ++dealPropertyCount;
                            //attr.Value = data.GetProp(BattleDef.Property.dead_hp);
                            attr.Value = data[BattleDef.Property.dead_hp];
                        }
                    }

                    if (dealPropertyCount == carePropertyCount)
                    {
                        break;
                    }
                }
            }
            //RemoveSceneObject(fighter.Uid);
            ResetFighter(fighter);
        }

        private void AddToFighterDict(fighter_info info)
        {
            if (info == null)
            {
                return;
            }

            var uid = info.Uid;
            if (_fighterInfoDict.ContainsKey(uid))
            {
#if UNITY_EDITOR
                AddInfo($"AddToFighterDict::重复数据: {uid}", true);
#endif
                return;
            }

            _fighterInfoDict.Add(uid, info);
        }

        private void AddToFighterDict(List<fighter_info> infos)
        {
            if (infos == null)
            {
                return;
            }

            for (int i = 0, l = infos.Count; i < l; i++)
            {
                var info = infos[i];
                AddToFighterDict(info);
            }
        }

        protected void CleanAllFighters()
        {
            OnTeamFighterEnd();
            _fighterInfoDict.Clear();
            AddToFighterDict(Attackers.AllFighterInfos);
            AddToFighterDict(Defenders.AllFighterInfos);
            AddToFighterDict(Attackers.petInfo);
            AddToFighterDict(Defenders.petInfo);
            for (int i = 0, l = AllFighter.Count; i < l; i++)
            {
                CleanFighter(AllFighter[i]);
            }
            CleanFighter(Attackers.pet);
            CleanFighter(Defenders.pet);
            _fighterInfoDict.Clear();
        }

        protected virtual void TeamFighterEnd()
        {
        }

        // protected override void InitTeamData()
        // {
        // }

        public override void OnBattleOver()
        {
            base.OnBattleOver();

            TeamFighterEnd();
        }

        public override void AllDead()
        {
            var isAttackOver = Attackers.IsTeamDead;
            var isDefendOver = Defenders.IsTeamDead;
            if (isAttackOver || isDefendOver)
            {
                IsAllDeadTag = false;
                gameOverState = isAttackOver ? BattleDef.BattleResult.Fail : BattleDef.BattleResult.Win;
                TeamFighterEnd();
            }
            else
            {
                throw new System.Exception("What's Wrong in KOFBattle::AllDead!");
            }
        }
    }
}