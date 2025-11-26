namespace BattleSystem
{
    /// <summary>
    /// 清理状态
    /// </summary>
    public class ClearStateEffect : BaseEffect
    {
        /// <summary>
        /// 需要清理的状态
        /// </summary>
        public BattleObject.State[] state;

        public override void Trigger()
        {
            if (state != null)
            {
                foreach (var s in state)
                {
                    if (target.IsState(s))
                    {
                        //更改了机制，通过效果器的类型来决定状态，所以不再直接清理
                        var effectType = EffectSys.StateToEffectType(s);
                        target.Effect.ClearEffectByType(effectType);

                        if (s == BattleObject.State.Shield) //shield没有一个effect
                        {
                            if (target is Fighter targetFighter)
                            {
                                //进target.IsState(s) && s == BattleObject.State.Shield这个if，说明有shield值
                                targetFighter.AddShield(-targetFighter.GetShield());
                                targetFighter.AddShield(-targetFighter.GetSpecialShield(),true);
                            }
                        }
                    }
                }
            }
        }
    }
}