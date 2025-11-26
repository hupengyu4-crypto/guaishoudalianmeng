using System.IO;

namespace RootScript.Config
{
    /// <summary>
    /// 可序列化接口
    /// </summary>
    public interface ISerializeable
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="writer">二进制写入器</param>
        void Serialize(BinaryWriter writer);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="reader">二进制读取器</param>
        void Deserialize(BinaryReader reader);
    }
}
