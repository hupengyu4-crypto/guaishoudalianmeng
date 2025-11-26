using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 立即行动
/// </summary>
[Serializable]
public class DoLogicConfig : BaseEffectCfg
{
    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<DoLogic>();
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