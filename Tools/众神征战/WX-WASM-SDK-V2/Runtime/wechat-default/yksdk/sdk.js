import './wxgamesdk_zs'
const sysInfo = wx.getSystemInfoSync();
let userInfo = null;

let accessBtn;


function init(args,callback) {
    console.log('js init');
    /*此处使用提供的参数*/
    let data = {
        yisdk_param: "mZhlY6GojeXc0s7H0bk%3D",
        ext_param: "ZZRsZ6Cr",
        test: true
    }
    /*jsLoad接口*/
    gameSDK.jsLoad(data).then(res=>{
        if(res.statusCode==0){
            console.log(res)
            if (res.is_ban) {
                // 切换审核版本的场景
                console.log('审核版')
            } else {
                // 切换正式游戏版本场景
                console.log('正式版')
            }
        }
    })
    let params = {
        showMenu:false,
        passShareTitle:'标题看看',
        passShareSrc:'https://xxx.com/1.jpg'
    }
    gameSDK.init(params).then(res => {
        console.log("小游戏初始化结果" + res);
        if(res.statusCode == 0){
            callback(null,res.status);
        }else{
            callback(res.status,res.status);
        }
    });
}


function login(args, callback) {
    console.log(args)
    /**
     * loginParams: {statusCode: 0, user_id: "33001807619", openId: "", time: "1711612092", new_sign: "645234bb9f39ae7470315ebf12c393d0", …}
     statusCode: 0
     */
    gameSDK.login().then(res => {
        console.log("登录结果", res);
        if (res.statusCode == 0) {
            callback(null,JSON.stringify(res.loginParams));
        }else{
            callback(res.statusCode,"login failed");
        }
    });
}
function createRole(args, callback){
    console.log("createRole");
    console.log(args);
    args = JSON.parse(args || "{}");
    const {
        isLogin,
        server_id,
        server_name,
        role_id,
        role_name,
        level,
        extra,
        createRoletime
    } = args;
    let gameData = {
        // 1. 通用字段
        // 1.1 融合和客户端通用字段
        roleId: role_id,
        roleName: role_name,
        roleLevel: level,
        serverId: server_id,
        serverName: server_name,
        vipLevel: '0',
        userMoney: '0',
        // 1.2 客户端额外通用字段
        roleCTime: createRoletime, // string	Y	角色创建时间（10位unix时间戳）
        gender: '', // string	Y	性别（若无传“”，有传男、女）
        professionId: 0, // int	Y	当前登录玩家的职业ID（若无传0）
        profession: '', // string	Y	当前登录玩家的职业名称（若无传“”）
        power: 0, // int	Y	战力值 （若无传0）
        partyid: '', // string	Y	当前玩家的所属帮派帮派ID（若无传“”）
        partyname: '', // string	Y	当前玩家的所属帮派帮派名称（若无传“”）
        partyroleid: 0, // int	Y	帮派称号ID（若无传0）
        partyrolename: '', // string	Y	帮派称号名称（若无传“”）
        friendList: [],
        banlance: [], // 钱包
        payTotal: 0, // 累计充值金额
        roleLevelUpTime: '0', // 角色升级时间
    }
    console.log("gameSDK.createRole");
    gameSDK.createRole(gameData).then(res => {
        console.log("上报创角结果", res);
    })
}
function enterGame(args, callback){
    console.log("enterGame");
    console.log(args);
    args = JSON.parse(args || "{}");
    const {
        isLogin,
        server_id,
        server_name,
        role_id,
        role_name,
        level,
        extra,
        createRoletime
    } = args;
    let gameData = {
        // 1. 通用字段
        // 1.1 融合和客户端通用字段
        roleId: role_id,
        roleName: role_name,
        roleLevel: level,
        serverId: server_id,
        serverName: server_name,
        vipLevel: '0',
        userMoney: '0',
        // 1.2 客户端额外通用字段
        roleCTime: createRoletime, // string	Y	角色创建时间（10位unix时间戳）
        gender: '', // string	Y	性别（若无传“”，有传男、女）
        professionId: 0, // int	Y	当前登录玩家的职业ID（若无传0）
        profession: '', // string	Y	当前登录玩家的职业名称（若无传“”）
        power: 0, // int	Y	战力值 （若无传0）
        partyid: '', // string	Y	当前玩家的所属帮派帮派ID（若无传“”）
        partyname: '', // string	Y	当前玩家的所属帮派帮派名称（若无传“”）
        partyroleid: 0, // int	Y	帮派称号ID（若无传0）
        partyrolename: '', // string	Y	帮派称号名称（若无传“”）
        friendList: [],
        banlance: [], // 钱包
        payTotal: 0, // 累计充值金额
        roleLevelUpTime: '0', // 角色升级时间
    }
    gameSDK.changeRole(gameData).then(res => {
        console.log("上报创角结果", res);
    })
}
function levelup(args, callback){
    console.log("levelup");
    console.log(args);
    args = JSON.parse(args || "{}");
    const {
        isLogin,
        server_id,
        server_name,
        role_id,
        role_name,
        level,
        extra,
        createRoletime
    } = args;
    let gameData = {
        // 1. 通用字段
        // 1.1 融合和客户端通用字段
        roleId: role_id,
        roleName: role_name,
        roleLevel: level,
        serverId: server_id,
        serverName: server_name,
        vipLevel: '0',
        userMoney: '0',
        // 1.2 客户端额外通用字段
        roleCTime: createRoletime, // string	Y	角色创建时间（10位unix时间戳）
        gender: '', // string	Y	性别（若无传“”，有传男、女）
        professionId: 0, // int	Y	当前登录玩家的职业ID（若无传0）
        profession: '', // string	Y	当前登录玩家的职业名称（若无传“”）
        power: 0, // int	Y	战力值 （若无传0）
        partyid: '', // string	Y	当前玩家的所属帮派帮派ID（若无传“”）
        partyname: '', // string	Y	当前玩家的所属帮派帮派名称（若无传“”）
        partyroleid: 0, // int	Y	帮派称号ID（若无传0）
        partyrolename: '', // string	Y	帮派称号名称（若无传“”）
        friendList: [],
        banlance: [], // 钱包
        payTotal: 0, // 累计充值金额
        roleLevelUpTime: '0', // 角色升级时间
    }
    gameSDK.upgradeRole(gameData).then(res => {
        console.log("上报创角结果", res);
    })
}

