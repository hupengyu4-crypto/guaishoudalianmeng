using System;
using System.Collections.Generic;
using System.IO;
using RootScript.Config;

namespace RootScript.Config
{
    public static class BinaryExpand
    {

        public static void WriteLink<T>(this BinaryWriter writer, T value) where T : Config
        {
            if (value != null && value.sheetId == 0)
            {
                //throw new Exception($"T cannot found {value.GetType().Name}");
                value = null;
            }

            writer.Write(value != null);
            if (value != null)
            {
                writer.Write(value.sheetId);
                writer.Write(value.id);
            }
        }

        public static void WriteLinks<T>(this BinaryWriter writer, IList<T> value) where T : Config
        {
            writer.Write(value != null);
            if (value != null)
            {
                writer.Write(value.Count);
                for (int i = 0, length = value.Count; i < length; i++)
                {
                    writer.WriteLink(value[i]);
                }
            }
        }
        

        public static void Write(this BinaryWriter writer, string[] value)
        {
            writer.Write(value != null);
            if (value != null)
            {
                writer.Write(value.Length);
                for (int i = 0, length = value.Length; i < length; i++)
                {
                    writer.Write(value[i]);
                }
            }
        }

        public static void Write(this BinaryWriter writer, int[] value)
        {
            writer.Write(value != null);
            if (value != null)
            {
                writer.Write(value.Length);
                for (int i = 0, length = value.Length; i < length; i++)
                {
                    writer.Write(value[i]);
                }
            }
        }

        public static void Write(this BinaryWriter writer, float[] value)
        {
            writer.Write(value != null);
            if (value != null)
            {
                writer.Write(value.Length);
                for (int i = 0, length = value.Length; i < length; i++)
                {
                    writer.Write(value[i]);
                }
            }
        }

        public static void Write(this BinaryWriter writer, double[] value)
        {
            writer.Write(value != null);
            if (value != null)
            {
                writer.Write(value.Length);
                for (int i = 0, length = value.Length; i < length; i++)
                {
                    writer.Write(value[i]);
                }
            }
        }

        public static void Write(this BinaryWriter writer, bool[] value)
        {
            writer.Write(value != null);
            if (value != null)
            {
                writer.Write(value.Length);
                for (int i = 0, length = value.Length; i < length; i++)
                {
                    writer.Write(value[i]);
                }
            }
        }

        public static void Write(this BinaryWriter writer, long[] value)
        {
            writer.Write(value != null);
            if (value != null)
            {
                writer.Write(value.Length);
                for (int i = 0, length = value.Length; i < length; i++)
                {
                    writer.Write(value[i]);
                }
            }
        }
        
        public static Config ReadLink(this BinaryReader reader, Config defaultValue = null)
        {
            if (!reader.ReadBoolean())
            {
                return defaultValue;
            }

            int group = reader.ReadInt32();
            long id = reader.ReadInt64();
            long position = reader.BaseStream.Position;
            var configBinaryReader = reader as ConfigManagerImpl.ConfigBinaryReader;
            var row = configBinaryReader.ConfigManager.GetSheetRow(group, id);
            reader.BaseStream.Position = position;

            return row;
        }

        private static readonly string[] _sA = new string[0];
        public static string[] Read(this BinaryReader reader, string[] defaultValue)
        {
            bool nullable = reader.ReadBoolean();
            if (!nullable)
            {
                return defaultValue ?? _sA;
            }

            int length = reader.ReadInt32();
            var array = new string[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadString();
            }
            return array;
        }

        private static readonly string[][] _sAA = new string[0][];
        public static string[][] Read(this BinaryReader reader, string[][] defaultValue)
        {
            bool nullable = reader.ReadBoolean();
            if (!nullable)
            {
                return defaultValue ?? _sAA;
            }

            int length = reader.ReadInt32();
            var array = new string[length][];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadStringArray();
            }
            return array;
        }

        private static readonly float[] _fA = new float[0];
        public static float[] Read(this BinaryReader reader, float[] defaultValue)
        {
            bool nullable = reader.ReadBoolean();
            if (!nullable)
            {
                return defaultValue ?? _fA;
            }

            int length = reader.ReadInt32();
            var array = new float[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadSingle();
            }
            return array;
        }

