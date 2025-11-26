using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 护盾承伤反弹
/// </summary>
[Serializable]
public class ShieldAbsorptionReflectiveEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 反伤百分比(1-100)
    /// </summary>
    [ConfigComment("反伤百分比(1-100)")] public double value;

    /// <summary>
    /// 索敌(反伤的目标)
    /// </summary>
    [ConfigComment("索敌(反伤的目标)")] public long findEffects;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ShieldAbsorptionReflectiveEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.value = value;
        effect.findEffects = findEffects;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(value);
        writer.Write(findEffects);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        value = reader.ReadDouble();
        findEffects = reader.ReadInt64();
    }

    #endregion
}