function pay(args, callback) {
    console.log("js pay");
    console.log(args);
    args = JSON.parse(args || "{}");
    const {
        name,
        orderid,
        amount,
        serverid,
        servername,
        userid,
        username,
        level,
        goodid,
        paycallback,
        pay_image
    } = args;
    console.log(orderid);
    gameSDK.recharge({
        amount: amount*100, //  金额，分
        cpProductId: goodid, // 商品ID
        productName: name,
        callbackURL: paycallback, // cp充值回调地址
        callbackInfo: orderid, // cp透传参数,可以是研发订单
        chargeDesc: name, //	支付描述信息	N	String
        chargeMount: 10, //	充值游戏币数量	N	Int
        orderId: orderid, //	订单id	Y	String
        per_price: 10, //	单价	Y	Int
        serverId: serverid, // 服务器id
        serverName: servername, // 服务器名称
        roleId: userid, // 角色id
        roleName: username, // 角色名称
        rate: 10, //	人民币与游戏币比例（比如 1 人民=100 元宝）	
        roleLevel: level, //	角色等级	
        sociaty: "无帮派", //	公会/帮派名	
        lastMoney: "0", //	用户余额
        vipLevel: level, //	Vip 等级
    }).then((res) => {
        console.log('支付',res)
    })

}

function requestSubscribeMessageArray(args, callback) {
    if (args == null) {
        callback("id=null", "id=null");
        return;
    }
    var array = args.split(",");
    console.log("Array:", array);

    var requestData = {
        'ids': {}
    }
    wx.requestSubscribeMessage({
        tmplIds: array,
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
        }
    })
}

