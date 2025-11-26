using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using WeChatWASM;
namespace YoukiaSDKSpace
{
    [FastResPathAttr("fast_network")]
    [FastResTypeAttr(FastResType.GameObject)]
    public class FastNetwork : FastSingleUI<FastNetwork>
    {
        private const int timeout = 30;
        public delegate void OnSuc(string result);
        public delegate void OnError(string errorMsg);



        public void DoPostJson(string url, string jsonData, OnSuc onSuc, OnError onError)
        {
            try
            {
                StartCoroutine(PostJson(url, jsonData, onSuc, onError));
            }
            catch (Exception e)
            {
                Debug.Log(e);
                if (onError != null)
                {
                    onError(e.Message);
                }
            }
        }


        private IEnumerator PostJson(string url, string jsonData, OnSuc onSuc, OnError onError)
        {
            //通过PUT方式构造HTTP请求
            byte[] myData = Encoding.UTF8.GetBytes(jsonData);
            UnityWebRequest request = UnityWebRequest.Put(url, myData);
            //构造好后，手动将请求方式更改为POST
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");

            yield return request.Send();


            if (request.isNetworkError || request.isHttpError)
            {
                //Log.i(request.error);
                Debug.Log(request.error);
                if (onError != null)
                {
                    onError(request.error);
                }
            }
            else
            {
                //Debug.Log(request.downloadHandler.text);
                //Log.i(request.downloadHandler.text);
                if (onSuc != null)
                {
                    onSuc(request.downloadHandler.text);
                }
            }
        }


        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onSuc"></param>
        /// <param name="onError"></param>
        public void DoGet(string url, OnSuc onSuc = null, OnError onError = null, bool showLoading = true)
        {
            try
            {
                if (url == null || "".Equals(url))
                {
                    onError?.Invoke("url null");

                }
                else
                {
                    if (showLoading)
                    {
                        WX.ShowLoading(new ShowLoadingOption()
                        {
                            title = "加载中",
                            mask = true,
                        });
                    }
                    StartCoroutine(Get(url, onSuc, onError, showLoading));
                }
            }
            catch (Exception e)
            {
                WX.HideLoading(new HideLoadingOption() { });
                Debug.Log(e);
                onError?.Invoke(e.Message);
            }
        }

        private IEnumerator Get(string url, OnSuc onSuc, OnError onError, bool showLoading)
        {
            // try
            // {

                Debug.Log(url);
                UnityWebRequest request = UnityWebRequest.Get(url);
                request.timeout = timeout;
                yield return request.SendWebRequest();
                Debug.Log("666");
                if (request.isNetworkError || request.isHttpError)
                {
                    if (showLoading)
                    {
                        WX.HideLoading(new HideLoadingOption() { });
                    }
                    Debug.Log(request.error);
                    onError?.Invoke(request.error);
                }
                else
                {
                    if (showLoading)
                    {
                        WX.HideLoading(new HideLoadingOption() { });
                    }
                    Debug.Log(request.downloadHandler.text);
                    onSuc?.Invoke(request.downloadHandler.text);
                }


            // }
            // catch (Exception e)
            // {

            // }
            if (showLoading)
            {
                WX.HideLoading(new HideLoadingOption() { });
            }
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramsDic"></param>
        /// <param name="onSuc"></param>
        /// <param name="onError"></param>
        public void DoPost(string url, Dictionary<string, object> paramsDic, OnSuc onSuc = null, OnError onError = null, bool showLoading = true)
        {
            try
            {
                if (url == null || "".Equals(url))
                {
                    onError?.Invoke("url null");

                }
                else
                {
                    if (showLoading)
                    {
                        WX.ShowLoading(new ShowLoadingOption()
                        {
                            title = "加载中",
                            mask = true,
                        });
                    }

                    StartCoroutine(Post(url, paramsDic, onSuc, onError, showLoading));
                }
            }
            catch (Exception e)
            {
                if (showLoading)
                {
                    WX.HideLoading(new HideLoadingOption() { });
                }
                Debug.Log(e);
                onError?.Invoke(e.Message);
            }

        }

        private IEnumerator Post(string url, Dictionary<string, object> paramsDic, OnSuc onSuc, OnError onError, bool showLoading)
        {
            Debug.Log($"request url:{url}");
            WWWForm form = new WWWForm();

            if (paramsDic != null)
            {
                foreach (string key in paramsDic.Keys)
                {
                    form.AddField(key, paramsDic[key].ToString());
                }
            }
            UnityWebRequest request = UnityWebRequest.Post(url, form);
            Debug.Log($"request request param:{Json.Serialize(paramsDic)}");
            request.timeout = timeout;
            yield return request.SendWebRequest();
            //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
            if (request.isHttpError || request.isNetworkError)
            {
                if (showLoading)
                {
                    WX.HideLoading(new HideLoadingOption() { });
                }
                Debug.Log(request.error);
                onError?.Invoke(request.error);
            }
            else
            {
                if (showLoading)
                {
                    WX.HideLoading(new HideLoadingOption() { });
                }
                Debug.Log($"{request.downloadHandler.text}");
                onSuc?.Invoke(request.downloadHandler.text);
            }
            if (showLoading)
            {
                WX.HideLoading(new HideLoadingOption() { });
            }
        }

    }

}
