import config from './config';
const ThinkingAnalyticsAPI = require("./ta_mg_sdk//thinkingdata.mg.wx.min.js");

// var config = {
//     appId: "", // 项目 APP ID
//     serverUrl: "", // 上报地址
//     enableLog: false, // 是否输出日志
//     autoTrack: {
//         appShow: true, // 是否自动采集 ta_mg_show, 默认true即可
//         appHide: true // 是否自动采集 ta_mg_hide, 默认true即可
//     }
// };

wx.onShow((result) => {
    if(ta === null || ta === undefined){
        return
    }
    ta.setSuperProperties({
        launch_url: JSON.stringify(result)
    });
})

// 创建 TA 实例

createTA(config.ta_config)
var ta;
export function createTA(mConfig){
    if (mConfig === null || mConfig === undefined || mConfig.appId.trim() === ""){
        return ta;
    }
    ta = new ThinkingAnalyticsAPI(mConfig);
    // 初始化
    //主动调用初始化方法后，数据才会真正上报。这允许您掌控数据开始上报的时机，确保在数据开始上报之前，您已经完成对  SDK 的必要设置 (用户 ID、公共事件属性等)
    ta.init();
    ta.setSuperProperties({session_id: Date.now()});
    return ta;
}

export function getTA(){
    return ta;
}

export function taTrack(event, params){
    if(ta === null || ta === undefined){
        return;
    }
    ta.track(event,params);
}

export function gameStepTrack(params){
    if(ta === null || ta === undefined){
        return;
    }
    ta.track(
        "app_step",
        params
    );
}

export function taUserSet(args){
    if(ta === null || ta === undefined){
        return;
    }
    ta.userSet(args);
}

export function taLogin(args){
    if(ta === null || ta === undefined){
        return;
    }
    ta.login(args)
}

export function taSetSuperProperties(args){
    if(ta === null || ta === undefined){
        return;
    }
    ta.setSuperProperties(args)
}

