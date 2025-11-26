using System.Collections.Generic;
using System;
using System.IO;

namespace RootScript.Config
{
    [Serializable]
    public class  Config : ISerializeable
    {
        [ConfigComment("ID")]
        public long id;

        public int sheetId;

        public Config(long id)
        {
            this.id = id;
        }

        public Config()
        {
        }

        [NonSerialized]
        private HashSet<string> _assets = new HashSet<string>();

        public HashSet<string> Assets
        {
            get { return _assets; }
        }

        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(id);
        }

        public virtual void Deserialize(BinaryReader reader)
        {
            id = reader.ReadInt64();
        }
    }
    public class ConfigCommentAttribute : Attribute
    {
        public readonly string comment;
        public ConfigCommentAttribute(string comment)
        {
            this.comment = comment;
        }
    }
   
    public class ConfigAssetAttribute : Attribute { }
    public class ConfigPathAttribute : Attribute { }
    public class ConfigOriginStringAttribute : Attribute { }
    public class ConfigReadonlyAttribute : Attribute { }
}
