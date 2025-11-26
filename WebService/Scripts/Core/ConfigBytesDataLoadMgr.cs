using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RootScript.Config
{
    /// <summary>
    /// 配置bytes加载管理器
    /// </summary>
    public class ConfigBytesDataLoadMgr : IConfigLoader
    {
        private static ConfigBytesDataLoadMgr instance;
        public static ConfigBytesDataLoadMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigBytesDataLoadMgr();
                    instance.Init();
                    return instance;
                }
                return instance;
            }
        }

        /// <summary>
        /// 配置所在根路径
        /// </summary>
        public string configRootPath = "ConfigByteNew";
        /// <summary>
        /// 已经加载的配置byte数据
        /// </summary>
        private Dictionary<string, byte[]> configDataDict;
        /// <summary>
        /// 配置名字典
        /// </summary>
        private Dictionary<string, string> configNameDict;

        private Regex mNameRegex;
        public void Init()
        {
            configDataDict = new Dictionary<string, byte[]>();
            configNameDict = new Dictionary<string, string>();
            mNameRegex = new Regex(@".*/(\w+)");
        }

        public void Release()
        {
            configDataDict.Clear();
            configDataDict = null;
            instance = null;
        }

        public byte[] GetConfigBytes(string configPath)
        {
            string configName = GetConfigNameByPath(configPath);
            byte[] bytes = null;

            if (!configPath.EndsWith(".bytes"))
            {
                configPath += ".bytes";
            }

            if (!configDataDict.TryGetValue(configName, out bytes))
            {
                var value = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + configPath);
                ImportConfigBytes(configName, value);
                configDataDict.TryGetValue(configName, out bytes);
            }

            return bytes;
        }

        private void OnLoadConfig(Object obj, string path)
        {
            //TextAsset asset = (TextAsset)obj;
            //ImportConfigBytes(asset.name, asset.bytes);
        }

        /// <summary>
        /// 直接导入byte数据
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="bytes"></param>
        public void ImportConfigBytes(string configName, byte[] bytes)
        {
            if (!configDataDict.ContainsKey(configName))
            {
                configDataDict.Add(configName, bytes);
            }
            else
            {
                //Debug.LogError("配置" + configName + "已经导入了，存在重复导入byte数据，请检查");
            }
        }

        /// <summary>
        /// 加载AB中的所有配置
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="callBack"></param>
        public void ImportConfigBytesByAb(object[] objs)
        {
            //for (int i = 0; i < objs.Length; i++)
            //{
            //    TextAsset asset = objs[i] as TextAsset;
            //    if (asset != null)
            //    {
            //        ImportConfigBytes(asset.name, asset.bytes);
            //    }
            //}
        }

        public string GetConfigNameByPath(string configPath)
        {
            if (configNameDict.TryGetValue(configPath, out var configName))
            {
                return configName;
            }

            var match = mNameRegex.Match(configPath);
            if (match.Groups.Count < 1)
            {
                //Log.error("配置表路径错误:"+configPath);
                return null;
            }
            configName = match.Groups[1].Value;
            configNameDict.Add(configPath, configName);
            return configName;
        }
    }
}
