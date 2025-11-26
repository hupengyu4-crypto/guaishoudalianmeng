using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 索敌效果
/// </summary>
[Serializable]
public class FindTargetEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 查找范围
    /// </summary>
    [ConfigComment("查找范围")] public BattleDef.EFindType findType;
    /// <summary>
    /// 种族选择
    /// </summary>
    [ConfigComment("种族选择")] public BattleDef.ERaceType[] raceTypes;
    /// <summary>
    /// 职业选择
    /// </summary>
    [ConfigComment("职业选择")] public BattleDef.EJobType[] jobTypes;
    /// <summary>
    /// 角色Sid 如果指定角色 BattleDef.EFindType 只能我方全体 或者敌方全体
    /// </summary>
    [ConfigComment("角色Sid 如果指定角色 BattleDef.EFindType 只能我方全体 或者敌方全体")] public long roleSid;
    /// <summary>
    /// 命中数量
    /// </summary>
    [ConfigComment("命中数量")] public int count;
    /// <summary>
    /// 状态选择
    /// </summary>
    [ConfigComment("状态选择")] public BattleObject.State[] checkState;
    /// <summary>
    /// 属性选择
    /// </summary>
    [ConfigComment("属性选择")] public BattleDef.Property property;
    /// <summary>
    /// 其他类型
    /// </summary>
    [ConfigComment("其他类型")] public BattleDef.OtherType otherType;
    /// <summary>
    /// 比较类型
    /// </summary>
    [ConfigComment("比较类型")] public BattleDef.CompareGradeType compareType;
    /// <summary>
    /// 其他类型的参数
    /// </summary>
    [ConfigComment("比较类型")] public long[] otherArgs;
    /// <summary>
    /// 不换排搜索
    /// </summary>
    [ConfigComment("不换排搜索")] public bool notSwitchRow; 

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<FindTargetEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.SetData(this);

        return effect; 
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((byte)findType);
        writer.Write(raceTypes != null);
        if(raceTypes != null)
        {
            writer.Write(raceTypes.Length);
            for(int a = 0; a < raceTypes.Length; a++)
            {
                var b = raceTypes[a];
                writer.Write((int)b);
            }
        }
        writer.Write(jobTypes != null);
        if(jobTypes != null)
        {
            writer.Write(jobTypes.Length);
            for(int a = 0; a < jobTypes.Length; a++)
            {
                var b = jobTypes[a];
                writer.Write((int)b);
            }
        }
        writer.Write(roleSid);
        writer.Write(count);
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
        writer.Write((int)property);
        writer.Write((byte)otherType);
        writer.Write((byte)compareType);
        writer.Write(otherArgs);
        writer.Write(notSwitchRow);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        findType = (BattleDef.EFindType)reader.ReadByte();
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            raceTypes = new BattleDef.ERaceType[aLength];
            for(int a = 0; a < aLength; a++)
            {
                raceTypes[a] = (BattleDef.ERaceType)reader.ReadInt32();
            }
        }
        else
        {
            raceTypes = null;
        }
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            jobTypes = new BattleDef.EJobType[aLength];
            for(int a = 0; a < aLength; a++)
            {
                jobTypes[a] = (BattleDef.EJobType)reader.ReadInt32();
            }
        }
        else
        {
            jobTypes = null;
        }
        roleSid = reader.ReadInt64();
        count = reader.ReadInt32();
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
        property = (BattleDef.Property)reader.ReadInt32();
        otherType = (BattleDef.OtherType)reader.ReadByte();
        compareType = (BattleDef.CompareGradeType)reader.ReadByte();
        otherArgs = reader.Read(otherArgs);
        notSwitchRow = reader.ReadBoolean();
    }

    #endregion
}