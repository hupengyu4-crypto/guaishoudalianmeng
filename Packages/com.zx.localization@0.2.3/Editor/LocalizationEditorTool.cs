using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
    public static class LocalizationEditorTool
    {
        //编辑器预制体默认语言选项
        public static string editorPrefabDefaultLanguage
        {
            get
            {
                return UnityEditor.EditorPrefs.GetString("Editor_PrefabDefaultLanguage", LanguageEnum.Cn.ToString()); 
            }
            set
            {
                UnityEditor.EditorPrefs.SetString("Editor_PrefabDefaultLanguage", value);
            }
        }

        [MenuItem("Assets/复制文件路径(加载使用的)")]
        static void CopyPath()
        {
            Object obj = Selection.activeObject;
            if (obj == null)
            {
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(obj);
            string loadPath = LocalizationUtility.AssetPathToLoadPath(assetPath);
            EditorGUIUtility.systemCopyBuffer = loadPath;
            Debug.Log("复制文件路径: " + loadPath);
        }
        #region 中文
        const string cnMenu = "Tools/多语言设置/预设默认语言-Cn";
        [MenuItem(cnMenu, false, 4444)]
        public static void SelectCn()
        {
            LanguageSpliteLoadInEditor = LanguageEnum.Cn;
            editorPrefabDefaultLanguage = LanguageEnum.Cn.ToString();
        }

        [MenuItem(cnMenu, true, 10)]
        public static bool SelectCnValidate()
        {
            Menu.SetChecked(cnMenu, LanguageSpliteLoadInEditor == LanguageEnum.Cn);
            return true;
        }
        #endregion
        #region 英文
        const string enMenu = "Tools/多语言设置/预设默认语言-En";
        [MenuItem(enMenu, false, 4444)]
        public static void SelectEn()
        {
            LanguageSpliteLoadInEditor = LanguageEnum.En;
            editorPrefabDefaultLanguage = LanguageEnum.En.ToString();
        }

        [MenuItem(enMenu, true, 10)]
        public static bool SelectEnValidate()
        {
            Menu.SetChecked(enMenu, LanguageSpliteLoadInEditor == LanguageEnum.En);
            return true;
        }
        #endregion
        #region 繁体
        const string twMenu = "Tools/多语言设置/预设默认语言-Tw";
        [MenuItem(twMenu, false, 4444)]
        public static void SelectTw()
        {
            LanguageSpliteLoadInEditor = LanguageEnum.Tw;
            editorPrefabDefaultLanguage = LanguageEnum.Tw.ToString();
        }

        [MenuItem(twMenu, true, 10)]
        public static bool SelectTwValidate()
        {
            Menu.SetChecked(twMenu, LanguageSpliteLoadInEditor == LanguageEnum.Tw);
            return true;
        }
        #endregion
        //设置及本地缓存
        static LanguageEnum m_LanguageSpliteInEditor = LanguageEnum.Cn;
        const string languageSpliteLoadInEditor = "LanguageSpliteLoadInEditor";
        internal static LanguageEnum LanguageSpliteLoadInEditor
        {
            get
            {
                if (m_LanguageSpliteInEditor == LanguageEnum.Cn)
                    m_LanguageSpliteInEditor = (LanguageEnum)EditorPrefs.GetInt(languageSpliteLoadInEditor, (int)m_LanguageSpliteInEditor);

                return m_LanguageSpliteInEditor;
            }
            set
            {
                LanguageEnum newValue = value;
                if (newValue != m_LanguageSpliteInEditor)
                {
                    m_LanguageSpliteInEditor = newValue;
                    EditorPrefs.SetInt(languageSpliteLoadInEditor, (int)value);
                }
            }
        }

    }
}
