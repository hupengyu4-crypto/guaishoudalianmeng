using System;
using System.Collections.Generic;
using System.Threading;
using WeChatWASM;
using UnityEngine;

namespace YoukiaSDKSpace
{
    class PayInfo
    {
        // 游戏订单号 由游戏服务器生成（透传）
        public String cpOrderId;

        // public int buyQuantity;
        // 商品id   
        public String productId;

        // 扩展参数 （透传）
        public String extra;
    }

    class YoukiaSDK
    {
        private static YoukiaSDK youkiaSDK;

        public delegate void Callback(int code, String msg);

        public static int CODE_SUCCESS = 1;
        public static int CODE_FAILED = 0;
        private string user_id = "";
        private string open_id = "";
        private string userinfo_url = "";
        private string serverlist_url = "";
        private string serverlist_status_url = "";
        private string login_url = "";
        private string createorder_url = "";
        private string payback_url = "";
        private string serverId = "";
        private string maintenanceMess = "";
        private string deviceid = "";
        private List<object> goodlist;

        private List<object> goodlist_pro;

        //广告来源
        private string from = null;

        // 腾讯广告
        private string gdt_vid = null;

        //巨量广告
        private string clickid = null;


        public static YoukiaSDK getIntence()
        {
            Debug.Log("YOUKIASDK getIntence");
            if (youkiaSDK == null)
            {
                youkiaSDK = new YoukiaSDK();
            }

            return youkiaSDK;
        }


        private Callback onLogin = null;
        private bool fistLogin = false;

        // private Callback iosPayCallback = null;
        private YoukiaSDK()
        {
            GetDeviceId();

            getSysteminfo();

            YoukiaSDKCore.Init();
            // //得到来源
            //
            var opt = WX.GetLaunchOptionsSync();
            if (opt.query.ContainsKey("from"))
            {
                from = opt.query["from"];
            }

            if (opt.query.ContainsKey("gdt_vid"))
            {
                gdt_vid = opt.query["gdt_vid"];
                TXAD.CLICK_ID = gdt_vid;
                JuliangAD.CLICK_ID = gdt_vid;
                WX.StorageSetStringSync("ad_click_id", gdt_vid);
                Debug.Log("gdt_vid=" + gdt_vid);
            }
            else
            {
                gdt_vid = WX.StorageGetStringSync("ad_click_id", "");
                if (gdt_vid != null && !gdt_vid.Equals(""))
                {
                    TXAD.CLICK_ID = gdt_vid;
                    JuliangAD.CLICK_ID = gdt_vid;
                    Debug.Log("gdt_vid=" + gdt_vid + " cache");
                }
                else
                {
                    gdt_vid = null;
                }
            }

            if (opt.query.ContainsKey("clickid"))
            {
                clickid = opt.query["clickid"];

                JuliangAD.CLICK_ID = clickid;
                WX.StorageSetStringSync("ad_jl_click_id", clickid);
                Debug.Log("clickid=" + clickid);
            }
            else
            {
                clickid = WX.StorageGetStringSync("ad_jl_click_id", "");
                if (clickid != null && !clickid.Equals(""))
                {
                    JuliangAD.CLICK_ID = clickid;
                    Debug.Log("clickid=" + clickid + " cache");
                }
                else
                {
                    clickid = null;
                }
            }
        }

        public string GetDeviceId()
        {
            deviceid = getUUID();
            YoukiaSDKCore.Call("getDeviceId", null, (err, id) =>
            {
                if (err == null)
                {
                    deviceid = id;
                }
            });
            return deviceid;
        }

        public void GetInitExtraData(Callback callback)
        {
            YoukiaSDKCore.Call("_getInitExtraData", null, (err, msg) =>
            {
                if (err == null)
                {
                    callback(CODE_SUCCESS, msg);
                }
                else
                {
                    callback(CODE_FAILED, "");
                }
            });
        }

        public void init(Callback callback)
        {
            Debug.Log("unity  to js init");
            YoukiaSDKCore.Call("init", null, (err, msg) =>
            {
                if (err != null)
                {
                    callback(CODE_FAILED, msg);
                }
                else
                {
                    callback(CODE_SUCCESS, "init success");
                }
            });


            // osPlatform = "android";
            // JuliangAD.CLICK_ID="EJiw267wvfQCGKf2g74ZIPD89-vIATAMOAFCIjIwMTkxMTI3MTQxMTEzMDEwMDI2MDc3MjE1MTUwNTczNTBIAQ==";
            JuliangAD.DoAction(JuliangAD.ActonType.INIT, osPlatform);
        }


