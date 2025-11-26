namespace BattleSystem
{
    /// <summary>
    /// 立即行动
    /// </summary>
    public class DoLogic : BaseEffect
    {

        public override void Trigger()
        {
            if (!(battle is NormalBattle normalBattle) || !(target is Fighter targetFighter))
            {
                return;
            }

            normalBattle.AddActOnceMoreFighter(targetFighter);
        }
    }
}