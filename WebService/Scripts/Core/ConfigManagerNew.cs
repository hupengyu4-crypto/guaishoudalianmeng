using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RootScript.Config
{
    public delegate void ConfigDataCallBack(long id, string fieldname, ref string data);

    /// <summary>
    /// 配置管理
    /// </summary>
    public class ConfigManagerNew
    {
        #region static

        private static ConfigManagerNew instance;
        public static ConfigManagerNew Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigManagerNew();
                }
                return instance;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// 加载行失败
        /// </summary>
        public Action<string> loadRowFailure
        {
            get => impl.loadRowFailure;
            set => impl.loadRowFailure = value;
        }

        #endregion

        #region public metho



        public ConfigManagerNew() { }

        public void Init(IConfigLoader iLoader, Action<ConfigManagerImpl> gcrFunc)
        {
            impl.Initialize(iLoader.GetConfigBytes, true);

            gcrFunc?.Invoke(impl);
            
        }

        public void Release()
        {
            impl.Reset();
        }

        public static void Reset()
        {
            ConfigManagerNew.instance.Release();
        }

        public void LoadAll()
        {
            impl.LoadAll(true);
        }

        #region get
        /// <summary>
        /// 获取表中行的数据
        /// </summary>
        /// <typeparam name="T">行数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="id">行Id</param>
        /// <param name="failLog">不存在打印日志开关</param>
        /// <returns>如果不存在返回null</returns>
        public Config Get(string configName, long id, bool failLog = true)
        {
            return impl.GetSheetRow(configName, id, failLog);
        }

        /// <summary>
        /// 获取表中行的数据
        /// </summary>
        /// <typeparam name="T">行数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="id">行Id</param>
        /// <param name="failLog">不存在打印日志开关</param>
        /// <returns>如果不存在返回null</returns>
        public Config Get(Type type, long id, bool failLog = true)
        {
            return impl.GetSheetRow(type.Name, id, failLog);
        }

        // TODO 修改成impl提供的接口
        public bool Contains<T>(long id) where T : Config
        {
            return impl.Contains<T>(id);
        }

        public bool Contains(string name, long id)
        {
            return impl.Contains(name, id);
        }

        /// <summary>
        /// 获取表中行的数据
        /// </summary>
        /// <typeparam name="T">行数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="id">行Id</param>
        /// <returns>如果不存在返回null</returns>
        public T Get<T>(long id, bool failLog = true) where T : Config
        {
            return impl.GetSheetRow<T>(id, failLog);
        }

        /// <summary>
        /// 获取表中行的数据
        /// </summary>
        /// <typeparam name="T">行数据类型</typeparam>
        /// <param name="sheetOrGroupId">组或者表的id</param>
        /// <param name="id">行Id</param>
        /// <returns>如果不存在返回null</returns>
        public T Get<T>(int sheetOrGroupId, long id, bool failLog = true) where T : Config
        {
            return impl.GetSheetRow(sheetOrGroupId, id, failLog) as T;
        }

        /// <summary>
        /// 获取表中行的数据
        /// </summary>
        /// <param name="name">表名</param>
        /// <returns>如果不存在返回null</returns>
        [Obsolete("该接口不可用于取大配置表的数据，否则将有不小的性能开销，如要取配置表所有ID请使用GetAllId")]
        public ReadOnlyCollection<Config> Get(string name)
        {
            var cfgs = impl.GetTableRows(name);
#if UNITY_EDITOR
            if (cfgs.Count >= 500)
            {
                Log.error($"配置表({name})过大，强行获取所有行数据将影响性能");
            }
#endif
            return cfgs;
        }

        /// <summary>
        /// 获取所有行数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("该接口不可用于取大配置表的数据，否则将有不小的性能开销，如要取配置表所有ID请使用GetAllId")]
        public ReadOnlyCollection<Config> Get<T>() where T : Config
        {
            var name = typeof(T).Name;
            var cfgs = impl.GetTableRows(name);
#if UNITY_EDITOR
            if (cfgs.Count >= 500)
            {
                Log.error($"配置表({name})过大，强行获取所有行数据将影响性能");
            }
#endif
            return cfgs;
        }

        /// <summary>
        /// 获取所有行数据
        /// </summary>
        /// <param name="sheetOrGroupId">组或者表的id</param>
        /// <returns></returns>
        [Obsolete("该接口不可用于取大配置表的数据，否则将有不小的性能开销，如要取配置表所有ID请使用GetAllId")]
        public ReadOnlyCollection<Config> Get(int sheetOrGroupId)
        {
            var cfgs = impl.GetTableRows(sheetOrGroupId);
#if UNITY_EDITOR
            if (cfgs.Count >= 500)
            {
                Log.error($"配置表({sheetOrGroupId})过大，强行获取所有行数据将影响性能");
            }
#endif
            return cfgs;
        }

        //public T[] Get<T>() where T : Config
        //{
        //    //return impl.GetTableRows(typeof(T).Name);
        //    // TODO 临时修改，后面不返回数组
        //    var readonlyArray = impl.GetTableRows(typeof(T).Name);
        //    if (readonlyArray == null || readonlyArray.Count == 0) return null;
        //    T[] ret = new T[readonlyArray.Count];
        //    for (int i = 0; i < readonlyArray.Count; i++)
        //    {
        //        ret[i] = readonlyArray[i] as T;
        //    }
        //    return ret;
        //}

        #endregion

        #endregion

        #region cannot use

        public bool UsedAssetBundle = false;

        /// <summary>
        /// 获取配置表中所有行的Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<long> GetAllId<T>() where T : Config
        {
            return impl.GetSheetRowIds<T>();
        }
        
        public bool IsExistConfigFromDic<T>(long id) where T : Config
        {
            return Contains<T>(id);
        }

        public void Import(Type t, string ta)
        {
            Log.error("Cannot call this !!!!");
        }

        public void Import<T>(string ta)
        {
            throw new NotImplementedException();
        }

        public void ImportByte(Type t, byte[] ta)
        {
            Log.error("Cannot call this !!!!");
        }

        public Dictionary<long, T> GetConfigDict<T>() where T : Config
        {
            Log.error("Cannot call this !!!!");
            return null;
        }

        public void ImportByte<T>(byte[] ta)
        {
            Log.error("Cannot call this !!!!");
        }
        public void Import(Type type, string strInfo, Type derivedType = null, ConfigDataCallBack callBack = null)
        {
            Log.error("Cannot call this !!!!");
        }

        public void ClearConfigDic(Type t)
        {
            Log.error("Cannot call this !!!!");
        }

        #region 编辑器里面的

        public static bool IsConvertMode;

        public void Destroy()
        {
            instance = null;
        }

        public void RefreshStrInfo(Type type, string sb)
        {
            Log.error("Cannot call this !!!!");
        }

        public void RefreshStrInfo(Type type, Type pool, string sb)
        {
            Log.error("Cannot call this !!!!");
        }

        public void ClearConfigDic<T>() where T : Config
        {
            Log.error("Cannot call this !!!!");
        }

        public void removeConfigFromDic<T>(long id) where T : Config
        {
            Log.error("Cannot call this !!!!");
        }

        public string ExportStrInfo<T>() where T : Config
        {
            Log.error("Cannot call this !!!!");
            return null;    
        }

        public byte[] getDataForExportByte(Type type)
        {
            Log.error("Cannot call this !!!!");
            return null;    
        }

        public Type[] GetConfigTypePriorityArray(Type type)
        {
            Log.error("Cannot call this !!!!");
            return null;
        }

        public string RuntimeTypeChangeToTypeName(Type t)
        {
            Log.error("Cannot call this !!!!");
            return null;    
        }

        public void addConfigToDic(Config cfg)
        {
            Log.error("Cannot call this !!!!");
        }

        #endregion

        #endregion

        #region private Properties 

        private readonly ConfigManagerImpl impl = new ConfigManagerImpl();

        #endregion

    }
}
