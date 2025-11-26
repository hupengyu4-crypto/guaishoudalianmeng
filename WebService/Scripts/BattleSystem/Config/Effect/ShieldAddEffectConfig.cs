using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 添加护盾
/// </summary>
[Serializable]
public class ShieldAddEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 参照目标类型
    /// </summary>
    [ConfigComment("参照目标类型")] public BattleDef.TargetType targetType;
    /// <summary>
    /// 参照属性
    /// </summary>
    [ConfigComment("参照属性")] public BattleDef.Property property;
    /// <summary>
    /// 属性系数(百分比)
    /// </summary>
    [ConfigComment("属性系数(百分比)")][ConfigOriginString] public double[] factor;
    /// <summary>
    /// 星级加成
    /// </summary>
    [ConfigComment("星级加成")] public double starValue;
    /// <summary>
    /// 护盾为特殊护盾
    /// </summary>
    [ConfigComment("护盾为特殊护盾")] public bool specialShield;
    // /// <summary>
    // /// 等级加成
    // /// </summary>
    // [ConfigComment("等级加成")] public double lvValue;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ShieldAddEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.targetType = targetType;
        effect.property = property;
        effect.factor = factor;
        effect.starValue = starValue;
        effect.specialShield = specialShield;
        // effect.lvValue = lvValue;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((byte)targetType);
        writer.Write((int)property);
        writer.Write(factor);
        writer.Write(starValue);
        writer.Write(specialShield);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        targetType = (BattleDef.TargetType)reader.ReadByte();
        property = (BattleDef.Property)reader.ReadInt32();
        factor = reader.Read(factor);
        starValue = reader.ReadDouble();
        specialShield = reader.ReadBoolean();
    }

    #endregion
}