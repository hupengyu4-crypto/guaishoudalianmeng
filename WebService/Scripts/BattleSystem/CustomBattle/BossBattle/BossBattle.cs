namespace BattleSystem
{
    /// <summary>
    /// Boss战斗
    /// </summary>
    public class BossBattle : NormalBattle
    {
        public BossBattle(long uid, BattleSceneCfg cfg) : base(uid, cfg)
        {
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