const systemInfo = tt.getSystemInfoSync();
let deviceId = null;
let accessBtn;
let onShowRes;
var isInit = false;
let videoAd = null;
var ta;
var isSidebar = false;
var ge;
var supportSideBar = true;
var isDeskEnter = false
let jsonBack = {
    result: 0
}
//var audioPlayers = tt.createInnerAudioContext();//音频对象

// 判断是否侧边栏或者桌面图标进入;
cqisdk
    .checkScene()
    .then((res) => {
        if (res.isSidebar) {
            //侧边栏进来
            isSidebar = true
        } else {
            //非侧边栏进来
            console.log("非侧边栏进来");
        }
        if (res.isDeskEnter) {
            //快捷方式
            isDeskEnter = true
        } else {
            //非快捷方式         
            console.log("非快捷方式进来");
        }
    })
    .catch((err) => {
        supportSideBar = false
        //失败的原因（一般是版本不支持侧边栏功能）
        console.log(`errmsg: ${err.errmsg}`);
    });

GameGlobal.SDK = (function(GameGlobal) {
    function ShowLog(args) {
        GameGlobal.YKSDKSendMessageToUnity("ShowLog:" + args);
    }

    //是否支持侧边栏
    function isSupportSideBar(args, callback) {
        return supportSideBar ? "1" : "0";
    }

    //是否侧边栏登录
    function isNavigateToSidebar(args, callback) {
        return isSidebar ? "1" : "0";
    }

    //是否快捷方式进入
    async function isDesktopShortcutEnter(args, callback) {
        try {
            if (isDeskEnter === true) {
                jsonBack.result = 1;
                callback(null, JSON.stringify(jsonBack))
            } else {
                jsonBack.result = 0;
                callback(null, JSON.stringify(jsonBack))
            }
        } catch (error) {
            jsonBack.result = 0;
            callback("error", JSON.stringify(error))
        }
    }


    //是否成功添加快捷方式(创建桌面图标) ----按照妙悦文档写入
    async function addDesktopShortcut(args, callback) {
        cqisdk
            .createShortcut("sidebar")
            .then(() => {
                jsonBack.result = 1;
                callback(null, JSON.stringify(jsonBack))
            })
            .catch((err) => {
                jsonBack.result = 0;
                callback("error", JSON.stringify(err));
            })
    }

    //是否有快捷方式
    async function hadDesktopShortcut(args, callback) {
        try {
            const res = await new Promise((resolve, reject) => {
                tt.checkShortcut({
                    success(res) {
                        if (res.status.exist) {
                            jsonBack.result = 1;
                            resolve(res);
                            callback(null, JSON.stringify(jsonBack))
                        } else {
                            jsonBack.result = 0;
                            callback(null, JSON.stringify(jsonBack))
                        }

                    },
                    fail(res) {
                        jsonBack.result = 0;
                        reject("failed");
                        callback("error", JSON.stringify(res))
                    }
                });
            });
        } catch (error) {
            jsonBack.result = 0;
            callback("error", JSON.stringify(error))
        }
    }

    //  侧边栏开启(复访) ----按照妙悦文档写入
    function openSidebar(args, callback) {
        cqisdk
            .navigateToScene("sidebar")
            .then(() => {
                callback(null, "success")
            })
            .catch((err) => {
                //失败的原因
                callback("error", "打开侧边栏失败" + err.errmsg)
            });
    }

    //客服----按照妙悦文档写入
    function openCustom(args, callback) {
        args = JSON.parse(args || '')
        cqisdk
            .openCustomerService(2)
            .then((res) => {
                callback(null, "拉起客服成功")
            })
            .catch((err) => {
                callback("failed", `errmsg: ${err.errmsg}`)
            });
    }

    function getOnShowRes(args, callback) {
        callback(null, onShowRes)
    }

    function init(options, callback) {
        cqisdk.init().then(() => {
            isInit = true
            callback(null, "success");
        }).catch((err) => {
            isInit = false
            callback("error", `[Init Error] ${err.errcode}: ${err.errmsg}`); // 初始化失败，传递错误信息
        });
    }

    //SDK初始化(unity内部的初始化)
    function _init(callback) {
        if (isInit == true) {
            callback(null, "_init success");
            return;
        }
        cqisdk.init().then(() => {
            isInit = true
            callback(null, "success");
        }).catch((err) => {
            isInit = false
            callback("error", `[Init Error] ${err.errcode}: ${err.errmsg}`); // 初始化失败，传递错误信息
        });
    }
    //SDK登录封装
    function _login(callback) {
        if (!isInit) {
            callback("SDK not initialized", "Please initialize SDK first");
            return;
        }
        cqisdk.login().then((res) => {
            var jsonObj = {
                "uid": res.uid,
                "username": res.username,
                "sign": res.sign
            };

            callback(null, JSON.stringify(jsonObj));
        }).catch((err) => {
            callback("error", "[" + err.errcode + "]" + err.errmsg);
        })
    }
    //登录（C#调用，会先判断是否初始化）
    function login(args, callback) {
        if (isInit) {
            _login(callback);
            return;
        } else {
            _init(function(res) {
                _login(callback);
                return;
            });
        }

    }
    //支付---按照妙悦文档写入
    function pay(args, callback) {
        args = JSON.parse(args || "{}");
        // roleId: args.RoleInfo.RoleId,
        //     roleName: args.RoleInfo.RoleName,
        //     serverId: args.RoleInfo.ServerId,
        //     serverName: args.RoleInfo.ServerName,
        //     roleLevel: Number(args.RoleInfo.RoleLevel),
        //     vipLevel: Number(args.RoleInfo.VipLevel),
        //     amount: Number(args.Price),
        //     cburl: args.Extension.payback_url,
        //     attach: args.OrderId,
        //     productId: args.ProductId,
        //     productName: args.ProductName,
        //     productDesc: Boolean(args.ProductDesc) || args.ProductName,
        let param = {
            roleId: "100001",
            roleName: "奥德赛",
            serverId: "1001",
            serverName: "1服",
            roleLevel: 0,
            vipLevel: 0,
            amount: 100,
            cburl: "12323",
            attach: "",
            productId: "101",
            productName: "100元宝",
            productDesc: "用于购买游戏道具",
        }
        cqisdk
            .pay(param)
            .then((res) => {
                console.log("支付成功11", res);
                callback(null, res)
            })
            .catch((err) => {
                console.log("支付失败22", err);
                callback("failed", `errcode: ${err.errcode}；errmsg: ${err.errmsg}`)
            });
    }

    // 获取头像
    function showAccessButton(args, callback) {
        args = JSON.parse(args || "{}");
        tt.getUserInfo({
            success(res) {
                callback(null, JSON.stringify(res.userInfo || ""))
            },
            fail(res) {
                callback("error", JSON.stringify(res.errMsg || ""))
            },
        });
    }

    function hideAccessButton(args, callback) {
        if (accessBtn) {
            accessBtn.destroy();
            accessBtn = null
        }
        callback(null, "hide success");
    }

    function getChannelId() {}

    function checkUpdate(args, callback) {
        var arg = {
            step: "dyn_update_info"
        }
        taTrack("app_step", arg)
        const updateManager = tt.getUpdateManager();
        updateManager.onCheckForUpdate((res) => {
            // 请求完新版本信息的回调
        });
        updateManager.onUpdateReady(() => {
            tt.showModal({
                title: '更新提示',
                content: '新版本已经准备好，是否重启应用？',
                success(res) {
                    if (res.confirm) {
                        // 新的版本已经下载好，调用 applyUpdate 应用新版本并重启
                        updateManager.applyUpdate();
                    }
                },
            });
        });
        updateManager.onUpdateFailed(() => {
            // 新版本下载失败
            callback('error', "download new version failed");
        });
    }

    function exitMiniProgram(args, callback) {
        tt.exitMiniProgram({
            isFullExit: true
        });
    }
    //上报角色---按照妙悦抖音sdk文档接入
    function reportRoleInfo(args, callback) {
        args = JSON.parse(args || "{}");
        switch (args.ReportType) {
            case 2:
                cqisdk.roleCreate({
                    roleId: args.RoleId,
                    roleName: args.RoleName,
                    serverId: args.ServerId,
                    serverName: args.ServerName,
                });
                break;
            case 3:
                cqisdk.enterGame({
                    roleId: args.RoleId,
                    roleName: args.RoleName,
                    serverId: args.ServerId,
                    serverName: args.ServerName,
                });
                break;
            case 4:
                console.log("sendRoleinfo roleUp")
                cqisdk.roleUp({
                    roleId: args.RoleId,
                    roleName: args.RoleName,
                    serverId: args.ServerId,
                    serverName: args.ServerName,
                    roleLevel: args.RoleLevel,
                })
                break;
            case 5:
                cqisdk.exitGame({
                    roleId: args.RoleId,
                    roleName: args.RoleName,
                    serverId: args.ServerId,
                    serverName: args.ServerName,
                    roleLevel: args.RoleLevel,
                });
                break;
        }
    }

    //上报角色
    function reportRoleInfoExtend(args, callback) {}

    function createTA(mConfig) {
        if (mConfig === null || mConfig === undefined || mConfig.appId.trim() === "") {
            return ta;
        }
        ta = new ThinkingDataAPI(mConfig)
        deviceId = ta.getDeviceId()
        ta.init();
        ta.setSuperProperties({
            session_id: Date.now()
        });
        return ta;
    }

    function getTA() {
        return ta;
    }

    function gameStepTrack(params) {
        if (ta === null || ta === undefined) {
            return;
        }
        ta.track(
            "app_step",
            params
        );
    }

    function taUserSet(args) {
        if (ta === null || ta === undefined) {
            return;
        }
        ta.userSet(args);
    }

    function taSetSuperProperties(args) {
        if (ta === null || ta === undefined) {
            return;
        }
        ta.setSuperProperties(args)
    }

    function taLogin(args) {
        if (ta === null || ta === undefined) {
            return;
        }
        ta.login(args)
    }

    function taTrack(event, params) {
        if (ta === null || ta === undefined) {
            return;
        }
        ta.track(event, params);
    }

    //初始化数数
    function setShushuConfig(args, callback) {
        args = JSON.parse(args || "{}");
        const {
            appId,
            serverUrl,
            enableLog
        } = args;
        console.log(appId + " " + serverUrl + " " + enableLog);
        var config = {
            appId: appId, // 项目 APP ID
            serverUrl: serverUrl, // 上报地址
            enableLog: enableLog,
            autoTrack: {
                appShow: true, // 自动采集 ta_mg_show
                appHide: true // 自动采集 ta_mg_hide
            }
        };

        //创建对象实例
        if (getTA() === null || getTA() === undefined) {
            createTA(config);
        }
    }

    //步骤统计打点
    function stepTrack(args, callback) {
        args = JSON.parse(args);
        if (args.step == "-20001") {
            console.log("load_completed");

        } else {
            gameStepTrack(
                args //事件属性
            );
        }
    }

    //游戏接口数据上报
    function track(args, callback) {
        const {
            event,
            param
        } = JSON.parse(args);
        taTrack(
            event,
            param
        );
    }

    function setSuperProperties(args, callback) {
        args = JSON.parse(args);
        taSetSuperProperties(
            args //公共属性
        );
    }

    // 设置用户属性
    function userSet(args, callback) {
        taUserSet(JSON.parse(args));
    }

    //设置账号ID
    function setAccountID(args, callback) {
        taLogin(args);
    }
    //直玩订阅能力
    function requestFeedSubscribe(args, callback) {
        args = JSON.parse(args || "{}")
        tt.requestFeedSubscribe({
            type: args.type,
            scene: args.scene,
            contentIDs: args.contentIDArray,
            success(res) {
                jsonBack.result = 1
                callback(null, JSON.stringify(jsonBack)) //成功了
            },
            fail(res) {
                jsonBack.result = 0
                callback("failed", JSON.stringify(jsonBack))
            },
        })

    }
    // 查询用户直玩订阅的授权情况
    function checkFeedSubscribeStatus(args, callback) {
        args = JSON.parse(args || "{}")
        tt.checkFeedSubscribeStatus({
            type: args.type,
            scene: args.scene,
            success(res) {
                jsonBack.result = 1
                callback(null, JSON.stringify(jsonBack)) //成功了
            },
            fail(res) {
                jsonBack.result = 0
                callback("failed", JSON.stringify(jsonBack))
            },
        })
    }
    // 上报加载完成时机 
    function reportScene(args, callback) {
        args = JSON.parse(args || "{}")
        let argParam
        try {
            if (typeof args.param === 'string') {
                argParam = JSON.parse(args.param);
            } else {
                argParam = args.param; // 如果 args.param 已经是对象，直接使用
            }
            // 验证 argParam 是否包含必要的属性
            if (!argParam || typeof argParam.sceneId !== 'number' || typeof argParam.costTime !== 'number') {
                callback("failed", 'Invalid param: sceneId or costTime is missing or invalid');
            }
            tt.reportScene({
                sceneId: argParam.sceneId,
                costTime: argParam.costTime,
                dimension: {},
                metric: {},
                success(res) {
                    jsonBack.result = 1
                    callback(null, JSON.stringify(jsonBack)) //成功了
                },
                fail(res) {
                    jsonBack.result = 0
                    callback("failed", JSON.stringify(jsonBack))
                }
            })
        } catch (error) {
            callback("failed", error); // 如果解析失败，捕获错误并调用回调函数，传递错误信息
        }
    }
    //激励广告
    function createRewardedVideoAd(args, callback) {
        args = JSON.parse(args || "{}")
        cqisdk.showRewardedVideoAd({
                name: args.goods_name,
                roleId: args.role_id,
                roleName: args.role_name,
                serverId: args.server_id,
                serverName: args.server_name,
                roleLevel: Number(args.level),
                vipLevel: Number(args.level),
            })
            .then((res) => {
                if (res.isEnded == 1) {
                    callback(null, "success");
                } else {
                    callback("error", "not end");
                }
            })
            .catch((err) => {
                callback("error", err.errmsg)
            });
    }

    function assambleRewardVideoErr(errCode, errMsg) {
        return JSON.stringify({
            state: "error",
            code: errCode,
            msg: errMsg
        })
    }

    //上报登录
    function loginStep(args, callback) {
        args = JSON.parse(args || "{}");
        const {
            accountId,
        } = args;
        taLogin(accountId);
    }

    function registerStep(args, callback) {
        args = JSON.parse(args || "{}");
        const {
            accountId,
        } = args;
    }

    function payStep(args, callback) {
        args = JSON.parse(args || "{}");
        const {
            transactionId,
            currencyAmount,
            currencyType,
            paymentType
        } = args;
    }


    //获取设备id
    function getDeviceId() {
        if (!deviceId && getTA() != null && getTA() != undefined) {
            deviceId = getTA().getDeviceId();
        }
        return deviceId;
    }

    function requestSubscribeMessage(args, callback) {
        args = JSON.parse(args || "{}")
        var requestData = {
            'ids': {}
        }
        cqisdk
            .subscribeMessage([args.tmplId])
            .then((res) => {
                for (var requestMessage in res) {
                    if (requestMessage != 'errMsg') {
                        requestData['ids'][requestMessage] = res[requestMessage]
                    }
                }
                callback(null, JSON.stringify(requestData))

            })
            .catch((res) => {
                console.log("订阅失败:" + res.errMsg + "  " + "code:" + res.errCode)
                callback(res.errMsg, res.errMsg + `(${res.errCode})`)
            });
    }

    function requestSubscribeSystemMessage(args, callback) {
        var requestData = {
            'ids': {}
        }
        tt.requestSubscribeSystemMessage({
            msgTypeList: ['SYS_MSG_TYPE_INTERACTIVE', 'SYS_MSG_TYPE_RANK', 'SYS_MSG_TYPE_WHATS_NEW'],
            success(res) {
                for (var requestMessage in res) {
                    if (requestMessage != 'errMsg') {
                        requestData['ids'][requestMessage] = res[requestMessage]
                    }
                }
                callback(null, JSON.stringify(requestData))
            },
            fail(res) {
                callback(res.errMsg, res.errMsg + `(${res.errCode})`)
            },
            complete(res) {
                console.log("requestSubscribeSystemMessage:", res)
            }
        })
    }

    //订阅
    function requestSubscribeMessageArray(args, callback) {
        args = JSON.parse(args || "{}");
        var array = args.tmplId.split(",");
        var requestData = {
            'ids': {}
        }
        cqisdk
            .subscribeMessage(array)
            .then((res) => {
                for (var requestMessage in res) {
                    if (requestMessage != 'errMsg') {
                        requestData['ids'][requestMessage] = res[requestMessage]
                    }
                }
                callback(null, JSON.stringify(requestData))
            })
            .catch((res) => {
                console.log("requestSubscribeMessageArray失败", res)
                callback(res.errMsg, res.errMsg + `(${res.errCode})`)
            });
    }

    //赋值剪切板内容
    function setClipboardData(args, callback) {
        args = JSON.parse(args || "{}")
        tt.setClipboardData({
            data: args.data,
            success(res) {
                callback(null, JSON.stringify({
                    'state': 'success'
                }))
            },
            fail(res) {
                callback("error", JSON.stringify({
                    'state': 'fail'
                }))
            }
        })
    }

    //获取剪切板内容
    function getClipboardData(args, callback) {
        tt.getClipboardData({
            success(res) {
                callback(null, JSON.stringify({
                    'state': res.data
                }))
            },
            fail(res) {
                callback("error", JSON.stringify({
                    'state': "fail"
                }))
            }
        })
    }

    //主动分享 shareAppMessage--妙悦-抖音文档
    function shareAppMessage(args, callback) {
        args = JSON.parse(args || "{}")
        let shareMessage = {
            "title": args.title || '',
            "imageUrl": args.imageUrl || '',
            "imageUrlId": args.imageUrlId || '',
            "path": args.path || '',
            "query": args.query || ''
        };
        cqisdk.share(shareMessage, true)
            .then((res) => {
                callback(null, "success")
            })
            .catch((err) => {
                callback("failed", `errcode:${err.errcode}；errmsg: ${err.errmsg}`)

            });
    }

    //被动分享 onShareAppMessage
    function setShareAppMessage(args, callback) {
        args = JSON.parse(args || "{}")
        let shareMessage = {
            "title": args.title || '',
            "imageUrl": args.imageUrl || '',
            "imageUrlId": args.imageUrlId || '',
            "path": args.path || '',
            "query": args.query || ''
        };
        cqisdk.share(shareMessage, true)
            .then((res) => {
                callback(null, "success")
            })
            .catch((err) => {
                callback("failed", `errcode:${err.errcode}；errmsg: ${err.errmsg}`)

            });
    }
    // （抖音官网文档无tt.onShareTimeline）
    function shareTimeline(args, callback) {
        args = JSON.parse(args || "{}")
        // 绑定分享参数
        tt.onShareTimeline(() => {
            callback(null, JSON.stringify({
                'state': 'onShareTimeline'
            }))
            return {
                title: args.title || '',
                imageUrl: args.imageUrl || '', // 图片 URL
                imageUrlId: args.imageUrlId || '',
                imagePreviewUrl: args.imagePreviewUrl || '',
                imagePreviewUrlId: args.imagePreviewUrlId || '',
                path: args.path || '',
                query: args.query || '',
                success() {
                    callback(null, "success")
                },
                fail(e) {
                    callback("failed", "error")
                },
            }
        })
    }

    function getCurrentTime(args, callback) {
        var currentTime = new Date().getTime(); // 创建一个Date对象，表示当前时间
        var currentTimePar = {
            'currentTime': currentTime,
        }
        callback(null, JSON.stringify(currentTimePar))
    }

    function getSettingState(args, callback) {
        tt.getSetting({
            success(res) {
                if (res.authSetting['scope.userInfo']) {
                    // 已经授权，可以直接调用 getUserInfo 获取头像昵称
                    tt.getUserInfo({
                        success: function(res) {
                            setTimeout(() => {
                                callback(null, JSON.stringify(res.userInfo));
                            }, 10);
                        }
                    });
                } else {
                    callback('没有授权', JSON.stringify(res.userInfo));
                }
            }
        });
    }

    function openNotchScreen(args, callback) {
        callback(null, "");
    }

    function getNotchScreen(args, callback) {
        function AddLocationDetail(value, addMsg) {
            if (value.trim() === '') {
                value = addMsg;
            } else {
                value = value + "|" + addMsg;
            }
            return value;
        }
        let systemInfo = tt.getSystemInfoSync();
        let safeArea = systemInfo.safeArea;
        let hasNotchScreen = false;
        let notchSize = 0;
        let locationDetail = "";
        if (safeArea.top >= 0) {
            locationDetail = AddLocationDetail(locationDetail, "top");
            notchSize = safeArea.top;
            hasNotchScreen = true;
        } else if (safeArea.left >= 0) {
            locationDetail = AddLocationDetail(locationDetail, "left");
            notchSize = safeArea.left;
            hasNotchScreen = true
        }
        let msg = JSON.stringify({
            'hasNotchScreen': hasNotchScreen,
            "notchWight": notchSize,
            "notchLocation": "top",
            "notchLocationDetail": locationDetail
        });
        callback(null, msg);
    }

    //获取网络类型
    function getNetworkType(args, callback) {
        tt.getNetworkType({
            success(res) {
                callback(null, JSON.stringify({
                    'networkType': res.networkType
                }));
            },
            fail(res) {
                callback("failed", res.errMsg);
            },
        });
    }

    //修改渲染帧率
    function setPreferredFramesPerSecond(args, callback) {
        args = JSON.parse(args || "{}")
        tt.setPreferredFramesPerSecond(args.fps);
    }

    const fs = tt.getFileSystemManager()
    const RecorderManager = tt.getRecorderManager()
    let isRecording = false; // 标记是否正在录音
    let back_json = {
        "id": null
    }; //返回值id
    let audioFlag // 标记是否异常
    let audioMsg = null //异常原因
    //开始录音
    function startRecordAudio(args, callback) {
        args = JSON.parse(args || "{}")
        audioFlag = 0
        if (isRecording == true) {
            RecorderManager.onStop(() => {});
            RecorderManager.stop();
        }
        RecorderManager.start({
            format: "mp3",
        })
        if (audioFlag == 0) {
            isRecording = true; // 设置录音状态为true
            back_json.id = args.id
            callback(null, JSON.stringify(back_json))
        } else {
            callback("error", audioMsg)
        }
    }

    RecorderManager.onError((res) => {
        audioFlag = 1
        audioMsg = res.errMsg
    });

    //停止录音
    function stopRecordAudio(args, callback) {
        args = JSON.parse(args || "{}")
        audioFlag = 0
        if (isRecording == true) {
            RecorderManager.stop()
            RecorderManager.onStop((res) => {
                let readRes = fs.readFileSync(res.tempFilePath, "base64");
                let json = {
                    byteMessage: readRes,
                    format: "mp3",
                    id: args.id
                }
                RecorderManager.onStop(() => {});
                isRecording = false
                if (audioFlag == 1) {
                    callback("error", audioMsg)
                } else {
                    callback(null, JSON.stringify(json))
                }
            });
        } else {
            callback("error", "not begin")
        }
    }

    //从URL播放音频
    function playAudioFromUrl(args, callback) {
        const innerAudioContext = tt.createInnerAudioContext();
        /*
        args = JSON.parse(args || "{}")
        audioFlag = 0
        audioPlayers.destroy()
        audioPlayers = tt.createInnerAudioContext()
        audioPlayers.src=args.url
        audioPlayers.play();
        back_json.id = args.id
        if(audioFlag == 0){
            callback(null,JSON.stringify(back_json))
        }else{
            callback("error",audioMsg)
        }
        */
    }

    /*
    audioPlayers.onEnded(() => {
        console.log('音频播放完成');
        unityInstance.SendMessage("YKSDK_MainGameObject", "OnJsMessage", JSON.stringify({"funcName":"playAudioFromUrlComplete","err":null,"args" :JSON.stringify({"id" : back_json.id}) }));
        // 在这里执行播放完成后的逻辑
    });
    */

    //继续播放url
    function resumeAudioFromUrl(args, callback) {
        /*
        args = JSON.parse(args || "{}")
        audioFlag = 0
        audioPlayers.play();
        back_json.id = args.id
        if(audioFlag == 0){
            callback(null,JSON.stringify(back_json))
        }else{
            callback("error",audioMsg)
        }
        */
    }

    /*
    //监听音频错误
    audioPlayers.onError((res) => {
        audioFlag = 1
        audioMsg = res.errMsg
    });
    */

    //暂停URL音频
    function pauseAudioFromUrl(args, callback) {
        /*
        args = JSON.parse(args || "{}")
        audioFlag = 0
        audioPlayers.pause()
        back_json.id = args.id
        if(audioFlag == 0){
            callback(null,JSON.stringify(back_json))
        }else{
            callback("error",audioMsg)
        }
        */
    }

    //停止URL音频
    function stopAudioFromUrl(args, callback) {
        /*
        args = JSON.parse(args || "{}")
        audioFlag = 0
        audioPlayers.stop();
        audioPlayers.destroy()
        back_json.id = args.id
        if(audioFlag == 0){
            callback(null,JSON.stringify(back_json))
        }else{
            callback("error",audioMsg)
        }
        */
    }

    return {
        "resumeAudioFromUrl": resumeAudioFromUrl,
        "stopAudioFromUrl": stopAudioFromUrl,
        "pauseAudioFromUrl": pauseAudioFromUrl,
        "playAudioFromUrl": playAudioFromUrl,
        "stopRecordAudio": stopRecordAudio,
        "startRecordAudio": startRecordAudio,
        "ShowLog": ShowLog,
        "openCustom": openCustom,
        "getOnShowRes": getOnShowRes,
        "init": init,
        "_init": _init,
        // "gg": openAdvertisement,
        "_login": _login,
        "login": login,
        "pay": pay,
        "showAccessButton": showAccessButton,
        "hideAccessButton": hideAccessButton,
        "checkUpdate": checkUpdate,
        "getChannelId":getChannelId,
        "exitMiniProgram": exitMiniProgram,
        "reportRoleInfo": reportRoleInfo,
        "reportRoleInfoExtend": reportRoleInfoExtend,
        "setShushuConfig": setShushuConfig,
        "stepTrack": stepTrack,
        "track": track,
        "setSuperProperties": setSuperProperties,
        "userSet": userSet,
        "setAccountID": setAccountID,
        "createRewardedVideoAd": createRewardedVideoAd, // 激励广告
        "requestFeedSubscribe": requestFeedSubscribe, //直玩订阅能力
        "checkFeedSubscribeStatus": checkFeedSubscribeStatus, //查询直玩订阅
        "reportScene": reportScene, //上报加载完成时机 
        "assambleRewardVideoErr": assambleRewardVideoErr,
        "loginStep": loginStep,
        "registerStep": registerStep,
        "payStep": payStep,
        "getDeviceId": getDeviceId,
        "requestSubscribeMessage": requestSubscribeMessage,
        "requestSubscribeSystemMessage": requestSubscribeSystemMessage,
        "requestSubscribeMessageArray": requestSubscribeMessageArray,
        "setClipboardData": setClipboardData,
        "getClipboardData": getClipboardData,
        "shareAppMessage": shareAppMessage,
        "setShareAppMessage": setShareAppMessage,
        "shareTimeline": shareTimeline,
        "getCurrentTime": getCurrentTime,
        "getSettingState": getSettingState,
        "openNotchScreen": openNotchScreen,
        "getNotchScreen": getNotchScreen,
        "openSidebar": openSidebar,
        "isNavigateToSidebar": isNavigateToSidebar,
        "getNetworkType": getNetworkType,
        "setPreferredFramesPerSecond": setPreferredFramesPerSecond,
        "isSupportSideBar": isSupportSideBar,
        "hadDesktopShortcut": hadDesktopShortcut,
        "addDesktopShortcut": addDesktopShortcut,
        "isDesktopShortcutEnter": isDesktopShortcutEnter
    };
})(GameGlobal);