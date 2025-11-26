using Engine.Unity.Editor;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
    [AssetImportCategory(".prefab")]
    public class LocalizationPrefabModifier : AssetImportProcessor
    {
        public override void OnPostProcess(AssetImporter importer)
        {
            base.OnPostProcess(importer);
            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(importer.assetPath);
            if (gameObject == null)
            {
                return;
            }
            LocalizationComponent[] localization = gameObject.GetComponentsInChildren<LocalizationComponent>(true);
            if (localization.Length > 0)
            {
                var isChange = ClearLocalizationTempData(gameObject);
                if (isChange)
                {
                    PrefabUtility.SavePrefabAsset(gameObject);
                    //EditorUtility.SetDirty(gameObject);
                    //AssetDatabase.Refresh();

                    // 保存完毕后, 内容可能会应用到视口中, 让视口的对象还原下
                    var go = GameObject.Find(gameObject.name);
                    if (go != null)
                    {
                        EditorApplication.delayCall += () =>
                        {
                            go = GameObject.Find(gameObject.name);
                            if (go != null)
                            {
                                ShowLocalizationTempData(go);
                            }
                        };
                    }
                }
            }
        }

        public static bool ClearLocalizationTempData(GameObject gameObject)
        {
            LocalizationComponent[] localization = gameObject.GetComponentsInChildren<LocalizationComponent>(true);
            bool flag = false;
            foreach (var item in localization)
            {
                if (item.Target == null)
                {
                    item.GetType().GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, null);
                    if (item.Target != null)
                    {
                        flag = true;
                    }
                }
                flag |= item.ClearDisplay();
            }
            return flag;
        }

        // 语言翻译.显示临时数据
        public static void ShowLocalizationTempData(GameObject gameObject)
        {
            LocalizationComponent[] localization = gameObject.GetComponentsInChildren<LocalizationComponent>(true);
            foreach (var item in localization)
            {
                item.Refresh();
            }
        }
    }
}