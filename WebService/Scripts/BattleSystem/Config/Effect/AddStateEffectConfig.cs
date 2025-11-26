using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 添加状态效果
/// </summary>
[Serializable]
public class AddStateEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 自己触发效果
    /// </summary>
    [ConfigComment("自己触发效果")] public long[] selfEffects;
    /// <summary>
    /// 目标触发效果
    /// </summary>
    [ConfigComment("目标触发效果")] public long[] targetEffects;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<AddStateEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.selfEffects = selfEffects;
        effect.targetEffects = targetEffects;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(selfEffects);
        writer.Write(targetEffects);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        selfEffects = reader.Read(selfEffects);
        targetEffects = reader.Read(targetEffects);
    }

    #endregion
}