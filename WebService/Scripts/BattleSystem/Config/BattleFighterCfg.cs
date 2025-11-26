using RootScript.Config;
using BattleSystem;
using Google.Protobuf.Collections;
using System;

[System.Serializable]
public class BattleFighterCfg : Config
{
    [ConfigComment("角色名字")] public string name;
    [ConfigComment("描述")] public string describe;
    [ConfigComment("角色所有技能")] public long[] skills;
    [ConfigComment("角色属性")] public long[][] props;
    [ConfigComment("种族")] public int race;
    [ConfigComment("职业")] public int job;


    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.WriteNullableString(name);
        writer.WriteNullableString(describe);
        writer.Write(skills);
        writer.Write(props != null);
        if(props != null)
        {
            writer.Write(props.Length);
            for(int a = 0; a < props.Length; a++)
            {
                var b = props[a];
                writer.Write(b);
            }
        }
        writer.Write(race);
        writer.Write(job);
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        name = reader.ReadNullableString();
        describe = reader.ReadNullableString();
        skills = reader.Read(skills);
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            props = new long[aLength][];
            for(int a = 0; a < aLength; a++)
            {
                props[a] = reader.Read(props[a]);
            }
        }
        else
        {
            props = null;
        }
        race = reader.ReadInt32();
        job = reader.ReadInt32();
    }

    #endregion
    /// <summary>
    /// 转换服务器战斗详情
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public fighter_info ToFighterInfo(long uid, long pos)
    {

        RepeatedField<attr_item> attr = new RepeatedField<attr_item>();
        foreach (var prop in props)
        {
            attr.Add(new attr_item()
            {
                Type = "integer",
                AttrName = Enum.GetName(typeof(BattleDef.Property), prop[0]),
                Value = prop[1],
            });
        }
        var ret = new fighter_info()
        {
            Uid = uid,
            Sid = id,
            Level = 1,
            Step = 1,
            Star = 1,
            Skin = 0,
            Pos = pos,
            Race = race,
            Job = job,
            Skills =
            {
                Capacity = skills.Length
            },
            Attr = { attr }
        };

        for (int i = 0, l = skills.Length; i < l; i++)
        {
            ret.Skills.Add(new kv_int
            {
                K = skills[i],
                V = 1,
            });
        }

        return ret;
    }
}
