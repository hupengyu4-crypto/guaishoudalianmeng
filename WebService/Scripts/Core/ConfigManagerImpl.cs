using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using RootScript.Config;
using BattleSystem;

namespace RootScript.Config
{
    /// <summary>
    /// 配置管理器实现部分
    /// </summary>
    public class ConfigManagerImpl
    {
        #region Properties
        private bool mappingLoaded = false;

        private const string MapAssetPath = "ConfigByteNew/ConfigMap";

        /// <summary>
        /// 分组对应行数据的Id映射到哪个表
        /// </summary>
        private Dictionary<int, Dictionary<long, int>> mapConfig = new Dictionary<int, Dictionary<long, int>>(20);
        /// <summary>
        /// 分组对应的所有表的ID
        /// </summary>
        private Dictionary<int, List<long>> mapAllIdDict = new Dictionary<int, List<long>>(20);

        /// <summary>
        /// 表数据
        /// </summary>
        private Dictionary<int, Sheet> sheetMap = new Dictionary<int, Sheet>();

        /// <summary>
        /// 分组的行数据配置
        /// </summary>
        private Dictionary<int, Dictionary<long, Config>> groupRowsCache = new Dictionary<int, Dictionary<long, Config>>();

        private Dictionary<int, ReadOnlyCollection<Config>> groupAllRowsCache = new Dictionary<int, ReadOnlyCollection<Config>>();
        #endregion

        #region Events
        /// <summary>
        /// 加载表实现，必须初始化
        /// </summary>
        public Func<string, byte[]> loadTableImpl;

        /// <summary>
        /// 加载行失败
        /// </summary>
        public Action<string> loadRowFailure;
        #endregion

        #region Public Methods
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="loadTableImpl">加载扩展实现</param>
        /// <param name="initMap">是否初始化映射文件</param>
        public void Initialize(Func<string, byte[]> loadTableImpl, bool initMap)
        {
            this.loadTableImpl = loadTableImpl;
            if (initMap)
            {
                TryLoadMapConfig();
            }
        }

        public int RegisterSheetConfig(string tableName, bool isPersistent, string assetPath, Func<Config> instanceImpl)
        {
            int id = tableName.HashCode();
            sheetMap.Add(id, new Sheet(this, loadTableImpl)
            {
                Group = id,
                Id = id,
                AssetPath = assetPath,
                RowType = null,
                IsPersistent = isPersistent,
                InstanceImpl = instanceImpl
            });

            return id;
        }


        /// <summary>
        /// 注册表的数据类型与数据
        /// </summary>
        /// <typeparam name="TGroup">分组类型</typeparam>
        /// <typeparam name="TConfig">配置类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="isPersistent">是否为常驻内存的</param>
        /// <param name="assetPath">资源路径</param>
        /// <returns>表Id</returns>`
        public int RegisterSheetConfig(string groupName, string tableName, bool isPersistent, string assetPath, Func<Config> instanceImpl)
        {
            int id = tableName.HashCode();
            sheetMap.Add(id, new Sheet(this, loadTableImpl)
            {
                Group = groupName.HashCode(),
                Id = id,
                AssetPath = assetPath,
                RowType = null,
                InstanceImpl = instanceImpl,
                IsPersistent = isPersistent
            });

            return id;
        }

        /// <summary>
        /// 加载所有已注册的表
        /// </summary>
        /// <param name="loadRows">是否将表中的所有行进行加载</param>

        public void LoadAll(bool loadRows)
        {
            TryLoadMapConfig();
            foreach (var pair in sheetMap)
            {
                Sheet sheet = pair.Value;
                sheet.TryLoadAssetData();
                if (loadRows)
                {
                    sheet.TryLoadAll();
                }
            }
        }

        /// <summary>
        /// 加载单张表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="loadRows">是否将表中的所有行进行加载</param>
        public void LoadTable(string tableName, bool loadRows)
        {
            TryLoadMapConfig();
            Sheet sheet;
            if (sheetMap.TryGetValue(tableName.HashCode(), out sheet))
            {
                sheet.TryLoadAssetData();
                if (loadRows)
                {
                    sheet.TryLoadAll();
                }
            }
        }

