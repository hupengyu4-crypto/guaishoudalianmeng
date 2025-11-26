using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks; 
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class AlipayEditorWindow : EditorWindow
{
    private static AlipayEditorWindow sInstance = null;
    public static AlipayEditorWindow Instance { get { return sInstance; } }
    private static AlipayBuildConfig alipayBuildConfig;
    private Vector2 scrollRoot;

    public AlipayEditorWindow()
    {
        sInstance = this;
    }

    [MenuItem("支付宝小游戏 / WebGL打包工具", false, 1)]
    public static void OpenAlipayEditor()
    {
        var window = GetWindow(typeof(AlipayEditorWindow), false, "支付宝WebGL打包工具");
        window.minSize = new Vector2(500, 700);

        Vector2 windowSize = window.minSize;
        Vector2 screenCenter = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) / 2;
        Rect position = new Rect(screenCenter.x - windowSize.x / 2,
                                  screenCenter.y - windowSize.y / 2,
                                  windowSize.x,
                                  windowSize.y);
        window.position = position;
        window.Show();
    }

    public void OnEnable()
    {
        sInstance = this;
        alipayBuildConfig = AlipayUtil.GetAlipayBuildConfig();
        AlipaySDKUpdate.CheckSDKUpdate();
    }
 
    public static AlipayBuildConfig GetEditorConfig()
    {
        return alipayBuildConfig;
    }

    public void OnGUI()
    {
        scrollRoot = EditorGUILayout.BeginScrollView(scrollRoot);
        {
            GUILayout.Space(ToolInfo.spaceHeight);
            BaseOption.RenderGUI();
            GUILayout.Space(ToolInfo.spaceHeight);

            BuildOption.RenderGUI();
            GUILayout.Space(ToolInfo.spaceHeight);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("生成并转换"), GUILayout.Width(100), GUILayout.Height(25)))
                {
                    if (AlipayConvertCore.WebglBuildAndConvert())
                    {
                        ShowNotification(new GUIContent("转换完成"));
                    }
                    GUIUtility.ExitGUI();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

}