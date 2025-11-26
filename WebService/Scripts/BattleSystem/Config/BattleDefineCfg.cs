using RootScript.Config;
using System;

/// <summary>
/// 公用配置 禁止更改ID 只能更改Value
/// </summary>
[Serializable]
public class BattleDefineCfg : Config
{
    /// <summary>
    /// 描述
    /// </summary>
    [ConfigComment("描述")]
    public string describe;
    /// <summary>
    /// int二维数组
    /// </summary>
    [ConfigComment("int二维数组")]
    public int[][] valueIntArray;
    /// <summary>
    /// string二维数组
    /// </summary>
    [ConfigComment("string二维数组")]
    public string[][] valueStringArray;
    /// <summary>
    /// double二维数组
    /// </summary>
    [ConfigComment("double二维数组")]
    public double[][] valueDoubleArray;

    #region AutoGenerate
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.WriteNullableString(describe);
        writer.Write(valueIntArray != null);
        if(valueIntArray != null)
        {
            writer.Write(valueIntArray.Length);
            for(int a = 0; a < valueIntArray.Length; a++)
            {
                var b = valueIntArray[a];
                writer.Write(b);
            }
        }
        writer.Write(valueStringArray != null);
        if(valueStringArray != null)
        {
            writer.Write(valueStringArray.Length);
            for(int a = 0; a < valueStringArray.Length; a++)
            {
                var b = valueStringArray[a];
                writer.Write(b);
            }
        }
        writer.Write(valueDoubleArray != null);
        if(valueDoubleArray != null)
        {
            writer.Write(valueDoubleArray.Length);
            for(int a = 0; a < valueDoubleArray.Length; a++)
            {
                var b = valueDoubleArray[a];
                writer.Write(b);
            }
        }
    }

    public override void Deserialize(System.IO.BinaryReader reader)
    {
        base.Deserialize(reader);
        describe = reader.ReadNullableString();
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            valueIntArray = new int[aLength][];
            for(int a = 0; a < aLength; a++)
            {
                valueIntArray[a] = reader.Read(valueIntArray[a]);
            }
        }
        else
        {
            valueIntArray = null;
        }
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            valueStringArray = new string[aLength][];
            for(int a = 0; a < aLength; a++)
            {
                valueStringArray[a] = reader.Read(valueStringArray[a]);
            }
        }
        else
        {
            valueStringArray = null;
        }
        if(reader.ReadBoolean())
        {
            int aLength = reader.ReadInt32();
            valueDoubleArray = new double[aLength][];
            for(int a = 0; a < aLength; a++)
            {
                valueDoubleArray[a] = reader.Read(valueDoubleArray[a]);
            }
        }
        else
        {
            valueDoubleArray = null;
        }
    }

    #endregion
}