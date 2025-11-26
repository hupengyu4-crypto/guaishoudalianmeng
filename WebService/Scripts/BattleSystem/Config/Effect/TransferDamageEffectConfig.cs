using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 转移伤害
/// </summary>
[Serializable]
public class TransferDamageEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 转移比例（百分比）
    /// </summary>
    [ConfigComment("转移比例（百分比）")] public double value;

    /// <summary>
    /// 伤害从自己转移到目标
    /// </summary>
    [ConfigComment("伤害从自己转移到目标")] public bool transferToTarget;

    /// <summary>
    /// 剩余伤害是否清零
    /// </summary>
    [ConfigComment("剩余伤害是否清零")] public bool noLeftDamage;

    /// <summary>
    /// 是否特定伤害类型生效
    /// </summary>
    [ConfigComment("是否特定伤害类型生效")] public bool careDamageType;

    /// <summary>
    /// 伤害类型
    /// </summary>
    [ConfigComment("伤害类型")] public BattleDef.DamageType damageType;

    /// <summary>
    /// 是否指定转移后的伤害类型
    /// </summary>
    [ConfigComment("是否指定转移后的伤害类型")] public bool careTransferredDamageType;

    /// <summary>
    /// 转移后的类型
    /// </summary>
    [ConfigComment("转移后的类型")] public BattleDef.DamageType transferredDamageType;

    /// <summary>
    /// 转移给攻击方
    /// </summary>
    [ConfigComment("转移给攻击方")] public bool transferToAttacker;

    /// <summary>
    /// 转移几次后删除自身
    /// </summary>
    [ConfigComment("转移几次后删除自身")] public int transferCount;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<TransferDamageEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.value = value;
        effect.transferToTarget = transferToTarget;
        effect.noLeftDamage = noLeftDamage;
        effect.careDamageType = careDamageType;
        effect.damageType = damageType;
        effect.careTransferredDamageType = careTransferredDamageType;
        effect.transferredDamageType = transferredDamageType;
        effect.transferToAttacker = transferToAttacker;
        effect.transferCount = transferCount;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(value);
        writer.Write(transferToTarget);
        writer.Write(noLeftDamage);
        writer.Write(careDamageType);
        writer.Write((byte)damageType);
        writer.Write(careTransferredDamageType);
        writer.Write((byte)transferredDamageType);
        writer.Write(transferToAttacker);
        writer.Write(transferCount);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        value = reader.ReadDouble();
        transferToTarget = reader.ReadBoolean();
        noLeftDamage = reader.ReadBoolean();
        careDamageType = reader.ReadBoolean();
        damageType = (BattleDef.DamageType)reader.ReadByte();
        careTransferredDamageType = reader.ReadBoolean();
        transferredDamageType = (BattleDef.DamageType)reader.ReadByte();
        transferToAttacker = reader.ReadBoolean();
        transferCount = reader.ReadInt32();
    }

    #endregion
}