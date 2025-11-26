using RootScript.Config;
using BattleSystem;
using System;

[Serializable]
public abstract class BaseEffectCfg : Config
{
    /// <summary>
    /// 描述
    /// </summary>
    [ConfigComment("描述")]
    public string describe;
    /// <summary>
    /// 持续大回合
    /// </summary>
    [ConfigComment("持续大回合")]
    public int bout;
    /// <summary>
    /// 最大叠加层数
    /// </summary>
    [ConfigComment("最大叠加层数")]
    public int addMax;
    /// <summary>
    /// 生效几次后自动删除
    /// </summary>
    [ConfigComment("生效几次后自动删除")]
    [ConfigReadonly]
    public int triggerCount;
    /// <summary>
    /// 效果类型(多状态)
    /// </summary>
    [ConfigComment("效果类型(多状态)")]
    public EffectSys.EffectType[] effectType;
    /// <summary>
    /// 效果结束附加效果
    /// </summary>
    [ConfigComment("效果结束附加效果")]
    public long[] endEffects;
    /// <summary>
    /// 显示层的配置
    /// </summary>
    [ConfigComment("显示层配置")]
    public long viewCfg;

    /// <summary>
    /// 创建效果
    /// </summary>
    /// <param name="target">效果目标(效果持有者)</param>
    /// <param name="authorUid">施法者Uid</param>
    /// <returns>效果</returns>
    public abstract BaseEffect Create(BattleObject target, long authorUid);

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.WriteNullableString(describe);
        writer.Write(bout);
        writer.Write(addMax);
        writer.Write(triggerCount);
        writer.Write(effectType != null);
        if(effectType != null)
        {
            writer.Write(effectType.Length);
            for(int a = 0; a < effectType.Length; a++)
            {
                var b = effectType[a];
                writer.Write((ulong)b);
            }
        }
        writer.Write(endEffects);
        writer.Write(viewCfg);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        describe = reader.ReadNullableString();
        bout = reader.ReadInt32();
        addMax = reader.ReadInt32();
        triggerCount = reader.ReadInt32();
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            effectType = new EffectSys.EffectType[aLength];
            for(int a = 0; a < aLength; a++)
            {
                effectType[a] = (EffectSys.EffectType)reader.ReadUInt64();
            }
        }
        else
        {
            effectType = null;
        }
        endEffects = reader.Read(endEffects);
        viewCfg = reader.ReadInt64();
    }

    #endregion
}
