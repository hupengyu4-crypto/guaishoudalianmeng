using UnityEngine;
using Engine.Unity;
using XUnityCore;
using UnityEngine.Scripting;
using System;
using System.Collections.Generic;

namespace Localization
{
    [Preserve]
    public enum LanguageEnum
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        [InspectorName("简体中文")]
        Cn = SystemLanguage.ChineseSimplified,
        /// <summary>
        /// 繁体中文
        /// </summary>
        [InspectorName("繁体中文")]
        Tw = SystemLanguage.ChineseTraditional,
        /// <summary>
        /// 英语
        /// </summary>
        [InspectorName("英语")]
        En = SystemLanguage.English,
        /// <summary>
        /// 韩语
        /// </summary>
        [InspectorName("韩语")]
        Ko = SystemLanguage.Korean,
        /// <summary>
        /// 印度尼西亚语
        /// </summary>
        [InspectorName("印度尼西亚语")]
        Id = SystemLanguage.Indonesian,
        /// <summary>
        /// 日语
        /// </summary>
        [InspectorName("日语")]
        Ja = SystemLanguage.Japanese,
        /// <summary>
        /// 越南语
        /// </summary>
        [InspectorName("越南语")]
        Vi = SystemLanguage.Vietnamese,
        /// <summary>
        /// 泰语
        /// </summary>
        [InspectorName("泰语")]
        Th = SystemLanguage.Thai,

        /// <summary>
        /// 南非荷兰语
        /// </summary>
        [InspectorName("南非荷兰语")]
        Af = SystemLanguage.Afrikaans,

        /// <summary>
        /// 阿拉伯语
        /// </summary>
        [InspectorName("阿拉伯语")]
        Ar = SystemLanguage.Arabic,

        /// <summary>
        /// 巴斯克语（西班牙地区）
        /// </summary>
        [InspectorName("巴斯克语（西班牙地区）")]
        Eu = SystemLanguage.Basque,

        /// <summary>
        /// 白俄罗斯语
        /// </summary>
        [InspectorName("白俄罗斯语")]
        Be = SystemLanguage.Belarusian,

        /// <summary>
        /// 保加利亚语
        /// </summary>
        [InspectorName("保加利亚语")]
        Bg = SystemLanguage.Bulgarian,

        /// <summary>
        /// 加泰罗尼亚语
        /// </summary>
        [InspectorName("加泰罗尼亚语")]
        Ca = SystemLanguage.Catalan,

        /// <summary>
        /// 捷克语
        /// </summary>
        [InspectorName("捷克语")]
        Cs = SystemLanguage.Czech,

        /// <summary>
        /// 丹麦语
        /// </summary>
        [InspectorName("丹麦语")]
        Da = SystemLanguage.Danish,

        /// <summary>
        /// 荷兰语
        /// </summary>
        [InspectorName("荷兰语")]
        Nl = SystemLanguage.Dutch,

        /// <summary>
        /// 爱沙尼亚语
        /// </summary>
        [InspectorName("爱沙尼亚语")]
        Et = SystemLanguage.Dutch,

        /// <summary>
        /// 法罗语（北欧地区）
        /// </summary>
        [InspectorName("法罗语（北欧地区）")]
        Fo = SystemLanguage.Faroese,

        /// <summary>
        /// 芬兰语
        /// </summary>
        [InspectorName("芬兰语")]
        Fi = SystemLanguage.Finnish,

        /// <summary>
        /// 法语
        /// </summary>
        [InspectorName("法语")]
        Fr = SystemLanguage.French,

        /// <summary>
        /// 德语
        /// </summary>
        [InspectorName("德语")]
        De = SystemLanguage.German,

        /// <summary>
        /// 希腊语
        /// </summary>
        [InspectorName("希腊语")]
        El = SystemLanguage.Greek,

        /// <summary>
        /// 希伯来语
        /// </summary>
        [InspectorName("希腊语")]
        He = SystemLanguage.Hebrew,

