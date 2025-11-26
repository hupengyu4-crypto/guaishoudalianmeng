namespace BattleSystem
{
    /// <summary>
    /// 吸血
    /// </summary>
    public class VampireEffect : BaseEffect
    {
        /// <summary>
        /// 角色攻击事件(仅限)
        /// </summary>
        public BattleObject.Event fighterEvent;
        /// <summary>
        /// 吸取系数(百分比)
        /// </summary>
        public double factor;

        public override void OnBegin()
        {
            if (fighterEvent != BattleObject.Event.None)
            {
                target.AddEvent(fighterEvent, CheckHandler, false, 1);
            }
            base.OnBegin();
        }

        private void CheckHandler(EventParams arg0)
        {
            if (battle.CanNotHeal)
            {
                return;
            }

            Fighter mAuthor = (Fighter)author;

            switch (fighterEvent)
            {
                case BattleObject.Event.Crit:
                case BattleObject.Event.BeCrit:
                case BattleObject.Event.Block:
                case BattleObject.Event.BeBlock:
                case BattleObject.Event.AttackDamagePre:
                case BattleObject.Event.AttackDamage:
                case BattleObject.Event.BeDamagePre:
                case BattleObject.Event.BeDamage:
                case BattleObject.Event.NormalDamage:
                case BattleObject.Event.NormalCritDamage:
                case BattleObject.Event.BeNormalDamage:
                case BattleObject.Event.BeNormalCritDamage:
                case BattleObject.Event.SkillDamage:
                case BattleObject.Event.SkillCritDamage:
                case BattleObject.Event.BeSkillDamage:
                case BattleObject.Event.BeSkillCritDamage:
                case BattleObject.Event.DeBuffDamage:
                case BattleObject.Event.BeDeBuffDamage:
                //case BattleObject.Event.IndirectDamagePre:
                case BattleObject.Event.IndirectDamage:
                //case BattleObject.Event.BeIndirectDamagePre:
                case BattleObject.Event.BeIndirectDamage:
                    if (arg0 is DamageParams { newValue: > 0 } param)
                    {
                        var oldProp = mAuthor.Data[BattleDef.Property.hp];
                        mAuthor.AddProp(BattleDef.Property.hp, (long)YKMath.Ceiling(param.newValue * factor * BattleDef.Percent), authorUid);
                        var healValue = mAuthor.Data[BattleDef.Property.hp] - oldProp;
                        if (healValue > 0)
                        {
                            var healParams = battle.CreateEventParam<HealParams>();
                            healParams.casterkUid = authorUid;
                            healParams.targetUid = authorUid;
                            healParams.oldValue = healValue;
                            healParams.newValue = healValue;
                            healParams.IsAutoRelease = false;
                            mAuthor.DispatchEvent(battle, BattleObject.Event.Heal, healParams);
                            healParams.IsAutoRelease = true;
                            battle.ReleaseEventParam(healParams);
                        }

                        //触发次数提供
                        TriggerBegin();
                    }
                    break;
                default:
                    Log.error($"配置了非伤害事件{fighterEvent}，无法获取有效数据，请检查配置{GetType().Name}-{sid}");
                    break;
            }
        }

        public override void Trigger()
        {

        }

        public override void OnEnd()
        {
            if (fighterEvent != BattleObject.Event.None)
            {
                target.RemoveEvent(fighterEvent, CheckHandler);
            }
            base.OnEnd();
        }
    }
}