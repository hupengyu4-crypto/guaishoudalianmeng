using Google.Protobuf;

namespace BattleSystem
{
    /// <summary>
    /// 添加英雄Cmd
    /// </summary>
    public class AddFighterCmd : BattleCommandHandler
    {
        /// <summary>
        /// 进攻方Uid
        /// </summary>
        public BattleDef.TeamCampType camp { get; set; }
        /// <summary>
        /// 技能Sid
        /// </summary>
        public fighter_info fighterInfo { get; set; }

        public override void Do(BaseBattle battle)
        {
            NormalBattle normalBattle = battle as NormalBattle;
            normalBattle.AddFighterInEmptyPos(camp, fighterInfo);
        }

        public override byte[] Serialize()
        {
            add_fighter_cmd cmd = new add_fighter_cmd();
            cmd.Camp = (long)camp;
            cmd.Fighter = fighterInfo;
            return cmd.ToByteArray();
        }

        public override bool UnSerialize(byte[] bytes)
        {
            if (bytes == null)
            {
                return false;
            }
            add_fighter_cmd cmd = PbManager<add_fighter_cmd>.ParseFrom(bytes);
            if (cmd != null)
            {
                camp = (BattleDef.TeamCampType)cmd.Camp;
                fighterInfo = cmd.Fighter;
                return true;
            }

            return false;
        }
    }
}