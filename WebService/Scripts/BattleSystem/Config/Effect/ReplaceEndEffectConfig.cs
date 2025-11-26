using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 替换技能的结束效果
/// </summary>
[Serializable]
public class ReplaceEndEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 目标技能sid
    /// </summary>
    [ConfigComment("目标技能sid")] public long targetSid;
    /// <summary>
    /// 先给自己的新效果
    /// </summary>
    [ConfigComment("给自己的新效果")] public long[] newSelfEffects;
    /// <summary>
    /// 给目标的新效果
    /// </summary>
    [ConfigComment("给目标的新效果")] public long[] newEndEffects;
    /// <summary>
    /// 后给自己的新效果
    /// </summary>
    [ConfigComment("给自己的新效果")] public long[] newSelfEndEffects;
    /// <summary>
    /// 效果结束是否还原
    /// </summary>
    [ConfigComment("效果结束是否还原")] public bool isRestore;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ReplaceEndEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.targetSid = targetSid;
        effect.newSelfEffects = newSelfEffects;
        effect.newEndEffects = newEndEffects;
        effect.newSelfEndEffects = newSelfEndEffects;
        effect.isRestore = isRestore;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(targetSid);
        writer.Write(newSelfEffects);
        writer.Write(newEndEffects);
        writer.Write(newSelfEndEffects);
        writer.Write(isRestore);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        targetSid = reader.ReadInt64();
        newSelfEffects = reader.Read(newSelfEffects);
        newEndEffects = reader.Read(newEndEffects);
        newSelfEndEffects = reader.Read(newSelfEndEffects);
        isRestore = reader.ReadBoolean();
    }

    #endregion
}