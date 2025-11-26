

namespace BattleSystem
{
    /// <summary>
    /// 斩杀
    /// </summary>
    public class KillEffect : BaseEffect
    {

        public override void Trigger()
        {
            var targetFighter = (Fighter)target;
            var damage = targetFighter.Data[BattleDef.Property.hp];
            var arg = BattleUtils.CreateSimpleDamageParams(battle, sid, author.Uid, target.Uid, damage, BattleDef.DamageType.Normal, true, true);
            targetFighter.DispatchEvent(battle, BattleObject.Event.BeDamageWithoutDamageEffect, arg);
            targetFighter.SetProp(BattleDef.Property.hp, 0, author.Uid);

            foreach (var e in targetFighter.Effect.AllEffectList) 
            {
                if (e is ImmuneDamage tempI) 
                {
                    if (tempI.checkType == ImmuneDamageType.Dead)
                    {
                        targetFighter.DispatchEvent(battle, BattleObject.Event.KillImmuneDamage, EventParams<Fighter>.Create(battle, targetFighter));
                        break;
                    }
                }
            }
           
        }
    }
}