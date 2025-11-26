import { getTA } from './ta-manager';
import config from './config';
import functions from './sdk';
// console.log((functions));
export const YKSDK = { ...functions, _getInitExtraData };

if (getTA() !== null && getTA() !== undefined) {
  getTA().track("launch", { step: 0 });
  getTA().flush();
}

export function track() {
  if (getTA() === null || getTA() === undefined) {
    return;
  }
  getTA().track(...arguments);
}

//发送消息到unity
export function callbackMessage(funcName,err,args) {
  console.log("FastSDK_JS callbackMessage: ",funcName,err,args)
  GameGlobal.YKSDKSendMessageToUnity(funcName,err,args)
}

export function launchStep(step, args) {
  if (getTA() === null || getTA() === undefined) {
    return;
  }
  getTA().track("launch", { step, ...args });
  console.debug("launch", step, args);
  getTA().flush();
}

//缓存请求到的自定义扩展数据
let YKSDK_init_config = null;
// 替换c#中getInitExtraData网络请求部分
function _getInitExtraData(_, callback) {
  callback(null, JSON.stringify(YKSDK_init_config));
}

// 启动游戏预加载
export function startGame() {
  // 请求挂接渠道配置
  wx.request({
    url: `${config.domain}/api/p${config.channelId}/channel_extra?f_c_version=${encodeURIComponent(config.FastSDK_Verison)}&version=${encodeURIComponent(config.mini_game_version)}`,
    success: (res) => {
      if (res.statusCode !== 200 || parseInt(res.data.status) !== 1) {
        console.error("YKSDK_JS\tload init config faild:", res.statusCode, res.data);
        GameGlobal.manager.startGame()
        showPromptTips("游戏资源获取异常！请尝试重启小游戏","确认重启", ()=>{
          wx.restartMiniProgram()
        });
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
        GameGlobal.manager.startGame()
        showPromptTips("游戏资源获取异常！请尝试重启小游戏","确认重启", ()=>{
          wx.restartMiniProgram()
        });
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
            console.error("YKSDK_JS\rconfig \"res_pre_list\" format error:", res.res_pre_list);
            GameGlobal.manager.startGame()
            showPromptTips("游戏资源获取异常！请尝试重启小游戏","确认重启", ()=>{
              wx.restartMiniProgram()
            });
            return;
          }
          parsePreLoadList(cdn, list);
        }
        catch (e) {
          console.error("YKSDK_JS\t parse config \"res_pre_list\" faild:", e);
          GameGlobal.manager.startGame()
          showPromptTips("游戏资源加载异常！请尝试重启小游戏","确认重启", ()=>{
            wx.restartMiniProgram()
          });
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
            GameGlobal.manager.startGame()
            showPromptTips("游戏资源加载异常！请尝试重启小游戏","确认重启", ()=>{
              wx.restartMiniProgram()
            });

            return;
          }
          try {
            let list = res.data.list;
            if (!Array.isArray(list)) {
              console.error("YKSDK_JS\tpreload-list.txt format error:", res.data);
              GameGlobal.manager.startGame()
              showPromptTips("游戏资源加载异常！请尝试重启小游戏","确认重启", ()=>{
                wx.restartMiniProgram()
              });
              return;
            }
            parsePreLoadList(cdn, list);
          } catch (e) {
            console.error("YKSDK_JS\tpreload-list.txt parse faild:", e);
            GameGlobal.manager.startGame()
            showPromptTips("游戏资源加载异常！请尝试重启小游戏","确认重启", ()=>{
              wx.restartMiniProgram()
            });
            return;
          }
          // 开始启动unity
          GameGlobal.manager.startGame();
        },
        fail: (err) => {
          console.error(err);
          GameGlobal.manager.startGame()
          showPromptTips("游戏资源加载异常！请尝试重启小游戏","确认重启", ()=>{
            wx.restartMiniProgram()
          });
        },
      });
    },
    fail: (e) => {
      console.error("YKSDK_JS\tload init config faild:", e);
      GameGlobal.manager.startGame()
      showPromptTips("游戏资源获取异常！请尝试重启小游戏","确认重启", ()=>{
        wx.restartMiniProgram()
      });
    },
  })
  YKSDK.preLogin();
}


//展示标识，避免弹出多层弹窗
let isDialogShow = false;

function showPromptTips(content, confirmText, onConfirm){
  wx.hideLoading()
  if (isDialogShow){
    return;
  }
  isDialogShow = true;
  wx.showModal({
    title: '提示',
    content: content,
    showCancel: false,
    confirmText: confirmText,
    success: () => {
      wx.showLoading({
        title: '加载中...',
      });
      isDialogShow = false;
      onConfirm();
    }
  })

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
