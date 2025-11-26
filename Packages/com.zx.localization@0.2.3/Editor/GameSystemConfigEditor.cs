using Engine.Unity.GUIExtensions;
using UnityEditor;

[CustomEditor(typeof(GameSystemConfig))]
public class GameSystemConfigInspector : XEditor
{
    [MenuItem("Tools/创建GameSystemConfig")]
    private static void CreateAsset()
    {
        if (AssetDatabase.LoadAssetAtPath<GameSystemConfig>(SavePath) == null)
        {
            AssetDatabase.CreateAsset(CreateInstance<GameSystemConfig>(), SavePath);
        }
    }
    /// <summary>
    /// 配置保存路径
    /// </summary>
    public static string SavePath = "Assets/Resources/Launcher/GameSystemConfig.asset";
}

