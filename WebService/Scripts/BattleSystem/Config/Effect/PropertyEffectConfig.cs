using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 属性效果
/// </summary>
[Serializable]
public class PropertyEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 参照目标类型
    /// </summary>
    [ConfigComment("参照目标")] public BattleDef.TargetType propTarget;
    /// <summary>
    /// 修改的属性
    /// </summary>
    [ConfigComment("修改的属性")] public BattleDef.Property property;
    /// <summary>
    /// 修改值(百分比)
    /// </summary>
    [ConfigComment("修改值(百分比)")][ConfigOriginString] public double[] value;
    /// <summary>
    /// 星级加成
    /// </summary>
    [ConfigComment("星级加成")] public double starValue;
    /// <summary>
    /// 等级加成
    /// </summary>
    [ConfigComment("等级加成")] public double lvValue;
    /// <summary>
    /// 结束时是否保留
    /// </summary>
    [ConfigComment("结束时是否保留")] public bool isRetain;
    /// <summary>
    /// 是否乘以当前回合数
    /// </summary>
    [ConfigComment("是否乘以当前回合数")] public bool isUseBout;
    /// <summary>
    /// 是否数值类型
    /// </summary>
    [ConfigComment("是否数值类型")] public bool isNumber;


    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<PropertyEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.propTarget = propTarget;
        effect.property = property;
        effect.value = value;
        effect.starValue = starValue;
        effect.lvValue = lvValue;
        effect.isRetain = isRetain;
        effect.isUseBout = isUseBout;
        effect.isNumber = isNumber;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write((byte)propTarget);
        writer.Write((int)property);
        writer.Write(value);
        writer.Write(starValue);
        writer.Write(lvValue);
        writer.Write(isRetain);
        writer.Write(isUseBout);
        writer.Write(isNumber);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        propTarget = (BattleDef.TargetType)reader.ReadByte();
        property = (BattleDef.Property)reader.ReadInt32();
        value = reader.Read(value);
        starValue = reader.ReadDouble();
        lvValue = reader.ReadDouble();
        isRetain = reader.ReadBoolean();
        isUseBout = reader.ReadBoolean();
        isNumber = reader.ReadBoolean();
    }

    #endregion
}