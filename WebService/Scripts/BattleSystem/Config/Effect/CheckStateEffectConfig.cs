using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 检测状态效果
/// </summary>
[Serializable]
public class CheckStateEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 通过给自己加效果
    /// </summary>
    [ConfigComment("通过给自己加效果")] public long[] passedSelfEffects;
    /// <summary>
    /// 通过给目标加效果
    /// </summary>
    [ConfigComment("通过给目标加效果")] public long[] passedTargetEffects;
    /// <summary>
    /// 未通过给自己加效果
    /// </summary>
    [ConfigComment("未通过给自己加效果")] public long[] failedSelfEffects;
    /// <summary>
    /// 未通过给目标加效果
    /// </summary>
    [ConfigComment("未通过给目标加效果")] public long[] failedTargetEffects;
    /// <summary>
    /// 检查状态
    /// </summary>
    [ConfigComment("检查状态")] public BattleObject.State[] checkState;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<CheckStateEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.passedSelfEffects = passedSelfEffects;
        effect.passedTargetEffects = passedTargetEffects;
        effect.failedSelfEffects = failedSelfEffects;
        effect.failedTargetEffects = failedTargetEffects;
        effect.checkState = checkState;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(passedSelfEffects);
        writer.Write(passedTargetEffects);
        writer.Write(failedSelfEffects);
        writer.Write(failedTargetEffects);
        writer.Write(checkState != null);
        if(checkState != null)
        {
            writer.Write(checkState.Length);
            for(int a = 0; a < checkState.Length; a++)
            {
                var b = checkState[a];
                writer.Write((ulong)b);
            }
        }
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        passedSelfEffects = reader.Read(passedSelfEffects);
        passedTargetEffects = reader.Read(passedTargetEffects);
        failedSelfEffects = reader.Read(failedSelfEffects);
        failedTargetEffects = reader.Read(failedTargetEffects);
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            checkState = new BattleObject.State[aLength];
            for(int a = 0; a < aLength; a++)
            {
                checkState[a] = (BattleObject.State)reader.ReadUInt64();
            }
        }
        else
        {
            checkState = null;
        }
    }

    #endregion
}