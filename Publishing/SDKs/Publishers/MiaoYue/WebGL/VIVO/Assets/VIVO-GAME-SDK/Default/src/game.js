/* eslint-disable  quotes */
require("./qgame-adapter.js");
require("./store.js");
require("./unityAdapter.js");
const md5Utils = require("./md5.js");

//使用该功能时，需判断引擎是否支持
//创建CustomizeLoading组件
var loading;
if (qg.createCustomizeLoading) {
  var bgImageSrc = UnityLoader.EnvConfig.getConfig("bgImageSrc");
  var background =
    "https://wwwstatic.vivo.com.cn/vivoportal/files/resource//funtouch/1612511921756/images/originos-night-img1-lg.jpg";
  if (bgImageSrc !== undefined && bgImageSrc !== null && bgImageSrc !== "") {
    console.log("background update", bgImageSrc);
    background = bgImageSrc;
  }
  loading = qg.createCustomizeLoading({
    background: background,
    text: "请耐心等待加载中...",
    textColor: "#ffffff",
    loadingColorTop: "#ffffff",
    loadingColorBottom: "#ffffff",
    loadingProgress: 0,
  });
}

function updateLoading(progress) {
  if (!qg.createCustomizeLoading || !loading) {
    return;
  }
  //根据实际场景进行更新进度、背景、文字以及文字颜色
  //更新CustomizeLoading样式
  loading.update({
    loadingProgress: progress * 100,
  });
}

function updateLoading(progress, textStr) {
  if (!qg.createCustomizeLoading || !loading) {
    return;
  }
  loading.update({
    loadingProgress: progress * 100,
    text: textStr,
  });
}

function updateLoadingError() {
  if (!qg.createCustomizeLoading || !loading) {
    return;
  }
  reportUnityWasmInfo();
  loading.update({
    text: "加载失败，请重启游戏",
  });
}

function removeLoading() {
  if (!qg.createCustomizeLoading || !loading) {
    return;
  }

  //移除CustomizeLoading组件
  loading.remove();
}

function compileRateSimulate() {
  if (!qg.createCustomizeLoading || !loading) {
    clearInterval(loadingTask);
    return;
  }
  loadCurRate += 0.02;
  if (loadCurRate >= 0.99) {
    clearInterval(loadingTask);
    return;
  }
  updateLoading(loadCurRate, "编译初始化中...");
}

var loadingTask = null;
const down_take_rate = 0.5; // 下载所占比例
var loadCurRate = 0;

var wasmUrl = UnityLoader.EnvConfig.getConfig("wasmUrl");
var wasmDataUrl = UnityLoader.EnvConfig.getConfig("wasmDataUrl");
var subUnityPkg = UnityLoader.EnvConfig.getConfig("subUnityPkg");

function execUnity() {
  /* eslint-disable  quotes */
  preloadAssets();
  loadCurRate = down_take_rate;
  loadingTask = setInterval(compileRateSimulate, 1000);
  window["unityInstance"] = window.UnityLoader.instantiate(
    "/buildUnity/webgl.json",
    {
      onProgress: function (_, i) {
        // 更新启动loading组件进度
        // unity自身进度逻辑：文件准备完成90%，编译完成99%，完成100%
        // 由于编译时间较久，此处仅使用100%的逻辑，其他使用模拟数据
        if (i === 1) {
          clearInterval(loadingTask);
          updateLoading(i, "开始游戏");
          window.UnityLoader.printLog("加载完成");
          UnityLoader.UnityInfo.unityWasmLoadTime = qg.getPerformance().now();

          setTimeout(function () {
            //移除启动loading组件
            removeLoading();
          }, 300);

          reportUnityWasmInfo();
        }
      },
    }
  );
}

function reportUnityWasmInfo() {
  const data = UnityLoader.UnityInfo;
  console.log("reportUnityWasmInfo start");
  if (qg.reportGameInfo) {
    const info = {
      unityVersion: data.unityVersion.string,
      unityPluginVersion: data.unityPluginVersion,
      unityStartTime: data.unityStartTime,
      unityWasmDownTime: data.unityWasmDownTime,
      unityWasmLoadTime: data.unityWasmLoadTime,
      unityCodeSize: data.unityCodeSize,
      unityDataSize: data.unityDataSize,
      unityWasmType: data.unityWasmType,
      unityWasmResult: data.unityWasmResult,
    };
    console.log("reportUnityWasmInfo real", info);
    qg.reportGameInfo(info);
  }
  //包体检测 提示
  var dataSize = data.unityDataSize / (1024 * 1024);
  if (dataSize > 15) {
    window.UnityLoader.printError(
      "data wasm 首包资源文件过大，建议优化缩减到15M以内"
    );
  }
  var codeSize = data.unityCodeSize / (1024 * 1024);
  if (codeSize > 30) {
    window.UnityLoader.printError(
      "code wasm 代码逻辑文件过大，建议优化缩减到30M以内"
    );
  }
}

