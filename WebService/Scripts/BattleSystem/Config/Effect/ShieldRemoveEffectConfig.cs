using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 护盾移除
/// </summary>
[Serializable]
public class ShieldRemoveEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 移除护盾百分比(1-100)
    /// </summary>
    [ConfigComment("移除护盾百分比(1-100)")] public double value;
    /// <summary>
    /// 星级加成
    /// </summary>
    [ConfigComment("星级加成")] public double starValue;
    /// <summary>
    /// 等级加成
    /// </summary>
    [ConfigComment("等级加成")] public double lvValue;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ShieldRemoveEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.value = value;
        effect.starValue = starValue;
        effect.lvValue = lvValue;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(value);
        writer.Write(starValue);
        writer.Write(lvValue);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        value = reader.ReadDouble();
        starValue = reader.ReadDouble();
        lvValue = reader.ReadDouble();
    }

    #endregion
}