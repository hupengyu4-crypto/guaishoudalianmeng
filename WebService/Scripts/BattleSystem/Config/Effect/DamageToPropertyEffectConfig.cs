using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 伤害转换成属性效果
/// </summary>
[Serializable]
public class DamageToPropertyEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 转换比例（百分比）
    /// </summary>
    [ConfigComment("转换比例（百分比）")] public double value;

    /// <summary>
    /// 属性
    /// </summary>
    [ConfigComment("属性")] public BattleDef.Property property;

    /// <summary>
    /// 转换成护盾值
    /// </summary>
    [ConfigComment("转换成护盾值")] public bool toShield;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<DamageToPropertyEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.value = value;
        effect.property = property;
        effect.toShield = toShield;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(value);
        writer.Write((int)property);
        writer.Write(toShield);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        value = reader.ReadDouble();
        property = (BattleDef.Property)reader.ReadInt32();
        toShield = reader.ReadBoolean();
    }

    #endregion
}