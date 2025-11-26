using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 传导效果
/// </summary>
[Serializable]
public class ConductionEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 添加的效果类型(不配表示所有debuff)
    /// </summary>
    [ConfigComment("添加的效果类型(不配表示所有debuff)")] public EffectSys.EffectType[] addedEffectType;

    /// <summary>
    /// 概率(0-100)
    /// </summary>
    [ConfigComment("几率(0-100)")] public double probability;

    /// <summary>
    /// 索敌(传导给谁，0是传给队友)
    /// </summary>
    [ConfigComment("索敌(传导给谁，0是传给队友)")] public long findEffect;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ConductionEffect>();
        effect.SetBaseData(this, target, authorUid);
        effect.addedEffectType = addedEffectType;
        effect.probability = probability;
        effect.findEffect = findEffect;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(addedEffectType != null);
        if(addedEffectType != null)
        {
            writer.Write(addedEffectType.Length);
            for(int a = 0; a < addedEffectType.Length; a++)
            {
                var b = addedEffectType[a];
                writer.Write((ulong)b);
            }
        }
        writer.Write(probability);
        writer.Write(findEffect);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            addedEffectType = new EffectSys.EffectType[aLength];
            for(int a = 0; a < aLength; a++)
            {
                addedEffectType[a] = (EffectSys.EffectType)reader.ReadUInt64();
            }
        }
        else
        {
            addedEffectType = null;
        }
        probability = reader.ReadDouble();
        findEffect = reader.ReadInt64();
    }

    #endregion
}