        public void getDynamicUpdate(Callback callback)
        {
            string url = YoukiaSDKConfig.LOGINTHREE_URL.Replace("LoginThree", "dynamicUpdate");

            var trackArgs = new Dictionary<string, object>();
            trackArgs["id"] = (DateTime.Now.ToUniversalTime().Ticks / 10000000).ToString();
            trackArgs["step"] = "dyn_update_info";
            //trackArgs["data"] = dic;
            Track("sdk_step", trackArgs);

            FastNetwork.Instance.DoGet(
                url: url + "?ver=" + UrlEncodeUtil.UrlEncode(YoukiaSDKConfig.GAME_VERSION),
                onSuc: (success) =>
                {
                    trackArgs["step"] = "dyn_update_info_ok";
                    //trackArgs["data"] = dic;
                    Track("sdk_step", trackArgs);
                    Debug.Log(success);

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic = Json.Deserialize(success) as Dictionary<string, object>;
                    //dic = dic["data"] as Dictionary<string, object>;
                    callback(CODE_SUCCESS, Json.Serialize(dic["data"]));
                },
                onError: (failed) =>
                {
                    trackArgs["step"] = "dyn_update_info_fail";
                    //trackArgs["data"] = dic;
                    Track("sdk_step", trackArgs);

                    Debug.Log(failed);
                    callback(CODE_FAILED, failed);
                }
            );
        }

        public void login(Callback callback)
        {
            if (fistLogin)
            {
                onLogin = callback;
                return;
            }

            if (String.IsNullOrEmpty(user_id))
            {
                login_(callback);
            }
            else
            {
                callback(CODE_SUCCESS, "login success");
            }
        }

        private void login_(Callback callback)
        {
            var trackArgs = new Dictionary<string, object>();
            trackArgs["id"] = (DateTime.Now.ToUniversalTime().Ticks / 10000000).ToString();
            trackArgs["step"] = "wx_login";
            //trackArgs["data"] = dic;
            Track("sdk_step", trackArgs);
            YoukiaSDKCore.Call("login", null, (err, msg) =>
            {
                Debug.Log("login result:error:" + err + " msg:" + msg);
                if (err != null)
                {
                    trackArgs["step"] = "wx_login_fail";
                    trackArgs["msg"] = err;
                    Track("sdk_step", trackArgs);
                    Debug.Log("fail " + msg);
                    callback(CODE_FAILED, msg);
                    fistLogin = false;
                    return;
                }
                else
                {
                    trackArgs["step"] = "wx_login_ok";
                    trackArgs["msg"] = err;
                    Track("sdk_step", trackArgs);

                    Debug.Log("login success:" + msg);

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic = Json.Deserialize(msg) as Dictionary<string, object>;

                    Debug.Log("LOGINTHREE_URL:" + YoukiaSDKConfig.LOGINTHREE_URL);
                    Debug.Log("dic:" + dic.ToString());

                    var trackArgs1 = new Dictionary<string, object>();
                    trackArgs1["id"] = (DateTime.Now.ToUniversalTime().Ticks / 10000000).ToString();
                    trackArgs1["step"] = "login";
                    //trackArgs["data"] = dic;
                    Track("sdk_step", trackArgs1);
                    try
                    {
                        FastNetwork.Instance.DoPost(
                            url: YoukiaSDKConfig.LOGINTHREE_URL,
                            paramsDic: dic,
                            onSuc: (success) =>
                            {
                                try
                                {
                                    loginResult(success, callback);
                                    trackArgs1["step"] = "login_ok";
                                    Track("sdk_step", trackArgs1);
                                }
                                catch (Exception e)
                                {
                                    trackArgs1.Add("msg", e.Message);
                                    trackArgs1["step"] = "login_fail";
                                    Track("sdk_step", trackArgs1);
                                    callback(CODE_FAILED, e.Message);
                                }

                                fistLogin = false;
                            },
                            onError: (failed) =>
                            {
                                trackArgs.Add("msg", failed);
                                trackArgs["step"] = "login_fail";
                                Track("sdk_step", trackArgs);
                                callback(CODE_FAILED, failed);
                                fistLogin = false;
                            },
                            showLoading: false
                        );
                    }
                    catch (Exception e)
                    {
                        trackArgs1.Add("msg", e.Message);
                        trackArgs1["step"] = "login_fail";
                        Track("sdk_step", trackArgs1);
                        Debug.LogError(e.Message);
                        callback(CODE_FAILED, e.Message);
                        fistLogin = false;
                    }
                }
            });
        }

