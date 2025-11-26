#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace YoukiaSDKSpace
{
    /// <summary>
    /// 监控资源导入
    /// </summary>
// #if MONITOR_ASSET
    public class SDKTools :  IPostprocessBuildWithReport, IPreprocessBuildWithReport
    {
        public int callbackOrder
        {
            get { return 10; }
        }

        /**
        * build 构建完成后回调
        */
        public void OnPostprocessBuild(BuildReport report)
        {
           
        }

        /**
        * build 构建开始前回调
        */
        public void OnPreprocessBuild(BuildReport report)
        {
            YoukiaSDKConfig.Gen();
        }


      
    }
}
#endif