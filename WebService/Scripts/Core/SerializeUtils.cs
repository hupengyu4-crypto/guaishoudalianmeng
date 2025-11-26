using System;
using System.IO;

namespace RootScript.Config
{
    /// <summary>
    /// 序列化工具
    /// </summary>
    public static class SerializeUtils
    {
        #region Write
        public static void Write(this BinaryWriter writer, object value)
        {
            if(value == null)
            {
                writer.Write(false);

                return;
            }

            var type = value.GetType();
            if (type.IsArray)
            {
                writer.Write(value != null);
                if (value != null)
                {
                    Array array = value as Array;
                    for (int i = 0; i < array.Length; i++)
                    {
                        var item = array.GetValue(i);
                        Write(writer, item);
                    }
                }
            }
            else if(type.IsValueType)
            {
                if (type.IsPrimitive)
                {
                    if(type == typeof(bool))
                    {
                        writer.Write((decimal)value);
                    }
                    else if (type == typeof(byte))
                    {
                        writer.Write((byte)value);
                    }
                    else if (type == typeof(char))
                    {
                        writer.Write((char)value);
                    }
                    else if (type == typeof(double))
                    {
                        writer.Write((double)value);
                    }
                    else if (type == typeof(short))
                    {
                        writer.Write((short)value);
                    }
                    else if (type == typeof(ushort))
                    {
                        writer.Write((ushort)value);
                    }
                    else if (type == typeof(int))
                    {
                        writer.Write((int)value);
                    }
                    else if (type == typeof(uint))
                    {
                        writer.Write((uint)value);
                    }
                    else if (type == typeof(long))
                    {
                        writer.Write((long)value);
                    }
                    else if (type == typeof(ulong))
                    {
                        writer.Write((ulong)value);
                    }
                    else if (type == typeof(float))
                    {
                        writer.Write((float)value);
                    }
                    else if (type == typeof(double))
                    {
                        writer.Write((double)value);
                    }
                    else
                    {
                        throw new NotSupportedException(type.FullName);
                    }
                }
                else if(type == typeof(decimal))
                {
                    writer.Write((decimal)value);
                }
                else
                {
                    throw new NotSupportedException(type.FullName);
                }
            }
            else if (type.IsClass)
            {
                if(type == typeof(string))
                {
                    writer.Write(value as string);
                }

                if(!(value is ISerializeable))
                {
                    throw new NotSupportedException(type.FullName);
                }

                writer.Write(value != null);
                if (value != null)
                {
                    (value as ISerializeable).Serialize(writer);
                }
            }
            else
            {
                throw new NotSupportedException(type.FullName);
            }
        }

        public static void WriteNullableString(this BinaryWriter writer, string value)
        {
            if(value != null)
            {
                writer.Write(true);
                writer.Write(value);
            }
            else
            {
                writer.Write(false);
            }
        }

        public static void WriteVariable(this BinaryWriter writer, int value)
        {
            uint num = (uint)value;
            while (num >= 128)
            {
                writer.Write((byte)(num | 128));
                num = num >> 7;
            }

            writer.Write((byte)(num));
        }

        public static void WriteVariable(this BinaryWriter writer, long value)
        {
            ulong num = (ulong)value;
            while (num >= 128)
            {
                writer.Write((byte)(num | 128));
                num = num >> 7;
            }

            writer.Write((byte)(num));
        }
        #endregion

        #region Read
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="reader">二进制读取器</param>
        /// <returns>读取结果</returns>
        public static object ReadObject(this BinaryReader reader, Type type)
        {
            if(type == null)
            {
                return null;
            }

