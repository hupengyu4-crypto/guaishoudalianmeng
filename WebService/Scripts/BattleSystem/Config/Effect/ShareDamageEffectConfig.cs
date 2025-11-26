using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 分摊伤害
/// </summary>
[Serializable]
public class ShareDamageEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 索敌(至少小回合结束)
    /// </summary>
    [ConfigComment("索敌(至少小回合结束)")] public long findEffects;
    /// <summary>
    /// 伤害效果
    /// </summary>
    [ConfigComment("伤害效果")] public long damageEffects;
    /// <summary>
    /// 伤害效果先作用于目标，如果目标死亡后再分摊
    /// </summary>
    [ConfigComment("伤害效果先作用于目标，如果目标死亡后再分摊")] public bool bekilled;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ShareDamageEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.findEffects = findEffects;
        effect.damageEffects = damageEffects;
        effect.bekilled = bekilled;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(findEffects);
        writer.Write(damageEffects);
        writer.Write(bekilled);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        findEffects = reader.ReadInt64();
        damageEffects = reader.ReadInt64();
        bekilled = reader.ReadBoolean();
    }

    #endregion
}