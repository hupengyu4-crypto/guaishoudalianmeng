using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 治疗效果
/// </summary>
[Serializable]
public class HealEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 参照目标类型
    /// </summary>
    [ConfigComment("参照目标")] public BattleDef.TargetType propTarget;
    /// <summary>
    /// 参照属性
    /// </summary>
    [ConfigComment("参照属性")] public BattleDef.Property[] properties;
    /// <summary>
    /// 属性系数(百分比)
    /// </summary>
    [ConfigComment("属性系数(百分比)")] public double[] factors;
    /// <summary>
    /// 是否参照损失血量
    /// </summary>
    [ConfigComment("是否参照损失血量")] public bool lossHp;
    /// <summary>
    /// 损失血量系数(百分比)
    /// </summary>
    [ConfigComment("损失血量系数(百分比)")] public double lossHpFactor;
    /// <summary>
    /// 溢出治疗量转换为护盾百分比
    /// </summary>
    [ConfigComment("溢出治疗量转换为护盾百分比")] public double overHealToShield;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<HealEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.propTarget = propTarget;
        effect.properties = properties;
        effect.factors = factors;
        effect.lossHp = lossHp;
        effect.lossHpFactor = lossHpFactor;
        effect.overHealToShield = overHealToShield;
        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((byte)propTarget);
        writer.Write(properties != null);
        if(properties != null)
        {
            writer.Write(properties.Length);
            for(int a = 0; a < properties.Length; a++)
            {
                var b = properties[a];
                writer.Write((int)b);
            }
        }
        writer.Write(factors);
        writer.Write(lossHp);
        writer.Write(lossHpFactor);
        writer.Write(overHealToShield);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        propTarget = (BattleDef.TargetType)reader.ReadByte();
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            properties = new BattleDef.Property[aLength];
            for(int a = 0; a < aLength; a++)
            {
                properties[a] = (BattleDef.Property)reader.ReadInt32();
            }
        }
        else
        {
            properties = null;
        }
        factors = reader.Read(factors);
        lossHp = reader.ReadBoolean();
        lossHpFactor = reader.ReadDouble();
        overHealToShield = reader.ReadDouble();
    }

    #endregion
}