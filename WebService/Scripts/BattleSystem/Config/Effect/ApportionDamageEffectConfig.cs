using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 按回合分摊伤害
/// </summary>
[Serializable]
public class ApportionDamageEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 首次伤害百分比
    /// </summary>
    [ConfigComment("首次伤害百分比")] public double firstFactor;
    /// <summary>
    /// 分摊回合
    /// </summary>
    [ConfigComment("分摊回合")] public int apportionBout;
    /// <summary>
    /// 分摊伤害是否忽略护盾
    /// </summary>
    [ConfigComment("分摊伤害是否忽略护盾")] public bool ignoreShield;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ApportionDamageEffect>();
        effect.SetBaseData(this, target, authorUid);
        effect.SetData(this);

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(firstFactor);
        writer.Write(apportionBout);
        writer.Write(ignoreShield);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        firstFactor = reader.ReadDouble();
        apportionBout = reader.ReadInt32();
        ignoreShield = reader.ReadBoolean();
    }

    #endregion
}