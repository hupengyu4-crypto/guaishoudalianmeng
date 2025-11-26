using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 满足条件时消耗效果
/// </summary>
[Serializable]
public class ConditionConsumeEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 消耗类型
    /// </summary>
    [ConfigComment("消耗类型")] public BattleObject.Event consumeType;

    /// <summary>
    /// 目标触发效果
    /// </summary>
    [ConfigComment("目标触发效果")] public long[] targetEffects;

    /// <summary>
    /// 可消耗次数
    /// </summary>
    [ConfigComment("可消耗次数")] public long consumeNum;

    /// <summary>
    /// 是否是自己消耗
    /// </summary>
    [ConfigComment("是否是自己消耗")] public bool authorConsume;

    /// <summary>
    /// 本效果结束时也结束后续效果
    /// </summary>
    [ConfigComment("本效果结束时也结束后续效果")] public bool endFollowUpEffects;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ConditionConsumeEffect>();
        effect.SetBaseData(this, target, authorUid);
        effect.consumeType = consumeType;
        effect.targetEffects = targetEffects;
        effect.consumeNum = consumeNum;
        effect.authorConsume = authorConsume;
        effect.endFollowUpEffects = endFollowUpEffects;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((int)consumeType);
        writer.Write(targetEffects);
        writer.Write(consumeNum);
        writer.Write(authorConsume);
        writer.Write(endFollowUpEffects);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        consumeType = (BattleObject.Event)reader.ReadInt32();
        targetEffects = reader.Read(targetEffects);
        consumeNum = reader.ReadInt64();
        authorConsume = reader.ReadBoolean();
        endFollowUpEffects = reader.ReadBoolean();
    }

    #endregion
}