namespace BattleSystem
{
    /// <summary>
    /// 集结战斗
    /// </summary>
    public class MassBattle : NormalBattle
    {
        public MassBattle(long uid, BattleSceneCfg cfg) : base(uid, cfg)
        {
        }

        protected override void InitTeamData()
        {
            //进攻方
            Attackers = new MassTeamData(BattleDef.TeamCampType.Attacker, this);
            Attackers.InitBattlers(BattleData.AtkCamp);
            //防守方
            Defenders = new MassTeamData(BattleDef.TeamCampType.Defender, this);
            Defenders.InitBattlers(BattleData.DfsCamp);
        }

        public override void OnBattleOver()
        {
            base.OnBattleOver();
            if (Bout > LimitBout)
            {
                IsBattleOver = true;
                gameOverState = BattleDef.BattleResult.Tie;//集结是可以平局的
            }
        }
    }
}