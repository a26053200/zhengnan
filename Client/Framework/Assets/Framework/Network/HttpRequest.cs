using System;
using UnityEngine;
using System.Collections;
using LitJson;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

//
// HttpRequest
// Author: zhengnan
// Create: 2018/6/7 22:06:20
// 
public class HttpRequest : MonoBehaviour
{
    //private Regex reg;
    private void Awake()
    {
        //reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");//正则表达式规定格式
    }

    public delegate void OnHttpRspd(string netData);
    
    
    public void StartUnityWebRequest(string requestUrl, string data, OnHttpRspd callback, OnHttpRspd errorCallback, OnHttpRspd timeoutCallback)
    {
        StartCoroutine(SendRequest(requestUrl, data, callback, errorCallback, timeoutCallback));
    }

    /// <summary>
    /// 开启一个协程，发送请求
    /// </summary>
    /// <returns></returns>
    IEnumerator SendRequest(string requestUrl, string dataString, OnHttpRspd callback, OnHttpRspd errorCallback, OnHttpRspd timeoutCallback)
    {
        byte[] b = System.Text.Encoding.Default.GetBytes(dataString);      
        var base64= Convert.ToBase64String(b);  
        using (UnityWebRequest uwr = UnityWebRequest.Get($"{requestUrl}/{base64}"))
        {
            uwr.SendWebRequest();//开始请求
            while (!uwr.isDone)
            {
                //Debug.Log(Math.Floor(uwr.downloadProgress * 100f) + "%");
                //Debug.Log(uwr.downloadProgress  + "%");
                yield return 1;
            }
            if (uwr.isDone)
            {
                //Debug.Log("Request Done!");
            }
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
                errorCallback?.Invoke(uwr.error);
            }
            else
            {
                byte[] results = uwr.downloadHandler.data;
                //去掉结尾的 '\0' 字符 要不然会解析不出json  这个查了很多资料 
                //最终通过打印2进制数组一个一个字节对比才发现的 - -!
                //string json = Encoding.UTF8.GetString(www.bytes,0, www.bytes.Length - 1);
                string json = Encoding.UTF8.GetString(results, 0, results.Length);
                //MyDebug.Log("[Http recv] " + json);
                //JsonData netData = JsonMapper.ToObject(json);
                callback.Invoke(json);
            }
        }
    }
    
    public void StartUnityWebPost(string requestUrl, string data, OnHttpRspd callback, OnHttpRspd errorCallback, OnHttpRspd timeoutCallback)
    {
        StartCoroutine(SendPost(requestUrl, data, callback, errorCallback, timeoutCallback));
    }
    
    /// <summary>
    /// 开启一个协程，发送POST请求
    /// </summary>
    /// <returns></returns>
    IEnumerator SendPost(string requestUrl, string dataString, OnHttpRspd callback, OnHttpRspd errorCallback, OnHttpRspd timeoutCallback)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("unityData", Encoding.UTF8.GetBytes(dataString));
        using (UnityWebRequest uwr = UnityWebRequest.Post(requestUrl, form))
        {
            uwr.SendWebRequest();//开始请求
            while (!uwr.isDone)
            {
//                Debug.Log(Math.Floor(uwr.downloadProgress * 100f) + "%");
//                Debug.Log(uwr.downloadProgress  + "%");
                yield return 1;
            }
            if (uwr.isDone)
            {
//                Debug.Log("Request Done!");
            }
            if (uwr.isHttpError)
            {
                Debug.LogError("isHttpError:" + uwr.error);
                errorCallback?.Invoke(uwr.error);
            }else if (uwr.isNetworkError)
            {
                Debug.LogError("isNetworkError:" + uwr.error);
                errorCallback?.Invoke(uwr.error);
            }
            else
            {
                byte[] results = uwr.downloadHandler.data;
                if (results.Length > 0)
                {
                    //去掉结尾的 '\0' 字符 要不然会解析不出json  这个查了很多资料 
                    //最终通过打印2进制数组一个一个字节对比才发现的 - -!
                    //string json = Encoding.UTF8.GetString(www.bytes,0, www.bytes.Length - 1);
                    string json = Encoding.UTF8.GetString(results, 0, results.Length);
                    //MyDebug.Log("[Http recv] " + json);
                    //JsonData netData = JsonMapper.ToObject(json);
                    callback.Invoke(json);
                }
                else
                {
                    Debug.LogError("Receive data is empty");
                }
            }
        }
    }
}