        private void loginResult(string result, Callback callback)
        {
            //result = '{"status":1,"msg":"ok","data":
            //{"user_id":"H5_ojCOW5QKdsKeXGHHqllYgYjpB7wE",
            //"open_id":"ojCOW5QKdsKeXGHHqllYgYjpB7wE",
            //"userinfo_url":"http:// rolelist.dskystudio.com/ index.php/ SelectUserinfo",
            //"serverlist_url":"http:// center.dskystudio.com/ sdk.php/ P1186/ server/ o_system/ android",
            //"serverlist_status_url":"http:// center.dskystudio.com/ sdk.php/ P1186/ server/ o_system/ android",
            //"login_url":"http:// center.dskystudio.com/ sdk.php/ P1186/ login/ o_system/ android",
            //"createorder_url":"http:// center.dskystudio.com/ sdk.php/ P1186/ genOrder/ o_system/ android",
            //"payback_url":"http:// center.dskystudio.com/ sdk.php/ P1186/ payback/ o_system/ android"}}';
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            var dic = Json.Deserialize(result) as Dictionary<string, object>;
            if (dic["status"].ToString().Equals("1"))
            {
                dic = dic["data"] as Dictionary<string, object>;
                user_id = dic["user_id"].ToString();
                open_id = dic["open_id"].ToString();

                trackWXLogin(user_id, open_id);
                userinfo_url = dic["userinfo_url"].ToString();
                serverlist_url = dic["serverlist_url"].ToString();
                serverlist_status_url = dic["serverlist_status_url"].ToString();
                login_url = dic["login_url"].ToString();
                createorder_url = dic["createorder_url"].ToString();
                payback_url = dic["payback_url"].ToString();
                TXAD.WX_OPENID = open_id;


                try
                {
                    callback(CODE_SUCCESS, "loginsuccess");
                }
                catch (Exception _)
                {
                }

                Debug.Log("user_id:" + user_id);
                Debug.Log("open_id:" + open_id);
                Debug.Log("userinfo_url:" + userinfo_url);
                Debug.Log("serverlist_url:" + serverlist_url);
                Debug.Log("login_url:" + login_url);
            }

            ;
        }


