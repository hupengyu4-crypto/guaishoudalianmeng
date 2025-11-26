import { getTA, createTA, taTrack, gameStepTrack, taSetSuperProperties, taLogin, taUserSet } from './ta-manager';
const {
  cqisdk
} = require('/sdk/cqisdk')
const sysInfo = wx.getSystemInfoSync();
let deviceId = null;
let accessBtn;
let onShowRes;

wx.onShow((result) => {
  console.log('onShow' + result)
  onShowRes = JSON.stringify(result)
})

function getOnShowRes(args, callback) {
  callback(null, onShowRes)
}

//JS初始化
function init(options, callback) {
  callback(null, "success");
  return;
}

var isInit = false;
//SDK初始化
function _init(callback) {
  if (isInit == true) {
    callback(null, "success");
    return;
  }
  cqisdk.init().then(() => {
    isInit = true
    callback(null, "success");
  })
}

let loginCallbackMessageCache = null  //登录返回信息缓存
let loginFlag = 0
//真正登录的实现
function doLogin(callback) {
  switch (loginFlag) {
    case 0:
      break;
    case 1:
      if (callback) {
        callback("error", loginCallbackMessageCache)
      }
      break;
    case 2:
      if (callback) {
        callback(null, loginCallbackMessageCache)
      }
      return;
  }
  cqisdk.login().then((res) => {
    var jsonObj = {
      "uid": res.uid,
      "username": res.username,
      "sign": res.sign
    }
    loginCallbackMessageCache = JSON.stringify(jsonObj)
    loginFlag = 2
    if (callback) {
      callback(null, loginCallbackMessageCache)
    }
  })
  .catch((err) => {
      console.error("渠道登录失败！", JSON.stringify(err))
      loginCallbackMessageCache = "[" + err.errcode + "]" + err.errmsg
      loginFlag = 1
      if (callback) {
        callback("error", loginCallbackMessageCache)
      }
  })
}

//预先登录
function preLogin() {
  _init(function (code, message) {
    if (code == null) {
      doLogin();
    }
  });
}

//登录（C#调用）
function login(args, callback) {
  _init(function (code, message) {
    if (code == null) {
      doLogin(callback);
    }
    else {
      callback("error", message)
    }
  });
}

//客服
function openCustom() {
  cqisdk.openCustomerService();
}

//支付
function pay(args, callback) {
  args = JSON.parse(args || "{}");
  cqisdk.pay({
    roleId: args.RoleInfo.RoleId,
    roleName: args.RoleInfo.RoleName,
    serverId: args.RoleInfo.ServerId,
    serverName: args.RoleInfo.ServerName,
    roleLevel: args.RoleInfo.RoleLevel,
    vipLevel: args.RoleInfo.VipLevel,
    amount: Number(args.Price),
    cburl: args.Extension.payback_url,
    attach: args.OrderId,
    productId: args.ProductId,
    productName: args.ProductName,
    productDesc: args.ProductDesc
  }).then(() => {
    callback(null, "pay_success");
  }).catch((err) => {
    console.log(`errcode: ${err.errcode}`)
    console.log(`errmsg: ${err.errmsg}`)
    callback("error", "[" + err.errcode + "]" + err.errmsg);
  })
}

// buttonInfo
// x y w h 按钮在游戏窗口坐标和大小
// ww hh 游戏窗口宽高
// dev 如果dev，则在按钮区域显示一个半透明红色遮罩（开发时确认位置用）
// 警告，如果离开当前界面，请一定调用hideAccessButton  获取头像
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
      backgroundColor: parseInt(dev) ? 'rgba(255, 0, 0, .5)' : 'transparent',
    }
  });
  accessBtn.onTap((res) => {
    if (accessBtn) {
      accessBtn.destroy();
      accessBtn = null
    }
    callback(!res.userInfo ? 'error' : null, JSON.stringify(res.userInfo || ""));
  });
}

function hideAccessButton(args, callback) {
  if (accessBtn) {
    accessBtn.destroy();
    accessBtn = null
  }
  callback(null, "hide success");
}

