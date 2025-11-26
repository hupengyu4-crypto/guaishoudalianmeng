GameGlobal.YKSDK_call = function (name, args, cid) {
    let callback = null;
    let callbackCalled = false;
    if (cid > 0) {
        callback = function (err, args) {
            callbackCalled = true;
            GameGlobal.YKSDKSendMessageToUnity(JSON.stringify({id: cid, err, args}));
        }
    }

    try {
        let method = GameGlobal.SDK[name];
        if (!method) {
            console.error("YKSDK_JS\t\tfunction " + name + " not found");
            let err = "call js faild function " + name + " not found";
            if (null != callback && !callbackCalled) {
                callback(err);
            }

            return;
        }

        let func = method(args, callback);
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

GameGlobal.YKSDKSendMessageToUnity = function (param) {
    setTimeout(function () {
        globalUnityInstance.SendMessage("YKSDK_MainGameObject", "OnJsCallbackMessage", param);
    }, 1);
}