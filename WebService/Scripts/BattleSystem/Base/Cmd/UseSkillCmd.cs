using Google.Protobuf;

namespace BattleSystem
{
    /// <summary>
    /// 制造伤害指令
    /// </summary>
    public class UseSkillCmd : BattleCommandHandler
    {
        /// <summary>
        /// 进攻方Uid
        /// </summary>
        public long AttackUid { get; set; }
        /// <summary>
        /// 技能Sid
        /// </summary>
        public long SkillSid { get; set; }

        public override void Do(BaseBattle battle)
        {
            var f = battle.GetSceneObject<Fighter>(AttackUid);
            if (f != null)
            {
                f.DoSkill(SkillSid, 1);
            }
        }

        public override byte[] Serialize()
        {
            skill_cmd cmd = new skill_cmd();
            cmd.AtkUid = AttackUid;
            cmd.SkillSid = SkillSid;
            return cmd.ToByteArray();
        }

        public override bool UnSerialize(byte[] bytes)
        {
            if (bytes == null)
            {
                return false;
            }
            skill_cmd cmd = PbManager<skill_cmd>.ParseFrom(bytes);
            if (cmd != null)
            {
                AttackUid = cmd.AtkUid;
                SkillSid = cmd.SkillSid;
                return true;
            }

            return false;
        }
    }
}