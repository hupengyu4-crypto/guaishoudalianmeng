import { ta, reyun } from './analytics';
ta.track("launch", { step: 0 });
ta.flush();

import config from "./config";

console.log(">>> launch args:", wx.getLaunchOptionsSync());


GameGlobal.YKSDK_init = function (options) {
    console.info("YKSDK_JS\t\tstart YKSDK_init 。。。", options);
    YKSDK.init && YKSDK.init(options);
}
GameGlobal.YKSDK_call = function (name, args, cid) {
    let callback = null;
    let callbackCalled = false;
    if (cid > 0) {
        callback = function (err, args) {
            callbackCalled = true;
            setTimeout(function () {
                GameGlobal.Module.SendMessage("YKSDK_MainGameObject", "OnJsCallbackMessage", JSON.stringify({ id: cid, err, args })); //send to unity
            }, 1);
        };
    }
    try {
        console.debug("YKSDK_JS\t\tcall function " + name + " from unity");
        const fn = YKSDK[name];
        if (!fn) {
            throw new Error("YKSDK_JS\t\tfunction " + name + " not found");
        }
        return fn(args, callback);
    } catch (e) {
        console.error("YKSDK_JS\t\tcall function " + name + " failed\n", args, e);
        const err = "call js faild";
        if (e) {
            err = "call js faild: " + (e.toString ? e.toString() : e + "");
        }
        if (null != callback && !callbackCalled) {
            callback(err);
        }
    }
}


GameGlobal.YKSDKSendMessageToUnity = function (args) {
    GameGlobal.Module.SendMessage("YKSDK_MainGameObject", "OnJsMessage", args);
}



export function launchStep(step, args) {
    ta.track("launch", { step, ...args });
    // console.debug("launch", step, args);
    ta.flush();
    if (config.reyun.customEvent) {
        reyun.event("event_1", { step, ...args });
    }
}




export function track(args) {
    args = JSON.parse(args || "null");
    if (!args || !args.event) {
        console.warn("error track args");
        return;
    }
    ta.track(args.event, args.args);
    if (!config.reyun.customEvent) {
        return;
    }
    const events = {
        sdk_step: "event_2",
        app_step: "event_3",
    };
    let e = events[args.event] || "event_30";
    reyun.event(args.event, { event_src: args.event, ...args.args });
}

function trackWXLogin(str) {

    let us = JSON.parse(str);

    console.log(us);
    GameGlobal.$openId = us.open_id;
    if (config.reyun.enable) {
        reyun.__initSelf(us.open_id);
    }
    // ta.login(uid);

    // reyun.register(uid);
    // reyun.loggedin(uid);
    // console.log(us);
}

function trackLogout() {
    ta.logout();
}

let deviceId = ta.getDeviceId();
function getDeviceId(_, callback) {
    if (!deviceId) {
        deviceId = ta.getDeviceId();
    }
    callback(null, deviceId);
}


