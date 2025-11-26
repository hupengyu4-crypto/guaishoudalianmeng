using RootScript.Config;
using BattleSystem;
using System;

/// <summary>
/// 抵抗Effect效果
/// </summary>
[Serializable]
public class ResistEffectConfig : BaseEffectCfg
{
	/// <summary>
	/// 受到效果类型
	/// </summary>
	[ConfigComment("受到效果类型")] public EffectSys.EffectType[] effectEvent;

	/// <summary>
	/// 自身状态
	/// </summary>
	[ConfigComment("自身状态")] public BattleObject.State state;

	/// <summary>
	/// 扣除效果类型
	/// </summary>
	[ConfigComment("扣除效果类型")] public EffectSys.EffectType resistEffectEvent;

	/// <summary>
	/// 扣除次数
	/// </summary>
	[ConfigComment("扣除次数")] public int resistNum;


	public override BaseEffect Create(BattleObject target, long authorUid)
	{
		var effect = target.Battle.Pool.Create<ResistEffect>();
		effect.SetBaseData(this, target, authorUid);
		effect.effectEvent = effectEvent;
		effect.state = state;
		effect.resistEffectEvent = resistEffectEvent;
		effect.resistNum = resistNum;

		return effect;
	}

	#region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(effectEvent != null);
        if(effectEvent != null)
        {
            writer.Write(effectEvent.Length);
            for(int a = 0; a < effectEvent.Length; a++)
            {
                var b = effectEvent[a];
                writer.Write((ulong)b);
            }
        }
        writer.Write((ulong)state);
        writer.Write((ulong)resistEffectEvent);
        writer.Write(resistNum);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            effectEvent = new EffectSys.EffectType[aLength];
            for(int a = 0; a < aLength; a++)
            {
                effectEvent[a] = (EffectSys.EffectType)reader.ReadUInt64();
            }
        }
        else
        {
            effectEvent = null;
        }
        state = (BattleObject.State)reader.ReadUInt64();
        resistEffectEvent = (EffectSys.EffectType)reader.ReadUInt64();
        resistNum = reader.ReadInt32();
    }

	#endregion
}