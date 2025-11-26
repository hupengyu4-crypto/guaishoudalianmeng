using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 伤害提升效果
/// </summary>
[Serializable]
public class DamageUpEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 参照目标类型
    /// </summary>
    [ConfigComment("参照目标类型")] public BattleDef.TargetType targetType;
    /// <summary>
    /// 伤害提升类型
    /// </summary>
    [ConfigComment("伤害提升类型")] public DamageUpEffect.UpType upType;
    /// <summary>
    /// 系数
    /// </summary>
    [ConfigComment("系数")] public double factor;
    /// <summary>
    /// 结束时是否保留
    /// </summary>
    [ConfigComment("结束时是否保留")] public bool isRetain;
    /// <summary>
    /// 影响始终生效的额外伤害系数
    /// </summary>
    [ConfigComment("影响始终生效的额外伤害系数")] public bool always;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<DamageUpEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.targetType = targetType;
        effect.upType = upType;
        effect.factor = factor;
        effect.isRetain = isRetain;
        effect.always = always;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((byte)targetType);
        writer.Write((int)upType);
        writer.Write(factor);
        writer.Write(isRetain);
        writer.Write(always);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        targetType = (BattleDef.TargetType)reader.ReadByte();
        upType = (DamageUpEffect.UpType)reader.ReadInt32();
        factor = reader.ReadDouble();
        isRetain = reader.ReadBoolean();
        always = reader.ReadBoolean();
    }

    #endregion
}