        /// <summary>
        /// 匈牙利语
        /// </summary>
        [InspectorName("匈牙利语")]
        Hu = SystemLanguage.Hungarian,

        /// <summary>
        /// 冰岛语
        /// </summary>
        [InspectorName("冰岛语")]
        Is = SystemLanguage.Icelandic,

        /// <summary>
        /// 意大利语
        /// </summary>
        [InspectorName("意大利语")]
        It = SystemLanguage.Italian,


        /// <summary>
        /// 拉脱维亚语
        /// </summary>
        [InspectorName("拉脱维亚语")]
        Lv = SystemLanguage.Latvian,

        /// <summary>
        /// 立陶宛语
        /// </summary>
        [InspectorName("立陶宛语")]
        Lt = SystemLanguage.Lithuanian,

        /// <summary>
        /// 挪威语
        /// </summary>
        [InspectorName("挪威语")]
        No = SystemLanguage.Norwegian,

        /// <summary>
        /// 波兰语
        /// </summary>
        [InspectorName("波兰语")]
        Pl = SystemLanguage.Polish,


        /// <summary>
        /// 葡萄牙语
        /// </summary>
        [InspectorName("葡萄牙语")]
        Pt = SystemLanguage.Portuguese,


        /// <summary>
        /// 罗马尼亚语
        /// </summary>
        [InspectorName("罗马尼亚语")]
        Ro = SystemLanguage.Romanian,

        /// <summary>
        /// 俄语
        /// </summary>
        [InspectorName("俄语")]
        Ru = SystemLanguage.Russian,


        /// <summary>
        /// 塞尔维亚-克罗地亚语
        /// </summary>
        [InspectorName("塞尔维亚-克罗地亚语")]
        Sh = SystemLanguage.SerboCroatian,


        /// <summary>
        /// 斯洛伐克语
        /// </summary>
        [InspectorName("斯洛伐克语")]
        Sk = SystemLanguage.Slovak,

        /// <summary>
        /// 斯洛文尼亚语
        /// </summary>
        [InspectorName("斯洛文尼亚语")]
        Sl = SystemLanguage.Slovenian,

        /// <summary>
        /// 西班牙语
        /// </summary>s
        [InspectorName("西班牙语")]
        Es = SystemLanguage.Spanish,


        /// <summary>
        /// 瑞典语
        /// </summary>
        [InspectorName("瑞典语")]
        Sv = SystemLanguage.Swedish,

        /// <summary>
        /// 土耳其语
        /// </summary>
        [InspectorName("土耳其语")]
        Tr = SystemLanguage.Turkish,