function otherTrack(args) {

    console.log("OtherTrack", args);
    if (!config.reyun.enable) {
        return;
    }
    const info = JSON.parse(args);
    const step = info.step;
    delete info.step;

    switch (step) {
        case "login":
            reyun.loggedin(info.user_id);
            return;

        case "register":
            reyun.register(info.user_id, args);
            return;
        case "pay":
            reyun.payment(info.order_id, info.price, "CNY", "weixinpay");
            return;
    }
    console.warn("错误广告打点信息:" + step);

}
//缓存请求到的自定义扩展数据
let YKSDK_init_config = null;
// 替换c#中getInitExtraData网络请求部分
function _getInitExtraData(_, callback) {
    console.log("_getInitExtraData:"+ JSON.stringify(YKSDK_init_config))
    callback(null, JSON.stringify(YKSDK_init_config));
}
export function startGame() {
    // 请求挂接渠道配置
    console.log(`${config.youkia.LOGINTHREE_URL}`.replace("LoginThree","resUpdate")+'?ver='+`${config.youkia.version}`)
    wx.request({
        url: `${config.youkia.LOGINTHREE_URL}`.replace("LoginThree","resUpdate")+'?ver='+`${config.youkia.version}`,
        success: (res) => {
            if (res.statusCode !== 200 || parseInt(res.data.status) !== 1) {
                console.error("YKSDK_JS\tload init config faild:", res.statusCode, res.data);
                preloadFaild();
                return;
            }
            const cfg = res.data.data;
            console.info("YKSDK_JS\tload init config success:", cfg);
            YKSDK_init_config = cfg;

            let cdn = cfg.res_cdn || GameGlobal.DATA_CDN;

            if (!cdn) {
                console.info("YKSDK_JS\tNo preloading is required")
                // 开始启动unity
                GameGlobal.manager.startGame();
                return;
            }
            if (!cdn.startsWith("https://")) {
                console.error("YKSDK_JS\tload init config faild: cdn url error: ", cdn);
                preloadFaild();
                return;
            }
            cdn = cdn.replace(/\/+$/, "");
            let res_pre_list = '';
            if (cfg.res_pre_list) {
                // 去除两端空白
                res_pre_list = cfg.res_pre_list.replace(/\s+$/, '').replace(/^\s+/, '');
            }

            if (res_pre_list && !isEmptyJson(res_pre_list)) {
                console.info("YKSDK_JS\t use config res_pre_list", res_pre_list);
                try {
                    const { list } = JSON.parse(res_pre_list);

                    if (!Array.isArray(list)) {
                        console.error("YKSDK_JS\rconfig \"es_pre_list\" format error:", res.res_pre_list);
                        preloadFaild();
                        return;
                    }
                    parsePreLoadList(cdn, list);
                }
                catch (e) {
                    console.error("YKSDK_JS\t parse config \"res_pre_list\" faild:", e);
                    preloadFaild();
                    return;
                }
                // 开始启动unity
                GameGlobal.manager.startGame();
                return;
            }


            const url = `${cdn}/${encodeURIComponent(cfg.res_version)}/preload-list.txt`
            console.info("YKSDK_JS\tuse preload-list.txt url", url);
            // 请求预加载列表
            wx.request({
                url,
                success: (res) => {
                    if (res.statusCode !== 200) {
                        console.error("YKSDK_JS\tload preload-list.txt faild:", res);
                        preloadFaild();
                        return;
                    }
                    try {
                        let list = res.data.list;
                        if (!Array.isArray(list)) {
                            console.error("YKSDK_JS\tpreload-list.txt format error:", res.data);
                            preloadFaild();
                            return;
                        }
                        parsePreLoadList(cdn, list);
                    } catch (e) {
                        console.error("YKSDK_JS\tpreload-list.txt parse faild:", e);
                        preloadFaild();
                        return;
                    }
                    // 开始启动unity
                    GameGlobal.manager.startGame();
                },
                fail: (err) => {
                    console.error(err);
                    preloadFaild();
                },
            });
        },
        fail: (e) => {
            console.error("YKSDK_JS\tload init config faild:", e);
            preloadFaild();
        },
    })

}


let preLoadFaileShow = false;
function preloadFaild() {
    wx.hideLoading();
    if (preLoadFaileShow) {
        return;
    }
    preLoadFaileShow = true;
    wx.showModal({
        title: '提示',
        content: '游戏资源加失败！！！',
        showCancel: false,
        confirmText: "重新加载",
        success: () => {
            wx.showLoading({
                title: '加载中...',
            });
            preLoadFaileShow = false;
            startGame();
        }
    });
}

function parsePreLoadList(cdn,list) {

    // 合并打包工具原配置列表
    list = list.concat((GameGlobal.managerConfig.preloadDataList || []).
        // 过滤空
        filter((u) => u));

    // 处理相对路径（相对cdn配置）
    for (let i = 0; i < list.length; i++) {
        let u = list[i];
        if (u.startsWith("http://") || u.startsWith("https://")) {
            continue;
        }
        u = u.replace(/^\/+/g, '');
        list[i] = `${cdn}/${u}`;
    }
    // 配置列表
    GameGlobal.manager.setPreloadList(list);
    console.info("YKSDK_JS\tpreload resource config success:", list);
    wx.hideLoading();
}

//检查字符串是否是一个空的json对象
function isEmptyJson(str) {
    try {
        var obj = JSON.parse(str);
        return Object.keys(obj).length === 0 && obj.constructor === Object;
    } catch (e) {
        return false;
    }
}

import functions from './sdk';
// console.log((functions));
const YKSDK = {
    ...functions,
    getDeviceId,
    track,
    trackWXLogin,
    trackLogout,
    otherTrack,
    _getInitExtraData
};