import cfg from "./config.js";
// 数数
// import ThinkingAnalyticsAPI from "./ss//thinkingdata.mg.wx.min.js";

// reyun
import ry from "./reyun/oreyunwx.min";


var config = {
    appId: cfg.ss.appId, // 项目 APP ID
    serverUrl: cfg.ss.server, // 上报地址
    enableLog: false,
    autoTrack: {
        appShow: true, // 自动采集 ta_mg_show
        appHide: true // 自动采集 ta_mg_hide
    }
};

const ThinkingAnalyticsAPI = require("./ss//thinkingdata.mg.wx.min.js");

// var config = {
//     appId: "0fc5f70482c345e8b7052015aab12733", // 项目 APP ID
//     serverUrl: "https://ta.youkia.net", // 上报地址
//     enableLog: false,
//     autoTrack: {
//         appShow: true, // 自动采集 ta_mg_show
//         appHide: true // 自动采集 ta_mg_hide
//     }
// };
// 创建 TA 实例
export const ta = new ThinkingAnalyticsAPI(config);
// 初始化
//主动调用初始化方法后，数据才会真正上报。这允许您掌控数据开始上报的时机，确保在数据开始上报之前，您已经完成对 SDK 的必要设置 (用户 ID、公共事件属性等)
ta.init();
ta.setSuperProperties({session_id: Date.now()});
wx.onShow((result) => {
  ta.setSuperProperties({
    launch_url: JSON.stringify(result)
  });
});
console.log("数数 init success");

ry.__initSelf = ((openId) => {
  ry.init(cfg.reyun.appKey, openId);
  console.log("reyun init success");
})

// wx.onShow((result) => {
//   reyun.__initSelf("");
// })
export const reyun = ry;
