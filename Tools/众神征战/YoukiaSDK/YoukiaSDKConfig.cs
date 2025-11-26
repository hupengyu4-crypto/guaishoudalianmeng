using System;
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
#endif


namespace YoukiaSDKSpace
{
    public class YoukiaSDKConfig
    {

        /**
        * 挂接平台
        */
        public const String PLATFORM = "1263";
        /**
        * 挂接中心 url
        */
       

        public const String LOGINTHREE_URL = "https://zs-center.gzmywl.cn/sdk.php/p1263/LoginThree/o_system/android";//正式地址
        /**
        虚拟支付 offid(米大师 id)
        */
        public const String WX_OFFER_ID = "1450053510";


        /**
        * 代币与人民币的比例
        */
        public const float PAY_RATE = 10;

        /**
        ios 支付 app_id
        */
        public const String IOS_PAY_APP_ID = "1";
        /**
        ios 支付创建订单 url
        */
        public const String IOS_PAY_URL = "https://webpay.cdsysz.com/payment/order/pre_order";
        // /**
        // * 老 bi
        // */
        // public const string stepinfourl = "https://jhh5bzti.youkia.net";


        /**
        * 游戏版本号 
        */
        public static string GAME_VERSION = "3.4.0";



        /**
        * 微信 AppId
        */
        public const string WX_APP_ID = "wx51abd3071918ea98";


        // 数数配置
        public struct SS
        {
            public const string SERVER = "https://ta.youkia.net";
            public const string APPID = "0fc5f70482c345e8b7052015aab12733";
        }

        // 热云
        public class Reyun
        {
            public static  bool ENABLE =true;
            public static string APPKEY = "7c18641cf3ec966f24eb1ffe9566bfec";
            public static bool CUSTOM_EVENT = false;
        }



        // 腾讯广告 product 
       
        public class TXAD {
            public static bool ENABLED = false;
            public static string URL = "https://api.e.qq.com/v1.3/";
            public static string TOKEN = "5c0aa63d35d5455a9b6ed9f547d70f9d";
            public static string ACCOUNT = "31843848";
            public static int ACTION_SET_ID = 1201347442;
        }


        public class JLAD {
            public static bool ENABLED = false;
            public static string URL = "https://analytics.oceanengine.com/api/v2/conversion";
            public static string APPID = "1768641642503181";
            public static string SECRET = "69bd5543614112eb7e2de475d6a66354c2a3ff6e";

        }

#if UNITY_EDITOR

        private const string WX_DEFAULT_CODE_PATH = "Assets/WX-WASM-SDK-V2/Runtime/wechat-default";
        public static void Gen()
        {
            Debug.Log("YoukiaSDKConfig\t自动生成小游戏 js 配置文件");

            // 数数
            var cfg = new Dictionary<string, object>{
                {"ss", new Dictionary<string, object>{
                    {"server", SS.SERVER},
                    {"appId", SS.APPID}
                }},
                {"reyun", new Dictionary<string, object>{
                    {"enable", Reyun.ENABLE},
                    {"appKey", Reyun.APPKEY},
                    {"customEvent", Reyun.CUSTOM_EVENT}
                }},
                {"youkia",new Dictionary<string,object>
                    {
                        {"version",GAME_VERSION},
                        {"LOGINTHREE_URL",LOGINTHREE_URL},
                    }
                },
               
            };
            

            var file = WX_DEFAULT_CODE_PATH + "/yksdk/config.js";
            File.WriteAllText(file,
            "// unity3d自动生成，请勿修改\n// 请在 unity3d 资源中 修改 YoukiaSDKConfig.cs\n\nexport default " + Json.Serialize(cfg), Encoding.UTF8);

            Debug.Log("YoukiaSDKConfig\t生成配置文件: " + file);
        }
#endif
    }

}