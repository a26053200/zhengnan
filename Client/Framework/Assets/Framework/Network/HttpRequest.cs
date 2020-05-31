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
    
    
    public void StartUnityWebRequest(string requestUrl, string data, OnHttpRspd callback)
    {
        StartCoroutine(SendRequest(requestUrl, data, callback));
    }

    /// <summary>
    /// 开启一个协程，发送请求
    /// </summary>
    /// <returns></returns>
    IEnumerator SendRequest(string requestUrl, string dataString, OnHttpRspd callback)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("unityData", Encoding.UTF8.GetBytes(dataString));
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
    public void SendMsg(string requestUrl, JsonData json, OnHttpRspd callback)
    {
        string dataString = json.ToJson();
        StartCoroutine(Request(requestUrl, dataString, callback));
    }
    private IEnumerator Request(string requestUrl, string dataString, OnHttpRspd callback)
    {
        string error = null;
//        if (GlobalConsts.EnableLogNetwork)
//            MyDebug.Log(string.Format("[Http] {0}/{1}", requestUrl, dataString));
        using (var www = new WWW(requestUrl, Encoding.UTF8.GetBytes(dataString)))
        {
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                error = www.error;
            }
            else if (string.IsNullOrEmpty(www.text))
            {
                error = "Receive data is Empty.";
            }
            else
            {
                //去掉结尾的 '\0' 字符 要不然会解析不出json  这个查了很多资料 
                //最终通过打印2进制数组一个一个字节对比才发现的 - -!
                //string json = Encoding.UTF8.GetString(www.bytes,0, www.bytes.Length - 1);
                string json = Encoding.UTF8.GetString(www.bytes, 0, www.bytes.Length);
                //MyDebug.Log("[Http recv] " + json);
                //JsonData netData = JsonMapper.ToObject(json);
                //SendMessage(callback, netData);
                callback.Invoke(json);
                yield break;
            }
        }
        if (error == null)
        {
            error = "Empty responding contents";
        }
        MyDebug.LogError("HTTP POST Error! Cmd:" + callback + " Error:" + error);
        //MyDebug.Log("进入游戏失败！服务器停机或维护。");
    }
}

