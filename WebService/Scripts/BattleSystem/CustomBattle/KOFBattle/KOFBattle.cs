using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace BattleSystem
{
    /// <summary>
    /// KOF(bo5)战斗
    /// </summary>
    public class KOFBattle : BoBattle
    {
        public KOFBattle(long uid, BattleSceneCfg cfg) : base(uid, cfg)
        {
        }

        protected override void TeamFighterEnd()
        {
            base.TeamFighterEnd();

            if (_allOver)
            {
                return;
            }

            var attackTeam = _attackTeam;
            var defenceTeam = _defendTeam;
            if (gameOverState == BattleDef.BattleResult.Win)
            {
                defenceTeam++;
            }
            else if (gameOverState == BattleDef.BattleResult.Fail)
            {
                attackTeam++;
            }

            if (attackTeam >= _attackTeamCount)
            {
                gameOverState = BattleDef.BattleResult.Fail;
                _allOver = true;
            }
            else if (defenceTeam >= _defenceTeamCount)
            {
                gameOverState = BattleDef.BattleResult.Win;
                _allOver = true;
            }

            if (!_allOver)
            {
                if (IsBattleOver)
                {
                    IsBattleOver = false;
                }

                if (IsBattleOverTag)
                {
                    IsBattleOverTag = false;
                }

                _attackTeam = attackTeam;
                _defendTeam = defenceTeam;

                // Bout = 0;
                // FightIndex = 0;
                // IsAllDeadTag = false;
                // if (gameOverState == BattleDef.BattleResult.Win)
                // {
                //     Defenders.ClearFighters();
                //     Defenders.InitBattlers(BattleData.DfsCamp);
                //     ResetFighter(Attackers.pet);
                //     Attackers.UpdateNextFighter();
                //     ResetFighters(Attackers.AllFighters);
                // }
                // else if (gameOverState == BattleDef.BattleResult.Fail)
                // {
                //     Attackers.ClearFighters();
                //     Attackers.InitBattlers(BattleData.AtkCamp);
                //     ResetFighter(Defenders.pet);
                //     Defenders.UpdateNextFighter();
                //     ResetFighters(Defenders.AllFighters);
                // }

                CleanAllFighters();
                DispatchEvent(this, BaseBattle.Event.BattleTeamOver);
                allSceneObjects.Clear();
#if UNITY_EDITOR
                AddInfo("重新整理上阵数据完成", true);
#endif
                Bout = 0;
                FightIndex = 0;
                IsAllDeadTag = false;

                //清理，重新初始化战斗者
                AllFighter.Clear();
                AllPet.Clear();
                //Random.SetSeed((int)Random.GetSeed());
                Random.SetSeed((int)BattleData.Seed);//不能用Random.GetSeed()，内部的seed值会变
                InitTeamData();
                InitFighter();
            }
            else
            {
                IsBattleOverTag = true;
            }
        }
    }
}