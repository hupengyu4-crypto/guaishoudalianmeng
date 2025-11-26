using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 检测事件效果
/// </summary>
[Serializable]
public class CheckEventEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 通过给自己的效果
    /// </summary>
    [ConfigComment("通过给自己的效果")] public long[] passedSelfEffects;
    /// <summary>
    /// 通过给索敌目标的效果
    /// </summary>
    [ConfigComment("通过给索敌目标的效果")] public long[] passedFindTargetEffects;
    /// <summary>
    /// 通过给造成目标事件的人的效果
    /// </summary>
    [ConfigComment("通过给造成目标事件的人的效果")] public long[] passedTargetEffects;
    /// <summary>
    /// 角色事件
    /// </summary>
    [ConfigComment("角色事件")] public BattleObject.Event fighterEvent;
    /// <summary>
    /// 战场事件
    /// </summary>
    [ConfigComment("战场事件")] public BaseBattle.Event battleEvent;
    /// <summary>
    /// 角色效果事件
    /// </summary>
    [ConfigComment("角色效果事件")] public EffectSys.Event effectEvent;
    /// <summary>
    /// 扩展信息
    /// </summary>
    [ConfigComment("扩展信息1")] public double extraOne;
    /// <summary>
    /// 扩展信息2
    /// </summary>
    [ConfigComment("扩展信息2")] public double extraTwo;
    /// <summary>
    /// 扩展信息3
    /// </summary>
    [ConfigComment("扩展信息3")] public double extraThree;
    /// <summary>
    /// 是否停止事件执行
    /// </summary>
    [ConfigComment("是否停止事件执行")] public bool isStopEvent;
    /// <summary>
    /// 大回合内最多检测几次
    /// </summary>
    [ConfigComment("大回合内最多检测几次")] public int boutCheckCount;
    /// <summary>
    /// 小回合内最多检测几次
    /// </summary>
    [ConfigComment("小回合内最多检测几次")] public int roundCheckCount;
    /// <summary>
    /// 通过几次后删除自身
    /// </summary>
    [ConfigComment("通过几次后删除自身")] public int passCount;
    /// <summary>
    /// 满足几次后才通过
    /// </summary>
    [ConfigComment("满足几次后才通过")] public int checkCount;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<CheckEventEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.passedSelfEffects = passedSelfEffects;
        effect.passedFindTargetEffects = passedFindTargetEffects;
        effect.passedTargetEffects = passedTargetEffects;
        effect.fighterEvent = fighterEvent;
        effect.battleEvent = battleEvent;
        effect.effectEvent = effectEvent;
        effect.extraOne = extraOne;
        effect.extraTwo = extraTwo;
        effect.extraThree = extraThree;
        effect.isStopEvent = isStopEvent;
        effect.boutCheckCount = boutCheckCount;
        effect.roundCheckCount = roundCheckCount;
        effect.passCount = passCount;
        effect.checkCount = checkCount;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(passedSelfEffects);
        writer.Write(passedFindTargetEffects);
        writer.Write(passedTargetEffects);
        writer.Write((int)fighterEvent);
        writer.Write((int)battleEvent);
        writer.Write((int)effectEvent);
        writer.Write(extraOne);
        writer.Write(extraTwo);
        writer.Write(extraThree);
        writer.Write(isStopEvent);
        writer.Write(boutCheckCount);
        writer.Write(roundCheckCount);
        writer.Write(passCount);
        writer.Write(checkCount);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        passedSelfEffects = reader.Read(passedSelfEffects);
        passedFindTargetEffects = reader.Read(passedFindTargetEffects);
        passedTargetEffects = reader.Read(passedTargetEffects);
        fighterEvent = (BattleObject.Event)reader.ReadInt32();
        battleEvent = (BaseBattle.Event)reader.ReadInt32();
        effectEvent = (EffectSys.Event)reader.ReadInt32();
        extraOne = reader.ReadDouble();
        extraTwo = reader.ReadDouble();
        extraThree = reader.ReadDouble();
        isStopEvent = reader.ReadBoolean();
        boutCheckCount = reader.ReadInt32();
        roundCheckCount = reader.ReadInt32();
        passCount = reader.ReadInt32();
        checkCount = reader.ReadInt32();
    }

    #endregion
}