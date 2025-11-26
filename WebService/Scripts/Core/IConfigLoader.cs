

namespace RootScript.Config
{
    public interface IConfigLoader
    {
        void ImportConfigBytes(string configPath, byte[] bytes);
        void ImportConfigBytesByAb(object[] objs);
        byte[] GetConfigBytes(string configPath);
    }
}
