/**
 * 抖音小游戏SDK v1.1.0
 * 包含MJG_GameSDK.js和MJG_TT_GAME_API.js
 * 
 * MJG_TT_GAME_API.js内容如下：
 * 
 * MJG_TT_GAME_API.js文件是MJG_GameSDK.js文件接口的详细代码实现文件
 * 
 * 
 * 较上一个版本改动地方有：
 *
 * 
 */

GameGlobal.TT_GAME_API = (function (GameGlobal) {
    var _String = String;

    //游戏相关配置参数
    var gameID = 0;
    var channelId = 0;
    var packageName = "";
    var wxxcxName = "";
    var packageVersion = 0;
    var wxxcxVersion = "0.0.0";
    var uuid = '';

    //SDK变量
    var sdkVersion = "1.1.0";
    var isStartMidasPayAndroid = true;
    var qd_query = null;
    var custom_device = "0";
    var loginCallBack = function () { };
    var sidebarCallBack = function () { };
    var onShowCallback = function () { };
    var addShortcutCallback = function () { };

      // 回调函数列表
    var callbackList = [];

    //系统信息
    var sysInfo = {
        mac: "",           //mac地址
        imei: "",          // 安卓设备
        idfa: "",    //iphone设备号
        sys_model: "",      //机型
        sys_version: "",    //系统版本
        avail_memory: "",  //有效内存
        custom_device: "",  //设备(android iphone)
        openGl: "",  //opengl版本
    }

    //用户信息
    var userInfo = {
        oid: ""
    }

    var domainName1 = "aHR0cHM6Ly9oNXNkay54aW5neWFvZ2FtZXMuY29t";    //服务器域名 https://h5sdk.xingyaogames.com
    var domainName2 = "aHR0cHM6Ly9wYXluZXdhcGkueGluZ3lhb2dhbWVzLmNvbQ==";     //支付域名  https://paynewapi.xingyaogames.com

    //服务器相关接口
    var loginUrl = domainName1 + "L2g1V3hHYW1lL2NvZGUyU2Vzc2lvbg==";           //服务器登录接口 /h5WxGame/code2Session
    var getCpUidUrl = domainName1 + "L2g1V3hHYW1lL2dldENwVWlk";           //获取cpUid接口 /h5WxGame/getCpUid
    var xiadanURL = "";         //下单接口
    var paySucceedCallBackUrl = "";          //支付成功回调接口
    var createRoleURL = domainName1 + "L2g1VXNlci9yb2xl";            //角色上报接口 /h5User/role
    var reportLogUrl =  domainName1 + "L2g1V3hHYW1lL3JlcG9ydExvZw==";


    //初始化游戏配置信息
    function onmjInitGameInfo(data) {
        tt.showShareMenu({
            success(res) {
                console.log("已成功显示转发按钮");
            },
        });
        gameID = data.game_id;
        channelId = data.channel_id;
        packageName = data.package_name;
        wxxcxName = data.wxxcx_name;
        packageVersion = data.package_version;
        wxxcxVersion = data.wxxcx_version;
        uuid = getUuid();

        console.log("sdk初始化，成功")
    }


    //登录
    function onmjWXLogin(callback) {
        loginCallBack = callback;
        console.log('sdk登陆，开始！')
        tt.login({
            success: (res) => {
                if (res.code) {
                    console.log('sdk登陆，抖音登录成功！')
                    let _obj = {
                        res: res,
                        fun: onSendLoginCodeToSever
                    }
                    onRequestSendToSever(_obj);
                } else {
                    let _ogj = {
                        state: 0,
                        data: {},
                        msg: res.errMsg
                    }
                    loginCallBack(_ogj.state, _ogj.data, _ogj.msg);
                    console.log('sdk登陆，微信登录失败！' + res.errMsg)
                }
            }
        });
    }

    function onmjWXPay(data) {
        //先去下单，根据不同的返回值选择不同的支付方式
        onPayToSever(data, onShowMjWxPay);
    }

    function onShowMjWxPay(res, data) {
        let _res = res;
        let _data = data;

        let _platform = tt.getSystemInfoSync().platform;
        // _platform = 'ios';
        console.log(_platform);

        switch (_platform) {
            case "ios":
                onIosPayAction(_res);
                break;
            default:
                onAndroidPayAction(_res);
                break;
        }
    }

    //ios支付
    function onIosPayAction(res) {
        let _res = res;
        var str = _res.data.data.o_data;// 换为安际的Base64输冯的字符单
        var decodedstr = decodeURIComponent(Base64.decode(str));
        let urlObj = getUrlParams(decodedstr);

        tt.openAwemeCustomerService({
            currencyType: "DIAMOND",
            buyQuantity: Number(urlObj.buyQuantity),
            zoneId: urlObj.zoneId,
            customId: urlObj.pt_order_no,
            extraInfo: tt.getStorageSync("se_id"),
            success(res) {
                console.log("sdk支付接口，抖音钻石支付拉起成功")
            },
            fail(res) {
                console.log("sdk支付接口，抖音钻石拉起失败：", res.errMsg)
            },
            complete(res) {
                console.log("sdk支付接口，抖音钻石调用完成");
            },
          });
    }

    function onAndroidPayAction(res) {
        let _res = res;

        var str = _res.data.data.o_data;// 换为安际的Base64输冯的字符单
        var decodedstr = decodeURIComponent(Base64.decode(str));
        let urlObj = getUrlParams(decodedstr);

        console.log("urlObj",urlObj)
        console.log("sdk支付接口，抖音支付开始");
        
        tt.requestGamePayment({
            mode: "game",
            env: Number(urlObj.env),
            currencyType: "CNY",
            platform: "android",
            buyQuantity: Number(urlObj.buyQuantity),
            zoneId: urlObj.zoneId,
            customId: urlObj.pt_order_no,
            extraInfo: tt.getStorageSync("se_id"),
            success(res) {
                console.log("sdk支付接口，抖音支付拉起成功")
            },
            fail(res) {
                console.log("sdk支付接口，虚拟支付拉起失败：", res.errMsg)
            },
            complete(res) {
                console.log("调用完成");
            },
          });
    }


    //角色创建/升级接口
    function onmjRole(data) {
        onRoleToSever(data);
    }

    //判断是否从微信外的渠道启动
    function onmjIsNoWXLaunch() {
        let _obj = tt.getLaunchOptionsSync();
        if ((JSON.stringify(_obj.query) == "{}")) {
            console.log("sdk判断是否微信外的渠道启动接口，否")
            return false;
        } else {
            console.log("sdk判断是否微信外的渠道启动接口，是")
            return true;
        }
    }

   
    //向服务端传递参数
    function onRequestSendToSever(_obj) {
        let tRequestLogin = _obj.fun(_obj.res);
    
        onRequest(
            tRequestLogin.url,
            tRequestLogin.tData,
            tRequestLogin.header,
            tRequestLogin.method,
            tRequestLogin.successCallBack,
            tRequestLogin.failCallBack
        );
    }

    function onSendLoginCodeToSever(res) {
        let _platform = tt.getSystemInfoSync().platform;
        let _device_type = 2;
        let _res = res;
        switch (_platform) {
            case "ios":
                _device_type = 1;
                break;
            default:
                _device_type = 2;
        }

        let _obj = tt.getLaunchOptionsSync();
        console.log('获取小程序冷启动时的参数',_obj);
        if ('query' in _obj) {
            qd_query = JSON.stringify(_obj.query);
        }
        let qd_scene = _obj.scene;
        let qd_referrerInfo = null;
        if ('refererInfo' in _obj) {
            qd_referrerInfo = JSON.stringify(_obj.refererInfo);
        }
    
        let tRequestLogin = {
            url: Base64.decode(loginUrl),    //远程文件路径(和服务器进行数据交互处理)
            tData: {
                code: _res.code,
                game_id: gameID,
                package_name: packageName,
                package_version: packageVersion,
                sdk_version: sdkVersion,
                device_type: _device_type,
                uuid: uuid,
                query: qd_query,
                scene: qd_scene,
                x_channel:"douyin",
                referrerInfo: qd_referrerInfo,
                platform: _platform,
            },
            header: { "Content-Type": "application/x-www-form-urlencoded" },
            method: "POST",
            successCallBack: function (res) {
                if (res.data.state == 1) {
                    tt.setStorageSync("uid", res.data.data.uid);
                    tt.setStorageSync("sdk_token", res.data.data.sdk_token);
                    tt.setStorageSync("se_id", res.data.data.se_id);
                    isStartMidasPayAndroid = res.data.data.isMdsPayAndroid == null ? true : res.data.data.isMdsPayAndroid;
                    domainName2 = res.data.data.payment_url;
                    xiadanURL = domainName2 + Base64.decode("b3JkZXIvZG91WWlu"); //order/douYin
                    paySucceedCallBackUrl = domainName2 + Base64.decode("Y2FsbGJhY2svZG91WWlu"); //callback/douYin
                    console.log('sdk登陆，服务器登录成功！')
                    tt.onShareAppMessage(function () {
                        // 用户点击了“转发”按钮
                        return {
                            title: res.data.data.jump_title,
                            imageUrl:res.data.data.jump_img
                        }
                    });
                    let _obj = {
                        res: res,
                        fun: onGetUidBySever
                    }
                    onRequestSendToSever(_obj);
                } else {
                    console.log('sdk登陆，服务器登录失败！ 失败信息： ' + res.data.msg)
                }
            },
            failCallBack: function (errMsg) {
                let _ogj = {
                    state: 0,
                    data: {},
                    msg: errMsg
                }
                loginCallBack(_ogj.state, _ogj.data, _ogj.msg);
                console.log("请求登录失败！  ", errMsg)
            }
        }
        return tRequestLogin;
    }

    function onGetUidBySever(res) {
        let _res = res;
        let tRequestGetUid = {
            url: Base64.decode(getCpUidUrl),    //远程文件路径(和服务器进行数据交互处理)
            tData: {
                uid: res.data.data.uid,
                game_id: gameID,
            },
            header: { "Content-Type": "application/x-www-form-urlencoded" },
            method: "POST",
            successCallBack: function (res) {
                if (res.data.state == 1) {
                    if (res.data.data.cp_uid != "") {
                        _res.data.data.uid = res.data.data.cp_uid;
                        //console.log("_res.data.data", _res.data.data)
                    }
                } else {
                    console.log("获取cpUid接口的请求成功，但获取失败，err：", res.data.msg)
                }
                //console.log("_res.data.data", _res.data.data)
                loginCallBack(_res.data.state, _res.data.data, _res.data.msg);
            },
            failCallBack: function (errMsg) { }
        }
        return tRequestGetUid;
    }

    //通过微信接口tt.request()向服务器发送请求
    function onRequest(sUrl, tData, header, method, successCallBack, failCallBack) {
        tt.request({
            url: sUrl,
            data: tData,
            header: header,
            method: method,
            success(res) {
                console.log("tt.request success");
                successCallBack(res);
            },
            fail(errMsg) {
                failCallBack(errMsg);
                console.log("tt.request errMsg", errMsg);
            }
        })
    }

    function onPayToSever(data, callBack) {
        let _data = data;
        let _paytype;
        let sys = tt.getSystemInfoSync();

        switch (sys.platform) {
            case "ios":
                _paytype = "101";
                break;
            default:
                _paytype = "100";

        }

        
        tt.checkSession({
            success() {
                console.log('session 未过期');
            },
            fail() {
                onmjWXLogin(loginCallBack);
            }
        });

        let tRequestOrders = {
            url: xiadanURL,    //远程文件路径(和服务器进行数据交互处理)
            tData: {
                game_id: gameID,
                channel_id: channelId,
                package_name: packageName,
                package_version: packageVersion,
                device_id: onmjGetDeviceTag(),
                device_name: sys.brand,
                device_version: sys.model,
                uuid: uuid,
                uid: tt.getStorageSync("uid"),
                token: tt.getStorageSync("sdk_token"),
                cp_order_num: _data.cp_order_num,
                total_fee: _data.total_fee,
                server_id: _data.server_id,
                server_name: _data.server_name,
                role_id: _data.role_id,
                role_name: _data.role_name,
                role_level: _data.role_level,
                product_id: _data.product_id,
                ext: _data.ext,
                paytype: _paytype,
                se_id: tt.getStorageSync("se_id"),
                vip_level: _data.vip_level == null ? 0 : _data.vip_level
            },
            header: { "Content-Type": "application/x-www-form-urlencoded" },
            method: "POST",
            successCallBack: function (res) {
                if (res.data.state == 1) {
                    console.log("下单成功")
                    callBack(res, _data);
                } else {
                    console.log("下单失败")
                }
            },
            failCallBack: function (errMsg) { }
        }

        onRequest(
            tRequestOrders.url,
            tRequestOrders.tData,
            tRequestOrders.header,
            tRequestOrders.method,
            tRequestOrders.successCallBack,
            tRequestOrders.failCallBack
        );
    }

    function onRoleToSever(data) {
        let _params = [];
        _params.uid = tt.getStorageSync("uid");
        _params.server_id = data.server_id;
        _params.server_name = data.server_name;
        _params.role_id = data.role_id;
        _params.role_name = data.role_name;
        _params.role_level = data.role_level;
        _params.game_id = gameID;
        _params.package_name = packageName;
        _params.update_time = data.update_time;
        _params.time = getTime();
        _params = onArrSortForKey(_params);  //按key大小排序
        _params.sign = getSign(_params)

        let tRequestOrders = {
            url: Base64.decode(createRoleURL),    //远程文件路径(和服务器进行数据交互处理)
            tData: _params,
            header: { "Content-Type": "application/x-www-form-urlencoded" },
            method: "POST",
            successCallBack: function (res) {
                console.log("sdk角色创建/升级上报接口")
                if (res.data.state == 1) {
                    console.log("sdk角色创建/升级上报接口，上报成功")
                } else {
                    console.log("sdk角色创建/升级上报接口，上报失败")
                }
            },
            failCallBack: function (errMsg) { }
        }

        onRequest(
            tRequestOrders.url,
            tRequestOrders.tData,
            tRequestOrders.header,
            tRequestOrders.method,
            tRequestOrders.successCallBack,
            tRequestOrders.failCallBack
        );
    }


    //模态提示框
    function onShowModal(sTitle, sContent, bShowCancel, successCallBack, sConfirmText) {
        tt.showModal({
            title: sTitle,
            content: sContent,
            showCancel: bShowCancel,
            confirmText: sConfirmText,
            success(res) {
                if (res.confirm) {
                    if (successCallBack) {
                        successCallBack();
                    }
                } else if (res.cancel) {
                    console.log('用户点击取消')
                }
            }
        });
    }

    function onmjGetParam(strName) {
        let _value;
        switch (strName) {
            case "options":
                let data = tt.getSystemInfoSync();
                let left = data.safeArea.left;
                let right = data.screenWidth - data.safeArea.right;
                let safeRatio = left > right ? left : right;
                safeRatio = safeRatio / data.screenWidth;
                safeRatio = Math.ceil(safeRatio * 1000) / 1000;
                let options = {
                    safeRatio: safeRatio
                }
                _value = JSON.stringify(options);
                break;

            default:
                break;
        }
        return _value;
    }

    function getUuid() {
        const deviceStorageKey = "XY_SDK_UUID";
        const deviceId = localStorage.getItem(deviceStorageKey);
        if (deviceId) return deviceId;
        const signValue = `${wxxcxVersion}${getRandomValues().join("")}${Date.now()}`;
        const deviceRandomId =hexMD5(signValue);
        localStorage.setItem(deviceStorageKey, deviceRandomId);
        return deviceRandomId;
      }

    function onReportLog(action){
        let _platform = tt.getSystemInfoSync().platform;
        let params = [];
        params.game_id = gameID;
        params.package_name = packageName;
        params.sdk_version = sdkVersion;
        params.wxxcx_version = wxxcxVersion;
        params.package_version = packageVersion;
        params.api = action;
        params.wxxcx_name = wxxcxName;
        params.uuid = uuid;
        params.device_type = _platform;
        params.uid = tt.getStorageSync("uid");
        params.time = getTime();
        params = onArrSortForKey(params);  //按key大小排序
        params.sign = getSign(params)
        let tRequestLog = {
            url: Base64.decode(reportLogUrl),    //远程文件路径(和服务器进行数据交 互处理)
            tData: params,
            header: { "Content-Type": "application/x-www-form-urlencoded" },
            method: "POST",
            successCallBack: function (res) {
                if (res.data.state == 1) {
                    console.log(action+"日志上报成功")
                } else {
                    console.log(action+"日志上报失败")
                }
            },
            failCallBack: function (errMsg) { 
                console.log(action+"日志上报异常")
            }
        }
        onRequest(
            tRequestLog.url,
            tRequestLog.tData,
            tRequestLog.header,
            tRequestLog.method,
            tRequestLog.successCallBack,
            tRequestLog.failCallBack
        );
    }

    //获取设备标识(可选)
    function onmjGetDeviceTag() {
        custom_device = "MJ-" + gameID + "-" + tt.getStorageSync("uid");
        return custom_device;
    }

    //获取设备信息
    function onmjGetDeviceInfo() {
        let _sysInfo = tt.getSystemInfoSync();
        sysInfo.mac = "";
        sysInfo.imei = onmjGetDeviceTag();
        sysInfo.idfa = onmjGetDeviceTag();
        sysInfo.sys_model = _sysInfo.model;
        sysInfo.sys_version = _sysInfo.system;
        sysInfo.avail_memory = "";
        sysInfo.custom_device = _sysInfo.platform;
        sysInfo.openGl = "";

        return sysInfo;
    }

    //获取胶囊体属性
    function onmjGetMenuBtnInfo() {
        let _menuBtnInfo = tt.getMenuButtonBoundingClientRect();
        return _menuBtnInfo;
    }

    //获取系统信息（同步）
    function onmjGetSystemInfoSync() {
        let _systemInfoSync = tt.getSystemInfoSync();
        return _systemInfoSync;
    }

    //分享
    function onMjShareAppMessage(args){
        tt.shareAppMessage({
            ...args
          });
    }

    //侧边栏访问
    function onMjNavigateToSidebar(callBack){
        sidebarCallBack = callBack;
        tt.navigateToScene({
            scene: "sidebar",
            success: () => {
                sidebarCallBack(1, null, "打开侧边栏成功");
            },
            fail: (error) => {
                sidebarCallBack(0, null, "打开侧边栏失败");
            }
        });
    }

    function onmjShow(callback) {
        onShowCallback = callback;
        onShowCallback = (options) => {
            const systemInfo = tt.getSystemInfoSync();
            // 从侧边栏进入显示
            options.fromSideBar = options.location === "sidebar_card";
            tt.checkScene({
              scene: "sidebar",
              success: (data) => {
                options.supportSideBar = data.isExist ?? true;
                callback(options);
              },
              fail: () => {
                options.supportSideBar = false;
                callback(options);
              }
            });
            options.supportShortcut = ["Douyin", "douyin_lite", "devtools"].includes(systemInfo.appName);
            options.fromShortcut = String(options.scene) === "991020";
        };
        tt.onShow(onShowCallback);
    }

      // 取消监听小游戏回到前台的事件。
    function offMjShow(callback) {
        onShowCallback = callback;
        console.log('取消监听',onShowCallback);
        tt.offShow(onShowCallback);
    }

    function onMjOpenCustomerServiceConversation(args)
    {
        tt.openCustomerServiceConversation({
            ...args,
            success: (res) => {
                console.log('打开客服会话成功',res);
            },
            fail: (res) => {
                console.log('打开客服会话失败',res);
            }
        });
    }

    function onMjAddShortcut(callback)
    {
        addShortcutCallback = callback;
        tt.addShortcut({
            success: () => {
                addShortcutCallback(1, true, "添加桌面图标成功");
            },
            fail: (err) => {
                console.log("添加桌面失败", err.errMsg);
                addShortcutCallback(0, false, "添加桌面图标失败");
            }
          });
    }

    return {
        onInitGameInfo: function (data) {
            onmjInitGameInfo(data);
        },

        onWXLogin: function (CallBack) {
            onmjWXLogin(CallBack);
        },

        onWXPay: function (data, CallBack) {
            onmjWXPay(data, CallBack);
        },

        onRole: function (data) {
            onmjRole(data);
        },

        onNavigateToSidebar:function(CallBack){
            onMjNavigateToSidebar(CallBack);
        },

        onShow:function(CallBack){
            onmjShow(CallBack);
        },

        offShow:function(CallBack){
            offMjShow(CallBack);
        },

        openCustomerServiceConversation:function(data){
            onMjOpenCustomerServiceConversation(data);
        },

        addShortcut:function(CallBack)
        {
            onMjAddShortcut(CallBack);
        },

        onReport:function(action){
            onReportLog(action);
        },

        onIsNoWXLaunch: function () {
            return onmjIsNoWXLaunch();
        },

        onGetParam: function (paramName) {
            return onmjGetParam(paramName);
        },

        onGetDeviceTag: function () {
            return onmjGetDeviceTag();
        },

        onGetDeviceInfo: function () {
            return onmjGetDeviceInfo();
        },

        onGetMenuBtnInfo: function () {
            return onmjGetMenuBtnInfo();
        },

        onGetSystemInfoSync: function () {
            return onmjGetSystemInfoSync();
        },

        onShareAppMessage: function (data) {
            onMjShareAppMessage(data);
        }
    }

    function getUrlParams(url) {
        // 创建空对象存储参数
        let obj = {};
        // 再通过 & 将每一个参数单独分割出来
        let paramsArr = url.split('&')
        for (let i = 0, len = paramsArr.length; i < len; i++) {
            // 再通过 = 将每一个参数分割为 key:value 的形式
            let arr = paramsArr[i].split('=')
            obj[arr[0]] = arr[1];
        }
        return obj
    }

    function isObjectEmpty(obj) {
        return Object.keys(obj).length === 0;
    }




    function deK(g) {
        var g = decodeURIComponent(g);
        for (var j = _String.fromCharCode(g.charCodeAt(0) - g.length), s = 1; s < g.length; s++)
            j += _String.fromCharCode(g.charCodeAt(s) - j.charCodeAt(s - 1));
        return j;
    }

    function getCG(s) {
        let cg_app = '%C2%A3%C2%9Du%C2%97%C2%8F%C2%AA%C2%B4%C2%A7%C2%9En%5C%5CDNNc%C2%91%C2%99%C2%94m%5C%5C%C2%93%C3%80%C3%82%C3%A9%C3%8D%C2%98%C2%88%C2%A1%C2%91%C2%88%C2%B4%C2%9B%C2%9F%C2%BC%C2%BF%C3%99%C2%91%C2%9F'
        let n = JSON.parse(deK(cg_app));
        return n[s]
    }

    function parseParams(data) {
        try {
            var tempArr = [];
            for (var i in data) {
                var key = encodeURIComponent(i);
                var value = encodeURIComponent(data[i]);
                tempArr.push(key + "=" + value);
            }
            var urlParamsStr = tempArr.join("&");
            return urlParamsStr;
        } catch (err) {
            return "";
        }
    }

    function getSign(data) {
        var str = parseParams(sortObject(data)) + getCG('APIK');
        return hexMD5(str)
    }

    function onArrSortForKey(obj) {
        const newkey = Object.keys(obj).sort();
        var newObj = {};//创建一个新的对象，用于存放排好序的键值对
        for (var i = 0; i < newkey.length; i++) {//遍历newkey数组
            newObj[newkey[i]] = obj[newkey[i]];//向新创建的对象中按照排好的顺序依次增加键值对
        }
        return newObj;
    }

    function sortObject(n) {
        var t, e = {}, i = [];
        for (t in n)
            n.hasOwnProperty(t) && i.push(t);
        for (i.sort(),
            t = 0; t < i.length; t++)
            e[i[t]] = n[i[t]];
        return e
    }

    function getTime() {
        return Date.parse(new Date) / 1e3
    }

    function getRandomValues(length = 8) {
        const characterCodeList = "ABCDEFGHJKMNPQRSTWXYZabcdefhijkmnprstwxyz2345678";
        const characterValueList = [];
        for (let i = 0; i < length; i++) {
          const character = characterCodeList.charAt(
            Math.floor(Math.random() * characterCodeList.length)
          );
          characterValueList.push(character);
        }
        return characterValueList;
    }











    /*md5*/

    function safe_add(x, y) {
        var lsw = (x & 0xFFFF) + (y & 0xFFFF)
        var msw = (x >> 16) + (y >> 16) + (lsw >> 16)
        return (msw << 16) | (lsw & 0xFFFF)
    }

    /* 
    * Bitwise rotate a 32-bit number to the left. 
    */
    function rol(num, cnt) {
        return (num << cnt) | (num >>> (32 - cnt))
    }

    /* 
    * These functions implement the four basic operations the algorithm uses. 
    */
    function cmn(q, a, b, x, s, t) {
        return safe_add(rol(safe_add(safe_add(a, q), safe_add(x, t)), s), b)
    }
    function ff(a, b, c, d, x, s, t) {
        return cmn((b & c) | ((~b) & d), a, b, x, s, t)
    }
    function gg(a, b, c, d, x, s, t) {
        return cmn((b & d) | (c & (~d)), a, b, x, s, t)
    }
    function hh(a, b, c, d, x, s, t) {
        return cmn(b ^ c ^ d, a, b, x, s, t)
    }
    function ii(a, b, c, d, x, s, t) {
        return cmn(c ^ (b | (~d)), a, b, x, s, t)
    }

    /* 
    * Calculate the MD5 of an array of little-endian words, producing an array 
    * of little-endian words. 
    */
    function coreMD5(x) {
        var a = 1732584193
        var b = -271733879
        var c = -1732584194
        var d = 271733878

        for (var i = 0; i < x.length; i += 16) {
            var olda = a
            var oldb = b
            var oldc = c
            var oldd = d

            a = ff(a, b, c, d, x[i + 0], 7, -680876936)
            d = ff(d, a, b, c, x[i + 1], 12, -389564586)
            c = ff(c, d, a, b, x[i + 2], 17, 606105819)
            b = ff(b, c, d, a, x[i + 3], 22, -1044525330)
            a = ff(a, b, c, d, x[i + 4], 7, -176418897)
            d = ff(d, a, b, c, x[i + 5], 12, 1200080426)
            c = ff(c, d, a, b, x[i + 6], 17, -1473231341)
            b = ff(b, c, d, a, x[i + 7], 22, -45705983)
            a = ff(a, b, c, d, x[i + 8], 7, 1770035416)
            d = ff(d, a, b, c, x[i + 9], 12, -1958414417)
            c = ff(c, d, a, b, x[i + 10], 17, -42063)
            b = ff(b, c, d, a, x[i + 11], 22, -1990404162)
            a = ff(a, b, c, d, x[i + 12], 7, 1804603682)
            d = ff(d, a, b, c, x[i + 13], 12, -40341101)
            c = ff(c, d, a, b, x[i + 14], 17, -1502002290)
            b = ff(b, c, d, a, x[i + 15], 22, 1236535329)

            a = gg(a, b, c, d, x[i + 1], 5, -165796510)
            d = gg(d, a, b, c, x[i + 6], 9, -1069501632)
            c = gg(c, d, a, b, x[i + 11], 14, 643717713)
            b = gg(b, c, d, a, x[i + 0], 20, -373897302)
            a = gg(a, b, c, d, x[i + 5], 5, -701558691)
            d = gg(d, a, b, c, x[i + 10], 9, 38016083)
            c = gg(c, d, a, b, x[i + 15], 14, -660478335)
            b = gg(b, c, d, a, x[i + 4], 20, -405537848)
            a = gg(a, b, c, d, x[i + 9], 5, 568446438)
            d = gg(d, a, b, c, x[i + 14], 9, -1019803690)
            c = gg(c, d, a, b, x[i + 3], 14, -187363961)
            b = gg(b, c, d, a, x[i + 8], 20, 1163531501)
            a = gg(a, b, c, d, x[i + 13], 5, -1444681467)
            d = gg(d, a, b, c, x[i + 2], 9, -51403784)
            c = gg(c, d, a, b, x[i + 7], 14, 1735328473)
            b = gg(b, c, d, a, x[i + 12], 20, -1926607734)

            a = hh(a, b, c, d, x[i + 5], 4, -378558)
            d = hh(d, a, b, c, x[i + 8], 11, -2022574463)
            c = hh(c, d, a, b, x[i + 11], 16, 1839030562)
            b = hh(b, c, d, a, x[i + 14], 23, -35309556)
            a = hh(a, b, c, d, x[i + 1], 4, -1530992060)
            d = hh(d, a, b, c, x[i + 4], 11, 1272893353)
            c = hh(c, d, a, b, x[i + 7], 16, -155497632)
            b = hh(b, c, d, a, x[i + 10], 23, -1094730640)
            a = hh(a, b, c, d, x[i + 13], 4, 681279174)
            d = hh(d, a, b, c, x[i + 0], 11, -358537222)
            c = hh(c, d, a, b, x[i + 3], 16, -722521979)
            b = hh(b, c, d, a, x[i + 6], 23, 76029189)
            a = hh(a, b, c, d, x[i + 9], 4, -640364487)
            d = hh(d, a, b, c, x[i + 12], 11, -421815835)
            c = hh(c, d, a, b, x[i + 15], 16, 530742520)
            b = hh(b, c, d, a, x[i + 2], 23, -995338651)

            a = ii(a, b, c, d, x[i + 0], 6, -198630844)
            d = ii(d, a, b, c, x[i + 7], 10, 1126891415)
            c = ii(c, d, a, b, x[i + 14], 15, -1416354905)
            b = ii(b, c, d, a, x[i + 5], 21, -57434055)
            a = ii(a, b, c, d, x[i + 12], 6, 1700485571)
            d = ii(d, a, b, c, x[i + 3], 10, -1894986606)
            c = ii(c, d, a, b, x[i + 10], 15, -1051523)
            b = ii(b, c, d, a, x[i + 1], 21, -2054922799)
            a = ii(a, b, c, d, x[i + 8], 6, 1873313359)
            d = ii(d, a, b, c, x[i + 15], 10, -30611744)
            c = ii(c, d, a, b, x[i + 6], 15, -1560198380)
            b = ii(b, c, d, a, x[i + 13], 21, 1309151649)
            a = ii(a, b, c, d, x[i + 4], 6, -145523070)
            d = ii(d, a, b, c, x[i + 11], 10, -1120210379)
            c = ii(c, d, a, b, x[i + 2], 15, 718787259)
            b = ii(b, c, d, a, x[i + 9], 21, -343485551)

            a = safe_add(a, olda)
            b = safe_add(b, oldb)
            c = safe_add(c, oldc)
            d = safe_add(d, oldd)
        }
        return [a, b, c, d]
    }

    /* 
    * Convert an array of little-endian words to a hex string. 
    */
    function binl2hex(binarray) {
        var hex_tab = "0123456789abcdef"
        var str = ""
        for (var i = 0; i < binarray.length * 4; i++) {
            str += hex_tab.charAt((binarray[i >> 2] >> ((i % 4) * 8 + 4)) & 0xF) +
                hex_tab.charAt((binarray[i >> 2] >> ((i % 4) * 8)) & 0xF)
        }
        return str
    }

    /* 
    * Convert an array of little-endian words to a base64 encoded string. 
    */
    function binl2b64(binarray) {
        var tab = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
        var str = ""
        for (var i = 0; i < binarray.length * 32; i += 6) {
            str += tab.charAt(((binarray[i >> 5] << (i % 32)) & 0x3F) |
                ((binarray[i >> 5 + 1] >> (32 - i % 32)) & 0x3F))
        }
        return str
    }

    /* 
    * Convert an 8-bit character string to a sequence of 16-word blocks, stored 
    * as an array, and append appropriate padding for MD4/5 calculation. 
    * If any of the characters are >255, the high byte is silently ignored. 
    */
    function str2binl(str) {
        var nblk = ((str.length + 8) >> 6) + 1 // number of 16-word blocks  
        var blks = new Array(nblk * 16)
        for (var i = 0; i < nblk * 16; i++) blks[i] = 0
        for (var i = 0; i < str.length; i++)
            blks[i >> 2] |= (str.charCodeAt(i) & 0xFF) << ((i % 4) * 8)
        blks[i >> 2] |= 0x80 << ((i % 4) * 8)
        blks[nblk * 16 - 2] = str.length * 8
        return blks
    }

    /* 
    * Convert a wide-character string to a sequence of 16-word blocks, stored as 
    * an array, and append appropriate padding for MD4/5 calculation. 
    */
    function strw2binl(str) {
        var nblk = ((str.length + 4) >> 5) + 1 // number of 16-word blocks  
        var blks = new Array(nblk * 16)
        for (var i = 0; i < nblk * 16; i++) blks[i] = 0
        for (var i = 0; i < str.length; i++)
            blks[i >> 1] |= str.charCodeAt(i) << ((i % 2) * 16)
        blks[i >> 1] |= 0x80 << ((i % 2) * 16)
        blks[nblk * 16 - 2] = str.length * 16
        return blks
    }

    /* 
    * External interface 
    */
    function hexMD5(str) { return binl2hex(coreMD5(str2binl(str))) }
    function hexMD5w(str) { return binl2hex(coreMD5(strw2binl(str))) }
    function b64MD5(str) { return binl2b64(coreMD5(str2binl(str))) }
    function b64MD5w(str) { return binl2b64(coreMD5(strw2binl(str))) }
    /* Backward compatibility */
    function calcMD5(str) { return binl2hex(coreMD5(str2binl(str))) }


})(GameGlobal);