        // <summary>
        /// 乌克兰语
        /// </summary>
        [InspectorName("乌克兰语")]
        Uk = SystemLanguage.Ukrainian,

    }


    [Preserve]
    public static class LocalizationUtility
    {
        #region Properties

        //语言标识
        public const string LANAGUAGE_TAGE = "#L";
        [NonSerialized]
        //保存语言标识符
        public const string LANAGUAGE_SAVE_FLAG = "SettingLanguageKey";



        //当前地区
        public static string currectCountry = "Cn";
        //当前语言表信息
        public static MultiLanguageConfig currectMultiLanguageConfig = null;
        //当前语言
        public static LanguageEnum currectLanguage = LanguageEnum.Cn;
        //默认语言，系统语言在支持语言列表中无法找到时
        public static LanguageEnum backupLanguage = LanguageEnum.Cn;
        /// <summary>
        /// 本地化根目录
        /// </summary>
        public static string EditorLocationRootDir = "Assets/Resources/Localization";
        /// <summary>
        /// 本地化根目录
        /// </summary>
        public static string RuntimeLocationRootDir = "Localization";
        /// <summary>
        /// 替换语言路径
        /// </summary>
        private static string replaceLanguagePath;
        #endregion



        #region Public Methods
        /// <summary>
        /// 获取本地默认语言
        /// </summary>
        /// <param name="multiConfig"></param>
        /// <returns></returns>
        public static LanguageEnum GetCurrectLanguageEnum(string country, List<MultiLanguageConfig> multiConfig)
        {
            // 查找对应发行或者国家配置
            currectCountry = country;
            currectMultiLanguageConfig = GetMultiLanguageConfig(country, multiConfig);
            if (currectMultiLanguageConfig == null)
            {
                return currectLanguage;
            }

            //检查获取语言
            int language = GetPrefsLanguage();
            if (language < 0)
            {
                language = GetMultiLangDefaultLang(currectMultiLanguageConfig, (int)Application.systemLanguage, currectMultiLanguageConfig.backupLanguage);
            }
            else
            {
                language = CheckMultiLangConfig(language, currectMultiLanguageConfig);
            }
            if (language < 0)
            {
                Debug.LogError("未找到语言配置，中断运行 " + language);
                throw new ArgumentNullException("未找到语言配置，中断运行", language.ToString());
            }

            backupLanguage = currectMultiLanguageConfig.backupLanguage;
            currectLanguage = (LanguageEnum)language;
            // 存储
            SavePrefsLanguage(language);


            Debug.LogWarning("当前语言为: " + currectLanguage.ToString());
            return currectLanguage;
        }

        /// <summary>
        /// 存储语言改变
        /// </summary>
        /// <param name="multiConfig"></param>
        /// <returns></returns>
        private static MultiLanguageConfig GetMultiLanguageConfig(string country, List<MultiLanguageConfig> multiConfig)
        {
            for (int i = 0; i < multiConfig.Count; i++)
            {
                var config = multiConfig[i];
                if (country == config.areaCountry)
                {

                    return config;
                }
            }
            return null;
        }


        /// <summary>
        /// 根据系统默认语言 找到相应配置
        /// </summary>
        /// <param name="multiConfig"></param>
        /// <param name="language"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static int GetMultiLangDefaultLang(MultiLanguageConfig multiConfig,int language, LanguageEnum defaulyKey)
        {
            // 先根据系统语言找是否有匹配的语言(中文这儿要处理下，unity里边枚举存在2中简体中文情况)
            //var system = Application.systemLanguage;
            int systemLanguage = -1;
            if (language == (int)SystemLanguage.Chinese)
            {
                systemLanguage = (int)SystemLanguage.ChineseSimplified;
            }
            else
            {
                systemLanguage = (int)language;
            }
            for (int i = 0; i < multiConfig.languages.Length; i++)
            {
                var _lang = (int)multiConfig.languages[i];
                if (systemLanguage == _lang)
                {
                    return _lang;
                }
            }
            // 否则返回地区默认语言
            return (int)defaulyKey;
        }


        private static int CheckMultiLangConfig(int lang, MultiLanguageConfig multiConfig)
        {
            for (int i = 0; i < multiConfig.languages.Length; i++)
            {
                var _lang = multiConfig.languages[i];
                if (lang == (int)_lang)
                {
                    return lang;
                }
            }
            Debug.LogError("无法检测到MultiLanguageConfig对应语言配置: " + ((LanguageEnum)lang).ToString());
            return -1;
        }


        ///// <summary>
        ///// 获取当前国家多语言列表
        ///// </summary>
        ///// <param name="multiConfig"></param>
        ///// <returns></returns>
        //public static List<string> GetMultiLanguageList()
        //{
        //    if (currectMultiLanguageConfig == null)
        //    {
        //        return new List<string>();
        //    }

        //    List<string> languages_list = new List<string>();
        //    var languages_arry = currectMultiLanguageConfig.Languages;
        //    for (int i = 0; i < languages_arry.Length; i++)
        //    {
        //        languages_list.Add((languages_arry[i].ToString()));
        //    }
        //    return languages_list;
        //}


        /// <summary>
        /// 获取语言类型
        /// </summary>
        /// <returns></returns>
        public static int GetPrefsLanguage()
        {
            return PlayerPrefs.GetInt(currectCountry + "." + LANAGUAGE_SAVE_FLAG, -1);
        }

        /// <summary>
        /// 存储语言类型
        /// </summary>
        /// <returns></returns>
        public static LanguageEnum SavePrefsLanguage(int language)
        {
            var lang = GetMultiLangDefaultLang(currectMultiLanguageConfig, language, currectMultiLanguageConfig.backupLanguage);
            //设置语言替换路径
            replaceLanguagePath = RuntimeLocationRootDir + "/" + currectLanguage.ToString().ToLower() + "/";
            PlayerPrefs.SetInt(currectCountry + "." + LANAGUAGE_SAVE_FLAG, lang);
            return (LanguageEnum)lang;
        }


        /// <summary>
        /// 替换资源加载
        /// </summary>
        public static void LoadReplaceRegist()
        {
            ResourceManager.Instance.GetAssetRawPath += assetPath =>
            {
                if (assetPath.StartsWith(LANAGUAGE_TAGE))
                {
                    return replaceLanguagePath + assetPath.Substring(LANAGUAGE_TAGE.Length + 1);
                }

                return assetPath;
            };
        }

        /// <summary>
        /// 资源地址转加载地址
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string AssetPathToLoadPath(string assetPath)
        {
            string loadAssetPath = string.Empty;
            assetPath = IOHelper.GetPathWithoutExt(assetPath);
            if (assetPath.StartsWith(EditorLocationRootDir))
            {
                loadAssetPath = assetPath.Substring(EditorLocationRootDir.Length + 1);
                int index = loadAssetPath.IndexOf('/');
                if (index > 0)
                {
                    return LANAGUAGE_TAGE + loadAssetPath.Substring(index);
                }

                return loadAssetPath;
            }
            else
            {
                loadAssetPath = assetPath.Replace("Assets/Resources/", "");
            }

            return loadAssetPath;
        }

        /// <summary>
        /// runtime时资源地址转加载地址
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string RuntimeAssetPathToLoadPath(string assetPath)
        {
            if (assetPath.StartsWith(RuntimeLocationRootDir))
            {
                string loadAssetPath = IOHelper.GetPathWithoutExt(assetPath.Substring(RuntimeLocationRootDir.Length + 1));
                int index = loadAssetPath.IndexOf('/');
                if (index > 0)
                {
                    return LANAGUAGE_TAGE + loadAssetPath.Substring(index);
                }

                return loadAssetPath;
            }

            return assetPath;
        }

        /// <summary>
        /// 加载地址转资源地址
        /// </summary>
        /// <param name="loadPath"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string LoadPathToAssetPath(string loadPath, string language = null)
        {
            if (loadPath.StartsWith(LANAGUAGE_TAGE))
            {
                string loadAssetPath;
                if (language == null)
                {
                    loadAssetPath = replaceLanguagePath + loadPath.Substring(LANAGUAGE_TAGE.Length + 1);
                }
                else
                {
                    loadAssetPath = RuntimeLocationRootDir + "/" + language + "/" + loadPath.Substring(LANAGUAGE_TAGE.Length + 1);
                }

                return loadAssetPath;
            }

            return loadPath;
        }

        public static T LoadAsset<T>(string assetPath, string language = null) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            if (assetPath.StartsWith(LANAGUAGE_TAGE))
            {
                string loadAssetPath;
                if (language == null)
                {
                    loadAssetPath = replaceLanguagePath + assetPath.Substring(LANAGUAGE_TAGE.Length + 1);
                }
                else
                {
                    loadAssetPath = RuntimeLocationRootDir + "/" + language + "/" + assetPath.Substring(LANAGUAGE_TAGE.Length + 1);
                }

                return Resources.Load<T>(loadAssetPath);
            }

            return null;
        }
        #endregion
    }
}