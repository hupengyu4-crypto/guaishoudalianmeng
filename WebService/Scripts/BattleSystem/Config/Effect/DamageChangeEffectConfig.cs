using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 检查条件，改变伤害
/// </summary>
[Serializable]
public class DamageChangeEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 检查状态
    /// </summary>
    [ConfigComment("检查状态")] public BattleObject.State[] checkState;
    /// <summary>
    /// 检查效果类型
    /// </summary>
    [ConfigComment("检查效果类型")] public EffectSys.EffectType[] checkEffectTypes;
    /// <summary>
    /// 职业选择
    /// </summary>
    [ConfigComment("检查职业")] public BattleDef.EJobType[] checkJobs;
    /// <summary>
    /// 检测的属性
    /// </summary>
    [ConfigComment("检测的属性")] public BattleDef.Property property;
    /// <summary>
    /// 检测的类型
    /// </summary>
    [ConfigComment("检测的类型")] public BattleDef.CompareType checkType;
    /// <summary>
    /// 属性值(百分比)
    /// </summary>
    [ConfigComment("属性值(百分比)")] public double value;
    /// <summary>
    /// 是否同时满足条件
    /// </summary>
    [ConfigComment("是否同时满足条件")] public bool isCombine;
    /// <summary>
    /// 是否计算数量
    /// </summary>
    [ConfigComment("是否计算数量")] public bool isCalculateCount;
    /// <summary>
    /// 最大数量
    /// </summary>
    [ConfigComment("最大数量")] public int maxCount;
    /// <summary>
    /// 单个增伤百分比
    /// </summary>
    [ConfigComment("单个增伤百分比")] public double factor;
    /// <summary>
    /// 差异的百分之多少算一份（百分比）
    /// </summary>
    [ConfigComment("差异的百分之多少算一份（百分比）")] public double step;
    /// <summary>
    /// 一份改变多少值
    /// </summary>
    [ConfigComment("一份改变多少值")] public double stepChangePerValue;
    /// <summary>
    /// 是否数值类型
    /// </summary>
    [ConfigComment("是否数值类型")] public bool isNumber;
    /// <summary>
    /// 被击时改变伤害
    /// </summary>
    [ConfigComment("被击时改变伤害")] public bool beDamaged;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<DamageChangeEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.checkState = checkState;
        effect.checkTypes = checkEffectTypes;
        effect.checkJobs = checkJobs;
        effect.property = property;
        effect.checkType = checkType;
        effect.value = value;
        effect.isCombine = isCombine;
        effect.isCalculateCount = isCalculateCount;
        effect.maxCount = maxCount;
        effect.factor = factor;
        effect.step = step;
        effect.stepChangePerValue = stepChangePerValue;
        effect.isNumber = isNumber;
        effect.beDamaged = beDamaged;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
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
        writer.Write(checkEffectTypes != null);
        if(checkEffectTypes != null)
        {
            writer.Write(checkEffectTypes.Length);
            for(int a = 0; a < checkEffectTypes.Length; a++)
            {
                var b = checkEffectTypes[a];
                writer.Write((ulong)b);
            }
        }
        writer.Write(checkJobs != null);
        if(checkJobs != null)
        {
            writer.Write(checkJobs.Length);
            for(int a = 0; a < checkJobs.Length; a++)
            {
                var b = checkJobs[a];
                writer.Write((int)b);
            }
        }
        writer.Write((int)property);
        writer.Write((byte)checkType);
        writer.Write(value);
        writer.Write(isCombine);
        writer.Write(isCalculateCount);
        writer.Write(maxCount);
        writer.Write(factor);
        writer.Write(step);
        writer.Write(stepChangePerValue);
        writer.Write(isNumber);
        writer.Write(beDamaged);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
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
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            checkEffectTypes = new EffectSys.EffectType[aLength];
            for(int a = 0; a < aLength; a++)
            {
                checkEffectTypes[a] = (EffectSys.EffectType)reader.ReadUInt64();
            }
        }
        else
        {
            checkEffectTypes = null;
        }
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            checkJobs = new BattleDef.EJobType[aLength];
            for(int a = 0; a < aLength; a++)
            {
                checkJobs[a] = (BattleDef.EJobType)reader.ReadInt32();
            }
        }
        else
        {
            checkJobs = null;
        }
        property = (BattleDef.Property)reader.ReadInt32();
        checkType = (BattleDef.CompareType)reader.ReadByte();
        value = reader.ReadDouble();
        isCombine = reader.ReadBoolean();
        isCalculateCount = reader.ReadBoolean();
        maxCount = reader.ReadInt32();
        factor = reader.ReadDouble();
        step = reader.ReadDouble();
        stepChangePerValue = reader.ReadDouble();
        isNumber = reader.ReadBoolean();
        beDamaged = reader.ReadBoolean();
    }

    #endregion
}