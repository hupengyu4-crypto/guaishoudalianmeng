namespace BattleSystem
{
    /// <summary>
    /// 使用技能
    /// </summary>
    public class UseSkillEffect : BaseEffect
    {
        /// <summary>
        /// 技能类型
        /// </summary>
        public BattleDef.ESkillType skillType;

        /// <summary>
        /// 技能Sid
        /// </summary>
        public long skillSid;

        /// <summary>
        /// 特定的目标
        /// </summary>
        public bool specificTarget;

        public override void Trigger()
        {
            NormalBattle normalBattle = battle as NormalBattle;
            if (normalBattle == null)
            {
                return;
            }

            var specificTargetUid = 0L;
            if (specificTarget)
            {
                if (_additionData != null && _additionData.specificTarget is Fighter targetFighter)
                {
                    specificTargetUid = targetFighter.Uid;
                }

                if (specificTargetUid <= 0)
                {
                    return;
                }
            }

            var doSid = skillSid;
            var doSkillType = BattleDef.ESkillType.None;
            if (doSid <= 0 && target is Fighter fighter)
            {
                var fighterData = fighter.Data;
                if (fighterData != null)
                {
                    switch (skillType)
                    {
                        case BattleDef.ESkillType.Normal:
                            if (fighterData.normalSkill != null)
                            {
                                doSid = fighterData.normalSkill.Sid;
                                doSkillType = BattleDef.ESkillType.Normal;
                            }

                            break;
                        case BattleDef.ESkillType.Rage:
                            if (fighterData.rageSkill != null)
                            {
                                doSid = fighterData.rageSkill.Sid;
                                doSkillType = BattleDef.ESkillType.Rage;
                            }

                            break;
                        case BattleDef.ESkillType.None:
                        case BattleDef.ESkillType.Opening:
                        case BattleDef.ESkillType.Passive:
                        default:
#if UNITY_EDITOR
                            Log.info($"UseSkillEffect,不支持的技能类型:{skillType},{target.GetBaseDesc()}");
#endif
                            break;
                    }
                }
            }

            if (doSid > 0)
            {
                normalBattle.AddDelaySkill(target.Uid, doSid, effectLevel, doSkillType, specificTargetUid);
            }
        }
    }
}