        /// <summary>
        /// 获取表中行的数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="rowId">行Id</param>
        /// <param name="failLog">不存在打印日志开关</param>
        /// <returns>如果不存在返回null</returns>
        public Config GetSheetRow(string tableName, long rowId, bool failLog = true)
        {
            var ret = GetSheetRow(tableName.HashCode(), rowId);
            if (ret == null && failLog)
            {
                if (loadRowFailure != null)
                {
                    loadRowFailure(string.Format("配置表错误：在表{0}.txt 中找不到 ID:{1} 的配置", tableName, rowId));
                }
            }

            return ret;
        }

        /// <summary>
        /// 获取表中行的数据
        /// </summary>
        /// <param name="sheetOrGroupId">表id</param>
        /// <param name="rowId">行Id</param>
        /// <param name="failLog">不存在打印日志开关</param>
        /// <returns>如果不存在返回null</returns>
        public Config GetSheetRow(int sheetOrGroupId, long rowId, bool failLog = true)
        {
            var ret = GetSheetRow(sheetOrGroupId, rowId);
            if (ret == null && failLog)
            {
                if (loadRowFailure != null)
                {
                    string tableName = ConfigHashCodeDefine.GetTableName(sheetOrGroupId);
                    loadRowFailure(string.Format("配置表错误：在表{0}.txt 中找不到 ID:{1} 的配置", tableName, rowId));
                }
            }

            return ret;
        }

        /// <summary>
        /// 获取表中行的数据
        /// </summary>
        /// <typeparam name="T">行数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="rowId">行Id</param>
        /// <returns>如果不存在返回null</returns>
        public TTable GetSheetRow<TTable>(long rowId, bool failLog = true) where TTable : Config
        {
            TryLoadMapConfig();

            var tableName = typeof(TTable).Name;
            var ret = GetSheetRow(tableName.HashCode(), rowId);
            if (ret == null && failLog)
            {
                if (loadRowFailure != null && rowId != -1)
                {
                    loadRowFailure(string.Format("配置表错误：在表{0}.txt 中找不到 ID:{1} 的配置", tableName, rowId));
                }
            }

            return ret as TTable;
        }


        /// <summary>
        /// 获取表中行的数据
        /// </summary>
        /// <param name="sheetOrGroupId">表名组名hashCode</param>
        /// <param name="rowId">行Id</param>
        /// <returns>如果不存在返回null</returns>
        public Config GetSheetRow(int sheetOrGroupId, long rowId)
        {
            TryLoadMapConfig();

            if (groupRowsCache.TryGetValue(sheetOrGroupId, out var goupLoadedList))
            {
                if (goupLoadedList.TryGetValue(rowId, out var row))
                {
                    return row;
                }
            }

            if (!mapConfig.TryGetValue(sheetOrGroupId, out var groupMap) || !groupMap.TryGetValue(rowId, out var sheetId))
            {
                //不是组id
                sheetId = sheetOrGroupId;
            }

            if (sheetMap.TryGetValue(sheetId, out Sheet sheet))
            {
                return sheet.GetRow(rowId);
            }

            return null;
        }

        /// <summary>
        /// 获取表中所有id
        /// </summary>
        /// <typeparam name="TTable"></typeparam>
        /// <returns></returns>
        public List<long> GetSheetRowIds<TTable>() where TTable : Config
        {
            return GetSheetRowIds(typeof(TTable).Name);
        }

        /// <summary>
        /// 获取表中所有id
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<long> GetSheetRowIds(string tableName, long groupId = 0)
        {
            TryLoadMapConfig();
            int sheetOrGroupId = tableName.HashCode();
            if (mapConfig.TryGetValue(sheetOrGroupId, out var groupMap))
            {
                //来自组
                if (!mapAllIdDict.TryGetValue(sheetOrGroupId,out var ret))
                {
                    ret = new List<long>();
                    mapAllIdDict.Add(sheetOrGroupId, ret);

                    //若主表有配置内容，先加入主表
                    if (sheetMap.TryGetValue(sheetOrGroupId, out Sheet sheet))
                    {
                        ret.AddRange(sheet.GetAllIds());
                    }

                    //再加入组内容
                    ret.AddRange(groupMap.Keys);
                }

                return ret;
            }
            else if (sheetMap.TryGetValue(sheetOrGroupId, out Sheet sheet))
            {
                //来自单配置表
                return sheet.GetAllIds();
            }
            else
            {
                if (loadRowFailure != null)
                {
                    loadRowFailure(string.Format("配置表错误：找不到配置表{0}", tableName));
                }
                return null;
            }
        }

