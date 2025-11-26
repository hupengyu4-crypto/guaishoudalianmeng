using RootScript.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BattleSystem
{
    public static class BattleUtils
    {
        #region 战斗本体

        /// <summary>
        /// 尝试创建小兵关卡，不满足条件则创建otherType的关卡
        /// </summary>
        /// <param name="otherType"></param>
        /// <param name="battleData"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        private static NormalBattle CreateSoldierBattleOr(Type otherType, battle_field battleData, BattleSceneCfg cfg)
        {
            var dfsEmpty = true;
            var dfsCamp = battleData.DfsCamp;
            for (int i = 0, l = dfsCamp.Count; i < l; i++)
            {
                var dfs = dfsCamp[i];
                if (dfs == null)
                {
                    continue;
                }

                if ((dfs.Fighter != null && dfs.Fighter.Count > 0) || (dfs.PetFighter != null && dfs.PetFighter.Count > 0))
                {
                    dfsEmpty = false;
                    break;
                }
            }

            return dfsEmpty ? new SoldierBattle(battleData.BattleKey, cfg) : Activator.CreateInstance(otherType, battleData.BattleKey, cfg) as NormalBattle;
        }

        /// <summary>
        /// 创建战斗
        /// </summary>
        /// <param name="battleData">后台战斗数据</param>
        /// <returns>战斗实体</returns>
        public static NormalBattle CreateBattle(battle_field battleData)
        {
            NormalBattle battle = null;
            if (BattleDef.BattleSceneInfo.TryGetValue(battleData.Type, out var id))
            {
                var cfg = ConfigManagerNew.Instance.Get<BattleSceneCfg>(ConfigHashCodeDefine.BattleSceneCfg, id);
                switch (cfg.battleType)
                {
                    case BattleDef.BattleType.Normal:
                        battle = CreateSoldierBattleOr(typeof(NormalBattle), battleData, cfg);
                        break;
                    case BattleDef.BattleType.Mass:
                        battle = new MassBattle(battleData.BattleKey, cfg);
                        break;
                    case BattleDef.BattleType.Boss:
                        battle = new BossBattle(battleData.BattleKey, cfg);
                        break;
                    case BattleDef.BattleType.Fire:
                        battle = CreateSoldierBattleOr(typeof(FireBattle), battleData, cfg);;
                        break;
                    case BattleDef.BattleType.PVP:
                        battle = new PVPBattle(battleData.BattleKey, cfg);
                        break;
                    case BattleDef.BattleType.KOF:
                        battle = new KOFBattle(battleData.BattleKey, cfg);
                        break;
                    case BattleDef.BattleType.Bo3:
                        battle = new Bo3Battle(battleData.BattleKey, cfg);
                        break;
                    default:
                        battle = CreateSoldierBattleOr(typeof(NormalBattle), battleData, cfg);
                        break;
                }
            }
            else
            {
                //测试战斗
                var cfg = ConfigManagerNew.Instance.Get<BattleSceneCfg>(ConfigHashCodeDefine.BattleSceneCfg, 1);
                switch (battleData.Type)
                {
                    case "fb_test":
                        battle = CreateSoldierBattleOr(typeof(NormalBattle), battleData, cfg);
                        break;
                    case "Mass_test":
                        battle = new MassBattle(battleData.BattleKey, cfg);
                        break;
                    default:
                        battle = CreateSoldierBattleOr(typeof(NormalBattle), battleData, cfg);
                        break;
                }
            }

            //神界探索不回血
            switch (battleData.Type)
            {
                case "map_divine_explore":
                    //battle.CanNotHeal = true; //继血也可以回血
                    break;
            }
            return battle;
        }

        #endregion

        #region 索敌
        private static int GetStateOverlayCount(Dictionary<Fighter, int> dict, Fighter fighter, EffectSys.EffectType effectType)
        {
            if (fighter == null || fighter.Effect == null || fighter.Effect.AllEffectList == null)
            {
                return 0;
            }

            if (dict.TryGetValue(fighter, out int ret))
            {
                return ret;
            }

            var allEffectList = fighter.Effect.AllEffectList;
            for (int i = 0, l = allEffectList.Count; i < l; i++)
            {
                var effect = allEffectList[i];
                if (effect == null)
                {
                    continue;
                }

                if (effect.IsType(effectType))
                {
                    ret += effect.overlayCount;
                }
            }

            dict.Add(fighter, ret);
            return ret;
        }

        /// <summary>
        /// 筛选目标
        /// </summary>
        /// <param name="owner">施法者</param>
        /// <param name="findType">查找范围</param>
        /// <param name="raceType">种族选择</param>
        /// <param name="jobType">职业选择</param>
        /// <param name="sid">角色Sid 如果指定角色 BattleDef.EFindType 只能我方全体 或者敌方全体</param>
        /// <param name="findCount">命中数量</param>
        /// <param name="checkState">状态选择</param>
        /// <param name="property">属性选择</param>
        /// <param name="otherType">其他类型</param>
        /// <param name="compareType">比较类型</param>
        /// <param name="otherArgs">其他类型的参数</param>
        /// <returns></returns>
        public static List<Fighter> GetFighters(Fighter owner, BattleDef.EFindType findType,
            BattleDef.ERaceType[] raceType, BattleDef.EJobType[] jobType, long sid, int findCount,
            BattleObject.State[] checkState, BattleDef.Property property,
            BattleDef.OtherType otherType, BattleDef.CompareGradeType compareType, long[] otherArgs,bool notSwitchRow= false)
        {
            List<Fighter> results = new List<Fighter>();

            if (owner == null)
            {
                Log.error($"看到这个大概率效果被清理了，无法正确执行，请检查技能配置的清理时机");
                return new List<Fighter>(0);
            }
            NormalBattle battle = (NormalBattle)owner.Battle;

            var isAttacker = owner.CampType == BattleDef.TeamCampType.Attacker;
            if (sid > 0)
            {
                List<Fighter> all;
                switch (findType)
                {
                    case BattleDef.EFindType.AllEnemy:
                        all = (isAttacker ? battle.Defenders.AllFighters : battle.Attackers.AllFighters);
                        break;
                    case BattleDef.EFindType.AllSelf:
                        all = (isAttacker ? battle.Attackers.AllFighters : battle.Defenders.AllFighters);
                        break;
                    default:
                        return results;
                }

                foreach (var fighter in all)
                {
                    if (findCount > 0 && results.Count >= findCount)
                    {
                        break;
                    }

                    if (fighter.Data.Sid == sid)
                    {
                        results.Add(fighter);
                    }
                }

                return results;
            }


            //这几个类型是不需要筛选的，直接返回，减少计算开销
            switch (findType)
            {
                case BattleDef.EFindType.Self:
                    results.Add(owner);
                    return results;
                case BattleDef.EFindType.Target:
                    {
                        var target = isAttacker ? battle.Defenders.Combatant : battle.Attackers.Combatant;
                        if (target != null)
                        {
                            results.Add(target);
                        }

                        return results;
                    }
                case BattleDef.EFindType.TargetSelf:
                    {
                        var targetSelf = isAttacker ? battle.Attackers.Combatant : battle.Defenders.Combatant;
                        if (targetSelf != null)
                        {
                            results.Add(targetSelf);
                        }

                        return results;
                    }
            }

            //以下是可以筛选的
            var attackers = new List<Fighter>();
            var defenders = new List<Fighter>();

            //种族筛选
            bool isNeedFilterRace = raceType != null && raceType.Length > 0 && System.Array.IndexOf(raceType, BattleDef.ERaceType.All) < 0;
            //职业筛选
            bool isNeedFilterJob = jobType != null && jobType.Length > 0 && System.Array.IndexOf(jobType, BattleDef.EJobType.All) < 0;

            for (var i = 0; i < battle.AllFighter.Count; i++)
            {
                var fighter = battle.AllFighter[i];
                if (fighter.IsDead || fighter.IsFakeDead)
                {
                    continue;
                }
                if (isNeedFilterRace && System.Array.IndexOf(raceType, fighter.Data.raceType) < 0)
                {
                    continue;
                }
                if (isNeedFilterJob && System.Array.IndexOf(jobType, fighter.Data.jobType) < 0)
                {
                    continue;
                }
                if (fighter.CampType == BattleDef.TeamCampType.Attacker)
                {
                    attackers.Add(fighter);
                }
                else
                {
                    defenders.Add(fighter);
                }
            }

            var isPve = owner.Battle.IsPve();
            switch (findType)
            {
                case BattleDef.EFindType.RandomEnemy:
                case BattleDef.EFindType.RandomSelf:
                    if (findType == BattleDef.EFindType.RandomEnemy)
                        results = (isAttacker ? defenders : attackers);
                    else
                        results = (isAttacker ? attackers : defenders);
                    int count = results.Count;
                    if (count > 0)
                    {
                        // 使用Fisher-Yates洗牌算法打乱列表
                        for (int i = count - 1; i > 0; i--)
                        {
                            var newIndex = battle.Random.RandomValue(0, count - 1);
                            Fighter temp = results[i];
                            results[i] = results[newIndex];
                            results[newIndex] = temp;
                        }
                    }
                    break;
                case BattleDef.EFindType.AllEnemy:
                case BattleDef.EFindType.AllSelf:
                    if (findType == BattleDef.EFindType.AllEnemy)
                        results = (isAttacker ? defenders : attackers);
                    else
                        results = (isAttacker ? attackers : defenders);
                    break;
                case BattleDef.EFindType.AllSelfExMe:
                    results = (isAttacker ? attackers : defenders);
                    results.Remove(owner);
                    break;
                case BattleDef.EFindType.FrontRowSelf:
                    FilterRow(isAttacker ? attackers : defenders, results, true, isPve,true, notSwitchRow);
                    break;
                case BattleDef.EFindType.BackRowSelf:
                    FilterRow(isAttacker ? attackers : defenders, results, false, isPve, true, notSwitchRow);
                    break;
                case BattleDef.EFindType.FrontRowEnemy:
                    FilterRow(isAttacker ? defenders : attackers, results, true, isPve, true, notSwitchRow);
                    break;
                case BattleDef.EFindType.BackRowEnemy:
                    FilterRow(isAttacker ? defenders : attackers, results, false, isPve, true, notSwitchRow);
                    break;
                case BattleDef.EFindType.SameRowSelf:
                    FilterRow(isAttacker ? attackers : defenders, results, IsFront(isPve, owner.Data.Pos), isPve, true, notSwitchRow);
                    break;
                case BattleDef.EFindType.OtherRowSelf:
                    FilterRow(isAttacker ? attackers : defenders, results, !IsFront(isPve, owner.Data.Pos), isPve, true, notSwitchRow);
                    break;
                case BattleDef.EFindType.SameRowEnemy:
                    FilterRow(isAttacker ? defenders : attackers, results, IsFront(isPve, owner.Data.Pos), isPve, true, notSwitchRow);
                    break;
                case BattleDef.EFindType.OtherRowEnemy:
                    FilterRow(isAttacker ? defenders : attackers, results, !IsFront(isPve, owner.Data.Pos), isPve, true, notSwitchRow);
                    break;
                default:
                    //没写就阻断
                    Log.error($"筛选条件未实现,请检查{findType}");
                    break;
            }

            //额外筛选项

            var toCheckState = BattleObject.State.Normal;
            if (checkState != null && checkState.Length > 0)
            {
                for (int j = 0, jl = checkState.Length; j < jl; j++)
                {
                    toCheckState |= checkState[j];
                }
            }

            if (toCheckState != BattleObject.State.Normal)
            {
                for (int i = results.Count - 1; i >= 0; i--)
                {
                    if (!results[i].IsState(toCheckState))
                        results.RemoveAt(i);
                }
            }
            else if (property != BattleDef.Property.None && results.Count > 1)
            {
                results.Sort((left, right) =>
                {
                    if (left == null && right == null)
                        return 0;
                    if (left == null)
                        return -1;
                    if (right == null)
                        return 1;
                    var leftProp = (left.Data.GetProp(property));
                    var rightProp = (right.Data.GetProp(property));
                    if (leftProp == rightProp)
                        return left.Uid < right.Uid ? -1 : 1; //return 0;
                    if (compareType == BattleDef.CompareGradeType.Highest)
                    {
                        return leftProp > rightProp ? -1 : 1;
                    }
                    return leftProp > rightProp ? 1 : -1;
                });
            }
            else if (otherType != BattleDef.OtherType.Null)
            {
                if (otherType == BattleDef.OtherType.NegativeEffect)
                {
                    for (int i = results.Count; i >= 0; i--)
                    {
                        if (results[i].IsState(BattleObject.State.Normal))
                            results.RemoveAt(i);
                    }
                }
                else if (otherType == BattleDef.OtherType.HpPercent && results.Count > 1)
                {
                    results.Sort((left, right) =>
                    {
                        if (left == null || right == null)
                            return 0;
                        var leftHpPercent = left.HpPercent;
                        var rightHpPercent = right.HpPercent;
                        if (Math.Abs(leftHpPercent - rightHpPercent) < 0.01)
                            return left.Uid < right.Uid ? -1 : 1; //return 0;
                        if (compareType == BattleDef.CompareGradeType.Highest)
                        {
                            return leftHpPercent > rightHpPercent ? -1 : 1;
                        }
                        return leftHpPercent > rightHpPercent ? 1 : -1;
                    });
                }
                else if (otherType == BattleDef.OtherType.StateOverlayCount && results.Count > 1)
                {
                    var state = EffectSys.EffectType.Null;
                    if (otherArgs != null && otherArgs.Length > 0)
                    {
                        for (int i = 0, l = otherArgs.Length; i < l; i++)
                        {
                            state |= EffectSys.StateToEffectType((BattleObject.State)otherArgs[i]);
                        }
                    }

                    if (state != EffectSys.EffectType.Null)
                    {
                        Dictionary<Fighter, int> stateOverlayCount = new();
                        results.Sort((left, right) =>
                        {
                            if (left == null || right == null)
                            {
                                return 0;
                            }

                            var leftStateOverlayCount = GetStateOverlayCount(stateOverlayCount, left, state);
                            var rightStateOverlayCount = GetStateOverlayCount(stateOverlayCount, right, state);
                            if (leftStateOverlayCount == rightStateOverlayCount)
                            {
                                return left.Uid < right.Uid ? -1 : 1;
                            }

                            if (compareType == BattleDef.CompareGradeType.Highest)
                            {
                                return leftStateOverlayCount > rightStateOverlayCount ? -1 : 1;
                            }

                            return leftStateOverlayCount > rightStateOverlayCount ? 1 : -1;
                        });
                        stateOverlayCount.Clear();
                    }
                }
            }

            if (findCount > 0 && results.Count > findCount)
            {
                while (results.Count > findCount)
                {
                    results.RemoveAt(results.Count - 1);
                }
            }

            return results;
        }

        /// <summary>
        /// 筛选种族
        /// </summary>
        /// <param name="list"></param>
        /// <param name="curRaceType">当前配置目标种族</param>
        private static List<Fighter> FilterRace(List<Fighter> list, BattleDef.ERaceType[] curRaceType)
        {
            var results = new List<Fighter>();
            for (int i = 0; i < list.Count; i++)
            {
                var fight = list[i];
                foreach (var raceType in curRaceType)
                {
                    if (raceType == BattleDef.ERaceType.All || raceType == fight.Data.raceType)
                    {
                        if (!results.Contains(fight))
                            results.Add(fight);
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// 筛选职业
        /// </summary>
        /// <param name="list"></param>
        /// <param name="curJobType">当前配置目标职业</param>
        private static List<Fighter> FilterJob(List<Fighter> list, BattleDef.EJobType[] curJobType)
        {
            var results = new List<Fighter>();
            for (int i = 0; i < list.Count; i++)
            {
                var fight = list[i];
                foreach (var jobType in curJobType)
                {
                    if (jobType == BattleDef.EJobType.All || jobType == fight.Data.jobType)
                    {
                        if (!results.Contains(fight))
                            results.Add(fight);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// 筛选前后排
        /// </summary>
        /// <param name="list">原数据</param>
        /// <param name="newList">新数据</param>
        /// <param name="isFront">是否为前排</param>
        /// <param name="isPve">是否为推图战斗</param>
        /// <param name="isFirst">是否第一次调用</param>
        private static void FilterRow(List<Fighter> list, List<Fighter> newList, bool isFront, bool isPve, bool isFirst = true, bool notSwitchRow = false)
        {
            if (list.Count == 0)
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                int index = list[i].Data.Pos;
                if (isFront && IsFront(isPve, index))
                {
                    newList.Add(list[i]);
                }
                else if (!isFront && !IsFront(isPve, index))
                {
                    newList.Add(list[i]);
                }
            }

            if (newList.Count == 0 && isFirst && !notSwitchRow)
            {
                //没人，换排选
                FilterRow(list, newList, !isFront, isPve, false);
            }
        }

        /// <summary>
        /// 是否为前排
        /// </summary>
        /// <param name="isPve">是否为推图战斗</param>
        /// <param name="pos">站位</param>
        /// <returns></returns>
        private static bool IsFront(bool isPve, int pos)
        {
            if (isPve)
            {
                return pos == 1 || pos == 2 || pos == 3;
            }

            return pos == 1 || pos == 2;
        }

        #endregion

        #region 工具/扩展

        public static long GenerateUid(BaseBattle battle)
        {
            long uid = battle.Random.RandomValue(100, int.MaxValue);
            while (battle.allSceneObjects.ContainsKey(uid))
            {
                uid = battle.Random.RandomValue(100, int.MaxValue);
                if (!battle.allSceneObjects.ContainsKey(uid))
                {
                    break;
                }
            }

            return uid;
        }

        public static double YkDistance(YKVector3d a, YKVector3d b, double distance)
        {
            double dx = a.x - b.x;
            double dz = a.z - b.z;
            return (dx * dx + dz * dz) - distance * distance;
        }

        /// <summary>
        /// 获取指定索引位置元素或者如果索引超出数组长度或者最后一个元素
        /// </summary>
        public static T GetIndexOrEnd<T>(this T[] arr, int index) => arr[index >= arr.Length ? arr.Length - 1 : index];

        public static Stack<V> AddStackItem<K, V>(this Dictionary<K, Stack<V>> dic, K k, V v)
        {
            if (!dic.TryGetValue(k, out var stack))
            {
                stack = new Stack<V>();
                dic.Add(k, stack);
            }

            stack.Push(v);
            return stack;
        }

        public static RingBuffer<V> AddRingBufferItem<K, V>(this Dictionary<K, RingBuffer<V>> dic, K k, V v, int capacity)
        {
            if (!dic.TryGetValue(k, out var ringBuffer))
            {
                ringBuffer = new RingBuffer<V>(capacity);
                dic.Add(k, ringBuffer);
            }

            ringBuffer.Push(v);
            return ringBuffer;
        }

        public static void SetValue(object obj, string fieldName, object value)
        {
            FieldInfo info = obj.GetType().GetField(fieldName);
            info.SetValue(obj, value);
        }

        public static object GetValue(object obj, string fieldName)
        {
            FieldInfo info = obj.GetType().GetField(fieldName);
            return info.GetValue(obj);
        }

        public static V TryGetValue<K, V>(this Dictionary<K, V> dic, K key)
        {
            if (dic.TryGetValue(key, out var v))
            {
                return v;
            }
            return default(V);
        }

        #endregion



        #region Web

        /// <summary>
        /// 检查MD5
        /// </summary>
        /// <param name="paths">路径</param>
        /// <param name="ignoreKeys">忽略关键字</param>
        /// <returns></returns>
        public static string CheckMd5(string[] paths, string[] ignoreKeys = null)
        {
            if (paths == null || paths.Length == 0)
                return "";
            Dictionary<string, string> allFile = new Dictionary<string, string>();
            foreach (var path in paths)
            {
                if (ignoreKeys != null)
                {
                    var isPass = false;
                    foreach (var ignoreStr in ignoreKeys)
                    {
                        if (path.Replace("\\", "/").Contains(ignoreStr))
                        {
                            isPass = true;
                            break;
                        }
                    }
                    if (isPass)
                    {
                        continue;
                    }
                }
                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    allFile.Add(fileInfo.Name, fileInfo.FullName);
                }
                else
                {
                    GetFileName(path, allFile, ignoreKeys);
                }
            }

            //allFile = allFile.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            var sortKeys = new List<string>(allFile.Keys);
            sortKeys.Sort();
            StringBuilder sb = new StringBuilder();
            //foreach (var variable in allFile)
            for (int i = 0, l = sortKeys.Count; i < l; i++)
            {
                if (!allFile.TryGetValue(sortKeys[i], out var fullName))
                {
                    continue;
                }

                FileToMd5(fullName, sb);
            }
            return StringToMd5(sb.ToString());
        }

        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="fileDic">文件信息</param>
        /// <param name="ignoreKeys">忽略关键字</param>
        private static void GetFileName(string path, Dictionary<string, string> fileDic, string[] ignoreKeys = null)
        {
            DirectoryInfo theFolder = new DirectoryInfo(path);
            FileInfo[] theFileInfo = theFolder.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo nextFile in theFileInfo)
            {
                if (ignoreKeys != null)
                {
                    var isPass = false;
                    foreach (var ignoreStr in ignoreKeys)
                    {
                        if (nextFile.FullName.Replace("\\", "/").Contains(ignoreStr))
                        {
                            isPass = true;
                            break;
                        }
                    }
                    if (isPass)
                    {
                        continue;
                    }
                }
                if (nextFile.Name.EndsWith(".pdb")) continue;
                if (nextFile.Name.EndsWith(".xls")) continue;
                if (nextFile.Name.EndsWith(".meta")) continue;
                if (!fileDic.ContainsKey(nextFile.Name))
                    fileDic.Add(nextFile.Name, nextFile.FullName);
            }
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirInfo)
            {
                FileInfo[] fileInfo = nextFolder.GetFiles("*.*", SearchOption.AllDirectories);

                foreach (FileInfo nextFile in fileInfo)
                {
                    if (ignoreKeys != null)
                    {
                        var isPass = false;
                        foreach (var ignoreStr in ignoreKeys)
                        {
                            if (nextFile.FullName.Replace("\\", "/").Contains(ignoreStr))
                            {
                                isPass = true;
                                break;
                            }
                        }
                        if (isPass)
                        {
                            continue;
                        }
                    }
                    if (nextFile.Name.EndsWith(".pdb")) continue;
                    if (nextFile.Name.EndsWith(".xls")) continue;
                    if (nextFile.Name.EndsWith(".meta")) continue;
                    if (!fileDic.ContainsKey(nextFile.Name))
                        fileDic.Add(nextFile.Name, nextFile.FullName);
                }
            }
        }

        /// <summary>
        /// 文件MD5
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="md5Builder">md5 StringBuilder</param>
        private static void FileToMd5(string path, StringBuilder md5Builder)
        {
            try
            {
                FileStream fileStream = new FileStream(path, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] r = md5.ComputeHash(fileStream);
                fileStream.Close();
                for (int i = 0; i < r.Length; i++)
                {
                    md5Builder.Append(r[i].ToString("x2"));
                }
            }
            catch (Exception e)
            {
                Log.error(e.ToString());
            }
        }

        /// <summary>
        /// 对字符串MD5
        /// </summary>
        /// <param name="source">文本</param>
        /// <returns>MD5</returns>
        private static string StringToMd5(string source)
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取指定战斗者的属性最大值
        /// </summary>
        /// <param name="fighter">战斗者</param>
        /// <param name="property">属性</param>
        /// <returns>最大值</returns>
        public static long GetMaxPropertyValue(Fighter fighter, BattleDef.Property property)
        {
            long maxValue = 0;
            if (BattleDef.MaxProps.TryGetValue((byte)property, out var values))
            {
                var maxCfgValue = values[1];
                if (maxCfgValue != 0)
                {
                    var targetProp = (byte)values[0];//参照目标
                    if (targetProp == 0)
                    {
                        maxValue = (long)maxCfgValue; //值类型
                    }
                    else
                    {
                        maxValue = (long)YKMath.Ceiling(fighter.Data.initProps[targetProp] * maxCfgValue);//参照基础属性的倍数类型

                        //生命的最大值取决于当前生命上限以及配置最大上限的最小值
                        if (property == BattleDef.Property.hp)
                        {
                            maxValue = Math.Min(maxValue, fighter.Data.nowProps[targetProp]);
                        }
                    }
                }
            }
            return maxValue;
        }

        /// <summary>
        /// 战斗者属性中文映射
        /// </summary>
        private static Dictionary<int, string> PropertyName;

        /// <summary>
        /// 初始化属性名
        /// </summary>
        public static void InitPropertyName()
        {
            if (PropertyName == null)
            {
                PropertyName = new Dictionary<int, string>((int)BattleDef.Property.MaxCount);

                Type enumType = typeof(BattleDef.Property);
                string[] es = Enum.GetNames(enumType);

                for (byte i = 0; i < es.Length; i++)
                {
                    var field = enumType.GetField(es[i]);
                    var des = es[i];
                    if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute
                        attribute)
                    {
                        des = attribute.Description;
                    }

                    if (!PropertyName.ContainsKey(i))
                    {
                        PropertyName.Add(i, des);
                    }
                }
            }
        }

        /// <summary>
        /// 获取属性名称
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns>名称</returns>
        public static string GetPropertyName(BattleDef.Property property)
        {
            return GetPropertyName((byte)property);
        }

        /// <summary>
        /// 获取属性名称
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns>名称</returns>
        public static string GetPropertyName(byte property)
        {
            if (PropertyName != null && PropertyName.TryGetValue(property, out var desc))
            {
                return desc;
            }

            return "";
        }

        #endregion

        #region 枚举

        /// <summary>
        /// 获取枚举定义的属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumName<T>(T e) where T : Enum
        {
            Type enumType = typeof(T);
            var field = enumType.GetField(e.ToString());
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }
            return field.ToString();
        }

        /// <summary>
        /// 获取枚举Mark的打印
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static string GetEnumMarkDesc<T>(T mark) where T : Enum
        {
            Type enumType = typeof(T);
            if (mark.Equals(Enum.ToObject(enumType, 0)))
                return GetEnumName<T>(mark);
            var values = enumType.GetEnumValues();
            StringBuilder sb = new StringBuilder();
            foreach (T value in values)
            {
                if (value.Equals(Enum.ToObject(enumType, 0)))
                    continue;
                if (mark.HasFlag(value))
                {
                    sb.Append(GetEnumName<T>(value)).Append(" ");
                }
            }
            return sb.ToString();
        }

        #endregion

        #region 火球伤害计算

        /// <summary>
        /// 通过玩家等级计算火球伤害
        /// </summary>
        /// <param name="lv">玩家等级</param>
        /// <returns>火球伤害</returns>
        public static long GetFireDamage(int lv, bool isBig = false)
        {
            var area = BattleDef.Fire;
            //向上取整(之前的区间系数*之前的区间最大等级 + ... +(当前等级-上一个区间最大等级)*当前区间系数)
            var len = area.Length;
            double damage = 0;
            for (var i = 0; i < len; i++)
            {
                var values = area[i];
                var maxLv = values[0];
                var per = values[1];

                if (lv >= maxLv)
                {
                    double lastMaxLv = 0;
                    if (i > 0)
                    {
                        lastMaxLv = area[i - 1][0];
                    }
                    damage += (maxLv - lastMaxLv) * per;
                    continue;
                }

                if (i == 0)
                    damage += lv * per;
                else
                    damage += (lv - area[i - 1][0]) * per;

                break;
            }
            if (isBig)
            {
                double bigFireCoef = BattleDef.BigFire;
                damage = damage * bigFireCoef;
            }

            return (long)YKMath.Ceiling(damage);
        }

        #endregion

        #region 其他一些计算

        /// <summary>
        /// 计算治疗系数
        /// </summary>
        /// <param name="author">主动方、施加方</param>
        /// <param name="target">被施加方</param>
        /// <returns>治疗系数</returns>
        public static double CalcHealValue(Fighter author, Fighter target)
        {
            var healValue = author.Data[BattleDef.Property.heal] * BattleDef.Percent100;
            var beHealValue = target.Data[BattleDef.Property.be_heal] * BattleDef.Percent100;
            var heal = healValue * beHealValue;
            heal = Math.Max(heal, 0);
            return heal;
        }

        public static DamageParams CreateSimpleDamageParams(BaseBattle battle, long effectSid, long attackUid, long defendUid, long damage, BattleDef.DamageType damageType, bool isRealDamage, bool autoRelease = true)
        {
            DamageParams args = battle.CreateEventParam<DamageParams>();
            args.effectSid = effectSid;
            args.attackUid = attackUid;
            args.defendUid = defendUid;
            args.oldValue = damage;
            args.newValue = damage;
            args.isCrit = false;
            args.isBlock = false;
            args.IsAutoRelease = autoRelease;
            args.damageType = damageType;
            args.isRealDamage = isRealDamage;

            return args;
        }

        private static bool GetLevel(ref int level, int arrayLength, bool toLastLevel = true)
        {
            --level; //start with 0
            if (level < 0 || arrayLength <= 0)
            {
                return false;
            }

            if (level >= arrayLength)
            {
                if (!toLastLevel)
                {
                    return false;
                }

                level = arrayLength - 1;
            }

            return true;
        }

        /// <summary>
        /// 得到等级相关的值
        /// </summary>
        /// <param name="array">array</param>
        /// <param name="level">level</param>
        /// <param name="value">返回值</param>
        /// <param name="toLastLevel">如果level>=array.Length, level = array.Length - 1</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>是否得到值</returns>
        public static bool GetLevelValue<T>(T[] array, int level, out T value, bool toLastLevel = true)
        {
            value = default;

            if (array == null)
            {
                return false;
            }

            if (!GetLevel(ref level, array.Length, toLastLevel))
            {
                return false;
            }

            value = array[level];
            return true;
        }

        /// <summary>
        /// 得到等级相关的值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="level"></param>
        /// <param name="value">返回值</param>
        /// <param name="toLastLevel">如果level>=array.Length, level = array.Length - 1</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool GetLevelValue<T>(T[][] array, int level, out T[] value, bool toLastLevel = true)
        {
            value = default;

            if (array == null)
            {
                return false;
            }

            if (!GetLevel(ref level, array.Length, toLastLevel))
            {
                return false;
            }

            value = array[level];
            return true;
        }

        #endregion
    }
}