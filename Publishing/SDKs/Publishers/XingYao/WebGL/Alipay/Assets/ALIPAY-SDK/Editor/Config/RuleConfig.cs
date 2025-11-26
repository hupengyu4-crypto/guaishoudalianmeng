using System;
using System.Collections.Generic;
using UnityEngine;

public class RuleConfig
{
  public string header;
  public List<NewRule> rules;
  public RuleConfig()
  {
    rules = new List<NewRule>();
    rules.Add(new NewRule() { old = @"delete\s*http(\b)", newStr = "null" });
    rules.Add(new NewRule() { old = @"new TextDecoder\([""']utf\-16le[""']\)", newStr = @"undefined/* $0 */" });
    rules.Add(new NewRule() { old = @"if\s*\(\!memoryprofiler\)\s*\{", newStr = "if (false) { $0" });
    rules.Add(new NewRule() { old = @"this\.uiUpdateIntervalMsecs\)", newStr = "$0/}" });
    rules.Add(new NewRule() { old = "UnityLoader.SystemInfo", newStr = "Module.SystemInfo" });
    rules.Add(new NewRule() { old = @"http\.open\( *_method *, *_url *, *true *\);", newStr = "var http = new (window.unityNamespace.UnityLoader.UnityCache.XMLHttpRequest || XMLHttpRequest)();http.open(_method, _url, true);" });
    rules.Add(new NewRule()
    {
      old = @"var UTF8Decoder=typeof TextDecoder!==" + "\"" + "undefined" + "\"" + @"\?new TextDecoder\(" + "\"" + "utf8" + "\"" + @"\):undefined;",
      newStr = "var UTF8Decoder=undefined;"
    });
    rules.Add(new NewRule()
    {
      old = @"FS\.staticInit\(\);",
      newStr = @"FS.staticInit();
window.unityNamespace.FS = FS;
window.unityNamespace.MEMFS = MEMFS;"
    });
#if UNITY_2018 || UNITY_2019
    rules.Add(new NewRule()
    {
        old = @"Module\.streamingAssetsUrl\(\);",
        newStr = "Module.streamingAssetsUrl;"
    });
#endif
#if UNITY_2021_3_OR_NEWER
    //2024.10.22 新增规则
    rules.Add(new NewRule()
    {
      old = @"function _JS_WebRequest_Send\(",
      newStr = @"function MYFetchResponse(xhr, url) {
            var headersPlain = xhr.getAllResponseHeaders();
            var headers = new Map();
            headersPlain
              .trim()
              .split(/[\r\n]+/)
              .forEach(function (e) {
                var t = e.split(': ');
                var n = t.shift();
                var a = t.join(': ');
                headers.set(n, a);
              });
            this.headers = headers;
            this.url = url;
            this.ok = !!(xhr.status >= 200 && xhr.status < 300);
            this.status = xhr.status;
            this.statusText = xhr.statusText;
            this.parsedBody = new Uint8Array(xhr.response);
        }
        function _JS_WebRequest_Send("
    });
    rules.Add(new NewRule()
    {
      old = @"fetchImpl\(",
      newStr = @"fetchImpl = function (url, init) {
            var xhr = new (window.unityNamespace.UnityLoader.UnityCache.XMLHttpRequest || XMLHttpRequest)();
            xhr.isFetchApi = true;
            xhr.open(init.method, url, true);
            xhr.responseType = 'arraybuffer';
            Object.keys(init.headers).forEach(function (key) {
              xhr.setRequestHeader(key, init.headers[key]);
            });
            xhr.timeout = init.timeout;
            xhr.onprogress = init.onProgress;
            return new Promise(function (resolve, reject) {
              xhr.onload = function () {
                resolve(new MYFetchResponse(xhr, url));
              };
              xhr.onerror = function (e) {
                var response = new MYFetchResponse(xhr, url);
                response.message = e.errorMessage || e.message || '';
                reject(response);
              };
              xhr.send(init.body);
            });
        };
        fetchImpl("
    });
    rules.Add(new NewRule()
    {
      old = @"var kWebRequestOK *= *0;",
      newStr =
        @"HandleProgress({response,loaded: true,lengthComputable: true,total: 100,type: 'progress'});var kWebRequestOK = 0;"
    });
    rules.Add(new NewRule { old = @"enableStreamingDownload: *true", newStr = "enableStreamingDownload: false" });
#endif
  }
}

[Serializable]
public class NewRule
{
  public string old;
  public string newStr;
}
