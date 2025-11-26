using RootScript.Config;
using BattleSystem;

public class BattleSceneCfg : Config
{
    [ConfigComment("描述")] public string describe;
    [ConfigComment("战斗类")] public BattleDef.BattleType battleType;
    [ConfigComment("最大回合")] public int maxBout;
    [ConfigComment("战斗场景")] public string sceneBgPath;

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.WriteNullableString(describe);
        writer.Write((byte)battleType);
        writer.Write(maxBout);
        writer.WriteNullableString(sceneBgPath);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        describe = reader.ReadNullableString();
        battleType = (BattleDef.BattleType)reader.ReadByte();
        maxBout = reader.ReadInt32();
        sceneBgPath = reader.ReadNullableString();
    }

    #endregion
}