namespace YoukiaSDKSpace
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SDK : MonoBehaviour
    {

        // Start is called before the first frame update
        void Awake()
        {
            YoukiaSDKCore.Init();
        }


        public delegate void LoginCallback(string[] args);

        void jsCall(string args)
        {

        }

        public void Login()
        {

            YoukiaSDKCore.Call("set_callback", null, (err, args) =>
            {
                jsCall(args);
            });

            YoukiaSDKCore.Call("wx_login", "jSONstring", (err, args) =>
            {
                if (err != null)
                {
                    Debug.LogError("login error " + err);
                }
                else
                {
                    Debug.Log("login success:" + args);
                }
            });

            YoukiaSDKCore.Call("wx_login", "jSONstring", (err, args) =>
            {
                if (err != null)
                {
                    Debug.LogError("login error " + err);
                }
                else
                {
                    Debug.Log("login success:" + args);
                }
            });


            YoukiaSDKCore.Call("wx_login1", "jSONstring", (err, args) =>
            {
                if (err != null)
                {
                    Debug.LogError("login error " + err);
                }
                else
                {
                    Debug.Log("login success:" + args);
                }
            });
        }

    }

}