function downloadSource(sourceUrl, cb) {
  var key = md5Utils.hex_md5(sourceUrl);
  var cache_key = window.qg.getStorageSync({
    key: "mini_wasm_cache_url_md5",
    default: "default",
  });
  if (
    cache_key === key &&
    "true" ===
      window.qg.accessFile({
        uri:
          window.qg.env.USER_DATA_PATH +
          "/" +
          key +
          "/online_mini.data.unityweb",
      }) &&
    "true" ===
      window.qg.accessFile({
        uri:
          window.qg.env.USER_DATA_PATH +
          "/" +
          key +
          "/online_mini.wasm.code.unityweb",
      })
  ) {
    cb(true);
    UnityLoader.UnityInfo.unityWasmDownTime = qg.getPerformance().now();
    UnityLoader.UnityInfo.unityWasmResult = 1;
    console.log("downloadSource prepare success by local");
    return;
  }
  if (cache_key !== "default") {
    window.qg.rmdir({
      uri: window.qg.env.USER_DATA_PATH + "/" + cache_key,
    });
  }
  qg.setStorage({
    key: "mini_wasm_cache_url_md5",
    value: key,
    success: function (data) {
      window.UnityLoader.printLog("downloadSource cache success");
    },
    fail: function (data, code) {
      window.UnityLoader.printError(
        `downloadSource cache fail, code = ${code}`
      );
    },
  });
  var downPath = window.qg.env.USER_DATA_PATH + "/" + key + "/wasm_zipsource";
  var downloadTask = window.qg.downloadFile({
    url: sourceUrl,
    filePath: downPath,
    success: function () {
      window.qg.unzipFile({
        srcUri: downPath,
        dstUri: window.qg.env.USER_DATA_PATH + "/" + key + "/",
        success: function (uri) {
          cb(true);
          UnityLoader.UnityInfo.unityWasmDownTime = qg.getPerformance().now();
          UnityLoader.UnityInfo.unityWasmResult = 0; // 下载成功，解压成功
          console.log("downloadSource prepare success");
        },
        fail: function (data, code) {
          cb(false);
          window.UnityLoader.printError(
            `downloadSource unzip handling fail, code = ${code}`
          );
          UnityLoader.UnityInfo.unityWasmResult = -2; // 下载成功，解压失败
        },
      });
    },
    fail: function (e) {
      cb(false);
      UnityLoader.UnityInfo.unityWasmResult = -1; // 下载失败
      window.UnityLoader.printError(
        "downloadSource download file fail " + JSON.stringify(e)
      );
    },
  });
  downloadTask.onProgressUpdate(function (msg) {
    var progress = msg["progress"];
    updateLoading((progress / 100) * down_take_rate);
  });
}

function preloadAssets() {
  var preloadUrl = UnityLoader.EnvConfig.getConfig("preloadUrl");
  var preloadUrlList = preloadUrl.split(";");
  if (preloadUrlList) {
    preloadUrlList.forEach((url, index) => {
      if (url && url.value.trim().length != 0) {
        // 创建网络请求
        qg.request({
          url: url,
          dataType: "arraybuffer",
          success: function (ret) {
            window.UnityLoader.printLog(
              "preloadAssets request success " + " url = " + url
            );
          },
          fail: function (error, code) {
            window.UnityLoader.printError(
              "preloadAssets request fail " +
                " url = " +
                url +
                " code = " +
                code
            );
          },
        });
      }
    });
  }
}

