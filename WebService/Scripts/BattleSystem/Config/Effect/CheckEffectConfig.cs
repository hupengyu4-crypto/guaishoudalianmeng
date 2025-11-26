using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 检测效果状态
/// </summary>
[Serializable]
public class CheckEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 通过给自己的效果
    /// </summary>
    [ConfigComment("通过给自己的效果")] public long[] passedSelfEffects;
    /// <summary>
    /// 通过给目标的效果
    /// </summary>
    [ConfigComment("通过给目标的效果")] public long[] passedTargetEffects;
    /// <summary>
    /// 未通过给自己的效果
    /// </summary>
    [ConfigComment("未通过给自己的效果")] public long[] failedSelfEffects;
    /// <summary>
    /// 未通过给目标的效果
    /// </summary>
    [ConfigComment("未通过给目标的效果")] public long[] failedTargetEffects;
    /// <summary>
    /// 检查条件
    /// </summary>
    [ConfigComment("检查条件")] public BattleDef.CheckType checkType;
    /// <summary>
    /// 扩展信息
    /// </summary>
    [ConfigComment("扩展信息(小数)")] public double[] extraDouble;
    /// <summary>
    /// 扩展信息(整数)
    /// </summary>
    [ConfigComment("扩展信息(整数)")] public long[] extraLong;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<CheckEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.passedSelfEffects = passedSelfEffects;
        effect.passedTargetEffects = passedTargetEffects;
        effect.failedSelfEffects = failedSelfEffects;
        effect.failedTargetEffects = failedTargetEffects;
        effect.checkType = checkType;
        effect.extraDouble = extraDouble;
        effect.extraLong = extraLong;

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
        writer.Write((byte)checkType);
        writer.Write(extraDouble);
        writer.Write(extraLong);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        passedSelfEffects = reader.Read(passedSelfEffects);
        passedTargetEffects = reader.Read(passedTargetEffects);
        failedSelfEffects = reader.Read(failedSelfEffects);
        failedTargetEffects = reader.Read(failedTargetEffects);
        checkType = (BattleDef.CheckType)reader.ReadByte();
        extraDouble = reader.Read(extraDouble);
        extraLong = reader.Read(extraLong);
    }

    #endregion
}