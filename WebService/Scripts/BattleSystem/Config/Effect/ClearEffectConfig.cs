using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 清除效果
/// </summary>
[Serializable]
public class ClearEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 清除效果的sid
    /// </summary>
    [ConfigComment("清除效果的sid")] public long[] clearSids;
    /// <summary>
    /// (选择类型清理时)禁止清理效果类型
    /// </summary>
    [ConfigComment("(选择类型清理时)禁止清理效果类型")] public EffectSys.EffectType[] forbidTypes;
    /// <summary>
    /// (选择类型清理时)清理效果类型
    /// </summary>
    [ConfigComment("(选择类型清理时)清理效果类型")] public EffectSys.EffectType[] checkTypes;
    /// <summary>
    /// 清理数量
    /// </summary>
    [ConfigComment("清理数量")] public int count;
    /// <summary>
    /// 清理前是否引爆伤害
    /// </summary>
    [ConfigComment("清理前是否引爆伤害")] public bool triggerAllDamage;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ClearEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.clearSids = clearSids;
        effect.forbidType = forbidTypes;
        effect.checkType = checkTypes;
        effect.count = count;
        effect.triggerAllDamage = triggerAllDamage;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(clearSids);
        writer.Write(forbidTypes != null);
        if(forbidTypes != null)
        {
            writer.Write(forbidTypes.Length);
            for(int a = 0; a < forbidTypes.Length; a++)
            {
                var b = forbidTypes[a];
                writer.Write((ulong)b);
            }
        }
        writer.Write(checkTypes != null);
        if(checkTypes != null)
        {
            writer.Write(checkTypes.Length);
            for(int a = 0; a < checkTypes.Length; a++)
            {
                var b = checkTypes[a];
                writer.Write((ulong)b);
            }
        }
        writer.Write(count);
        writer.Write(triggerAllDamage);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        clearSids = reader.Read(clearSids);
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            forbidTypes = new EffectSys.EffectType[aLength];
            for(int a = 0; a < aLength; a++)
            {
                forbidTypes[a] = (EffectSys.EffectType)reader.ReadUInt64();
            }
        }
        else
        {
            forbidTypes = null;
        }
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            checkTypes = new EffectSys.EffectType[aLength];
            for(int a = 0; a < aLength; a++)
            {
                checkTypes[a] = (EffectSys.EffectType)reader.ReadUInt64();
            }
        }
        else
        {
            checkTypes = null;
        }
        count = reader.ReadInt32();
        triggerAllDamage = reader.ReadBoolean();
    }

    #endregion
}