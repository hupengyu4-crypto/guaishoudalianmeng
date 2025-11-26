using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 免疫伤害
/// </summary>
[Serializable]
public class ImmuneDamageConfig : BaseEffectCfg
{
    /// <summary>
    /// 免疫伤害类型
    /// </summary>
    [ConfigComment("免疫伤害类型")] public ImmuneDamageType checkType;
    /// <summary>
    /// 血量数值
    /// </summary>
    [ConfigComment("血量数值")] public long hpValue;
    /// <summary>
    /// 触发后下回合移除
    /// </summary>
    [ConfigComment("触发后下回合移除")] public bool triggerNextBoutClear;
    /// <summary>
    /// 触发几个小回合后移除
    /// </summary>
    [ConfigComment("触发几个小回合后移除")] public int triggerNextRoundClearCount;
    /// <summary>
    /// 触发几次免死后移除(0不生效)
    /// </summary>
    [ConfigComment("触发几次免死后移除(0不生效)")] public int triggerCountClear;
    /// <summary>
    /// 检测的属性
    /// </summary>
    [ConfigComment("检测的属性")] public BattleDef.Property checkPropType;
    /// <summary>
    /// 检测的属性类型
    /// </summary>
    [ConfigComment("检测的属性类型")] public BattleDef.CompareType compareType;
    /// <summary>
    /// 属性值(百分比)
    /// </summary>
    [ConfigComment("属性值(百分比)")] public double value;
    /// <summary>
    /// 忽略来自队友的伤害
    /// </summary>
    [ConfigComment("忽略来自队友的伤害")] public bool ingoreTeammate;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ImmuneDamage>();
        effect.SetBaseData(this, target, authorUid);

        effect.checkType = checkType;
        effect.hpValue = hpValue;
        effect.triggerNextBoutClear = triggerNextBoutClear;
        effect.triggerNextRoundClearCount = triggerNextRoundClearCount;
        effect.triggerCountClear = triggerCountClear;
        effect.checkPropType = checkPropType;
        effect.compareType = compareType;
        effect.value = value;
        effect.ingoreTeammate = ingoreTeammate;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((int)checkType);
        writer.Write(hpValue);
        writer.Write(triggerNextBoutClear);
        writer.Write(triggerNextRoundClearCount);
        writer.Write(triggerCountClear);
        writer.Write((int)checkPropType);
        writer.Write((byte)compareType);
        writer.Write(value);
        writer.Write(ingoreTeammate);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        checkType = (ImmuneDamageType)reader.ReadInt32();
        hpValue = reader.ReadInt64();
        triggerNextBoutClear = reader.ReadBoolean();
        triggerNextRoundClearCount = reader.ReadInt32();
        triggerCountClear = reader.ReadInt32();
        checkPropType = (BattleDef.Property)reader.ReadInt32();
        compareType = (BattleDef.CompareType)reader.ReadByte();
        value = reader.ReadDouble();
        ingoreTeammate = reader.ReadBoolean();
    }

    #endregion
}