using RootScript.Config;
using BattleSystem;

[System.Serializable]
public class BattleSkillCfg : Config
{
    [ConfigComment("描述")] public string describe;
    [ConfigComment("消耗怒气")] public int rage;
    [ConfigComment("技能类型")] public BattleDef.ESkillType skillType;
    [ConfigComment("先给自己的效果")] public long[] selfEffects;
    [ConfigComment("给目标的效果")] public long[] effects;
    [ConfigComment("后给自己的效果")] public long[] selfEndEffects;
    [ConfigComment("初始cd(回合)")] public double initCd;
    [ConfigComment("冷却cd(回合)")] public double cd;
    [ConfigComment("查找范围")] public BattleDef.EFindType findType;
    [ConfigComment("种族选择")] public BattleDef.ERaceType[] raceTypes;
    [ConfigComment("职业选择")] public BattleDef.EJobType[] jobTypes;
    [ConfigComment("角色Sid 如果指定角色 BattleDef.EFindType 只能我方全体 或者敌方全体")] public long roleSid;
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
    [ConfigComment("其他类型的参数")] public long[] otherArgs;
    /// <summary>
    /// 技能显示配置(SkinSkillViewConfig)
    /// </summary>
    [ConfigComment("技能显示配置(SkinSkillViewConfig)")] public long[] skillViewConfig;

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.WriteNullableString(describe);
        writer.Write(rage);
        writer.Write((byte)skillType);
        writer.Write(selfEffects);
        writer.Write(effects);
        writer.Write(selfEndEffects);
        writer.Write(initCd);
        writer.Write(cd);
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
        writer.Write(skillViewConfig);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        describe = reader.ReadNullableString();
        rage = reader.ReadInt32();
        skillType = (BattleDef.ESkillType)reader.ReadByte();
        selfEffects = reader.Read(selfEffects);
        effects = reader.Read(effects);
        selfEndEffects = reader.Read(selfEndEffects);
        initCd = reader.ReadDouble();
        cd = reader.ReadDouble();
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
        skillViewConfig = reader.Read(skillViewConfig);
    }

    #endregion
}