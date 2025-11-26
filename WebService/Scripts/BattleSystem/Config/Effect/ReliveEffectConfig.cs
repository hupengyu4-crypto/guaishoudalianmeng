using System;
using RootScript.Config;
using BattleSystem;

/// <summary>
/// 复活效果
/// </summary>
[Serializable]
public class ReliveEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 回血系数(百分比)
    /// </summary>
    [ConfigComment("回血系数(百分比)")] public long hpValue;

    /// <summary>
    /// 新复活
    /// </summary>
    [ConfigComment("新复活")] public bool newReliveSystem;

    /// <summary>
    /// 新复活的索敌
    /// </summary>
    [ConfigComment("新复活的索敌")] public long findEffects;

    /// <summary>
    /// 新复活的扣血系数（百分比）
    /// </summary>
    [ConfigComment("新复活的扣血系数（百分比）")] public double factor;


    /// <summary>
    /// 扣血最大值关联属性
    /// </summary>
    [ConfigComment("扣血最大值关联属性")] public BattleDef.Property property;

    /// <summary>
    /// 扣血最大值关联属性比例（百分比）
    /// </summary>
    [ConfigComment("扣血最大值关联属性比例（百分比）")] public double propertyFactor;

    /// <summary>
    /// 触发次数
    /// </summary>
    [ConfigComment("触发次数")] public int checkCount;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ReliveEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.hpValue = hpValue;
        effect.newReliveSystem = newReliveSystem;
        effect.findEffects = findEffects;
        effect.factor = factor;
        effect.property = property;
        effect.propertyFactor = propertyFactor;
        effect.checkCount = checkCount;
        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(hpValue);
        writer.Write(newReliveSystem);
        writer.Write(findEffects);
        writer.Write(factor);
        writer.Write((int)property);
        writer.Write(propertyFactor);
        writer.Write(checkCount);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        hpValue = reader.ReadInt64();
        newReliveSystem = reader.ReadBoolean();
        findEffects = reader.ReadInt64();
        factor = reader.ReadDouble();
        property = (BattleDef.Property)reader.ReadInt32();
        propertyFactor = reader.ReadDouble();
        checkCount = reader.ReadInt32();
    }

    #endregion
}