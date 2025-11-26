using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 吸血
/// </summary>
[Serializable]
public class VampireEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 角色攻击事件(仅限)
    /// </summary>
    [ConfigComment("角色攻击事件(仅限)")] public BattleObject.Event fighterEvent;
    /// <summary>
    /// 吸取系数(百分比)
    /// </summary>
    [ConfigComment("吸取系数(百分比)")] public double factor;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<VampireEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.fighterEvent = fighterEvent;
        effect.factor = factor;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((int)fighterEvent);
        writer.Write(factor);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        fighterEvent = (BattleObject.Event)reader.ReadInt32();
        factor = reader.ReadDouble();
    }

    #endregion
}