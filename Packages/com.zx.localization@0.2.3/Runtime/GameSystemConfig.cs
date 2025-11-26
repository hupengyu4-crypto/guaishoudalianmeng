using Localization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemConfig : ScriptableObject
{
    //[Header("AB加密偏移，修改要重新出包")][SerializeField]
    //public int BinaryOffset = 5;
    //运行时使用
    [SerializeField]
    public List<MultiLanguageConfig> MultiConfig = new List<MultiLanguageConfig>();

    //编辑器使用
    [SerializeField]
    public List<MultiLanguageConfig> EditorMultiConfig = new List<MultiLanguageConfig>();

//#if UNITY_EDITOR
//    /// <summary>
//    /// 渠道配置和配置路径映射
//    /// </summary>
//    [SerializeField]
//    public List<LocalizationCountryConfigPathPair> LocalizationCountryConfigPathPairs = new List<LocalizationCountryConfigPathPair>();
//#endif


    //[SerializeField]
    //public List<LanguageLogoPackage> LanguageLogoList = new List<LanguageLogoPackage>();


    ///// <summary>
    ///// logo图标
    ///// </summary>
    //static public string logoicon;

    // 数据
    public static GameSystemConfig Config;

    public static void LoadGamesystemconfig()
    {
        if (Config == null)
        {
            string loadPath = "Launcher/GameSystemConfig";
            Config = Resources.Load(loadPath) as GameSystemConfig;
        }
    }

    //public static void GetLogoPath(string packageName, LanguageEnum language)
    //{
    //    foreach (var item in Config.LanguageLogoList)
    //    {
    //        if (item.key == packageName)
    //        {
    //            foreach (var logo in item.logoList)
    //            {
    //                if (logo.language == language)
    //                {
    //                    logoicon = logo.logoPath;
    //                }
    //            }
    //            logoicon = item.defaultLogoIconPath;
    //        }
    //    }
    //}

  //[Serializable]
  //  public class LanguageLogoPackage
  //  {
  //      /// <summary>
  //      /// 唯一标识
  //      /// </summary>
  //      public string key;

  //      /// <summary>
  //      /// 默认logo图标路径
  //      /// </summary>
  //      [SerializeField]
  //      public string defaultLogoIconPath;

  //      /// <summary>
  //      /// 语言和对应的logo路径
  //      /// </summary>
  //      public LogoSimple[] logoList;
  //  }

  //  [Serializable]
  //  public class LogoSimple
  //  {
  //      /// <summary>
  //      /// 语言
  //      /// </summary>
  //      public LanguageEnum language;

  //      public string logoPath;
  //  }
}
[Serializable]
public class MultiLanguageConfig
{
    // 静态委托用于外部注入选项逻辑
    public static Func<string[]> OptionsProvider;

    // 静态方法供 StringDropdown 调用
    private static string[] GetOptions()
    {
        return OptionsProvider?.Invoke() ?? new[] { "OptionA", "OptionB", "OptionC" };
    }

    /// <summary>
    /// 地区
    /// </summary>
    [StringDropdown(typeof(MultiLanguageConfig), "GetOptions")]
    public string areaCountry;
    /// <summary>
    /// 备用语言，系统语言在支持语言列表中无法找到时
    /// </summary>
    public LanguageEnum backupLanguage = LanguageEnum.En;
    /// <summary>
    /// 语言
    /// </summary>
    public LanguageEnum[] languages;
}
//#if UNITY_EDITOR
//[Serializable]
//public class LocalizationCountryConfigPathPair
//{
//    public eLocalizationCountryEnum AreaCountry;
//    public string ConfigPath;
//}
//#endif