        public void pay(PayInfo payInfo, Callback callback)
        {
            if (payInfo.cpOrderId == null)
            {
                payInfo.cpOrderId = "";
            }

            if (payInfo.extra == null)
            {
                payInfo.extra = "";
            }

            Dictionary<string, object> good;
            String good_name = "";
            String good_price = "";
            for (int i = 0; i < goodlist.Count; i++)
            {
                good = goodlist[i] as Dictionary<string, object>;
                if (payInfo.productId.Equals(good["id"] as String))
                {
                    Debug.Log(payInfo);
                    good_name = good["name"] as String;
                    good_price = good["amount"] as String;
                    break;
                }
            }


            if (good_price.Equals(""))
            {
                callback(CODE_FAILED, "商品不存在");
                return;
            }

            WX.ShowLoading(new ShowLoadingOption
            {
                title = "支付中",
                mask = true,
            });

            var args = new Dictionary<string, object>();

            args["user_id"] = user_id;
            args["server_id"] = serverId;
            args["product_id"] = payInfo.productId;
            // args["role_id"] = roleId;
            args["price"] = good_price;
            // args["amount"] = amount;


            TXAD.DoAction(TXAD.ActonType.COMPLETE_ORDER, new Dictionary<string, object>
            {
                { "action_param", args },
            });


            Dictionary<string, object> getorderparam = new Dictionary<string, object>();
            getorderparam["user_id"] = user_id;
            getorderparam["open_id"] = open_id;
            getorderparam["sid"] = serverId;
            getorderparam["product_id"] = payInfo.productId;
            getorderparam["amount"] = good_price;
            getorderparam["type"] = osPlatform;
            getorderparam.Add("cp_order_id", payInfo.cpOrderId);
            getorderparam.Add("active_item_sid", payInfo.extra);


            FastNetwork.Instance.DoPost(createorder_url, getorderparam,
                onSuc: (msg) =>
                {
                    Debug.Log("success:" + msg);
                    try
                    {
                        var res = Json.Deserialize(msg) as Dictionary<string, object>;
                        var data = res["data"] as Dictionary<string, object>;
                        //data["order_id"].ToString()
                        var status = res["status"].ToString();
                        Dictionary<string, object> param = new Dictionary<string, object>();
                        param["name"] = good_name;
                        param["orderid"] = data["order_id"].ToString();
                        param["amount"] = good_price;
                        param["serverid"] = serverId;
                        param["servername"] = zone_name;
                        param["userid"] = role_id;
                        param["username"] = role_name;
                        param["level"] = level;
                        param["goodid"] = payInfo.productId;
                        param["paycallback"] = payback_url;
                        param["pay_image"] = "/images/pay_face.jpg";

                        YoukiaSDKCore.Call("pay", Json.Serialize(param), (error, msg2) =>
                        {
                            if (error == null)
                            {
                                Debug.Log("success pay");
                                callback(CODE_SUCCESS, msg2);
                            }
                            else
                            {
                                Debug.Log("failed pay");
                                callback(CODE_FAILED, msg2);
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        callback(CODE_FAILED, e.ToString());
                    }
                    finally
                    {
                        WX.HideLoading(new HideLoadingOption());
                    }
                },
                onError: (paymsg) =>
                {
                    callback(CODE_FAILED, paymsg);
                    WX.HideLoading(new HideLoadingOption());
                    Debug.LogError("failed:" + paymsg);
                });
        }

        public void getServerlist(Callback callback)
        {
            Debug.Log("getServerlist");
            var trackArgs = new Dictionary<string, object>();
            trackArgs["id"] = (DateTime.Now.ToUniversalTime().Ticks / 10000000).ToString();
            trackArgs["step"] = "get_server_list";
            //trackArgs["data"] = dic;
            Track("sdk_step", trackArgs);

            FastNetwork.Instance.DoGet(
                //url: "http://center.dskystudio.com/sdk.php/P1186/server/o_system/android",
                url: serverlist_url + "?ver=" + UrlEncodeUtil.UrlEncode(YoukiaSDKConfig.GAME_VERSION),
                onSuc: (msg) =>
                {
                    trackArgs["step"] = "get_server_list_ok";
                    //trackArgs["data"] = dic;
                    Track("sdk_step", trackArgs);
                    Debug.Log("msg:" + msg);
                    Dictionary<string, object> dic = Json.Deserialize(msg) as Dictionary<string, object>;
                    //{"status":1,"msg":"ok","data":
                    //{"pid":1186,"gid":"607","rnd_num":null,
                    //"moment_mess":null,
                    //"data":[["1","1\u670d",0,0,0,"0",0],["2","2\u670d",0,0,0,"0",0]],
                    //"maintenanceMess":null}}
                    if ("1".Equals(dic["status"].ToString()))
                    {
                        Debug.Log("msg：" + dic["msg"].ToString());
                        Dictionary<string, object> data = dic["data"] as Dictionary<string, object>;
                        Debug.Log("data[pid]：" + data["pid"].ToString());

                        List<object> serverlist = data["data"] as List<object>;
                        if (null != data["maintenanceMess"])
                        {
                            maintenanceMess = data["maintenanceMess"].ToString();
                        }

                        // callback(CODE_SUCCESS, Json.Serialize(serverlist));
                        DoGetUserInfo(serverlist, callback);
                    }
                },
                onError: (msg) =>
                {
                    trackArgs.Add("msg", msg);
                    trackArgs["step"] = "get_server_list_fial";
                    //trackArgs["data"] = dic;
                    Track("sdk_step", trackArgs);
                    callback(CODE_FAILED, msg);
                }
            );
        }

        private void DoGetUserInfo(List<object> serverlist, Callback callback)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["userid"] = user_id; //"H5_ojCOW5QKdsKeXGHHqllYgYjpB7wE";

            var trackArgs = new Dictionary<string, object>();
            trackArgs.Add("id", (DateTime.Now.ToUniversalTime().Ticks / 10000000).ToString());
            trackArgs.Add("step", "get_role_info");

            Track("sdk_step", trackArgs);
            FastNetwork.Instance.DoPost(
                // url: "http://rolelist.dskystudio.com/index.php/SelectUserinfo",
                url: userinfo_url,
                paramsDic: param,
                onSuc: (msg) =>
                {
                    trackArgs["step"] = "get_role_info_ok";
                    Track("sdk_step", trackArgs);
                    //{"userid":"H5_ojCOW5QKdsKeXGHHqllYgYjpB7wE","userinfo":[],"sid_loginTime":""}
                    Debug.Log(msg);
                    Dictionary<string, object> result = Json.Deserialize(msg) as Dictionary<string, object>;
                    List<object> userinfo = result["userinfo"] as List<object>;
                    string sid_loginTime = result["sid_loginTime"].ToString();

                    Dictionary<string, object> serverresult = new Dictionary<string, object>();
                    serverresult["servers"] = serverlist;
                    serverresult["serverUserinfo"] = userinfo;
                    serverresult["currLoginServers"] = sid_loginTime;
                    callback(CODE_SUCCESS, Json.Serialize(serverresult));
                },
                onError: (msg) =>
                {
                    trackArgs.Add("msg", msg);
                    trackArgs["step"] = "get_role_info_fail";
                    Track("sdk_step", trackArgs);
                    Dictionary<string, object> serverresult = new Dictionary<string, object>();
                    serverresult["servers"] = serverlist;
                    serverresult["serverUserinfo"] = "";
                    serverresult["currLoginServers"] = "";
                    callback(CODE_SUCCESS, Json.Serialize(serverresult));
                }
            );
        }

        public void maintainNotice(Callback callback)
        {
            callback(CODE_SUCCESS, maintenanceMess);
        }


        public void loginServer(String serverId, Callback callback)
        {
            this.serverId = serverId;
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["platformuserid"] = user_id;
            param["sid"] = serverId;

            var trackArgs = new Dictionary<string, object>();
            trackArgs.Add("id", (DateTime.Now.ToUniversalTime().Ticks / 10000000).ToString());
            trackArgs.Add("step", "login_server");
            trackArgs.Add("data", Json.Serialize(param));

            Track("sdk_step", trackArgs);

            FastNetwork.Instance.DoPost(
                url: login_url,
                paramsDic: param,
                onSuc: (msg) =>
                {
                    trackArgs["step"] = "login_server_ok";
                    Track("sdk_step", trackArgs);

                    Debug.Log(msg);
                    // {"status":1,"msg":"ok","data":
                    // {"game_tongji_id":null,
                    // "loginurl":"?userid=H5_ojCOW5QKdsKeXGHHqllYgYjpB7wE&platform=H5&bi_platform_id=1186&nickname=&face=&time=1668140982&callback=&sig=116c2ee736b8a95f8f81451a8eecc2a&isfangchenmi=0&server_name=1002",
                    // "game_base_url":"",
                    // "plateform_userid":"H5_ojCOW5QKdsKeXGHHqllYgYjpB7wE",
                    // "goods_list":null,
                    // "goods_list_pro":null}} 
                    Dictionary<string, object> result = Json.Deserialize(msg) as Dictionary<string, object>;
                    if ("1".Equals(result["status"].ToString()))
                    {
                        result = result["data"] as Dictionary<string, object>;
                        Debug.Log(result["goods_list"]);
                        Debug.Log(result["goods_list_pro"]);
                        if (result["goods_list"] != null)
                        {
                            goodlist = Json.Deserialize(result["goods_list"].ToString()) as List<object>;
                        }

                        if (result["goods_list_pro"] != null)
                        {
                            goodlist_pro = Json.Deserialize(result["goods_list_pro"].ToString()) as List<object>;
                        }

                        sendCurrentServerid();
                        Dictionary<string, object> loginserver = new Dictionary<string, object>();
                        loginserver["loginurl"] = result["loginurl"].ToString() +
                                                  "&wx_open_id=" + UrlEncodeUtil.UrlEncode(open_id) + "&from=" +
                                                  UrlEncodeUtil.UrlEncode(from) + "&os=" +
                                                  UrlEncodeUtil.UrlEncode(osPlatform) + "&deviceid=" +
                                                  UrlEncodeUtil.UrlEncode(deviceid);
                        loginserver["serverId"] = serverId;
                        loginserver["game_base_url"] = result["game_base_url"].ToString();
                        callback(CODE_SUCCESS, Json.Serialize(loginserver));
                    }
                },
                onError: (msg) =>
                {
                    trackArgs["step"] = "login_server_fail";
                    trackArgs.Add("msg", msg);
                    Track("sdk_step", trackArgs);
                    Debug.Log(msg);
                    callback(CODE_FAILED, msg);
                }
            );
        }

        private void sendCurrentServerid()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["userid"] = user_id;
            param["sid"] = serverId;
            FastNetwork.Instance.DoPost(
                url: userinfo_url.Replace("SelectUserinfo", "insertLoginInfo"),
                paramsDic: param,
                onSuc: (msg) => { Debug.Log("success:" + msg); },
                onError: (msg) => { Debug.Log("onError:" + msg); }
            );
        }

        public void getGoodList(Callback callback)
        {
            callback(CODE_SUCCESS, Json.Serialize(goodlist));
        }

        public void getGoodList_pro(Callback callback)
        {
            callback(CODE_SUCCESS, Json.Serialize(goodlist_pro));
        }

        public void getAppinfo(Callback callback)
        {
            //WX.GetSystemInfo(new GetSystemInfoOption()
            //{

            //});
        }

        /// <summary> 
        /// 要分享的图片地址，必须为本地路径或临时路径
        /// </summary>
        public void shareWX(string title, String imageUrl)
        {
            Debug.Log("sharewx imageUrl:" + imageUrl);
            // WX.ShowShareImageMenu(
            //     new ShowShareImageMenuOption()
            //     {
            //         path = ImagePath,
            //         complete = (result) =>
            //         {
            //             Debug.Log("share complete");
            //         },
            //         success = (success) =>
            //         {
            //             Debug.Log("share success");
            //             callback(CODE_SUCCESS, success.ToString());
            //         },
            //         fail = (fail) =>
            //         {
            //             Debug.Log("share fail");
            //             callback(CODE_FAILED, fail.errMsg);
            //         }
            //     }
            // );
            WX.ShareAppMessage(new ShareAppMessageOption()
            {
                imageUrl = imageUrl,
                title = title,
            });
        }

        /// 分享小游戏卡片sharewi
        /// </summary>
        /// <param name="ticketId">用户点击卡片进入游戏后，可以通过getShareTicketId获取到tickId</param>
        /// <param name="title">卡片标题</param>
        /// <param name="imageUrl">卡片图片 留空则使用当前画面截图</param>
        public void shareWithTicket(string ticketId, string title, string imageUrl = null)
        {
            Debug.Log("shareWith ticketId:" + ticketId + " title:" + title + " imageUrl:" + imageUrl);
            WX.ShareAppMessage(new ShareAppMessageOption()
            {
                imageUrl = imageUrl,
                title = title,
                // path = "pages/index/index,
                query = "_game_ticket_id_=" + UrlEncodeUtil.UrlEncode(ticketId),
            });
        }

        /// <summary>
        /// 获取分享卡片的ticketId ，请尽量在初始化完成后调用
        /// </summary>
        /// <param name="callback">回调函数，，，永远成功，没有 tickid 为空字符串</param>
        public void getShareTicketId(Callback callback)
        {
            // 获取特殊参数
            var opt = WX.GetLaunchOptionsSync();
            if (opt.query.ContainsKey("_game_ticket_id_"))
            {
                Debug.Log("getShareTicketId:" + opt.query["_game_ticket_id_"]);
                callback(CODE_SUCCESS, opt.query["_game_ticket_id_"]);
            }
            else
            {
                callback(CODE_SUCCESS, "");
            }
        }

        private bool isfirst = true;
        private string role_id = "";
        private string role_name = "";
        private string zone_id = "";
        private string zone_name = "";
        private string level = "";

        public enum GAMETYPE
        {
            createRole,
            enterGame,
            levelup
        }

        public void gameRoleInfo(GAMETYPE gametype, String roleId, String roleName,
            String roleLevel, String zoneId,
            String zoneName, String createRoletime,
            String extra)
        {
            Debug.Log("roleId:" + roleId + " roleName:" + roleName + " roleLevel:"
                      + roleLevel + " zoneId:" + zoneId + " zoneName:" + zoneName + " extra:"
                      + extra + " createroletime:" + createRoletime);
            role_id = roleId;
            role_name = roleName;
            zone_id = zoneId;
            zone_name = zoneName;
            level = roleLevel;


            Dictionary<string, object> dic = new Dictionary<string, object>();
            //    isLogin,
            if (isfirst)
            {
                dic["isLogin"] = true;
                isfirst = false;
            }
            else
            {
                dic["isLogin"] = false;
            }

            dic["server_id"] = serverId;
            dic["server_name"] = zoneName;
            dic["role_id"] = roleId;
            dic["role_name"] = roleName;
            dic["level"] = roleLevel;
            dic["extra"] = extra;
            dic["createRoletime"] = createRoletime;
            switch (gametype)
            {
                case GAMETYPE.createRole:
                    YoukiaSDKCore.Call("createRole", Json.Serialize(dic),
                        (err, msg) => { Debug.Log("createRole err:" + err + " msg:" + msg); });
                    break;
                case GAMETYPE.enterGame:
                    YoukiaSDKCore.Call("enterGame", Json.Serialize(dic),
                        (err, msg) => { Debug.Log("enterGame err:" + err + " msg:" + msg); });
                    break;
                case GAMETYPE.levelup:
                    YoukiaSDKCore.Call("levelup", Json.Serialize(dic),
                        (err, msg) => { Debug.Log("levelup err:" + err + " msg:" + msg); });
                    break;
            }

            Dictionary<string, object> param = new Dictionary<string, object>();
            string time = GetTimeStamp();
            param["platfromId"] = user_id;
            param["mcurrserverid"] = serverId;
            param["rolename"] = roleName;
            param["rolelevel"] = roleLevel;
            param["role_name"] = roleName;
            param["role_uid"] = roleId;
            param["zone_name"] = zoneName;
            param["sign"] = MD5Utils.MD5String(time + "6062251784973be4e60c5d0d51229579");
            param["time"] = time;
            if (!extra.Equals(""))
            {
                if (!extra.Contains("="))
                {
                    param["extra"] = extra;
                }
                else
                {
                    string[] temp = extra.Split('&');
                    foreach (string item in temp)
                    {
                        string[] itemsplit = item.Split('=');
                        if (itemsplit.Length > 1)
                        {
                            param[UrlEncodeUtil.UrlEncode(itemsplit[0])] = UrlEncodeUtil.UrlEncode(itemsplit[1]);
                        }
                    }
                }
            }

            var trackArgs = new Dictionary<string, object>();
            trackArgs.Add("id", (DateTime.Now.ToUniversalTime().Ticks / 10000000).ToString());
            trackArgs.Add("step", "update_role_info");
            //trackArgs.Add("data", param);

            Track("sdk_step", trackArgs);

            FastNetwork.Instance.DoPost(
                url: userinfo_url.Replace("SelectUserinfo", "InsertUserinfo"),
                paramsDic: param,
                onSuc: (msg) =>
                {
                    trackArgs["step"] = "update_role_info_ok";
                    Track("sdk_step", trackArgs);
                    Debug.Log("success:" + msg);
                },
                onError: (msg) =>
                {
                    trackArgs["step"] = "update_role_info_fail";
                    trackArgs.Add("msg", msg);
                    Track("sdk_step", trackArgs);
                    Debug.Log("onError:" + msg);
                }
            );
        }

        private string GetTimeStamp()
        {
            return ((System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
        }

        private string getUUID()
        {
            string uuid = WX.StorageGetStringSync("uuid", "");
            if ("".Equals(uuid))
            {
                uuid = Guid.NewGuid().ToString();
                WX.StorageSetStringSync("uuid", uuid);
            }

            return uuid;
        }

        private string osPlatform = "";
        private string osVersion = "";
        private string model = "";
        private string brand = "";


        private void getSysteminfo()
        {
            var res = WX.GetSystemInfoSync();
            osPlatform = res.platform;
            osVersion = res.system;
            model = res.model;
            brand = res.brand;
        }

        public void stepinfo(string step, Dictionary<string, object> args = null)
        {
            //string url = YoukiaSDKConfig.stepinfourl + "?device_id=" + deviceid
            //    + "&client_time=" + GetTimeStamp()
            //    + "&game_name="
            //    + "&step=" + step
            //    + "&platformId=" + YoukiaSDKConfig.PLATFORM
            //    + "&carrierType=&networkType=&youkiaid=&versionName=&phoneName=" + model + "&memoryTotal=&osVersion=" + osPlatform + "&from=" + from;
            //Debug.Log(url);
            if (args == null)
            {
                args = new Dictionary<string, object>();
            }

            args["step"] = step;

            Track("app_step", args);
            //FastNetwork.Instance.DoGet(url, showLoading: false);
        }

        public void showuserinfo(int x, int y, int w, int h, int gamew, int gameh, bool isdebug, Callback callback)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["x"] = x;
            dic["y"] = y;
            dic["w"] = w;
            dic["h"] = h;
            dic["ww"] = gamew;
            dic["hh"] = gameh;
            dic["dev"] = isdebug;
            Debug.Log(Json.Serialize(dic));
            YoukiaSDKCore.Call("showAccessButton", Json.Serialize(dic), (error, msg) =>
            {
                if (error == null)
                {
                    Debug.Log("success showuserinfo");
                    callback(CODE_SUCCESS, msg);
                }
                else
                {
                    Debug.Log("failed showuserinfo");
                    callback(CODE_FAILED, msg);
                }
            });
        }

        public void hideuserinfo(Callback callback)
        {
            Debug.Log("hideuserinfo");
            YoukiaSDKCore.Call("hideAccessButton", null, (error, msg) =>
            {
                if (error == null)
                {
                    Debug.Log("success hideAccessButton");
                    callback(CODE_SUCCESS, msg);
                }
                else
                {
                    Debug.Log("failed hideAccessButton");
                    callback(CODE_FAILED, msg);
                }
            });
        }

        public void checkUpdate()
        {
            Debug.Log("checkUpdate");
            YoukiaSDKCore.Call("checkUpdate", null, (error, msg) =>
            {
                if (error != null)
                {
                    Debug.Log(msg);
                }
            });
        }

        public void getBatteryInfo(Callback callback)
        {
            Debug.Log("getBatteryInfo");
            YoukiaSDKCore.Call("getBatteryInfo", null, (error, msg) =>
            {
                if (error == null)
                {
                    Debug.Log("success getBatteryInfo");
                    callback(CODE_SUCCESS, msg);
                }
                else
                {
                    Debug.Log("failed getBatteryInfo");
                    callback(CODE_FAILED, msg);
                }
            });
        }

        public void getNetworkType(Callback callback)
        {
            Debug.Log("getNetworkType");
            WX.GetNetworkType(new GetNetworkTypeOption()
            {
                success = (success) =>
                {
                    Debug.Log("getNetworkType success：" + success.networkType);
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic["networkType"] = success.networkType;
                    // dic["signalStrength"] = success.signalStrength;
                    callback(CODE_SUCCESS, Json.Serialize(dic));
                },
                fail = (failed) =>
                {
                    Debug.Log("getNetworkType success：" + failed.errMsg);
                    callback(CODE_FAILED, failed.errMsg);
                }
            });
        }

        private Callback rewardedCallback;

        public void createRewardedVideoAd(string cp_order_id, string goods_id, string goods_name, string extend_params,
            Callback callback)
        {
            Debug.Log("createRewardedVideoAd cp_order_id:" + cp_order_id);
            rewardedCallback = callback;

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["cp_order_id"] = cp_order_id;
            dic["goods_id"] = goods_id;
            dic["goods_name"] = goods_name;
            dic["role_id"] = role_id;
            dic["role_name"] = role_name;
            dic["server_id"] = zone_id;
            dic["server_name"] = zone_name;
            dic["level"] = level;
            dic["extend_params"] = extend_params;

            YoukiaSDKCore.Call("createRewardedVideoAd", Json.Serialize(dic));
        }

        public void rewardedResult(string result)
        {
            Debug.Log("createRewardedVideoAd rewardedResult:" + result);
            rewardedCallback(CODE_SUCCESS, result);
        }

        public void restartMiniProgram()
        {
            Debug.Log("restartMiniProgram");
            WX.RestartMiniProgram(new RestartMiniProgramOption()
            {
                complete = (complete) => { },
                success = (success) => { },
                fail = (failed) => { }
            });
        }

        public void exitMiniProgram(Callback callback)
        {
            Debug.Log("exitMiniProgram");
            WX.ExitMiniProgram(new ExitMiniProgramOption()
            {
                complete = (complete) => { },
                success = (success) =>
                {
                    Debug.Log("exitMiniProgram success");
                    callback(CODE_SUCCESS, success.ToString());
                },
                fail = (failed) =>
                {
                    Debug.Log("exitMiniProgram failed");
                    callback(CODE_FAILED, failed.ToString());
                }
            });
        }

        public void Track(string events, Dictionary<string, object> args = null)
        {
            Dictionary<string, object> a = new Dictionary<string, object>();
            a["event"] = events;
            if (args != null)
            {
                a["args"] = args;
            }

            YoukiaSDKCore.Call("track", Json.Serialize(a));


            var act = new Dictionary<string, object>();
            act["custom_action"] = events;
            if (args != null)
            {
                act["action_param"] = args;

                if (args.ContainsKey("step"))
                {
                    act["external_action_id"] = args["step"];
                }
            }

            TXAD.DoAction(TXAD.ActonType.CUSTOM, act);
        }


        private void trackWXLogin(string userId, string openId)
        {
            var args = new Dictionary<string, object>();
            args["user_id"] = userId;
            args["open_id"] = openId;
            YoukiaSDKCore.Call("trackWXLogin", Json.Serialize(args));
        }

        /**
        注册打点
        serverId //服务器id
        roleId //角色id
        nickname //角色昵称
        */
        public void registerStep(string serverId, string roleId = null, string nickname = null)
        {
            TXAD.DoAction(TXAD.ActonType.REGISTER);

            TXAD.DoAction(TXAD.ActonType.CREATE_ROLE);
            JuliangAD.DoAction(JuliangAD.ActonType.REGISTER, osPlatform);
            // kv object 
            var args = new Dictionary<string, object>();
            args["user_id"] = user_id;
            args["server_id"] = serverId;
            args["roleId"] = roleId;
            args["nickname"] = nickname;
            args["step"] = "register";
            YoukiaSDKCore.Call("otherTrack", Json.Serialize(args));
        }

        /**
        登录打点
        serverId //服务器id
        roleId //角色id
        */
        public void loginStep(string serverId, string roleId)
        {
            WX.ReportGameStart();


            TXAD.DoAction(TXAD.ActonType.LOGIN);
            JuliangAD.DoAction(JuliangAD.ActonType.LOGIN, osPlatform);

            // kv object 
            var args = new Dictionary<string, object>();
            args["user_id"] = user_id;
            args["server_id"] = serverId;
            args["role_id"] = roleId;
            args["step"] = "login";
            YoukiaSDKCore.Call("otherTrack", Json.Serialize(args));
        }

        /**
        支付打点
        productId //商品id
        price //商品价格
        amount //商品数量（虚拟币）
        */
        public void payStep(string productId, string orderId, float price = 0, float amount = 0, string roleId = null)
        {
            for (int i = 0; i < goodlist.Count; i++)
            {
                var goods = goodlist[i] as Dictionary<string, object>;

                if (productId.Equals(goods["id"].ToString()))
                {
                    price = float.Parse(goods["amount"].ToString());
                }
            }


            var args = new Dictionary<string, object>();
            args["user_id"] = user_id;
            args["server_id"] = serverId;
            args["product_id"] = productId;
            args["role_id"] = roleId;
            args["price"] = price;
            args["amount"] = amount;
            if (String.IsNullOrEmpty(orderId))
            {
                orderId = user_id + "_" + DateTime.Now.ToUniversalTime().Ticks.ToString();
            }

            args["order_id"] = orderId;


            TXAD.DoAction(TXAD.ActonType.PURCHASE, new Dictionary<string, object>
            {
                { "action_param", args }
            });
            var juliang = new Dictionary<string, object>();
            juliang["user_id"] = user_id;
            juliang["server_id"] = serverId;
            juliang["product_id"] = productId;
            juliang["role_id"] = roleId;
            juliang["price"] = price;
            juliang["pay_amount"] = amount;
            JuliangAD.DoAction(JuliangAD.ActonType.PAY, osPlatform, args);

            args["step"] = "pay";
            YoukiaSDKCore.Call("otherTrack", Json.Serialize(args));
        }

        /**
        退出打点
        */
        public void logoutStep()
        {
            YoukiaSDKCore.Call("otherTrack", "");
        }

        public void setDataOpen(bool TXADopen, bool JLADopen)
        {
            YoukiaSDKConfig.TXAD.ENABLED = TXADopen;
            YoukiaSDKConfig.JLAD.ENABLED = JLADopen;
        }

        public void requestSubscribeMessageArray(string ids, Callback callback)
        {
            Debug.Log("requestSubscribeMessageArray:" + ids);
            YoukiaSDKCore.Call("requestSubscribeMessageArray", ids, (error, msg) =>
            {
                if (error == null)
                {
                    Debug.Log("requestSubscribeMessageArray:success:" + msg);
                    callback(CODE_SUCCESS, msg);
                }
                else
                {
                    Debug.Log("requestSubscribeMessageArray:failed:" + msg);
                    callback(CODE_FAILED, msg);
                }
            });
        }


        public void OpenCustomerServiceConversation(Callback callback)
        {
            WX.OpenCustomerServiceConversation(new OpenCustomerServiceConversationOption
            {
                fail = (fail) =>
                {
                    // iosPayCallback = null;
                    callback(CODE_FAILED, fail.errMsg);
                },
                success = (success) =>
                {
                    // iosPayCallback = null;
                    callback(CODE_SUCCESS, "支付完成");
                },
            });
        }
    }
}