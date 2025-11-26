using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace BattleSystem
{
    /// <summary>
    /// KOF(bo5)战斗
    /// </summary>
    public class Bo3Battle : BoBattle
    {
        private int _attackWinCount = 0;
        private int _defenceWinCount = 0;
        public Bo3Battle(long uid, BattleSceneCfg cfg) : base(uid, cfg)
        {
        }

        protected override void TeamFighterEnd()
        {
            if (_allOver)
            {
                return;
            }

            var attackTeam = _attackTeam + 1;
            var defenceTeam = _defendTeam + 1;
            if (gameOverState == BattleDef.BattleResult.Win)
            {
                _attackWinCount++;
            }
            else if (gameOverState == BattleDef.BattleResult.Fail)
            {
                _defenceWinCount++;
            }

            if (attackTeam >= _attackTeamCount)
            {
                _allOver = true;
            }
            else if (defenceTeam >= _defenceTeamCount)
            {
                _allOver = true;
            }

            //gameOverState = attackTeam <= 2 ? BattleDef.BattleResult.Win : BattleDef.BattleResult.Fail; //test, to delete
            //gameOverState = _attackWinCount > _defenceWinCount ? BattleDef.BattleResult.Win : BattleDef.BattleResult.Fail;

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

                CleanAllFighters();
                DispatchEvent(this, BaseBattle.Event.BattleTeamOver);
                Attackers.ClearFighters();
                Defenders.ClearFighters();
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

        public override void OnBattleOver()
        {
            base.OnBattleOver();

            if (IsBattleOver)
            {
                gameOverState = _attackWinCount > _defenceWinCount ? BattleDef.BattleResult.Win : BattleDef.BattleResult.Fail;
            }
        }
    }
}