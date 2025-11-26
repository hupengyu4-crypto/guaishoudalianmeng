using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 斩杀
/// </summary>
[Serializable]
public class ComboEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 概率(0-100)
    /// </summary>
    [ConfigComment("几率(0-100)")] public double probability;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ComboEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.probability = probability;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(probability);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        probability = reader.ReadDouble();
    }

    #endregion
}