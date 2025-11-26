using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 检查战斗者效果
/// </summary>
[Serializable]
public class CheckFighterEffectConfig : BaseEffectCfg
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
    [ConfigComment("角色Sid 如果指定角色 BattleDef.EFindType 只能我方全体 或者敌方全体")] public long[] roleSid;
    /// <summary>
    /// 检测筛选出的种族
    /// </summary>
    [ConfigComment("检测筛选出的种族")] public BattleDef.ERaceType checkRaceType;
    /// <summary>
    /// 通过给自己的效果
    /// </summary>
    [ConfigComment("通过给自己的效果")] public long[] passedSelfEffects;
    /// <summary>
    /// 通过给索敌目标的效果
    /// </summary>
    [ConfigComment("通过给目标的效果")] public long[] passedTargetEffects;
    /// <summary>
    /// 通过给造成目标事件的效果
    /// </summary>
    [ConfigComment("通过给造成目标事件的效果")] public long[] passedAuthorEffects; 
    /// <summary>
    /// 目标中是否包含自己
    /// </summary>
    [ConfigComment("目标中是否包含自己")] public bool targetIncludeSelf;
    /// <summary>
    /// 筛选数量
    /// </summary>
    [ConfigComment("筛选数量")] public int filtrateCount;
    /// <summary>
    /// 比较类型
    /// </summary>
    [ConfigComment("比较类型")] public BattleDef.SimpleCompareType compareType;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<CheckFighterEffect>();
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
        writer.Write((int)checkRaceType);
        writer.Write(passedSelfEffects);
        writer.Write(passedTargetEffects);
        writer.Write(passedAuthorEffects);
        writer.Write(targetIncludeSelf);
        writer.Write(filtrateCount);
        writer.Write((byte)compareType);
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
        roleSid = reader.Read(roleSid);
        checkRaceType = (BattleDef.ERaceType)reader.ReadInt32();
        passedSelfEffects = reader.Read(passedSelfEffects);
        passedTargetEffects = reader.Read(passedTargetEffects);
        passedAuthorEffects = reader.Read(passedAuthorEffects);
        targetIncludeSelf = reader.ReadBoolean();
        filtrateCount = reader.ReadInt32();
        compareType = (BattleDef.SimpleCompareType)reader.ReadByte();
    }

    #endregion
}