        /// <summary>
        /// 获取一个表中所有的行数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>如果不存在返回null</returns>
        public ReadOnlyCollection<Config> GetTableRows(string tableName)
        {
            TryLoadMapConfig();

            return GetTableRows(tableName.HashCode());
        }

        /// <summary>
        /// 获取一个表中所有的行数据
        /// </summary>
        /// <param name="sheetOrGroupId">组或者表的id</param>
        /// <returns>如果不存在返回null</returns>
        public ReadOnlyCollection<Config> GetTableRows(int sheetOrGroupId)
        {
            TryLoadMapConfig();
            if (mapConfig.TryGetValue(sheetOrGroupId, out var groupMap))
            {
                //来自组
                if (!groupAllRowsCache.TryGetValue(sheetOrGroupId, out var ret))
                {
                    var list = new List<Config>();
                    if (sheetMap.TryGetValue(sheetOrGroupId, out Sheet sheet))
                    {
                        list.AddRange(sheet.Rows);
                    }

                    var searchCache = new HashSet<int>();
                    foreach (var it in groupMap)
                    {
                        if (!searchCache.Contains(it.Value))
                        {
                            searchCache.Add(it.Value);
                            if (sheetMap.TryGetValue(it.Value, out Sheet sheet2))
                            {
                                list.AddRange(sheet2.Rows);
                            }
                        }
                    }

                    ret = new ReadOnlyCollection<Config>(list);
                    groupAllRowsCache.Add(sheetOrGroupId, ret);
                }

                return ret;
            }
            else if (sheetMap.TryGetValue(sheetOrGroupId, out Sheet sheet))
            {
                //来自单配置表
                return sheet.Rows;
            }

            if (loadRowFailure != null)
            {
                loadRowFailure(string.Format("配置表错误：找不到配置表{0}", sheetOrGroupId));
            }
            return null;
        }

        /// <summary>
        /// 在分组中是否包含Id
        /// </summary>
        /// <typeparam name="TGroup">分组类型</typeparam>
        /// <param name="id">id</param>
        /// <returns>如果包含返回true</returns>
        public bool Contains<TGroup>(long id) where TGroup : Config
        {
            return Contains(typeof(TGroup), id);
        }

        /// <summary>
        /// 在分组中是否包含Id
        /// </summary>
        /// <typeparam name="TGroup">分组类型</typeparam>
        /// <param name="id">id</param>
        /// <returns>如果包含返回true</returns>
        public bool Contains(Type tableType, long id)
        {
            if (tableType == null)
            {
                return false;
            }

            string groupName = tableType.Name;

            return Contains(groupName, id);
        }

        /// <summary>
        /// 在分组中是否包含Id
        /// </summary>
        /// <param name="tableName">名</param>
        /// <param name="id">id</param>
        /// <returns>如果包含返回true</returns>
        public bool Contains(string tableName, long id)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return false;
            }
            TryLoadMapConfig();

            int sheetOrGroupId = tableName.HashCode();
            if (!mapConfig.TryGetValue(sheetOrGroupId, out var groupMap) || !groupMap.TryGetValue(id, out var sheetId))
            {
                sheetId = sheetOrGroupId;
            }
            
