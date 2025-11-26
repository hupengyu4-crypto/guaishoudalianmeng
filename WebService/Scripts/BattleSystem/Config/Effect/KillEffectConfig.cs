using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 斩杀
/// </summary>
[Serializable]
public class KillEffectConfig : BaseEffectCfg
{
    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<KillEffect>();
        effect.SetBaseData(this, target, authorUid);

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
    }

    #endregion
}