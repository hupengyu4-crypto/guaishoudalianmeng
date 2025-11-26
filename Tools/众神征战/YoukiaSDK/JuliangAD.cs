using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukiaSDKSpace;
using WeChatWASM;


namespace YoukiaSDKSpace
{
    static class JuliangAD
    {
        private static string url = YoukiaSDKConfig.JLAD.URL;
        public static string CLICK_ID = "";
        public class ActonType
        {
            public static string INIT = "active";
            public static string REGISTER = "active_register";
            public static string PAY = "active_pay";
            public static string LOGIN = "login";

        }
        private static long GetTimeStamp()
        {
            return (System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        public static void DoAction(string type, string osPlatform, Dictionary<string, object> pay = null)
        {


            if (!YoukiaSDKConfig.JLAD.ENABLED)
            {
                return;
            }
            if (String.IsNullOrEmpty(CLICK_ID))
            {
                Debug.Log("jlad click_id is null");
                return;
            }
            /** '{
    "event_type": "active",
    "context": {
        "ad": {
            "callback": "EJiw267wvfQCGKf2g74ZIPD89-vIATAMOAFCIjIwMTkxMTI3MTQxMTEzMDEwMDI2MDc3MjE1MTUwNTczNTBIAQ=="
        },
        "device": {
            "platform": "ios",
            "idfa": "FCD369C3-F622-44B8-AFDE-12065659F34B"
        }
    },
    "timestamp": 1659887308000,
    "attribute_key": "idfa"
    }'*/
            var post = new Dictionary<string, object>();
            post.Add("event_type", type);

            var context = new Dictionary<string, object>();
            var ad = new Dictionary<string, object>();
            ad.Add("callback", CLICK_ID);
            context.Add("ad", ad);
            var device = new Dictionary<string, object>();
            device.Add("platform", osPlatform);
            var key = "";
            if (osPlatform.Equals("ios"))
            {
                key = "idfa";
            }
            else
            {
                key = "imei";
            }
            device.Add(key,MD5Utils.MD5String(YoukiaSDK.getIntence().GetDeviceId()));
            context.Add("device", device);
            if (pay != null)
            {
                context.Add("properties", pay);
            }
            post.Add("context", context);
            post.Add("timestamp", GetTimeStamp());
            post.Add("attribute_key", key);
            var data = Json.Serialize(post);
            Debug.Log("jl url：" + url);
            Debug.Log("jl data:" + data);
            FastNetwork.Instance.DoPostJson(url, data, (success) =>
            {


                Debug.Log("jlad success:" + success.ToString());


            }, (err) =>
            {
                Debug.Log("jlad err:" + err.ToString());
            });
        }
    }
}