#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using Google.Protobuf;

namespace BattleSystem
{
    /// <summary>
    /// 制造伤害指令
    /// </summary>
    public class MakeDamageCmd : BattleCommandHandler
    {
        /// <summary>
        /// 进攻方Uid
        /// </summary>
        public long AttackUid { get; set; }
        /// <summary>
        /// 防守方Uid
        /// </summary>
        public long DefendUid { get; set; }
        /// <summary>
        /// 是否暴击
        /// </summary>
        public bool IsCrit { get; set; }
        /// <summary>
        /// 是否有格挡
        /// </summary>
        public bool IsBlock { get; set; }
        /// <summary>
        /// 造成的伤害
        /// </summary>
        public long Damage { get; set; }

        public override void Do(BaseBattle battle)
        {
            var f = battle.GetSceneObject<Fighter>(DefendUid);
            if (f != null)
            {
#if UNITY_EDITOR
                battle.AddInfo("第{").AddInfo(Frame).AddInfo("}帧，伤害指令，AttackUid:").AddInfo(AttackUid).AddInfo("，DefendUid:")
                    .AddInfo(DefendUid).AddInfo("，伤害:").AddInfo(Damage, true);
#endif
                f.AddProp(BattleDef.Property.hp, -Damage, AttackUid);

                DamageParams damageParams = battle.CreateEventParam<DamageParams>();

                damageParams.effectSid = -9999;
                damageParams.damageType = BattleDef.DamageType.Cmd;
                damageParams.newValue = Damage;
                damageParams.oldValue = Damage;
                damageParams.attackUid = AttackUid;
                damageParams.defendUid = DefendUid;
                damageParams.isBlock = IsBlock;
                damageParams.isCrit = IsCrit;
                battle.DispatchEvent(battle, BaseBattle.Event.CmdDamage, damageParams);
            }
        }

        public override byte[] Serialize()
        {
            make_dmg_cmd cmd = new make_dmg_cmd();
            cmd.AtkUid = AttackUid;
            cmd.DfsUid = DefendUid;
            cmd.IsCrit = IsCrit ? 1 : 0;
            cmd.IsBlock = IsBlock ? 1 : 0;
            cmd.Damage = Damage;
            return cmd.ToByteArray();
        }

        public override bool UnSerialize(byte[] bytes)
        {
            if (bytes == null)
            {
                return false;
            }
            make_dmg_cmd cmd = PbManager<make_dmg_cmd>.ParseFrom(bytes);
            if (cmd != null)
            {
                AttackUid = cmd.AtkUid;
                DefendUid = cmd.DfsUid;
                IsCrit = cmd.IsCrit > 1;
                IsBlock = cmd.IsBlock > 1;
                Damage = cmd.Damage;
                return true;
            }

            return false;
        }

        public long GetRealDamage(BaseBattle battle, long damage = -1)
        {
            if (damage < 0)
            {
                damage = Damage;
            }

            if (damage < 0)
            {
                damage = 0;
                return damage;
            }

            if (battle == null)
            {
                return damage;
            }

            var defender = battle.GetSceneObject<Fighter>(DefendUid);
            if (defender == null)
            {
                return damage;
            }

            DamageParams damageParams = battle.CreateEventParam<DamageParams>();

            damageParams.effectSid = -9999;
            damageParams.damageType = BattleDef.DamageType.Cmd;
            damageParams.newValue = Damage;
            damageParams.oldValue = Damage;
            damageParams.attackUid = AttackUid;
            damageParams.defendUid = DefendUid;
            damageParams.isBlock = IsBlock;
            damageParams.isCrit = IsCrit;
            damageParams.IsAutoRelease = false;
            defender.DispatchEvent(battle, BaseBattle.Event.CmdDamagePre, damageParams);
            Damage = damageParams.newValue;
            damageParams.IsAutoRelease = true;
            battle.ReleaseEventParam(damageParams);
            return Damage;
        }
    }
}