function checkUpdate(args, callback) {
  console.log("check upate")
  var arg = {
    step: "dyn_update_info"
  }
  taTrack("sdk_step", arg)
  const updateManager = wx.getUpdateManager();
  updateManager.onCheckForUpdate((res) => {
    // 请求完新版本信息的回调
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

function exitMiniProgram(args, callback) {
  wx.exitMiniProgram({
    success: (success) => {
      callback(null, success)
    },
    fail: (fail) => {
      callback("failed", fail);
    }
  });
}

//上报角色
function reportRoleInfo(args, callback) {
  args = JSON.parse(args || "{}");
  switch (args.ReportType) {
    case 2:
      console.log("sendRoleinfo roleCreate")
      cqisdk.roleCreate({
        roleId: args.RoleId,
        roleName: args.RoleName,
        serverId: args.ServerId,
        serverName: args.ServerName,
      })
      break;
    case 3:
      console.log("sendRoleinfo enterGame")
      cqisdk.enterGame({
        roleId: args.RoleId,
        roleName: args.RoleName,
        serverId: args.ServerId,
        serverName: args.ServerName,
      })
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
      console.log("sendRoleinfo exitGame")
      cqisdk.exitGame({
        roleId: args.RoleId,
        roleName: args.RoleName,
        serverId: args.ServerId,
        serverName: args.ServerName,
        roleLevel: args.RoleLevel,
      })
      break;
  }
}

//上报角色
function reportRoleInfoExtend(args, callback) { }

//#region 数数接口声明
//var ThinkingAnalyticsAPI = require("./ta_mg_sdk/thinkingdata.mg.wx.min.js");
// 创建 TA 变量
// var ta;
//初始化数数
function setShushuConfig(args, callback) {
  args = JSON.parse(args || "{}");
  const {
    appId,
    serverUrl,
    enableLog
  } = args;
  console.log(appId + " " + serverUrl + " " + enableLog);
  // TA SDK 配置对象
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
  gameStepTrack(
    args//事件属性
  );
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

//广告
function createRewardedVideoAd(args, callback) {
  args = JSON.parse(args || "{}");
  cqisdk.showRewardedVideoAd({
    name: args.goods_name,
    roleId: args.role_id,
    roleName: args.role_name,
    serverId: args.server_id,
    serverName: args.server_name,
    roleLevel: args.level,
    vipLevel: args.level,
  }).then((res) => {
    if (res.isEnded) {
      callback(null, "success");
    } else {
      callback(0, "not end");
    }
  }).catch((err) => {
    console.log(`errcode: ${err.errcode}`)
    console.log(`errmsg: ${err.errmsg}`)
    callback(err.errcode, err.errmsg);
  })
}

function assambleRewardVideoErr(errCode, errMsg) {
  return JSON.stringify({ state: "error", code: errCode, msg: errMsg })
}

//上报登录
function loginStep(args, callback) {
  console.log("loginStep", args)
  args = JSON.parse(args || "{}");
  const {
    accountId,
  } = args;
  taLogin(accountId);
}

function registerStep(args, callback) {
  console.log("registerStep")
  args = JSON.parse(args || "{}");
  const {
    accountId,
  } = args;
}

function payStep(args, callback) {
  console.log("payStep")
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
  console.log("GetDeviceId sdk js callback:" + deviceId);
  return deviceId;
}

function requestSubscribeMessage(args, callback) {
  args = JSON.parse(args || "{}")
  var requestData = { 'ids': {} }
  wx.requestSubscribeMessage({
    tmplIds: [args.tmplId],
    success(res) {
      for (var requestMessage in res) {
        if (requestMessage != 'errMsg') {
          requestData['ids'][requestMessage] = res[requestMessage]
        }
      }
      callback(null, JSON.stringify(requestData))
    }, fail(res) {
      console.log("失败:" + res.errMsg + "  " + "code:" + res.errCode)
      callback(res.errMsg, res.errMsg + `(${res.errCode})`)
    }
  })
}

function requestSubscribeSystemMessage(args, callback) {
  var requestData = { 'ids': {} }
  wx.requestSubscribeSystemMessage({
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
      console.log("失败:" + res.errMsg + "  " + "code:" + res.errCode)
      callback(res.errMsg, res.errMsg + `(${res.errCode})`)
    },
    complete(res) {
      console.log(res)
    }
  })
}

//订阅
function requestSubscribeMessageArray(args, callback) {
  console.log("requestSubscribeMessageArray")
  args = JSON.parse(args || "{}");
  var array = args.tmplId.split(",");
  var requestData = { 'ids': {} }
  wx.requestSubscribeMessage({
    tmplIds: array,
    success(res) {
      for (var requestMessage in res) {
        if (requestMessage != 'errMsg') {
          requestData['ids'][requestMessage] = res[requestMessage]
        }
      }
      callback(null, JSON.stringify(requestData))
    }, fail(res) {
      callback(res.errMsg, res.errMsg + `(${res.errCode})`)
    }
  })
}

//赋值剪切板内容
function setClipboardData(args, callback) {
  console.log("setClipboardData")
  args = JSON.parse(args || "{}")
  wx.setClipboardData({
    data: args.data,
    success(res) {
      callback(null, JSON.stringify({ 'state': 'success' }))
    },
    fail(res) {
      callback(null, JSON.stringify({ 'state': 'fail' }))
    }
  })
}

//获取剪切板内容
function getClipboardData(args, callback) {
  console.log("getClipboardData")
  wx.getClipboardData({
    success(res) {
      callback(null, JSON.stringify({ 'state': res.data }))
    }, fail(res) {
      callback(null, JSON.stringify({ 'state': "fail" }))
    }
  })
}

//主动分享 shareAppMessage
function shareAppMessage(args, callback) {
  console.log("shareAppMessage", args)
  args = JSON.parse(args || "{}")
  let shareMessage = {
    "title": args.title || '',
    "imageUrl": args.imageUrl || '',
    "imageUrlId": args.imageUrlId || '',
    "path": args.path || '',
    "query": args.query || ''
  };
  cqisdk.share(shareMessage, true);
}

//被动分享 onShareAppMessage
function setShareAppMessage(args, callback) {
  console.log("setShareAppMessage", args)
  args = JSON.parse(args || "{}")
  let shareMessage = {
    "title": args.title || '',
    "imageUrl": args.imageUrl || '',
    "imageUrlId": args.imageUrlId || '',
    "path": args.path || '',
    "query": args.query || ''
  };
  cqisdk.share(shareMessage, true);
}

function shareTimeline(args, callback) {
  console.log("showShareMenu", args)
  args = JSON.parse(args || "{}")
  // 绑定分享参数
  wx.onShareTimeline(() => {
    callback(null, JSON.stringify({ 'state': 'onShareTimeline' }))
    return {
      title: args.title || '',
      imageUrl: args.imageUrl || '', // 图片 URL
      imageUrlId: args.imageUrlId || '',
      imagePreviewUrl: args.imagePreviewUrl || '',
      imagePreviewUrlId: args.imagePreviewUrlId || '',
      path: args.path || '',
      query: args.query || ''
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
  console.log("getSettingState")
  wx.getSetting({
    success(res) {
      if (res.authSetting['scope.userInfo']) {
        // 已经授权，可以直接调用 getUserInfo 获取头像昵称
        wx.getUserInfo({
          success: function (res) {
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
    }
    else {
      value = value + "|" + addMsg;
    }
    return value;
  }
  let systemInfo = wx.getSystemInfoSync();
  let safeArea = systemInfo.safeArea;
  let hasNotchScreen = false;
  let notchSize = 0;
  let locationDetail = "";
  if (safeArea.top >= 0) {
    locationDetail = AddLocationDetail(locationDetail, "top");
    notchSize = safeArea.top;
    hasNotchScreen = true;
  }
  else if (safeArea.left >= 0) {
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


export default {
  init, //必须
  preLogin,
  login, //必须
  pay,
  showAccessButton,
  hideAccessButton,
  checkUpdate,
  exitMiniProgram,
  reportRoleInfo,
  setShushuConfig,
  stepTrack,
  track,
  userSet,
  setSuperProperties,
  setAccountID,
  createRewardedVideoAd,
  loginStep,
  registerStep,
  payStep,
  requestSubscribeMessage,
  requestSubscribeSystemMessage,
  getDeviceId,
  requestSubscribeMessageArray,
  setClipboardData,
  getClipboardData,
  shareAppMessage,
  shareTimeline,
  setShareAppMessage,
  getCurrentTime,
  getSettingState,
  reportRoleInfoExtend,
  getOnShowRes,
  openNotchScreen,
  getNotchScreen,
  openCustom,
}