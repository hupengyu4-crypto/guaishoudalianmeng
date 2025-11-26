using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 检测属性效果
/// </summary>
[Serializable]
public class CheckPropertyEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 检测的属性
    /// </summary>
    [ConfigComment("检测的属性")] public BattleDef.Property property;
    /// <summary>
    /// 检测的类型
    /// </summary>
    [ConfigComment("检测的类型")] public BattleDef.CompareType checkType;
    /// <summary>
    /// 属性值(百分比)
    /// </summary>
    [ConfigComment("属性值(百分比)")] public double value;
    /// <summary>
    /// ͨ通过给自己加效果
    /// </summary>
    [ConfigComment("ͨ通过给自己加效果")] public long[] passedSelfEffects;
    /// <summary>
    /// ͨ通过给索敌目标加效果
    /// </summary>
    [ConfigComment("ͨ通过给索敌目标加效果")] public long[] passedTargetEffects;
    /// <summary>
    /// 通过给造成事件者加效果
    /// </summary>
    [ConfigComment("通过给造成事件者加效果")] public long[] passedEventMakerEffects;
    /// <summary>
    /// 未通过给自己加效果
    /// </summary>
    [ConfigComment("未通过给自己加效果")] public long[] failedSelfEffects;
    /// <summary>
    /// 未通过给目标加效果
    /// </summary>
    [ConfigComment("未通过给目标加效果")] public long[] failedTargetEffects;
    /// <summary>
    /// 未通过给造成事件者加效果
    /// </summary>
    [ConfigComment("未通过给造成事件者加效果")] public long[] failedEventMakerEffects;
    /// <summary>
    /// 通过立即清理事件
    /// </summary>
    [ConfigComment("通过立即清理事件")] public bool passClearEvent;
    /// <summary>
    /// 差异的百分之多少算一份（百分比）
    /// </summary>
    [ConfigComment("差异的百分之多少算一份（百分比）")] public double step;
    /// <summary>
    /// 一份改变多少值
    /// </summary>
    [ConfigComment("一份改变多少值")] public double stepChangePerValue;
    /// <summary>
    /// 通过一次后百分比累加
    /// </summary>
    [ConfigComment("通过一次后百分比累加")] public bool accumulation;
    /// <summary>
    /// 与受到的普攻伤害比较
    /// </summary>
    [ConfigComment("与受到的总普攻伤害比较")] public bool compareWithAllDmg;
    /// <summary>
    /// 与受到的伤害比较
    /// </summary>
    [ConfigComment("与受到的伤害比较")] public bool compareWithDmg;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<CheckPropertyEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.property = property;
        effect.checkType = checkType;
        effect.value = value;
        effect.passedSelfEffects = passedSelfEffects;
        effect.passedTargetEffects = passedTargetEffects;
        effect.failedSelfEffects = failedSelfEffects;
        effect.failedTargetEffects = failedTargetEffects;
        effect.passClearEvent = passClearEvent;
        effect.step = step;
        effect.stepChangePerValue = stepChangePerValue;
        effect.accumulation = accumulation;
        effect.compareWithAllDmg = compareWithAllDmg;
        effect.compareWithDmg = compareWithDmg;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((int)property);
        writer.Write((byte)checkType);
        writer.Write(value);
        writer.Write(passedSelfEffects);
        writer.Write(passedTargetEffects);
        writer.Write(passedEventMakerEffects);
        writer.Write(failedSelfEffects);
        writer.Write(failedTargetEffects);
        writer.Write(failedEventMakerEffects);
        writer.Write(passClearEvent);
        writer.Write(step);
        writer.Write(stepChangePerValue);
        writer.Write(accumulation);
        writer.Write(compareWithAllDmg);
        writer.Write(compareWithDmg);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        property = (BattleDef.Property)reader.ReadInt32();
        checkType = (BattleDef.CompareType)reader.ReadByte();
        value = reader.ReadDouble();
        passedSelfEffects = reader.Read(passedSelfEffects);
        passedTargetEffects = reader.Read(passedTargetEffects);
        passedEventMakerEffects = reader.Read(passedEventMakerEffects);
        failedSelfEffects = reader.Read(failedSelfEffects);
        failedTargetEffects = reader.Read(failedTargetEffects);
        failedEventMakerEffects = reader.Read(failedEventMakerEffects);
        passClearEvent = reader.ReadBoolean();
        step = reader.ReadDouble();
        stepChangePerValue = reader.ReadDouble();
        accumulation = reader.ReadBoolean();
        compareWithAllDmg = reader.ReadBoolean();
        compareWithDmg = reader.ReadBoolean();
    }

    #endregion
}
