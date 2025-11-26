using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildOption
{
    private static bool showBuildLayer = true;
    static CompileOptions CompileOptions = AlipayEditorWindow.GetEditorConfig().CompileOptions;
    public static void RenderGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("打包调试", ToolInfo.LabelStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(ToolInfo.groupSpaceHeight);

        if (showBuildLayer)
        {
            GUILayout.BeginVertical("frameBox");
            GUILayout.Space(ToolInfo.groupSpaceHeight);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Development Build"), GUILayout.Width(140));
            CompileOptions.DevelopBuild = EditorGUILayout.Toggle(CompileOptions.DevelopBuild);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Auto Profile"), GUILayout.Width(140));
            CompileOptions.AutoProfile = EditorGUILayout.Toggle(CompileOptions.AutoProfile);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Script Only Build"), GUILayout.Width(140));
            CompileOptions.ScriptOnly = EditorGUILayout.Toggle(CompileOptions.ScriptOnly);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Il2Cpp Optimize Size"), GUILayout.Width(140));
            CompileOptions.Il2CppOptimizeSize = EditorGUILayout.Toggle(CompileOptions.Il2CppOptimizeSize);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Profiling Funcs"), GUILayout.Width(140));
            CompileOptions.profilingFuncs = EditorGUILayout.Toggle(CompileOptions.profilingFuncs);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Profiling Memory"), GUILayout.Width(140));
            CompileOptions.ProfilingMemory = EditorGUILayout.Toggle(CompileOptions.ProfilingMemory);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Use StreamingAssets"), GUILayout.Width(140));
            CompileOptions.UseStreamingAssets = EditorGUILayout.Toggle(CompileOptions.UseStreamingAssets);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

    }


}
