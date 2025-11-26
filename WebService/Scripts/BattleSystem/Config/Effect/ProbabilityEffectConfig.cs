using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 概率效果
/// </summary>
[Serializable]
public class ProbabilityEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 通过给自己加效果
    /// </summary>
    [ConfigComment("通过给自己加效果")] public long[] passedSelfEffects;
    /// <summary>
    /// 通过给目标加效果
    /// </summary>
    [ConfigComment("通过给目标加效果")] public long[] passedTargetEffects;
    /// <summary>
    /// 不通过给自己加效果
    /// </summary>
    [ConfigComment("不通过给自己加效果")] public long[] failedSelfEffects;
    /// <summary>
    /// 不通过给目标加效果
    /// </summary>
    [ConfigComment("不通过给目标加效果")] public long[] failedTargetEffects;
    /// <summary>
    /// 目标抵抗属性
    /// </summary>
    [ConfigComment("目标抵抗属性")] public BattleDef.Property targetProperty;
    /// <summary>
    /// 概率(0-100)
    /// </summary>
    [ConfigComment("几率(0-100)")][ConfigOriginString] public double[] probability;
    /// <summary>
    /// 是否随机其中一个效果
    /// </summary>
    [ConfigComment("是否随机其中一个效果")] public bool isRandomOne;
    /// <summary>
    /// 星级概率
    /// </summary>
    [ConfigComment("星级概率")] public double starValue;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ProbabilityEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.passedSelfEffects = passedSelfEffects;
        effect.passedTargetEffects = passedTargetEffects;
        effect.failedSelfEffects = failedSelfEffects;
        effect.failedTargetEffects = failedTargetEffects;
        effect.targetProperty = targetProperty;
        effect.probability = probability;
        effect.isRandomOne = isRandomOne;
        effect.starValue = starValue;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(passedSelfEffects);
        writer.Write(passedTargetEffects);
        writer.Write(failedSelfEffects);
        writer.Write(failedTargetEffects);
        writer.Write((int)targetProperty);
        writer.Write(probability);
        writer.Write(isRandomOne);
        writer.Write(starValue);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        passedSelfEffects = reader.Read(passedSelfEffects);
        passedTargetEffects = reader.Read(passedTargetEffects);
        failedSelfEffects = reader.Read(failedSelfEffects);
        failedTargetEffects = reader.Read(failedTargetEffects);
        targetProperty = (BattleDef.Property)reader.ReadInt32();
        probability = reader.Read(probability);
        isRandomOne = reader.ReadBoolean();
        starValue = reader.ReadDouble();
    }

    #endregion
}