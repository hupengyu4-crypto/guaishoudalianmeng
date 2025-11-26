/**
 * 抖音小游戏SDK v1.1.0
 * MJG_GameSDK.js和MJG_TT_GAME_API.js
 * 
 * MJG_GameSDK.js内容如下：
 * 
 * GameSDK 是一个全局对象，包括多个方法，分别是 :
 * 初始游戏配置接口：GameSDK.initGameInfo(data)
 * 登陆接口：GameSDK.login(callback)
 * 角色创建/升级接口：GameSDK.role(data)
 * 支付接口：GameSDK.pay(data)
 * 侧边栏复访接口：GameSDK.navigateToSidebar(callback)
 * 监听小游戏回到前台接口：GameSDK.onShow(callback) | 非必接
 * 取消监听小游戏回到前台接口：GameSDK.offShow(callback) | 非必接
 * 打开客服会话：GameSDK.openCustomerServiceConversation(data) | 非必接
 * 创建快捷方式：GameSDK.addShortcut(callback) | 非必接
 * 获取字段参数的值接口：GameSDK.getParam(paramName) | 非必接
 * 获取设备标识接口：GameSDK.getDeviceTag() | 非必接
 * 获取设备信息接口：GameSDK.getDeviceInfo() | 非必接
 * 获取菜单按钮（右上角胶囊按钮）的布局位置信息接口（横屏时）：GameSDK.getMenuBtnInfo() | 非必接
 * 获取系统信息接口（同步）：GameSDK.getSystemInfoSync() | 非必接
 * 转发分享接口：GameSDK.share(data) | 非必接
 * 
 * 
 * 较上一个版本改动地方有：
 * 
 */

GameGlobal.GameSDK = (function (GameGlobal) {

    //初始化游戏配置
    function mjInitGameInfo(data) {
        TT_GAME_API.onInitGameInfo(data);
    }

    //登录
    function mjlogin(callback) {
        TT_GAME_API.onWXLogin(callback);
    }

    //支付
    function mjPay(data) {
        TT_GAME_API.onWXPay(data);
    }

    //角色创建/升级接口
    function mjrole(data) {
        TT_GAME_API.onRole(data);
    }

    //侧边栏访问
    function mjNavigateToSidebar(callback)
    {
        TT_GAME_API.onNavigateToSidebar(callback);
    }

    //判断是否从微信外的渠道启动，返回bool，true或false
    function mjIsNoWXLaunch() {
        return TT_GAME_API.onIsNoWXLaunch();
    }

    //根据某个参数字段获取参数值
    function mjGetParam(paramName) {
        return TT_GAME_API.onGetParam(paramName);
    }

    //获取设备标识(可选)
    function mjGetDeviceTag() {
        return TT_GAME_API.onGetDeviceTag();
    }

    //获取设备信息(可选)
    function mjGetDeviceInfo() {
        return TT_GAME_API.onGetDeviceInfo();
    }
    //获取胶囊体属性信息(可选)
    function mjGetJiaonangInfo() {
        return TT_GAME_API.onGetMenuBtnInfo();
    }

    //获取系统信息(同步)(可选)
    function mjGetSystemInfoSync() {
        return TT_GAME_API.onGetSystemInfoSync();
    }

    //分享
    function mjShare(data) {
        return TT_GAME_API.onShareAppMessage(data);
    }

    //监听小游戏回到前台的事件
    function mjOnShow(callback) {
        return TT_GAME_API.onShow(callback);
    }

    // 取消监听小游戏回到前台的事件。
    function mjOffShow(callback) {
        return TT_GAME_API.offShow(callback);
    }

    //打开客服会话
    function mjOpenCustomerServiceConversation(data)
    {
        return TT_GAME_API.openCustomerServiceConversation(data);
    }

    //创建快捷方式
    function mjAddShortcut(callback)
    {
        return TT_GAME_API.addShortcut(callback);
    }

      //行为上报
    function mjReport(action){
        return TT_GAME_API.onReport(action);
    }

    return {
        initGameInfo: function (data) {
            mjInitGameInfo(data);
        },

        login: function (callback) {
            mjlogin(callback);
        },

        pay: function (data) {
            mjPay(data);
        },

        role: function (data) {
            mjrole(data);
        },

        navigateToSidebar:function(callback){
            mjNavigateToSidebar(callback);
        },

        onShow:function(callback){
            mjOnShow(callback);
        },

        offShow:function(callback){
            mjOffShow(callback);
        },

        openCustomerServiceConversation:function(data)
        {
            mjOpenCustomerServiceConversation(data);
        },

        report:function(action){
            mjReport(action);
        },

        addShortcut:function(callback){
            mjAddShortcut(callback);
        },

        isNoWXLaunch: function () {
            return mjIsNoWXLaunch();
        },

        getParam: function (paramName) {
            return mjGetParam(paramName)
        },

        getDeviceTag: function () {
            return mjGetDeviceTag()
        },

        getDeviceInfo: function () {
            return mjGetDeviceInfo()
        },

        getMenuBtnInfo: function () {
            return mjGetJiaonangInfo()
        },

        getSystemInfoSync: function () {
            return mjGetSystemInfoSync()
        },

        share: function (data) {
            mjShare(data);
        },
    }
})(GameGlobal);