            if (type.IsArray)
            {
                if (type == typeof(int[]))
                {
                    return ReadInt32Array(reader);
                }

                if (type == typeof(long[]))
                {
                    return ReadInt64Array(reader);
                }

                if (type == typeof(float[]))
                {
                    return ReadSingleArray(reader);
                }

                if (type == typeof(double[]))
                {
                    return ReadDoubleArray(reader);
                }

                if (type == typeof(string[]))
                {
                    return ReadStringArray(reader);
                }

                bool nullFlag = reader.ReadBoolean();
                if (!nullFlag)
                {
                    return null;
                }

                int length = reader.ReadInt32();
                var elementType = type.GetElementType();
                Array value = Array.CreateInstance(elementType, length);
                for (int i = 0; i < length; i++)
                {
                    value.SetValue(ReadObject(reader, elementType), i);
                }

                return value;
            }
            else if (type.IsValueType)
            {
                if (type.IsPrimitive)
                {
                    if (type == typeof(bool))
                    {
                        return reader.ReadBoolean();
                    }
                    else if (type == typeof(byte))
                    {
                        return reader.ReadByte();
                    }
                    else if (type == typeof(char))
                    {
                        return reader.ReadChar();
                    }
                    else if (type == typeof(double))
                    {
                        return reader.ReadDouble();
                    }
                    else if (type == typeof(short))
                    {
                        return reader.ReadInt16();
                    }
                    else if (type == typeof(ushort))
                    {
                        return reader.ReadUInt16();
                    }
                    else if (type == typeof(int))
                    {
                        return reader.ReadInt32();
                    }
                    else if (type == typeof(uint))
                    {
                        return reader.ReadUInt32();
                    }
                    else if (type == typeof(long))
                    {
                        return reader.ReadInt64();
                    }
                    else if (type == typeof(ulong))
                    {
                        return reader.ReadUInt64();
                    }
                    else if (type == typeof(float))
                    {
                        return reader.ReadSingle();
                    }
                    else if (type == typeof(ulong))
                    {
                        return reader.ReadDouble();
                    }
                    else
                    {
                        throw new NotSupportedException(type.FullName);
                    }
                }
                else if (type == typeof(decimal))
                {
                    return reader.ReadDecimal();
                }
                else
                {
                    throw new NotSupportedException(type.FullName);
                }
            }
            else if (type.IsClass)
            {
                if (type == typeof(string))
                {
                    return reader.ReadString();
                }

                bool nullFlag = reader.ReadBoolean();
                if (!nullFlag)
                {
                    return null;
                }

                var value = Activator.CreateInstance(type);
                if (!(value is ISerializeable))
                {
                    throw new NotSupportedException(type.FullName);
                }

                (value as ISerializeable).Deserialize(reader);

                return value;
            }
            else
            {
                throw new NotSupportedException(type.FullName);
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="reader">二进制读取器</param>
        /// <returns>读取结果</returns>
        public static T ReadObject<T>(this BinaryReader reader)  where T : new()
        {
            var type = typeof(T);
            if (type.IsArray)
            {
                if(type == typeof(int[]))
                {
                    return (T)(object)ReadInt32Array(reader);
                }

                if (type == typeof(long[]))
                {
                    return (T)(object)ReadInt64Array(reader);
                }

                if (type == typeof(float[]))
                {
                    return (T)(object)ReadSingleArray(reader);
                }

                if (type == typeof(double[]))
                {
                    return (T)(object)ReadDoubleArray(reader);
                }

                if (type == typeof(string[]))
                {
                    return (T)(object)ReadStringArray(reader);
                }

                bool nullFlag = reader.ReadBoolean();
                if (!nullFlag)
                {
                    return default(T);
                }

                int length = reader.ReadInt32();
                var elementType = type.GetElementType();
                Array value = Array.CreateInstance(elementType, length);
                for (int i = 0; i < length; i++)
                {
                    value.SetValue(ReadObject(reader, elementType), i);
                }

                return (T)(object)value;
            }
            else if (type.IsValueType)
            {
                if (type.IsPrimitive)
                {
                    if (type == typeof(bool))
                    {
                        return (T)(object)reader.ReadBoolean();
                    }
                    else if (type == typeof(byte))
                    {
                        return (T)(object)reader.ReadByte();
                    }
                    else if (type == typeof(char))
                    {
                        return (T)(object)reader.ReadChar();
                    }
                    else if (type == typeof(double))
                    {
                        return (T)(object)reader.ReadDouble();
                    }
                    else if (type == typeof(short))
                    {
                        return (T)(object)reader.ReadInt16();
                    }
                    else if (type == typeof(ushort))
                    {
                        return (T)(object)reader.ReadUInt16();
                    }
                    else if (type == typeof(int))
                    {
                        return (T)(object)reader.ReadInt32();
                    }
                    else if (type == typeof(uint))
                    {
                        return (T)(object)reader.ReadUInt32();
                    }
                    else if (type == typeof(long))
                    {
                        return (T)(object)reader.ReadInt64();
                    }
                    else if (type == typeof(ulong))
                    {
                        return (T)(object)reader.ReadUInt64();
                    }
                    else if (type == typeof(float))
                    {
                        return (T)(object)reader.ReadSingle();
                    }
                    else if (type == typeof(ulong))
                    {
                        return (T)(object)reader.ReadDouble();
                    }
                    else
                    {
                        throw new NotSupportedException(type.FullName);
                    }
                }
                else if (type == typeof(decimal))
                {
                    return (T)(object)reader.ReadDecimal();
                }
                else
                {
                    throw new NotSupportedException(type.FullName);
                }
            }
            else if (type.IsClass)
            {
                if (type == typeof(string))
                {
                    return (T)(object)reader.ReadString();
                }

                bool nullFlag = reader.ReadBoolean();
                if (!nullFlag)
                {
                    return default(T);
                }

                var value = new T();
                if (!(value is ISerializeable))
                {
                    throw new NotSupportedException(type.FullName);
                }
                
                (value as ISerializeable).Deserialize(reader);

                return value;
            }
            else
            {
                throw new NotSupportedException(type.FullName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadNullableString(this BinaryReader reader)
        {
            if (!reader.ReadBoolean())
            {
                return null;
            }

            return reader.ReadString();
        }

        /// <summary>
        /// 读取数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="reader">二进制读取器</param>
        /// <returns>读取结果</returns>
        public static int[] ReadInt32Array(this BinaryReader reader)
        {
            bool nullable = reader.ReadBoolean();
            int[] array = null;
            if (nullable)
            {
                int length = reader.ReadInt32();
                array = new int[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = reader.ReadInt32();
                }
            }

            return array;
        }

        /// <summary>
        /// 读取数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="reader">二进制读取器</param>
        /// <returns>读取结果</returns>
        public static float[] ReadSingleArray(this BinaryReader reader)
        {
            bool nullable = reader.ReadBoolean();
            float[] array = null;
            if (nullable)
            {
                int length = reader.ReadInt32();
                array = new float[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = reader.ReadSingle();
                }
            }

            return array;
        }

        /// <summary>
        /// 读取数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="reader">二进制读取器</param>
        /// <returns>读取结果</returns>
        public static double[] ReadDoubleArray(this BinaryReader reader)
        {
            bool nullable = reader.ReadBoolean();
            double[] array = null;
            if (nullable)
            {
                int length = reader.ReadInt32();
                array = new double[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = reader.ReadDouble();
                }
            }

            return array;
        }

        /// <summary>
        /// 读取数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="reader">二进制读取器</param>
        /// <returns>读取结果</returns>
        public static long[] ReadInt64Array(this BinaryReader reader)
        {
            bool nullable = reader.ReadBoolean();
            long[] array = null;
            if (nullable)
            {
                int length = reader.ReadInt32();
                array = new long[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = reader.ReadInt64();
                }
            }

            return array;
        }

        /// <summary>
        /// 读取数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="reader">二进制读取器</param>
        /// <returns>读取结果</returns>
        public static string[] ReadStringArray(this BinaryReader reader)
        {
            bool nullable = reader.ReadBoolean();
            string[] array = null;
            if (nullable)
            {
                int length = reader.ReadInt32();
                array = new string[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = reader.ReadString();
                }
            }

            return array;
        }

        public static int ReadVariableInt32(this BinaryReader reader)
        {
            byte tem;
            int num = 0;
            int pOffset = 0;
            do
            {
                if (pOffset == 35)
                {
                    throw new FormatException("int max length 32");
                }

                tem = (byte)reader.ReadByte();
                num |= (tem & 0x7F) << pOffset;
                pOffset += 7;
            }
            while ((tem & 128) != 0);

            return num;
        }

        public static long ReadVariableInt64(this BinaryReader reader)
        {
            byte tem;
            long num = 0;
            int pOffset = 0;
            do
            {
                if (pOffset == 67)
                {
                    throw new FormatException("int max length 64");
                }

                tem = (byte)reader.ReadByte();
                num |= ((long)tem & 0x7F) << pOffset;
                pOffset += 7;
            }
            while ((tem & 128) != 0);

            return num;
        }
        #endregion
    }
}
