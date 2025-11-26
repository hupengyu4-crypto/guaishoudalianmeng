import { YKSDK } from './index'

GameGlobal.YKSDK_init = function (options,cid) {
    console.info("YKSDK_JS\t\tstart YKSDK_init 。。。", options, cid);
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
    YKSDK.init && YKSDK.init(options,callback);
}

//
GameGlobal.YKSDK_call = function (name, args, cid) {
    let callback = null;
    let globalCallback = null;
    let callbackCalled = false;
    if (cid > 0) {
        callback = function (err, args) {
            callbackCalled = true;
            setTimeout(function () {
                GameGlobal.Module.SendMessage("YKSDK_MainGameObject", "OnJsCallbackMessage", JSON.stringify({ id: cid, err, args })); //send to unity
            }, 1);
        };
        // globalCallback = function (err, args, isglobal){
        //     GameGlobal.YKSDKSendMessageToUnity(name, err, args)
        // }
    }
    try {
        console.debug("YKSDK_JS\t\tcall function " + name + " from unity");
        const fn = YKSDK[name];
        if (!fn) {
            //TODO 优化，当调用不存在的方法时不抛出异常
            // throw new Error("YKSDK_JS\t\tfunction " + name + " not found");
            console.error("YKSDK_JS\t\tfunction " + name + " not found")
            return 
        }
        let func = fn(args, callback)
        return func;
    } catch (e) {
        console.error("YKSDK_JS\t\tcall function " + name + " failed\n", e);
        let err = "call js faild";
        if (e) {
            err = "call js faild: " + (e.toString ? e.toString() : e + "");
        }
        if (null != callback && !callbackCalled) {
            callback(err);
        }
    }
}


GameGlobal.YKSDKSendMessageToUnity = function (funcName,err,args) {
    setTimeout(function () {
        GameGlobal.Module.SendMessage("YKSDK_MainGameObject", "OnJsMessage", JSON.stringify({funcName,err,args}));
    },1)
}