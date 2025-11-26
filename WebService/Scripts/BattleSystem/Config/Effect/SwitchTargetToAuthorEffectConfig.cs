using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 将施法者切换为效果持有者的效果
/// </summary>
[Serializable]
public class SwitchTargetToAuthorEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 传递施法者
    /// </summary>
    [ConfigComment("传递施法者")] public bool keepAuthor;
    /// <summary>
    /// 切换后施加效果
    /// </summary>
    [ConfigComment("切换后施加效果")] public long[] addEffects;
    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<SwitchTargetToAuthorEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.SetData(this);
        effect.keepAuthor = keepAuthor;
        effect.addEffects = addEffects;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(keepAuthor);
        writer.Write(addEffects);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        keepAuthor = reader.ReadBoolean();
        addEffects = reader.Read(addEffects);
    }

    #endregion
}