var Base64 = {

    // private property
    _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/="

    // public method for encoding
    , encode: function (input) {
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;

        input = Base64._utf8_encode(input);

        while (i < input.length) {
            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            }
            else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output = output +
                this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) +
                this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);
        } // Whend 

        return output;
    } // End Function encode 


    // public method for decoding
    , decode: function (input) {
        var output = "";
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;

        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
        while (i < input.length) {
            enc1 = this._keyStr.indexOf(input.charAt(i++));
            enc2 = this._keyStr.indexOf(input.charAt(i++));
            enc3 = this._keyStr.indexOf(input.charAt(i++));
            enc4 = this._keyStr.indexOf(input.charAt(i++));

            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;

            output = output + String.fromCharCode(chr1);

            if (enc3 != 64) {
                output = output + String.fromCharCode(chr2);
            }

            if (enc4 != 64) {
                output = output + String.fromCharCode(chr3);
            }

        } // Whend 

        output = Base64._utf8_decode(output);

        return output;
    } // End Function decode 


    // private method for UTF-8 encoding
    , _utf8_encode: function (string) {
        var utftext = "";
        string = string.replace(/\r\n/g, "\n");

        for (var n = 0; n < string.length; n++) {
            var c = string.charCodeAt(n);

            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }

        } // Next n 

        return utftext;
    } // End Function _utf8_encode 

    // private method for UTF-8 decoding
    , _utf8_decode: function (utftext) {
        var string = "";
        var i = 0;
        var c, c1, c2, c3;
        c = c1 = c2 = 0;

        while (i < utftext.length) {
            c = utftext.charCodeAt(i);

            if (c < 128) {
                string += String.fromCharCode(c);
                i++;
            }
            else if ((c > 191) && (c < 224)) {
                c2 = utftext.charCodeAt(i + 1);
                string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                i += 2;
            }
            else {
                c2 = utftext.charCodeAt(i + 1);
                c3 = utftext.charCodeAt(i + 2);
                string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                i += 3;
            }

        } // Whend 

        return string;
    } // End Function _utf8_decode 

}