function downloadWebglData(sourceUrl, cb) {
  var key = md5Utils.hex_md5(sourceUrl);
  var cache_key = window.qg.getStorageSync({
    key: "mini_wasm_cache_url_md5",
    default: "default",
  });
  if (
    cache_key === key &&
    "true" ===
      window.qg.accessFile({
        uri:
          window.qg.env.USER_DATA_PATH +
          "/" +
          key +
          "/online_mini.data.unityweb",
      })
  ) {
    cb(true);
    UnityLoader.UnityInfo.unityWasmDownTime = qg.getPerformance().now();
    UnityLoader.UnityInfo.unityWasmResult = 4;
    console.log("downloadWebglData success by local");
    return;
  }
  if (cache_key !== "default") {
    window.qg.rmdir({
      uri: window.qg.env.USER_DATA_PATH + "/" + cache_key,
    });
  }
  qg.setStorage({
    key: "mini_wasm_cache_url_md5",
    value: key,
    success: function (data) {
      window.UnityLoader.printLog("downloadWebglData cache success");
    },
    fail: function (data, code) {
      window.UnityLoader.printError(
        `downloadWebglData cache fail, code = ${code}`
      );
    },
  });
  var downPath = window.qg.env.USER_DATA_PATH + "/" + key + "/wasm_zipsource";
  var downloadTask = window.qg.downloadFile({
    url: sourceUrl,
    filePath: downPath,
    success: function () {
      window.qg.unzipFile({
        srcUri: downPath,
        dstUri: window.qg.env.USER_DATA_PATH + "/" + key + "/",
        success: function (uri) {
          cb(true);
          UnityLoader.UnityInfo.unityWasmDownTime = qg.getPerformance().now();
          UnityLoader.UnityInfo.unityWasmResult = 0; // 下载成功，解压成功
          console.log("downloadWebglData prepare success");
        },
        fail: function (data, code) {
          cb(false);
          window.UnityLoader.printError(
            `downloadWebglData unzip handling fail, code = ${code}`
          );
          UnityLoader.UnityInfo.unityWasmResult = -2; // 下载成功，解压失败
        },
      });
    },
    fail: function (e) {
      cb(false);
      UnityLoader.UnityInfo.unityWasmResult = -1; // 下载失败
      window.UnityLoader.printError(
        "downloadWebglData file fail " + JSON.stringify(e)
      );
    },
  });
  downloadTask.onProgressUpdate(function (msg) {
    var progress = msg["progress"];
    updateLoading((progress / 100) * down_take_rate);
  });
}

function loadUnityWasmZip() {
  downloadSource(wasmUrl, (data) => {
    if (data) {
      updateLoading(down_take_rate);
      execUnity();
    } else {
      updateLoadingError();
    }
  });
}

function loadSubUnity() {
  console.log("loadSubUnity start");
  qg.loadSubpackage({
    name: "unitySubPkg",
    success: function (info) {
      console.log("loadSubUnity success", info);
      UnityLoader.UnityInfo.unityWasmResult = 0;
      UnityLoader.UnityInfo.unityWasmDownTime = qg.getPerformance().now();
      execUnity();
    },
    fail: function (e) {
      console.log("loadSubUnity failed", e);
      UnityLoader.UnityInfo.unityWasmResult = -1; // 下载失败
      updateLoadingError();
    },
    complete: function () {},
  });
  // wasm.data部分存放到自有cdn，一般是因为这部分数据>30MB以上需要这样做
  loadSubUnityWasmData();
}

function loadSubUnityWasmData() {
  if (wasmDataUrl && wasmDataUrl.trim().length != 0) {
    console.log("downloadWebglData start", wasmDataUrl);
    downloadWebglData(wasmDataUrl, function (data) {
      console.log("downloadWebglData end", data);
      if (data) {
        updateLoading(down_take_rate);
        window.wasmDataLoadStatus = true;
        var unityInstance = window["unityInstance"];
        if (unityInstance && unityInstance.Module.wasmBinary) {
          console.log("downloadWebglData aready");
          UnityLoader.loadModuleData(unityInstance.Module, {});
        } else {
          console.log("downloadWebglData not ready");
        }
      } else {
        updateLoadingError();
      }
    });
  } else {
    window.wasmDataLoadStatus = true;
  }
}

UnityLoader.UnityInfo.unityStartTime = qg.getPerformance().now();
window.wasmDataLoadStatus = true;
if (wasmUrl && wasmUrl.trim().length != 0) {
  //自定义loading 网络下载入口
  UnityLoader.UnityInfo.unityWasmType = 1;
  loadUnityWasmZip()
} else if (subUnityPkg) {
  //分包加载自定义loading
  UnityLoader.UnityInfo.unityWasmType = 2;
  window.wasmDataLoadStatus = false;
  loadSubUnity();
} else {
  UnityLoader.UnityInfo.unityWasmType = 3;
  //自定义loading 本地加载入口
  execUnity();
}
