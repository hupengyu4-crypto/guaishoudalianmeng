using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 伤害减免效果
/// </summary>
[Serializable]
public class DamageReduceEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 减免数值是否叠加
    /// </summary>
    [ConfigComment("减免数值是否叠加")] public bool isOverlay;
    /// <summary>
    /// 系数
    /// </summary>
    [ConfigComment("系数")] public double factor;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<DamageReduceEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.isOverlay = isOverlay;
        effect.factor = factor;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(isOverlay);
        writer.Write(factor);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        isOverlay = reader.ReadBoolean();
        factor = reader.ReadDouble();
    }

    #endregion
}