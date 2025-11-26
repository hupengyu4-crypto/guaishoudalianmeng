using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 嘲讽
/// </summary>
[Serializable]
public class TauntEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 嘲讽次数
    /// </summary>
    [ConfigComment("嘲讽次数")] public int count;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<TauntEffect>();
        effect.SetBaseData(this, target, authorUid);
        effect.SetData(this);

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(count);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        count = reader.ReadInt32();
    }

    #endregion
}