        private static readonly float[][] _ffA = new float[0][];
        public static float[][] Read(this BinaryReader reader, float[][] defaultValue)
        {
            bool nullable = reader.ReadBoolean();
            if (!nullable)
            {
                return defaultValue ?? _ffA;
            }

            int length = reader.ReadInt32();
            var array = new float[length][];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadSingleArray();
            }
            return array;
        }

        private static readonly double[] _dA = new double[0];
        public static double[] Read(this BinaryReader reader, double[] defaultValue)
        {
            bool nullable = reader.ReadBoolean();
            if (!nullable)
            {
                return defaultValue ?? _dA;
            }
                
            int length = reader.ReadInt32();
            var array = new double[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadDouble();
            }
            return array;
        }

        private static readonly int[] _iA = new int[0];
        public static int[] Read(this BinaryReader reader, int[] defaultValue)
        {
            bool nullable = reader.ReadBoolean();
            if (!nullable)
            {
                return defaultValue ?? _iA;
            }

            int length = reader.ReadInt32();
            var array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadInt32();
            }
            return array;
        }

        private static readonly int[][] _iiA = new int[0][];
        public static int[][] Read(this BinaryReader reader, int[][] defaultValue)
        {
            bool nullable = reader.ReadBoolean();
            if (!nullable)
            {
                return defaultValue ?? _iiA;
            }

            int length = reader.ReadInt32();
            var array = new int[length][];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadInt32Array();
            }
            return array;
        }

        private static readonly long[] _lA = new long[0];
        public static long[] Read(this BinaryReader reader, long[] defaultValue)
        {
            bool nullable = reader.ReadBoolean();
            if (!nullable)
            {
                return defaultValue ?? _lA;
            }

            int length = reader.ReadInt32();
            var array = new long[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadInt64();
            }
            return array;
        }

        private static readonly long[][] _llA = new long[0][];
        public static long[][] Read(this BinaryReader reader, long[][] defaultValue)
        {
            bool nullable = reader.ReadBoolean();
            if (!nullable)
            {
                return defaultValue ?? new long[0][];
            }

            int length = reader.ReadInt32();
            var array = new long[length][];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadInt64Array();
            }
            return array;
        }

        private static readonly bool[] _bA = new bool[0];
        public static bool[] Read(this BinaryReader reader, bool[] defaultValue)
        {
            bool nullable = reader.ReadBoolean();
            if (!nullable)
            {
                return defaultValue ?? _bA;
            }

            int length = reader.ReadInt32();
            var array = new bool[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadBoolean();
            }
            return array;
        }

        #region object

        #region ReadonlyArray<string>

        public static void Write(this BinaryWriter writer, ReadonlyArray<string> value)
        {
            writer.Write(value == null);
            if (value == null)
            {
                return;
            }

            writer.Write(value.Length);
            for (int i = 0, length = value.Length; i < length; i++)
            {
                writer.Write(value[i]);
            }
        }

        private static readonly ReadonlyArray<string> _rsA = new ReadonlyArray<string>();
        public static ReadonlyArray<string> Read(this BinaryReader reader, ReadonlyArray<string> defaultValue = null)
        {
            if (reader.ReadBoolean())
            {
                return defaultValue ?? _rsA;
            }

            int length = reader.ReadInt32();
            var array = new string[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadString();
            }

            return new ReadonlyArray<string>(array);
        }

        #endregion ReadonlyArray<long>

        #region ReadonlyArray<long>

        public static void Write(this BinaryWriter writer, ReadonlyArray<long> value)
        {
            writer.Write(value == null);
            if (value == null)
            {
                return;
            }

            writer.Write(value.Length);
            for (int i = 0, length = value.Length; i < length; i++)
            {
                writer.Write(value[i]);
            }
        }

        private static readonly ReadonlyArray<long> _rlA = new ReadonlyArray<long>();
        public static ReadonlyArray<long> Read(this BinaryReader reader, ReadonlyArray<long> defaultValue = null)
        {
            if (reader.ReadBoolean())
            {
                return defaultValue ?? _rlA;
            }

            int length = reader.ReadInt32();
            long[] array = new long[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadInt64();
            }

            return new ReadonlyArray<long>(array);
        }

        #endregion ReadonlyArray<long>

        #region ReadonlyArray<bool>

        public static void Write(this BinaryWriter writer, ReadonlyArray<bool> value)
        {
            writer.Write(value == null);
            if (value == null)
            {
                return;
            }

            writer.Write(value.Length);
            for (int i = 0, length = value.Length; i < length; i++)
            {
                writer.Write(value[i]);
            }
        }

        private static readonly ReadonlyArray<bool> _rbA = new ReadonlyArray<bool>();
        public static ReadonlyArray<bool> Read(this BinaryReader reader, ReadonlyArray<bool> defaultValue = null)
        {
            if (reader.ReadBoolean())
            {
                return defaultValue ?? _rbA;
            }

            int length = reader.ReadInt32();
            var array = new bool[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadBoolean();
            }

            return new ReadonlyArray<bool>(array);
        }

        #endregion ReadonlyArray<bool>

        #region ReadonlyArray<short>

        public static void Write(this BinaryWriter writer, ReadonlyArray<short> value)
        {
            writer.Write(value == null);
            if (value == null)
            {
                return;
            }

            writer.Write(value.Length);
            for (int i = 0, length = value.Length; i < length; i++)
            {
                writer.Write(value[i]);
            }
        }
        
        private static readonly ReadonlyArray<short> _rsA2 = new ReadonlyArray<short>();
        public static ReadonlyArray<short> Read(this BinaryReader reader, ReadonlyArray<short> defaultValue = null)
        {
            if (reader.ReadBoolean())
            {
                return defaultValue ?? _rsA2;
            }

            int length = reader.ReadInt32();
            var array = new short[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadInt16();
            }

            return new ReadonlyArray<short>(array);
        }

        #endregion ReadonlyArray<short>

        #region ReadonlyArray<int>

        public static void Write(this BinaryWriter writer, ReadonlyArray<int> value)
        {
            writer.Write(value == null);
            if (value == null)
            {
                return;
            }

            writer.Write(value.Length);
            for (int i = 0, length = value.Length; i < length; i++)
            {
                writer.Write(value[i]);
            }
        }

        private static readonly ReadonlyArray<int> _riA = new ReadonlyArray<int>();
        public static ReadonlyArray<int> Read(this BinaryReader reader, ReadonlyArray<int> defaultValue = null)
        {
            if (reader.ReadBoolean())
            {
                return defaultValue ?? _riA;
            }

            int length = reader.ReadInt32();
            var array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadInt32();
            }

            return new ReadonlyArray<int>(array);
        }

        #endregion ReadonlyArray<int>

        #region ReadonlyArray<float>

        public static void Write(this BinaryWriter writer, ReadonlyArray<float> value)
        {
            writer.Write(value == null);
            if (value == null)
            {
                return;
            }

            writer.Write(value.Length);
            for (int i = 0, length = value.Length; i < length; i++)
            {
                writer.Write(value[i]);
            }
        }

        private static readonly ReadonlyArray<float> _rfA = new ReadonlyArray<float>();
        public static ReadonlyArray<float> Read(this BinaryReader reader, ReadonlyArray<float> defaultValue = null)
        {
            if (reader.ReadBoolean())
            {
                return defaultValue ?? _rfA;
            }

            int length = reader.ReadInt32();
            var array = new float[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadSingle();
            }

            return new ReadonlyArray<float>(array);
        }

        #endregion ReadonlyArray<float>

        #region ReadonlyArray<double>

        public static void Write(this BinaryWriter writer, ReadonlyArray<double> value)
        {
            writer.Write(value == null);
            if (value == null)
            {
                return;
            }

            writer.Write(value.Length);
            for (int i = 0, length = value.Length; i < length; i++)
            {
                writer.Write(value[i]);
            }
        }

        private static readonly ReadonlyArray<double> _rdA = new ReadonlyArray<double>();
        public static ReadonlyArray<double> Read(this BinaryReader reader, ReadonlyArray<double> defaultValue = null)
        {
            if (reader.ReadBoolean())
            {
                return defaultValue ?? _rdA;
            }

            int length = reader.ReadInt32();
            double[] array = new double[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadDouble();
            }

            return new ReadonlyArray<double>(array);
        }

        #endregion ReadonlyArray<double>

        #region ReadonlyArray<ReadonlyArray<double>>

        public static void Write(this BinaryWriter writer, ReadonlyArray<ReadonlyArray<double>> value)
        {
            writer.Write(value == null);
            if (value == null)
            {
                return;
            }

            writer.Write(value.Length);
            for (int i = 0, length = value.Length; i < length; i++)
            {
                writer.Write(value[i]);
            }
        }

        private static readonly ReadonlyArray<ReadonlyArray<double>> _rrdA = new ReadonlyArray<ReadonlyArray<double>>();
        public static ReadonlyArray<ReadonlyArray<double>> Read(this BinaryReader reader, ReadonlyArray<ReadonlyArray<double>> defaultValue = null)
        {
            if (reader.ReadBoolean())
            {
                return defaultValue ?? _rrdA;
            }

            int length = reader.ReadInt32();
            var array = new ReadonlyArray<double>[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.Read(array[i]);
            }
            return new ReadonlyArray<ReadonlyArray<double>>(array);
        }

        #endregion ReadonlyArray<ReadonlyArray<double>>

        #endregion

    }
}
