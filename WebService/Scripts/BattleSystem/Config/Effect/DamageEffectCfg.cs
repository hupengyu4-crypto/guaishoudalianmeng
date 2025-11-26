using System;
using RootScript.Config;
using BattleSystem;

/// <summary>
/// 伤害效果
/// </summary>
[Serializable]
public class DamageEffectCfg : BaseEffectCfg
{
    /// <summary>
    /// 伤害类型
    /// </summary>
    [ConfigComment("伤害类型")] public BattleDef.DamageType damageType = BattleDef.DamageType.Normal;
    /// <summary>
    /// 参照目标类型
    /// </summary>
    [ConfigComment("参照目标")] public BattleDef.TargetType propTarget;
    /// <summary>
    /// 参照属性
    /// </summary>
    [ConfigComment("参照属性")] public BattleDef.Property[] properties;
    /// <summary>
    /// 参照属性系数(百分比)
    /// </summary>
    [ConfigComment("参照属性系数(百分比)")][ConfigOriginString] public double[][] factors;
    /// <summary>
    /// 最大值参照目标类型
    /// </summary>
    [ConfigComment("最大值参照目标")] public BattleDef.TargetType maxPropTarget;
    /// <summary>
    /// 最大值参照属性
    /// </summary>
    [ConfigComment("最大值参照属性")] public BattleDef.Property maxProperty;
    /// <summary>
    /// 最大值参照属性系数(百分比)
    /// </summary>
    [ConfigComment("最大值参照属性系数(百分比)")] public double maxValue = 100;
    /// <summary>
    /// 伤害倍率
    /// </summary>
    [ConfigComment("伤害倍率")] public double rate = 1;
    /// <summary>
    /// 是否真实伤害
    /// </summary>
    [ConfigComment("是否真实伤害")] public bool isRealDamage;
    /// <summary>
    /// 伤害回血系数(百分比)
    /// </summary>
    [ConfigComment("伤害回血系数(百分比)")] public double healFactor;
    /// <summary>
    /// 技能固定伤害
    /// </summary>
    [ConfigComment("技能固定伤害")] public double fixedValue;
    /// <summary>
    /// 星级伤害
    /// </summary>
    [ConfigComment("星级伤害")] public double starValue;
    /// <summary>
    /// 伤害回血系数(百分比)
    /// </summary>
    [ConfigComment("等级伤害")] public double lvValue;
    /// <summary>
    /// 乘效果的层数
    /// </summary>
    [ConfigComment("乘效果的层数")] public EffectSys.EffectType multiplyByEffect;
    /// <summary>
    /// 附带累计受到普攻伤害百分比
    /// </summary>
    [ConfigComment("附带累计受到普攻伤害百分比")] public double allNormalDmg;
    /// <summary>
    /// 回血索敌
    /// </summary>
    [ConfigComment("回血索敌")] public long healFindEffects;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<DamageEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.damageType = damageType;
        effect.propTarget = propTarget;
        effect.properties = properties;
        effect.factors = factors;
        effect.maxPropTarget = maxPropTarget;
        effect.maxProperty = maxProperty;
        effect.maxValue = maxValue;
        effect.rate = rate;
        effect.isRealDamage = isRealDamage;
        effect.healFactor = healFactor;
        effect.otherFactor = 0;
        effect.fixedValue = fixedValue;
        effect.starValue = starValue;
        effect.lvValue = lvValue;
        effect.multiplyByEffect = multiplyByEffect;
        effect.allNormalDmg = allNormalDmg;
        effect.healFindEffects = healFindEffects;
        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((byte)damageType);
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
        writer.Write(factors != null);
        if(factors != null)
        {
            writer.Write(factors.Length);
            for(int a = 0; a < factors.Length; a++)
            {
                var b = factors[a];
                writer.Write(b);
            }
        }
        writer.Write((byte)maxPropTarget);
        writer.Write((int)maxProperty);
        writer.Write(maxValue);
        writer.Write(rate);
        writer.Write(isRealDamage);
        writer.Write(healFactor);
        writer.Write(fixedValue);
        writer.Write(starValue);
        writer.Write(lvValue);
        writer.Write((ulong)multiplyByEffect);
        writer.Write(allNormalDmg);
        writer.Write(healFindEffects);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        damageType = (BattleDef.DamageType)reader.ReadByte();
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
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            factors = new double[aLength][];
            for(int a = 0; a < aLength; a++)
            {
                factors[a] = reader.Read(factors[a]);
            }
        }
        else
        {
            factors = null;
        }
        maxPropTarget = (BattleDef.TargetType)reader.ReadByte();
        maxProperty = (BattleDef.Property)reader.ReadInt32();
        maxValue = reader.ReadDouble();
        rate = reader.ReadDouble();
        isRealDamage = reader.ReadBoolean();
        healFactor = reader.ReadDouble();
        fixedValue = reader.ReadDouble();
        starValue = reader.ReadDouble();
        lvValue = reader.ReadDouble();
        multiplyByEffect = (EffectSys.EffectType)reader.ReadUInt64();
        allNormalDmg = reader.ReadDouble();
        healFindEffects = reader.ReadInt64();
    }

    #endregion
}