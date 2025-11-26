using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 添加护盾
/// </summary>
[Serializable]
public class ShieldStealEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 属性系数(百分比)
    /// </summary>
    [ConfigComment("属性系数(百分比)")] public double factor;

    /// <summary>
    /// 索敌
    /// </summary>
    [ConfigComment("索敌")] public long findEffects;

    /// <summary>
    /// 星级加成（百分比）
    /// </summary>
    [ConfigComment("星级加成（百分比）")] public double starValue;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ShieldStealEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.factor = factor;
        effect.findEffects = findEffects;
        effect.starValue = starValue;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(factor);
        writer.Write(findEffects);
        writer.Write(starValue);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        factor = reader.ReadDouble();
        findEffects = reader.ReadInt64();
        starValue = reader.ReadDouble();
    }

    #endregion
}