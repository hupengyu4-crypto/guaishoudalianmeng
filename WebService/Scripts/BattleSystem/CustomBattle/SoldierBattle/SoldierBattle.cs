namespace BattleSystem
{
	public class SoldierBattle : NormalBattle
	{
		public System.Func<bool> battleIsOver;

		public SoldierBattle(long uid, BattleSceneCfg cfg)
			: base(uid, cfg)
		{
		}

		public override void UpdateLogic()
		{
			if (battleIsOver == null || battleIsOver())
			{
				gameOverState = BattleDef.BattleResult.Win;
				BattleOver();
			}
		}

		public override void Dispose()
		{
			base.Dispose();

			battleIsOver = null;
		}
	}
}