using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using UnityEngine;

[assembly: Preserve]

namespace AlipaySdk.Bridge
{
    public class AlipayWebGLInterface
    {
#if UNITY_WEBPLAYER || UNITY_WEBGL
        // 以下接口为 Web 使用，用于调用 JS 代码
        [method: Preserve]
        [DllImport("__Internal", EntryPoint = "unityCallJs")]
        public static extern void unityCallJs(string eventId, string paramJson);

        [DllImport("__Internal", EntryPoint = "unityCallJsSync")]
        public static extern string unityCallJsSync(string eventId, string paramJson);

        [DllImport("__Internal", EntryPoint = "GetAlipayEnv")]
        public static extern string GetAlipayEnv();

        [DllImport("__Internal", EntryPoint = "AlipayWriteBinFileSync")]
        public static extern string WriteBinFileSync(string fileNamePtr, byte[] dataPtr, int dataLength);

        [DllImport("__Internal", EntryPoint = "AlipayReadBinFileSync")]
        public static extern IntPtr ReadBinFileSync(string fileNamePtr);

        [DllImport("__Internal", EntryPoint = "AlipayWriteFileSync")]
        public static extern string WriteFileSync(string fileName, string data, string encoding);

        [DllImport("__Internal", EntryPoint = "AlipayReadFileSync")]
        public static extern string ReadFileSync(string fileName, string encoding);

        [DllImport("__Internal", EntryPoint = "AlipayWriteBinFile")]
        public static extern string WriteBinFile(string fileNamePtr, byte[] dataPtr, int dataLength, string callbackIDPtr);

        [DllImport("__Internal", EntryPoint = "AlipayReadBinFile")]
        public static extern void ReadBinFile(string fileNamePtr, string callbackIDPtr);

        [DllImport("__Internal")]
        public static extern void _free(IntPtr ptr);

        [DllImport("__Internal", EntryPoint = "GetFSStatsSync")]
        public static extern IntPtr GetFSStatsSync(string path, bool recursive);

        [DllImport("__Internal", EntryPoint = "StatsIsDirectory")]
        public static extern int StatsIsDirectory(string path, bool recursive);

        [DllImport("__Internal", EntryPoint = "StatsIsFile")]
        public static extern int StatsIsFile(string path, bool recursive);

#else
        public static void unityCallJs(string eventId, string paramJson)
        {
            Debug.LogError("message dropped, please check platform");
        } 
        public static string unityCallJsSync(string eventId, string paramJson)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 
        public static string GetAlipayEnv()
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 
        public static string WriteBinFileSync(string fileNamePtr, byte[] dataPtr, int dataLength)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 

        public static IntPtr ReadBinFileSync(string fileNamePtr)
        {
            Debug.LogError("message dropped, please check platform");
            return IntPtr.Zero;
        } 
      
        public static string WriteFileSync(string fileName, string data, string encoding)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 
        public static string ReadFileSync(string fileName, string encoding)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        }

        public static void _free(IntPtr ptr)
        {
            Debug.LogError("message dropped, please check platform");
        } 

        public static string WriteBinFile(string fileNamePtr, byte[] dataPtr, int dataLength, string callbackID)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 

        public static void ReadBinFile(string fileNamePtr, string callbackID)
        {
            Debug.LogError("message dropped, please check platform");
        } 

        public static IntPtr GetFSStatsSync(string path, bool recursive)
        {
            Debug.LogError("message dropped, please check platform");
            return IntPtr.Zero;
        } 

        public static int StatsIsDirectory(string path, bool recursive)
        {
            Debug.LogError("message dropped, please check platform");
            return 0;
        } 

        public static int StatsIsFile(string path, bool recursive)
        {
            Debug.LogError("message dropped, please check platform");
            return 0;
        }
#endif
    }
}
