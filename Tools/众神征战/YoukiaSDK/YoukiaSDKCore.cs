namespace YoukiaSDKSpace
{
    using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
    using System.Runtime.InteropServices;
#endif

    using LitJson;

    using System.Collections.Generic;
    using System;
    using WeChatWASM;
    public class YoukiaSDKCoreProxy : MonoBehaviour
    {


        public delegate void JSCallback(string error, string args);


        public class Task
        {
            public Task(JSCallback c)
            {
                callback = c;
            }

            private JSCallback callback;

            public void call(string error, string args)
            {
                callback(error, args);
            }
        }


        public static JSCallback globalCallbacl;




        private void ONJSCallback(int id, string error, string args)
        {
            if (callbackList.ContainsKey(id))
            {
                var callback = callbackList[id];
                callbackList.Remove(id);
                try
                {
                    callback.call(error, args);
                }
                catch
                {

                }
            }

        }


        // js 普通调用
        public void OnJsMessage(string args)
        {
            if (globalCallbacl != null)
            {
                globalCallbacl(null, args);
            }

        }

        public void OnJsBackMessage(string msg)
        {
            YoukiaSDK.getIntence().rewardedResult(msg);
        }
        // callback 调用

        public void OnJsCallbackMessage(string msg)
        {
            //Debug.Log("YKSDK_CS\t\trecv js sent message: " + msg);
            var id = 0;
            try
            {
                JsonData json = JsonMapper.ToObject(msg);
                
             
                
                id = (int)json["id"];
                string err = null;
                try
                {
                    err = (string)json["err"];
                }
                catch (Exception e)
                {

                }
                string args = null;
                try
                {
                    args = (string)json["args"];

                }
                catch (Exception e)
                {

                }

                ONJSCallback(id, err, args);
            }
            catch (Exception e)
            {
                Debug.LogError("YKSDK_CS\t\t" + e.ToString());
                if (id > 0)
                {
                    ONJSCallback(id, e.Message, null);
                }
            }
        }



        static int callbackIDBase = 0;

        static private Dictionary<int, Task> callbackList = new Dictionary<int, Task>();




        public static int PushCallback(JSCallback callback)
        {
            callbackIDBase++;
            int id = callbackIDBase;
            callbackList.Add(id, new Task(callback));
            return id;
        }




    }


    public class YoukiaSDKCore
    {



        public static void Init()
        {
           
            var gameObject = new GameObject();
            //gameObject.AddComponent<Transform>();
            gameObject.name = "YKSDK_MainGameObject";
            gameObject.AddComponent<YoukiaSDKCoreProxy>();
            GameObject.DontDestroyOnLoad(gameObject);
           // YKSDK_init("{v: 2013 ...}");
        }



#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string YKSDK_Call_JS(string name, string args, int callbackID);
#else
        private static string YKSDK_Call_JS(string name, string args, int callbackID)
        {
            Debug.LogWarning("YKSDK_CS\t\tcall YKSDK_Call_JS on native " + name + ", " + args + ", " + callbackID);
            return null;
        }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void YKSDK_init(string options);
#else
        private static void YKSDK_init(string options)
        {
            Debug.LogWarning("NoMinigame\tYKSDK_CS\t\tcall YKSDK_init" + options);
        }
#endif


        public static string Call(string name, string args)
        {

            //Debug.Log("YKSDK_CS\t\tready to calling js function: " + name + ", args: " + args);
            return YKSDK_Call_JS(name, args, 0);
        }

        public static string Call(string name, string args, YoukiaSDKCoreProxy.JSCallback callback)
        {

            var callbackID = YoukiaSDKCoreProxy.PushCallback(callback);
            //Debug.Log("ready to calling js function:" + name + "\n\targs: " + args + "\n\tcallbackID: " + callbackID);
            return YKSDK_Call_JS(name, args, callbackID);
        }



    }

}