function exit() {

}


// buttonInfo
// x y w h 按钮在游戏窗口坐标和大小
// ww hh 游戏窗口宽高
// dev 如果dev，则在按钮区域显示一个半透明红色遮罩（开发时确认位置用）
// 警告，如果离开当前界面，请一定调用hideAccessButton
function showAccessButton(args, callback) {

    args = JSON.parse(args || "{}");
    const {
        x,
        y,
        w,
        h,
        ww,
        hh,
        dev
    } = args;

    const rx = sysInfo.windowWidth / ww;
    const ry = sysInfo.windowHeight / hh;

    accessBtn = wx.createUserInfoButton({
        type: 'text',
        text: '',
        style: {
            left: x * rx,
            top: y * ry,
            width: w * rx,
            height: h * ry,
            backgroundColor: dev ? 'rgba(255, 0, 0, .5)' : 'transparent',
        }
    });
    accessBtn.onTap((res) => {
        accessBtn.destroy();
        accessBtn = null;
        if (res.userInfo) {
            userInfo = res.userInfo;
        }
        callback(!userInfo ? 'error' : null, JSON.stringify(userInfo));
    });


}

function hideAccessButton(args, callback) {
    console.log("hideAccessButton")
    if (accessBtn) {
        accessBtn.destroy();
    }
    accessBtn = null;
    callback(null, "hide success");

}

function checkUpdate(args, callback) {
    console.log("check upate")
    const updateManager = wx.getUpdateManager();

    updateManager.onCheckForUpdate((res) => {
        // 请求完新版本信息的回调
        // console.log(res.hasUpdate)
    });

    updateManager.onUpdateReady(() => {
        wx.showModal({
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

function getBatteryInfo(args, callback) {
    wx.getBatteryInfo({

        success(res) {
            var resultjson = {
                "level": res.level,
                'isCharging': res.isCharging
            };
            callback(null, JSON.stringify(resultjson));
        },
        fail(res) {
            callback('error', "")
        }
    })
}

// console.log("加载成功")
// var result = {
//     'rewarded': 'load',
//     'adUnitId': adUnitId,
//     'state': 'success'
// }

// console.log("展示成功")
// result = {
//     'rewarded': 'open',
//     'adUnitId': adUnitId,
//     'state': 'success'
// }

// console.log("加载失败", err)
// result = {
//     'rewarded': 'load',
//     'adUnitId': adUnitId,
//     'state': 'failed',
//     'msg': err
// }

// // 正常播放结束，可以下发游戏奖励
// var result = {
//     'rewarded': 'show',
//     'adUnitId': adUnitId,
//     'state': 'success'
// }

// 播放中途退出，不下发游戏奖励
// var result = {
//     'rewarded': 'show',
//     'adUnitId': adUnitId,
//     'state': 'failed'
// }

// sendToUnityMessage(JSON.stringify(result))
function createRewardedVideoAd(args, callback) {
    console.log("createRewardedVideoAd", args)
    args = JSON.parse(args || "{}");
    const {
        cp_order_id,
        goods_id,
        goods_name,
        role_id,
        role_name,
        server_id,
        server_name,
        level,
        extend_params
    } = args;
    var adOrderJson = {
        cp_order_id:cp_order_id,
        goods_id:goods_id,
        goods_name:goods_name,
        role_id:role_id,
        role_name:role_name,
        server_id:server_id,
        server_name:server_name,
        level:level,
        extend_params:extend_params,
    }

}

function sendToUnityMessage(msg) {
    GameGlobal.Module.SendMessage("YKSDK_MainGameObject", "OnJsBackMessage", msg); //send to unity
}
export default {
    init, //必须
    login,
    pay,
    showAccessButton,
    hideAccessButton,
    checkUpdate,
    requestSubscribeMessageArray,
    getBatteryInfo,
    createRewardedVideoAd,
    createRole,
    enterGame,
    levelup
}