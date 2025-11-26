#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using Google.Protobuf;


namespace BattleSystem
{
    /// <summary>
    /// 火把强制死亡指令
    /// 兰阳
    /// 2023-11-21
    /// </summary>
    public class FireKillCommand : BattleCommandHandler
    {
        /// <summary>
        /// 进攻方Uid
        /// </summary>
        public long AttackUid { get; set; }
        /// <summary>
        /// 防守方Uid
        /// </summary>
        public long DefendUid { get; set; }
        public override void Do(BaseBattle battle)
        {
            var f = battle.GetSceneObject<Fighter>(DefendUid);
            if (f != null)
            {
#if UNITY_EDITOR
                battle.AddInfo("第{").AddInfo(Frame).AddInfo("}帧，火把强制死亡指令，AttackUid:").AddInfo(AttackUid).AddInfo("，DefendUid:")
                    .AddInfo(DefendUid, true);
#endif
                f.AddProp(BattleDef.Property.hp, -f.Data[BattleDef.Property.hp], AttackUid);

                DamageParams damageParams = battle.CreateEventParam<DamageParams>();

                f.DispatchEvent(battle, BattleObject.Event.CmdFireKill);
            }
        }

        public override byte[] Serialize()
        {
            fire_kill_cmd cmd = new fire_kill_cmd();
            cmd.AtkUid = AttackUid;
            cmd.DfsUid = DefendUid;
            return cmd.ToByteArray();
        }

        public override bool UnSerialize(byte[] bytes)
        {
            if (bytes == null)
            {
                return false;
            }
            fire_kill_cmd cmd = PbManager<fire_kill_cmd>.ParseFrom(bytes);
            if (cmd != null)
            {
                AttackUid = cmd.AtkUid;
                DefendUid = cmd.DfsUid;
                return true;
            }
            return false;
        }
    }
}
