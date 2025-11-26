using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 使用技能
/// </summary>
[Serializable]
public class UseSkillEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 技能类型
    /// </summary>
    [ConfigComment("技能类型")] public BattleDef.ESkillType skillType;

    /// <summary>
    /// 技能Sid
    /// </summary>
    [ConfigComment("技能Sid")] public long skillSid;

    /// <summary>
    /// 特定的目标
    /// </summary>
    [ConfigComment("特定的目标")] public bool specificTarget;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<UseSkillEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.skillType = skillType;
        effect.skillSid = skillSid;
        effect.specificTarget = specificTarget;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((byte)skillType);
        writer.Write(skillSid);
        writer.Write(specificTarget);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        skillType = (BattleDef.ESkillType)reader.ReadByte();
        skillSid = reader.ReadInt64();
        specificTarget = reader.ReadBoolean();
    }

    #endregion
}