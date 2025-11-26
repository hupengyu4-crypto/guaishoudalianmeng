using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 替换技能
/// </summary>
[Serializable]
public class ReplaceSkillEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 目标技能sid
    /// </summary>
    [ConfigComment("目标技能sid")] public long targetSid;
    /// <summary>
    /// 替换sid
    /// </summary>
    [ConfigComment("替换sid")] public long replaceSid;
    /// <summary>
    /// 效果结束是否还原技能
    /// </summary>
    [ConfigComment("效果结束是否还原技能")] public bool isRestore;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ReplaceSkillEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.targetSid = targetSid;
        effect.replaceSid = replaceSid;
        effect.isRestore = isRestore;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(targetSid);
        writer.Write(replaceSid);
        writer.Write(isRestore);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        targetSid = reader.ReadInt64();
        replaceSid = reader.ReadInt64();
        isRestore = reader.ReadBoolean();
    }

    #endregion
}