            if (sheetMap.TryGetValue(sheetId, out Sheet sheet))
            {
                //来自单配置表
                return sheet.Contains(id);
            }
            else
            {
                if (loadRowFailure != null)
                {
                    loadRowFailure(string.Format("配置表错误：找不到配置表{0}", tableName));
                }
                return false;
            }
        }
        

        /// <summary>
        /// 卸载表（包括常驻内存的）
        /// </summary>
        /// <param name="tableName">表名</param>
        public void UnloadTable(string tableName)
        {
            int sheetId = tableName.HashCode();
            Sheet sheet;
            if (sheetMap.TryGetValue(sheetId, out sheet))
            {
                sheet.Clear();
            }
        }

        /// <summary>
        /// 卸载所有表
        /// </summary>
        /// <param name="includePersistent">是否卸载常驻内存的表</param>
        public void UnloadAll(bool includePersistent)
        {
            foreach (var pair in sheetMap)
            {
                Sheet sheet = pair.Value;
                if (includePersistent || !sheet.IsPersistent)
                {
                    sheet.Clear();
                }
            }

            sheetMap.Clear();
        }

        /// <summary>
        /// 重置所有数据,还原到原始状态
        /// </summary>
        public void Reset()
        {
            sheetMap.Clear();
            mapConfig.Clear();
            groupRowsCache.Clear();
            loadTableImpl = null;
            loadRowFailure = null;
            mappingLoaded = false;
        }
        #endregion

        #region Internal Methods

        /// <summary>
        /// 加载映射文件
        /// </summary>
        private void TryLoadMapConfig()
        {
            if (mappingLoaded)
            {
                return;
            }

            mappingLoaded = true;
            byte[] bytes = loadTableImpl(MapAssetPath);
            if (bytes == null)
            {
                return;
            }

            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader reader = new ConfigBinaryReader(ms, this);

            int length = reader.ReadVariableInt32();
            for (int i = 0; i < length; i++)
            {
                int groupId = reader.ReadVariableInt32();
                int subLength = reader.ReadVariableInt32(); ;
                Dictionary<long, int> idToPosition = new Dictionary<long, int>(subLength);
                if (subLength > 0)
                {
                    int readedCount = 0;
                    while (readedCount < subLength)
                    {
                        int sheetItemCount = reader.ReadVariableInt32();
                        int sheetId = reader.ReadVariableInt32();

                        for (int j = 0; j < sheetItemCount; j++)
                        {
                            var key = reader.ReadVariableInt64();
                            idToPosition.Add(key, sheetId);
                        }

                        readedCount += sheetItemCount;
                    }
                }

                mapConfig.Add(groupId, idToPosition);
            }

            reader.Close();
            ms.Close();
        }
        #endregion

        #region Declarations
        private class Sheet
        {
            #region Properties
            private Dictionary<long, long> posMap;
            private List<long> allIds;
            private Dictionary<long, Config> rowDict = new Dictionary<long, Config>();

            private List<Config> rows = new List<Config>();
            private ReadOnlyCollection<Config> readOnlyRows;

            private bool isAllLoaded = false;
            private bool isLoadedAsset = false;
            private BinaryReader reader;
            private Func<string, byte[]> loadTableImpl;
            private ConfigManagerImpl manager;

            /// <summary>
            /// 分组
            /// </summary>
            public int Group;

            /// <summary>
            /// 自己的Id
            /// </summary>
            public int Id;

            /// <summary>
            /// 资源路径
            /// </summary>
            public string AssetPath;

            /// <summary>
            /// 行数据的类型
            /// </summary>
            public Type RowType;

            /// <summary>
            /// 是否进行持久化，常驻内存
            /// </summary>
            public bool IsPersistent;

            private Func<Config> _InstanceImpl;
            private bool customInstanceImpl;
            public Func<Config> InstanceImpl
            {
                get
                {
                    return _InstanceImpl;
                }
                set
                {
                    _InstanceImpl = value;
                    customInstanceImpl = value != null;
                }
            }

            public ReadOnlyCollection<Config> Rows
            {
                get
                {
                    TryLoadAll();
                    if (rows == null)
                    {
                        return null;
                    }

                    if (readOnlyRows == null)
                    {
                        readOnlyRows = new ReadOnlyCollection<Config>(rows);
                    }

                    return readOnlyRows;
                }
            }
            #endregion

            #region Constructs
            public Sheet(ConfigManagerImpl manager, Func<string, byte[]> loadTableImpl)
            {
                this.loadTableImpl = loadTableImpl;
                this.manager = manager;
            }
            #endregion

            #region Public Methods

            public Config GetRow(long id)
            {
                TryLoadAssetData();

                Config config = null;
                if (rowDict.TryGetValue(id, out config) || isAllLoaded)
                {
                    return config;
                }

                long position;
                if (!posMap.TryGetValue(id, out position))
                {
                    return null;
                }

                return LoadRow(position, id);
            }

            public bool Contains(long id)
            {
                TryLoadAssetData();

                return posMap.ContainsKey(id);
            }

            public List<long> GetAllIds()
            {
                TryLoadAssetData();

                return allIds;
            }

            public int GetCount()
            {
                TryLoadAssetData();
                return allIds.Count;
            }

            /// <summary>
            /// 清理，回到初始化状态，卸载已经加载的资源数据
            /// </summary>
            public void Clear()
            {
                Dictionary<long, Config> goupLoadedList;
                if (manager.groupRowsCache.TryGetValue(Group, out goupLoadedList))
                {
                    foreach (var row in rows)
                    {
                        goupLoadedList.Remove(row.id);
                    }
                }

                if (posMap != null)
                {
                    posMap.Clear();
                }

                if (allIds != null)
                {
                    allIds.Clear();
                }

                rowDict.Clear();
                rows.Clear();
                isAllLoaded = false;
                isLoadedAsset = false;
                if (reader != null)
                {
                    reader.Close();
                    if (reader.BaseStream != null)
                    {
                        reader.BaseStream.Close();
                    }

                    reader = null;
                }
            }
            #endregion

            #region Internal Methods
            internal void TryLoadAssetData()
            {
                if (isLoadedAsset)
                {
                    return;
                }

                byte[] bytes = loadTableImpl(AssetPath);
                if (bytes == null)
                {
                    manager.loadRowFailure?.Invoke("表加载失败:" + AssetPath);

                    return;
                }

                isLoadedAsset = true;
                MemoryStream ms = new MemoryStream(bytes);
                reader = new ConfigBinaryReader(ms, manager);
                ms.Position = reader.ReadInt64();

                int length = reader.ReadVariableInt32();
                posMap = new Dictionary<long, long>(length);
                allIds = new List<long>(length);
                for (int i = 0; i < length; i++)
                {
                    var key = reader.ReadVariableInt64();
                    var value = reader.ReadVariableInt64();
                    posMap.Add(key, value);
                    allIds.Add(key);
                }
            }

            internal void TryLoadAll()
            {
                if (isAllLoaded)
                {
                    return;
                }

                TryLoadAssetData();
                foreach (var pair in posMap)
                {
                    long position = pair.Value;
                    long id = pair.Key;

                    if (rowDict.ContainsKey(id))
                    {
                        continue;
                    }

                    LoadRow(position, id);
                }

                isAllLoaded = true;
            }

            private Config LoadRow(long position, long id)
            {
                if (reader == null)
                {
                    return null;
                }

                reader.BaseStream.Position = position;
                Config row = customInstanceImpl ? InstanceImpl() : (Config)Activator.CreateInstance(RowType);
                row.id = id;
                rowDict.Add(id, row);
                rows.Add(row);

                //加入全局缓存
                Dictionary<long, Config> goupLoadedList;
                if (!manager.groupRowsCache.TryGetValue(Group, out goupLoadedList))
                {
                    goupLoadedList = new Dictionary<long, Config>();
                    manager.groupRowsCache.Add(Group, goupLoadedList);
                }

                goupLoadedList.Add(id, row);

                isAllLoaded = rowDict.Count == posMap.Count;
                (row as ISerializeable).Deserialize(reader);

                return row;
            }
            #endregion
        }
        #endregion

        #region Declarations
        public class ConfigBinaryReader : BinaryReader
        {
            #region Properties
            public ConfigManagerImpl ConfigManager { get; private set; }
            #endregion

            #region Constructors
            public ConfigBinaryReader(Stream input, ConfigManagerImpl configManager) : base(input)
            {
                ConfigManager = configManager;
            }
            #endregion
        }
        #endregion
    }
}
