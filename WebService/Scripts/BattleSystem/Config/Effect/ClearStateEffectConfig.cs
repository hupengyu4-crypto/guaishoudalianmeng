using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 
/// </summary>
[Serializable]
public class ClearStateEffectConfig : BaseEffectCfg
{
    /// <summary>
    /// 需要清理的状态
    /// </summary>
    [ConfigComment("需要清理的状态")] public BattleObject.State[] state;

    public override BaseEffect Create(BattleObject target, long authorUid)
    {
        var effect = target.Battle.Pool.Create<ClearStateEffect>();
        effect.SetBaseData(this, target, authorUid);

        effect.state = state;

        return effect;
    }

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(state != null);
        if(state != null)
        {
            writer.Write(state.Length);
            for(int a = 0; a < state.Length; a++)
            {
                var b = state[a];
                writer.Write((ulong)b);
            }
        }
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            state = new BattleObject.State[aLength];
            for(int a = 0; a < aLength; a++)
            {
                state[a] = (BattleObject.State)reader.ReadUInt64();
            }
        }
        else
        {
            state = null;
        }
    }

    #endregion
}