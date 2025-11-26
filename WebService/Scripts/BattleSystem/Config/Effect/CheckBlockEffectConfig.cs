using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 检测格挡效果
/// </summary>
[Serializable]
public class CheckBlockEffectConfig : BaseEffectCfg
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
    /// 通过给攻击索敌目标的人加效果
    /// </summary>
    [ConfigComment("通过给攻击索敌目标的人加效果")] public long[] passedAttackTargetEffects;
    /// <summary>
    /// 未通过给自己加效果
    /// </summary>
    [ConfigComment("未通过给自己加效果")] public long[] failedSelfEffects;
    /// <summary>
    /// 未通过给目标加效果
    /// </summary>
    [ConfigComment("未通过给目标加效果")] public long[] failedTargetEffects;
    /// <summary>
    /// 未通过给攻击目标的人加效果
    /// </summary>
    [ConfigComment("未通过给攻击目标的人加效果")] public long[] failedAttackTargetEffects;
    /// <summary>
    /// 检测格挡次数
    /// </summary>
    [ConfigComment("检测格挡次数")] public int count;
    /// <summary>
    /// 切换回合是否重置次数统计
    /// </summary>
    [ConfigComment("切换回合是否重置次数统计")] public bool clearOnBout;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<CheckBlockEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.passedSelfEffects = passedSelfEffects;
        effect.passedTargetEffects = passedTargetEffects;
        effect.passedAttackTargetEffects = passedAttackTargetEffects;
        effect.failedSelfEffects = failedSelfEffects;
        effect.failedTargetEffects = failedTargetEffects;
        effect.failedAttackTargetEffects = failedAttackTargetEffects;
        effect.count = count;
        effect.clearOnBout = clearOnBout;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(passedSelfEffects);
        writer.Write(passedTargetEffects);
        writer.Write(passedAttackTargetEffects);
        writer.Write(failedSelfEffects);
        writer.Write(failedTargetEffects);
        writer.Write(failedAttackTargetEffects);
        writer.Write(count);
        writer.Write(clearOnBout);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        passedSelfEffects = reader.Read(passedSelfEffects);
        passedTargetEffects = reader.Read(passedTargetEffects);
        passedAttackTargetEffects = reader.Read(passedAttackTargetEffects);
        failedSelfEffects = reader.Read(failedSelfEffects);
        failedTargetEffects = reader.Read(failedTargetEffects);
        failedAttackTargetEffects = reader.Read(failedAttackTargetEffects);
        count = reader.ReadInt32();
        clearOnBout = reader.ReadBoolean();
    }

    #endregion
}